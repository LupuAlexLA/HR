import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import {
    CotaTVAListDto, CotaTVAServiceProxy, CurrencyListDto, DepartamentListDto, DepartamentServiceProxy, EnumServiceProxy, EnumTypeDto, ExchangeRateForecastServiceProxy,
    ImoAssetClassCodeListDDDto, ImoAssetClassCodeServiceProxy, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceServiceProxy, PaapEditDto, PaapServiceProxy,
    PersonServiceProxy
} from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './paapAdd.component.html',
    animations: [appModuleAnimation()]
})
export class PaapAddComponent extends AppComponentBase implements OnInit {

    paap: PaapEditDto = new PaapEditDto();
    categoryElementsList: InvoiceElementsDetailsCategoryListDTO[] = [];
    elementDetailsList: InvoiceElementsDetailsDTO[] = [];
    elementDetail: InvoiceElementsDetailsDTO = new InvoiceElementsDetailsDTO();
    sursaFinantareList: EnumTypeDto[] = [];
    modalitateDerulareList: EnumTypeDto[] = [];
    obiectTranzactieList: EnumTypeDto[] = [];
    imoAssetClassCodeList: ImoAssetClassCodeListDDDto[] = [];
    departamentList: DepartamentListDto[] = [];
    currencyList: CurrencyListDto[] = [];
    cotaTvaList: CotaTVAListDto[] = [];
    paapId: number;
    isLoading: boolean = false;
    vat: number = 0;

