import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import {
    DecontEditDto, DecontServiceProxy, EnumServiceProxy, EnumTypeDto, GetThirdPartyOutput, InvoiceDTO, InvoiceServiceProxy, PersonServiceProxy,
    CurrencyServiceProxy, CurrencyDto, DiurnaServiceProxy, DiurnaListDto, GetCountryOutput, DiurnaEditDto} from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './decontAdd.component.html',
    animations: [appModuleAnimation()]
})
export class DecontAddComponent extends AppComponentBase implements OnInit {

    thirdParties: GetThirdPartyOutput;
    decontTypeList: EnumTypeDto[] = [];
    decontEdit: DecontEditDto = new DecontEditDto();
    decontId: any;
    invoicesForDecont: InvoiceDTO[] = [];
    currency: CurrencyDto[];
    countries: GetCountryOutput;
    diurnaList: DiurnaListDto[] = [];
    totalDiurnaAcordata: any;
    totalDiurnaImpozabila: any;
    diurna: DiurnaEditDto = new DiurnaEditDto();
    isDiurnaExterna: boolean;
    startDate: any;
    endDate: any;
    scopDeplasareList: EnumTypeDto[] = [];
    scopDeplasareId: number;
    isLoading: boolean = false;
    showInfo: boolean = true;

    constructor(inject: Injector,
        private _decontService: DecontServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _diurnaService: DiurnaServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Deconturi.Modificare')) {

            this.decontId = this.route.snapshot.queryParamMap.get('decontId');

            this._decontService.getDecont(this.decontId || 0).subscribe(result => {
                this.decontEdit = result;
                this.scopDeplasareId = this.decontEdit.scopDeplasareTypeId;
                console.log('scop deplasare', this.scopDeplasareId);
                this.getCurrencyList();
                this.decontTypes();
                this.searchThirdParty(this.decontEdit.thirdPartyName);
                this.getInvoices();
                this.countryList();
                this.getScopDeplasareList();
                this.hideInfo();

                if (this.decontId !== null) {
                    var stillUtc = moment.utc(this.decontEdit.dateStart).toDate();
                    this.startDate = moment(stillUtc).local(true).format('YYYY-MM-DD HH:mm:ss');

                    var stillUtc = moment.utc(this.decontEdit.dateEnd).toDate();
                    this.endDate = moment(stillUtc).local(true).format('YYYY-MM-DD HH:mm:ss');
                }
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchThirdParty(thirdPartyName: string) {
        this._personService.thirdPartySearchForDecont(thirdPartyName).subscribe(result => {
            this.thirdParties = result;
        })
    }

    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearchForDecont(search.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        return this.decontEdit.thirdPartyName;
    }

    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currency = result;
        });
    }

