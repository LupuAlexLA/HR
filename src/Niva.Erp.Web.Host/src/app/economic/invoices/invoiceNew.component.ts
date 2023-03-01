import { Component, Injector, OnInit } from '@angular/core';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from "../../../shared/AppConsts";
import {
    ActivityTypeDto, ActivityTypeServiceProxy, ContractDto, ContractsServiceProxy, CotaTVAListDto, CotaTVAServiceProxy, CurrencyServiceProxy, DocumentTypeServiceProxy, GetCurrencyOutput,
    GetDocumentTypeOutput, GetThirdPartyOutput, InvoiceDetailsDTO, InvoiceDTO, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceServiceProxy, PersonServiceProxy,
    PrepaymentsListDto, PrepaymentsServiceProxy, ThirdPartyQuality
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invoiceNew.component.html',
    animations: [appModuleAnimation()]
})
export class InvoiceNewComponent extends AppComponentBase implements OnInit {

    invoiceId: number;
    invoice: InvoiceDTO = new InvoiceDTO();
    thirdParties: GetThirdPartyOutput = new GetThirdPartyOutput();
    contracts: ContractDto[] = [];
    exchangeRate: any;
    invoiceElementsDetails: InvoiceElementsDetailsDTO[][] = [];
    invoiceElementDetail: InvoiceElementsDetailsDTO = new InvoiceElementsDetailsDTO();
    currencies: GetCurrencyOutput;
    prepayments: PrepaymentsListDto[] = [];
    elementDetailsCategories: InvoiceElementsDetailsCategoryListDTO[] = [];
    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    activityTypes: ActivityTypeDto[] = [];
    otherDocumentType: string = '';
    decontId: any;
    localCurrencyId: number;
    cotaTvaList: CotaTVAListDto[] = [];

    invoiceElementsDetailsCategoryId: any;
    showDataAmortizare: boolean = false;