    constructor(inject: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private _paapService: PaapServiceProxy,
        private _enumService: EnumServiceProxy,
        private _imoAssetClassCodeService: ImoAssetClassCodeServiceProxy,
        private _departamentService: DepartamentServiceProxy,
        private _currencyService: PersonServiceProxy,
        private _cotaTvaService: CotaTVAServiceProxy,
        private _exchangeRateService: ExchangeRateForecastServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.PAAP.Acces')) {
            this.paapId = + this.route.snapshot.queryParamMap.get('paapId');

            this.isLoading = true;
            this._paapService.getPaap(this.paapId).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false
                })
            ).subscribe(result => {
                this.paap = result;

                if (this.paap.invoiceElementsDetailsCategoryId !== null) {
                    this.getInvoiceElementDetailsByCategoryId(this.paap.invoiceElementsDetailsCategoryId);
                    this.getElementDetailsById(this.paap.invoiceElementsDetailsId);
                }

                this.getInvoiceElementsDetailsCategories();
                this.getSursaFinantareList();
                this.getModalitateDerulare();
                this.getObiecteTranzactieList();
                this.getImoAssetClassCode();
                this.getDepartamentList();
                this.getCurrencyList();
                this.getCotaTvaList();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    ExtractVAT() {
        if (this.cotaTvaList.filter(d => d.id == this.paap.cotaTVA_Id)[0]) {
            this.vat = this.cotaTvaList.filter(d => d.id == this.paap.cotaTVA_Id)[0].vat;
        }

        if (this.paap.currencyId == this.paap.localCurrencyId) {
            this.paap.valoareEstimataFaraTvaLei = Number((this.paap.valoareTotalaLei / (100 + this.vat) * 100).toFixed(2));           
        }
        else {
            this._exchangeRateService.getExchangeRateForecastCurrency(this.paap.currencyId,this.paap.dataEnd.year()).subscribe(result => {
                this.paap.valoareTotalaLei = Number((this.paap.valoareTotalaValuta * result).toFixed(2));
                this.paap.valoareEstimataFaraTvaLei = Number((this.paap.valoareTotalaLei / (100 + this.vat) * 100).toFixed(2));
                this.paap.valoareTotalaLei = Number((this.paap.valoareEstimataFaraTvaLei / 100 * (100 + this.vat)).toFixed(2));
            })
        }
    }
    
    //CalculateVAT() {

        
    //    let vat = 0
    //    if (this.cotaTvaList.filter(d => d.id == this.paap.cotaTVA_Id)[0]) {

    //        vat = this.cotaTvaList.filter(d => d.id == this.paap.cotaTVA_Id)[0].vat;
            
    //    }

    //    if (this.paap.currencyId == this.paap.localCurrencyId) {
    //        this.paap.valoareTotalaLei = Number((this.paap.valoareEstimataFaraTvaLei / 100 * (100 + vat)).toFixed(2));
    //    }
    //}
    getInvoiceElementsDetailsCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.categoryElementsList = result;
        });
    }

    // populez lista elementelor in fucntie de categorie selectata
    getInvoiceElementDetailsByCategoryId(categoryId: number) {
        if (categoryId !== null) {
            this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).subscribe(result => {
                this.elementDetailsList = result;
                this.getAvailableSumForCategory(categoryId);
            });
        }
    }

    //returnez suma disponibila pentru categoria selectata
    getAvailableSumForCategory(categoryId: number) {
        if (this.paap.departamentId !== null) {
            this._paapService.getAvailableSum(categoryId, this.paap.departamentId, this.paap.dataEnd).subscribe(result => {
                this.paap.availableValue = result;
            });
        }
    }

    getElementDetailsById(invoiceElementsDetailsId: number) {
        this._invoiceService.getInvoiceElementsDetail(invoiceElementsDetailsId).subscribe(result => {
            this.elementDetail = result;
            if (this.elementDetail.showCA === false && this.elementDetail.showMF === false) {
                this.paap.durationInMonths = null;
            }
        });
    }

    getSursaFinantareList() {
        this._enumService.sursaFinantareList().subscribe(result => {
            this.sursaFinantareList = result;
        });
    }

    getModalitateDerulare() {
        this._enumService.modalitateDerulareList().subscribe(result => {
            this.modalitateDerulareList = result;
        });
    }

    getObiecteTranzactieList() {
        this._enumService.obiecteTranzactieList().subscribe(result => {
            this.obiectTranzactieList = result;
        });

        if (this.paap.obiectTranzactieId === 3) {
            this.getImoAssetClassCode();
        }
    }

    getCurrencyList() {
        this._currencyService.currencyList().subscribe(result => {
            this.currencyList = result.getCurrency;
        });
    }

    getCotaTvaList() {
        this._cotaTvaService.getTVAListByYear(this.paap.dataEnd).subscribe(result => {
            this.cotaTvaList = result;
            this.ExtractVAT();
        });      
    }

    getImoAssetClassCode() {
        this._imoAssetClassCodeService.imoAssetClassCodeList().subscribe(result => {
            this.imoAssetClassCodeList = result.getImoAssetClassCode;
        });
    }

    getDurationMin(imoAssetClassCodeId: number) {
        this._imoAssetClassCodeService.getClassCodeById(imoAssetClassCodeId).subscribe(result => {
            this.paap.durationInMonths = result.durationMin;
        });
    }

    getDepartamentList() {
        this._departamentService.getDepartamentList().subscribe(result => {
            this.departamentList = result;
        });
    }

    save() {
        this._paapService.paapSave(this.paap).subscribe(result => {
            this.router.navigate(['/app/buget/paap/paapList']);
            abp.notify.info("Achizitia a fost inregistrata");
        });
    }

    generateTranse() {
        this._paapService.generateTranse(this.paap).subscribe(result => {
            this.paap = result;
            abp.notify.info("Transele au fost generate");
        });
    }

    reglareTranse(index: number) {
        this._paapService.reglareTranse(index, this.paap).subscribe(result => {
            this.paap = result;
        });
    }

    calculValoareFaraTVA(index: number) {
        this.paap.transe[index].valoareLeiFaraTVA = Number(((this.paap.transe[index].valoareLei / (100 + this.vat)) * 100).toFixed(2));
    }

    aplicaModificarile(index) {
        for (var i = index + 1; i < this.paap.nrTranse; i++) {
            this.paap.transe[i].valoareLei = this.paap.transe[index].valoareLei;
            this.paap.transe[i].valoareLeiFaraTVA = this.paap.transe[index].valoareLeiFaraTVA;
        }
    }

    deleteTransa(index: number) {
        this.paap.transe.splice(index, 1);
    }

    addTransa() {
        this._paapService.addTransa(this.paap).subscribe(result => {
            this.paap = result;
        });
    }
}