    countryList() {
        this._diurnaService.diurnaLegalaList().subscribe(result => {
            this.diurnaList = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.decontEdit.thirdPartyId = thirdPartyId;
        this.decontEdit.thirdPartyName = thirdPartyName;
    }

    decontTypes() {
        this._enumService.decontTypeList().subscribe(result => {
            this.decontTypeList = result;
        });
    }

    getInvoices() {
        this._invoiceService.getInvoicesByDecontId(this.decontId).subscribe(result => {
            this.invoicesForDecont = result;
        });
    }

    getScopDeplasareList() {
        this._enumService.scopeDeplasareList().subscribe(result => {
            this.scopDeplasareList = result;
        });
    }

    saveDecont(actionName: string) {
        if (this.scopDeplasareId !== 2) { // data start si data end doar daca scopul deplasarii este diferit de AlteDeconturi
            this.decontEdit.dateStart = moment(this.startDate);
            this.decontEdit.dateEnd = moment(this.endDate);
        }

        this._decontService.saveDecont(this.decontEdit).subscribe(result => {
          
            this.decontEdit = result;
            if (actionName == 'add') {
                this.router.navigate(['/app/economic/invoices/invoiceNew'], { queryParams: { decontId: this.decontEdit.id } });
            } else {
                this.router.navigate(['/app/economic/invoices/searchInvoice'], { queryParams: { decontId: this.decontEdit.id, currencyId: this.decontEdit.currencyId } });
            }
        });
    }

    deleteInvoice(invoiceId: number) {
        abp.message.confirm('Documentul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invoiceService.deleteInvoiceFromDecont(invoiceId, this.decontId)
                        .subscribe(() => {
                            this.getInvoices();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }

    addToConta() {
        this.isLoading = true;

        if (this.scopDeplasareId !== 2) { // data start si data end doar daca scopul deplasarii este diferit de AlteDeconturi
            this.decontEdit.dateStart = moment(this.startDate);
            this.decontEdit.dateEnd = moment(this.endDate);
        }

        this._decontService.saveDecontToConta(this.decontEdit)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                abp.notify.info("AddedMessage");
            });
    }

    getDiurnaValues(diurnaLegalaId: number) {
        this._diurnaService.getDiurnaLegalaValue(diurnaLegalaId).subscribe(result => {
            this.decontEdit.diurnaLegala = result;
        });

        this._diurnaService.getDiurnaZiValue(diurnaLegalaId).subscribe(result => {
            this.decontEdit.diurnaZi = result;
        });

        this.calculateDiff();
    }

    setDateStart(event: any) {
        this.startDate = event;
        this.decontEdit.dateStart = moment(this.startDate);
        this.calculateDiff();
    }

    setDateEnd(event: any) {
        this.endDate = event;
        this.decontEdit.dateEnd = moment(this.endDate);
        this.calculateDiff();
    }

    calculateDiff() {
        const MIN_PER_DAY = 1440;
        const MIN_PER_HOUR = 60;
        const H_PER_HALF_DAY = 12;
        const H_PER_DAY = 24;

        this._diurnaService.isDiurnaExterna(this.decontEdit.diurnaLegalaId).subscribe(result => {
            this.isDiurnaExterna = result;
        });

        if (this.decontEdit.dateEnd !== null && this.decontEdit.dateEnd !== undefined && this.decontEdit.dateStart !== null && this.decontEdit.dateEnd != undefined) {
            var endDate = moment(this.decontEdit.dateEnd, "YYYY-MM-DD h:mm");
            var startDate = moment(this.decontEdit.dateStart, "YYYY-MM-DD h:mm");
            this._decontService.computeDiurnaAcordataForDecont(this.decontEdit.diurnaLegalaId, moment(startDate), moment(endDate)).subscribe(result => {
                this.decontEdit.nrZile = result.nrZile;
                this.decontEdit.totalDiurnaAcordata = result.totalDiurnaAcordata;
                this.decontEdit.totalDiurnaImpozabila = result.totalDiurnaImpozabila;
                this.decontEdit.diurnaImpozabila = result.diurnaImpozabila;
            });
        }

        // calculez diferenta in ore
        //var durationInMinutes = Math.floor(moment.duration(endDate.diff(startDate)).asMinutes());

        //var days = Math.trunc(durationInMinutes / MIN_PER_DAY);

        //durationInMinutes -= days * MIN_PER_DAY;

        //var hours = Math.trunc(durationInMinutes / MIN_PER_HOUR);

        //if (hours !== 0) {
        //    if (hours > 12 && this.isDiurnaExterna === false) {
        //        days++;
        //    } else if (this.isDiurnaExterna === true && hours < 12) {
        //        days += H_PER_HALF_DAY / H_PER_DAY;
        //    } else if (this.isDiurnaExterna === true && hours > 12) {
        //        days++;
        //    }
        //}

        //this.decontEdit.nrZile = days;

        //var diurnaImpozabila = this.decontEdit.diurnaZi - this.decontEdit.diurnaLegala;
        //if (diurnaImpozabila < 0) {
        //    this.decontEdit.diurnaImpozabila = 0;
        //} else {
        //    this.decontEdit.diurnaImpozabila = diurnaImpozabila;
        //}

        //this.decontEdit.totalDiurnaAcordata = this.decontEdit.nrZile * this.decontEdit.diurnaZi;
        //this.decontEdit.totalDiurnaImpozabila = this.decontEdit.nrZile * this.decontEdit.diurnaImpozabila;

    /*    this.calculateDiurna();*/
    }

    calculateDiurna() {
        var diurnaImpozabila = this.decontEdit.diurnaZi - this.decontEdit.diurnaLegala;
        if (diurnaImpozabila < 0) {
            this.decontEdit.diurnaImpozabila = 0;
        } else {
            this.decontEdit.diurnaImpozabila = diurnaImpozabila;
        }

        this.decontEdit.totalDiurnaAcordata = this.decontEdit.nrZile * this.decontEdit.diurnaZi;
        this.decontEdit.totalDiurnaImpozabila = this.decontEdit.nrZile * this.decontEdit.diurnaImpozabila;
    }

    changeNumber() {
        this._decontService.getNextDecontNumber(this.decontEdit.documentType).subscribe(result => {
            this.decontEdit.decontNumber = result;
        });
    }

    resetInfo() {
        this.decontEdit.diurnaLegalaId = null;
        this.decontEdit.diurnaZi = null;
        this.decontEdit.diurnaLegala = null;
        this.decontEdit.diurnaImpozabila = null;
        this.decontEdit.nrZile = null;
        this.decontEdit.dateStart = null;
        this.decontEdit.dateEnd = null;
        this.decontEdit.totalDiurnaAcordata = 0;
        this.decontEdit.totalDiurnaImpozabila = 0;
    }

    hideInfo() {
        if (this.scopDeplasareId === 2) {
            this.showInfo = false;
            this.resetInfo();
            this.decontEdit.scopDeplasareTypeId = this.scopDeplasareId;
        } else {
            this.showInfo = true;
            this._decontService.getDecont(+this.decontId).subscribe(result => {
                this.decontEdit = result;
                this.decontEdit.scopDeplasareTypeId = this.scopDeplasareId;
                  this.showInfo = true;
            });
        }
    }

    actDiurnaImpoz() {
        this.decontEdit.diurnaImpozabila = this.decontEdit.diurnaZi - this.decontEdit.diurnaLegala;
    }
}