    constructor(injector: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private _prepaymentService: PrepaymentsServiceProxy,
        private _personService: PersonServiceProxy,
        private _contractService: ContractsServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _cotaTvaService: CotaTVAServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Documente.Acces')) {
            this.invoiceId = + this.route.snapshot.queryParamMap.get('facturaId');
            this.decontId = this.route.snapshot.queryParamMap.get('decontId');

            if (this.invoiceId != 0) {
                this._invoiceService.getInvoice(this.invoiceId).subscribe(result => {
                    this.invoice = result;
                   
                    if (this.invoice.documentTypeShortName !== 'FF' && this.invoice.documentTypeShortName !== 'BF') {
                        this.invoice.documentTypeShortName = 'OTH';
                    }

                    this.searchThirdParty(this.invoice.thirdPartyName);
                    this.searchContracts();
                    this.getExchangeRate();
                    //this.getInvoiceElementDetailCategories();
                    this.getInvoiceElementDetailCategoriesForThirdPartyQuality();
                    this._prepaymentService.getPrepaymentsForInvoiceDetails(this.invoiceId).subscribe(result => {
                        this.prepayments = result;
                    });

                    // setez dimensiunea array-ului `invoiceElementsDetails` in functie de numarul detaliilor
                    //this.invoiceElementsDetails = new Array(this.invoice.invoiceDetails.length).fill([]);

                    //populez lista elementelor pentru factura selectata
                    for (let i = 0; i < this.invoice.invoiceDetails.length; i++) {

                        this.invoiceElementsDetails.push([this.invoice.invoiceDetails[i].invoiceElementsDetails]);
                        this.getInvoiceElementDetailsByCategoryId(this.invoice.invoiceDetails[i].invoiceElementsDetails.invoiceElementsDetailsCategoryId, i);
                    }
                    this.getCoteTVAList();
                });

            } else {
                this.invoice.activityTypeId = null;
                this.invoice.enableEdit = true;
                this.invoice.invoiceDetails = [];
                this.invoice.invoiceDetails.push(new InvoiceDetailsDTO());
                this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].itemState = 0;
                this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].durationInMonths = 0;
                this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].invoiceElementsDetails = new InvoiceElementsDetailsDTO();
                this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].cotaTVA_Id = null;
                //this.getInvoiceElementDetailCategories();
                this.invoice.documentTypeShortName = 'FF';
                this.changeDocumentType(this.invoice.documentTypeShortName);
                this.invoice.thirdPartyQuality = 1;
                this.getInvoiceElementDetailCategoriesForThirdPartyQuality();
                this.invoice.operationDate = moment().startOf('day');
                this.invoice.invoiceDate = moment().startOf('day');
                this.getLocalCurrencyId();
                //this.invoice.currencyId = this.localCurrencyId;
                //    this.getCoteTVAList();
            }


            this.getCurrenciesList();
            this.getActivityTypeList();
            this.getDocumentTypeList();
            /* this.getCoteTVAList();*/


            if (this.decontId != 0) {
                this.invoice.decontId = this.decontId;
            }
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    test() {
        console.log(this.invoice)
    }

    getInvoiceElementDetailsByCategoryId(categoryId: number, index: number) {
        this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).subscribe(result => {
            this.invoiceElementsDetails[index] = result;
        });
    }

    getInvoiceElementDetail(elementId: number, index: number) {
        this._invoiceService.getInvoiceElementsDetail(elementId).subscribe(result => {
            var invoiceElementDetail = result;

            this.invoice.invoiceDetails[index].invoiceElementsDetailsId = invoiceElementDetail.id;
            this.invoice.invoiceDetails[index].invoiceElementsDetails = invoiceElementDetail;
            this.invoice.invoiceDetails[index].invoiceElementsDetails.invoiceElementsDetailsCategoryId = invoiceElementDetail.invoiceElementsDetailsCategoryId;
        });
    }

    // populez lista cu categoriile de elemente pentru factura
    getInvoiceElementDetailCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.elementDetailsCategories = result;
        });
    }

    // populez lista cu categoriile de elemente pentru factura in functie de calitatea tertului
    getInvoiceElementDetailCategoriesForThirdPartyQuality() {

        //if (this.invoice.thirdPartyQuality == ThirdPartyQuality._0) {
        //    this.invoice.invoiceSeries = "FGDB"
        //    this._invoiceService.getFacturaNumberIncremented().subscribe(result => {
        //        this.invoice.invoiceNumber = result.toString();
        //    });

        //}
        
        this._invoiceService.getInvoiceElementsDetailsCategoriesforThirdPartyQuality(this.invoice.thirdPartyQuality).subscribe(result => {
            this.elementDetailsCategories = result;

            if (this.invoiceId == 0) {
                this.getSeriesAndNumberForInvoice();
            }

            for (var i = 0; i < this.invoice.invoiceDetails.length; i++) {

                var x = this.elementDetailsCategories.find(f => f.id == this.invoice.invoiceDetails[i].invoiceElementsDetails.invoiceElementsDetailsCategoryId);
                if (x === undefined) {
                    this.invoiceElementsDetailsCategoryId = this.elementDetailsCategories[0].id;
                } else {
                    this.invoiceElementsDetailsCategoryId = x.id;
                }
                this.getInvoiceElementDetailsByCategoryId(this.invoiceElementsDetailsCategoryId, i);
            }
        });
    }

    getSeriesAndNumberForInvoice() {
        this._invoiceService.getInvoiceSeriesAndNumber(this.invoice.thirdPartyQuality).subscribe(result => {
            this.invoice.invoiceSeries = result.invoiceSeries;
            this.invoice.invoiceNumber = result.invoiceNumber;
        });
    }

    // populeaza lista monedelor
    getCurrenciesList() {
        this._personService.currencyList().subscribe(result => {
            this.currencies = result;
        });
    }

    // lista pentru tipurile de documente
    getDocumentTypeList() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result;
        });
    }

    // lista pentru tipul de activitate
    getActivityTypeList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypes = result;
        })
    };

    /**
     * Populeaza lista tertilor 
     * @param thidPartyName
     */
    searchThirdParty(thidPartyName: string) {
        this._personService.thirdPartySearch(thidPartyName).subscribe(result => {
            this.thirdParties = result;
        });
    }

    /**
     * Populare lista tertilor dupa numele cautat
     * @param search
     */
    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.invoice.thirdPartyId = thirdPartyId;
        this.invoice.thirdPartyName = thirdPartyName;
        this.searchContracts();
    }

    // Returneaza numele tertului daca thirdPartyId != 0
    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        return this.invoice.thirdPartyName;
    }

    /**
     * Returneaza contractele pentru thirdpartyId
     * */
    searchContracts() {
        this._contractService.getContractsForInvoiceThirdParty(this.invoice.thirdPartyId).subscribe(result => {
            this.contracts = result;
        });
    }

    /**
     * Returneaza valoarea schimbului valutar in  functie de data facturii
     * */
    getExchangeRate() {

        if (this.invoice.invoiceDate !== undefined) {
            this.getCoteTVAList();
            let date = this.invoice.invoiceDate;
            let currencyId = this.invoice.monedaFacturaId;
            this.invoice.currencyName = this.currencies?.getCurrency.find(f => f.id == currencyId).currencyName;

            this._currencyService.getLocalExchangeRateForOper(date, currencyId).subscribe(result => {
                this.exchangeRate = result;
                this.invoice.cursValutar = result;
            }, (error: any) => { this.exchangeRate = 0; },
                () => { this.updateTotalPlata(); this.updateLocalCurrency(); })
        }

        
    }

    getExchangeRateForPlata() {

        if (this.invoice.invoiceDate !== undefined) {
            this.getCoteTVAList();
            let date = this.invoice.invoiceDate;
            let currencyId = this.invoice.currencyId;
            // this.invoice.currencyName = this.currencies?.getCurrency.find(f => f.id == currencyId).currencyName;

            this._currencyService.getLocalExchangeRateForOper(date, currencyId).subscribe(result => {
                this.exchangeRate = Number((1/result).toFixed(3));
                this.invoice.cursValutar = Number((1 / result).toFixed(2));
            }, (error: any) => { this.exchangeRate = 0; },
                () => { this.updateTotalPlata(); this.updateLocalCurrency(); })
        }

        
    }



    /**
     * Adaugare rand nou in invoiceDetails
     * */
    addDetail() {
        let newInvoiceDetail = new InvoiceDetailsDTO();
        newInvoiceDetail.durationInMonths = 0;
        this.invoice.invoiceDetails.push(newInvoiceDetail);
        this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].itemState = 0;
        this.invoice.invoiceDetails[this.invoice.invoiceDetails.length - 1].invoiceElementsDetails = new InvoiceElementsDetailsDTO();
    }

    /**
     * Stergere rand din invoiceDetails
     * @param index
     */
    deleteDetail(index: number) {
        this.invoice.invoiceDetails[index].itemState = 3;
        this.updateValue();
    }

    /**
     * Resetare valori din randul {{index}}
     * @param index
     */
    restoreDetail(index: number) {
        this.invoice.invoiceDetails[index].itemState = 0;
        this.updateValue();
    }

    updateTotalValueDetail(detail: any) {
        if (this.invoice.currencyId !== this.invoice.monedaFacturaId) {
            detail.value = Number((detail.unitValue * detail.quantity).toFixed(2));
            detail.valoareTotalaDetaliu = Number((detail.value * this.invoice.cursValutar).toFixed(2));
        } else {
            detail.value = Number((detail.unitValue * detail.quantity).toFixed(2));
            detail.valoareTotalaDetaliu = Number(detail.value);
        }
        console.log()
        this.updateValue();
    }

    updateVatDetail(detail: any) {
        detail.vat = Number((detail.value * detail.procVAT / 100).toFixed(2));
        this.updateValue();
    }

    updateValue() {
        let totalAmount = 0;
        let totalAmountFactura = 0;
        let totalVat = 0;
        for (var i = 0; i < this.invoice.invoiceDetails.length; i++) {
            if (this.invoice.invoiceDetails[i].itemState === 0) {
                if (this.invoice.currencyId != this.invoice.monedaFacturaId) {
                    totalAmount += Number(this.invoice.invoiceDetails[i].valoareTotalaDetaliu);
                    totalAmountFactura += Number(this.invoice.invoiceDetails[i].value);
                    /*totalVat += this.invoice.invoiceDetails[i].vat;*/
                } else {
                    totalAmount += Number(this.invoice.invoiceDetails[i].valoareTotalaDetaliu);
                    totalAmountFactura += Number(this.invoice.invoiceDetails[i].value);
                }

            }
        }
        if (!Number.isNaN(totalAmount)) {
            this.invoice.value = +Number(totalAmount).toFixed(2);
            this.invoice.valoareTotalaFactura = Number(totalAmountFactura);
            /*this.invoice.vat = totalVat;*/
            this.updateLocalCurrency();
        }
    }

    updateLocalCurrency() {
        this._currencyService.getLocalCurrencyId().subscribe(result => {
            if (this.invoice.currencyId == result) {
                this.invoice.valueLocalCurr = this.invoice.value;
            }
            else {
                this.invoice.valueLocalCurr = this.invoice.valoareTotalaFactura;
            }

        });
        //if (!Number.isNaN(this.invoice.valoareTotalaFactura) && this.invoice.valoareTotalaFactura !== undefined) {
        //    this.invoice.valueLocalCurr = Number((this.invoice.valoareTotalaFactura * this.exchangeRate).toFixed(2));
        //    /*this.invoice.vatLocalCurr = Number((this.invoice.vat * this.exchangeRate).toFixed(2));*/
        //}
    }

    updateTotalPlata() {
        //this.getExchangeRate();


        this.invoice.invoiceDetails.forEach(f => { this.updateTotalValueDetail(f) });
        

        //let totalAmountFactura = 0;
        //if (this.invoice.monedaFacturaId !== this.invoice.currencyId) {
        //    for (var i = 0; i < this.invoice.invoiceDetails.length; i++) {
        //        if (this.invoice.invoiceDetails[i].itemState === 0) {
        //            this.invoice.invoiceDetails[i].unitValue = +Number(this.invoice.invoiceDetails[i].valoareTotalaDetaliu / this.invoice.cursValutar).toFixed(0);
        //            this.invoice.invoiceDetails[i].value = this.invoice.invoiceDetails[i].quantity * this.invoice.invoiceDetails[i].unitValue;
        //            totalAmountFactura += Number(this.invoice.invoiceDetails[i].value);
        //        }
                

        //    }
        //} else {

        //    for (var i = 0; i < this.invoice.invoiceDetails.length; i++) {
        //        if (this.invoice.invoiceDetails[i].itemState === 0) {
        //            this.invoice.invoiceDetails[i].unitValue = this.invoice.invoiceDetails[i].valoareTotalaDetaliu;
        //            this.invoice.invoiceDetails[i].value = this.invoice.invoiceDetails[i].valoareTotalaDetaliu;
        //            totalAmountFactura += Number(this.invoice.invoiceDetails[i].value);
        //        }
                
        //    }
        //}

        //if (!Number.isNaN(totalAmountFactura)) {
        //    this.invoice.valoareTotalaFactura = totalAmountFactura;
        //}
    }



    changeDocumentType(documentShortName: string) {
        this._documentTypeService.getDocumentTypeId(documentShortName).subscribe(result => {
            this.invoice.documentTypeId = result;
        });
    }

    clearDocumentTypeId() {
        this.invoice.documentTypeId = null;
    }

    saveInvoice() {
        //remove deleted elements
        this.invoice.invoiceDetails = this.invoice.invoiceDetails.filter(item => item.itemState === 0);

        if (this.invoice.currencyId === undefined) {
            abp.notify.error("Eroare", "Nu ati specificat moneda");
        } else {
            this._invoiceService.saveInvoice(this.invoice).subscribe(() => {
                if (this.invoice.decontId !== null) {
                    this.router.navigate(['/app/decontAdd'], { queryParams: { decontId: this.decontId } });
                } else {
                    this.router.navigate(['/app/economic/invoices/invoicesList']);
                    abp.notify.info("invoiceAddedMessage");
                }
            });
        }
    }

    openNewTab() {
        window.open(AppConsts.appBaseUrl + '/app/nomenclatoare/person/personNew', '_blank');
    }

    getLocalCurrencyId() {
        this._currencyService.getLocalCurrencyId().subscribe(result => {
            this.invoice.currencyId = result;
            this.invoice.monedaFacturaId = result;
            this.getExchangeRate();
        })
    };

    // lista cote TVa in functie de data selectata
    getCoteTVAList() {
        this._cotaTvaService.getTVAListByYear(this.invoice.invoiceDate).subscribe(result => {
            this.cotaTvaList = result;
        });
    }

    showDataStartAmortizare(index: number) {
        if (this.invoice.invoiceDetails[index].durationInMonths > 0) {
            this.showDataAmortizare = true;
            const startOfNextMonth = moment(this.invoice.invoiceDate).add(1, 'M').startOf('month').format('YYYY-MM-DD hh:mm:ss');
            this.invoice.invoiceDetails[index].dataStartAmortizare = moment(startOfNextMonth);
        } else {
            this.showDataAmortizare = false;

        }

    }
}