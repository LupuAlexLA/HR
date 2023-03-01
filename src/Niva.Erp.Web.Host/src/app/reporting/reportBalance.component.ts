import { Component, Injector, OnInit } from '@angular/core';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { AppConsts } from '../../shared/AppConsts';
import {
    AccountListDDDto, AccountServiceProxy, BalanceDDDto, BalanceServiceProxy, CurrencyDto, CurrencyServiceProxy, DocumentTypeServiceProxy, EnumServiceProxy, EnumTypeDto, GetDocumentTypeOutput,
    GetThirdPartyOutput, PersonServiceProxy, RegInventarServiceProxy, ReportsServiceProxy
} from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './reportBalance.component.html',
    animations: [appModuleAnimation()]
})
export class ReportBalanceComponent extends AppComponentBase implements OnInit {

    balanceList: BalanceDDDto[] = [];
    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    currencyList: CurrencyDto[] = [];

    balanceStruct: number = 5;
    selectedReport: any = "0";
    url: string = '';
    reportName: string = "";

    balanceParams: any = {};
    registruJurnalParams: any = {};
    registruInventarParams: any = {};
    dataStart: any;
    dataEnd: any;

    soldCurentParams: any = {};
    isDateRange = false;
    isLoading: boolean = false;
    accountList: AccountListDDDto[] = [];
    search: string = null;
    tipPerioadaSoldList: EnumTypeDto[] = [];

    soldFurnizorParams: any = {};
    thirdParties: GetThirdPartyOutput = new GetThirdPartyOutput();
    thirdPartyId: number;
    thirdPartyName: string;
    nivelRandMax: any;

    cursValutarParams: any = {};
    balanceId: number;
    dataStartCurs: any;
    dataEndCurs: any;
    dataCursValutar: any;
    depozitBancarParams: any = {};
    detaliuSoldParams: any = {};

    constructor(inject: Injector,
        private _balanceService: BalanceServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _regInventarService: RegInventarServiceProxy,
        private _accountService: AccountServiceProxy,
        private _enumService: EnumServiceProxy,
        private _reportService: ReportsServiceProxy,
        private _personService: PersonServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this._balanceService.balanceDDList().subscribe(result => {
            this.balanceList = result;
        });

        this._currencyService.currencyDDList().subscribe(result => {
            this.currencyList = result;
        });

        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result;
        });

        this.dataEnd = new Date();
        this.getTipPerioadaSold();
        this.getNivelRandMax();
    }

    getTipPerioadaSold() {
        this._enumService.tipPerioadaSoldList().subscribe(result => {
            this.tipPerioadaSoldList = result;
        });
    }

    showDateRange() {
        var currentDate = new Date();
        if (this.selectedReport === "3") {
            this.dataEnd = moment(currentDate);
            this.dataStart = moment(currentDate.setMonth(currentDate.getMonth() - 1));
            this.dataCursValutar = moment(currentDate);
        }
        else if (this.selectedReport === "4" || this.selectedReport == "10") {
            this.dataStart = moment(currentDate);
        }

        if (this.isDateRange === false) {
            this.dataEnd = moment(currentDate);
        } else {
            this.dataEnd = moment(currentDate);
            this.dataStart = moment(currentDate.setMonth(currentDate.getMonth() - 1));
        }
    }

    regInvRecalcul() {
        this.isLoading = true;

        this._regInventarService.recalculRegInventar(this.dataStart).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            abp.notify.info("Registrul a fost recalculat");
        });
    }

    accountListFnc() {
        this._accountService.accountListAll(this.search).subscribe(result => {
            this.accountList = result;
        });
    }

    getNivelRandMax() {
        this._accountService.getAccountNivelRandMax().subscribe(result => {
            this.nivelRandMax = result;
        });
    }
    searchAccountByInput(search: any) {
        this._accountService.accountListAll(search.target.value).subscribe(result => {
            this.accountList = result;
        });
    }

    getAccountName(accountId: number) {
        if (!accountId)
            return '';
        return this.accountList.find(f => f.id == accountId).name ?? '';
    }

    selectedAccount(accountId: number) {
        this.soldCurentParams.accountId = accountId;
    }

    verifyDatePeriod() {
        if (this.selectedReport == 5 && this.soldCurentParams.periodTypeId && this.selectedReport == 7) {
            this._reportService.verifySoldPeriod(moment(this.dataStart), moment(this.dataEnd), this.soldCurentParams.periodTypeId).subscribe(() => {
                this.showReport();
            });
        } else {
            this.showReport();
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        switch (this.selectedReport) {
            case "0":
                this.balanceParams.nivelRand = this.nivelRandMax;
                if (this.balanceStruct == 5) {
                    this.url += "BalantaRon5Egalitati";
                    this.url += '?' + this.ConvertToQueryStringParameters(this.balanceParams);
                } else {
                    this.url += "BalantaRon6Egalitati";
                    this.url += '?' + this.ConvertToQueryStringParameters(this.balanceParams);
                }
                break;
            case "1":
                this.balanceParams.nivelRand = this.nivelRandMax;
                if (this.balanceStruct == 5) {
                    this.url += "BalantaValuta5Egalitati";
                    this.url += '?' + this.ConvertToQueryStringParameters(this.balanceParams);
                } else {
                    this.url += "BalantaValuta6Egalitati";
                    this.url += '?' + this.ConvertToQueryStringParameters(this.balanceParams);
                }
                break;
            case "3":
                this.registruJurnalParams.startDate = moment(this.dataStart).format('M.DD.YYYY');

                this.registruJurnalParams.endDate = moment(this.dataEnd).format('M.DD.YYYY');
                console.log(this.registruJurnalParams.docTypeId);

                if (this.registruJurnalParams.raportExtins) {
                    this.registruJurnalParams.dataCursValutar = moment(this.dataCursValutar).format('M.DD.YYYY');
                    this.url += "RegistruJurnalExtinsReport";
                } else {
                    this.url += "RegistruJurnalReport";
                }
                this.url += '?' + this.ConvertToQueryStringParameters(this.registruJurnalParams);
                break;
            case "4":
                this.registruInventarParams.startDate = moment(this.dataStart).format('M.DD.YYYY');
                this.url += "RegistruInventar";

                this.url += "?" + this.ConvertToQueryStringParameters(this.registruInventarParams);
                break;
            case "5":
                if (this.isDateRange == true) {
                    this.soldCurentParams.dataStart = moment(this.dataStart).format('M.DD.YYYY');
                    this.soldCurentParams.dataEnd = moment(this.dataEnd).format('M.DD.YYYY');

                } else {
                    this.soldCurentParams.dataEnd = moment(this.dataEnd).format('M.DD.YYYY');
                }
                this.soldCurentParams.isDateRange = this.isDateRange;
                this.url += "SoldContCurentReport?";
                this.url += this.ConvertToQueryStringParameters(this.soldCurentParams);
                break;
            case "6":
                this.soldFurnizorParams.startDate = moment(this.dataStart).format('M.DD.YYYY');
                this.soldFurnizorParams.thirdPartyId = this.thirdPartyId;
                this.url += "SoldFurnizoriDebitoriReport?";
                this.url += this.ConvertToQueryStringParameters(this.soldFurnizorParams);
                break;
            case "7":
                
                this.url += "CursValutarBNR?";
                this.url += this.ConvertToQueryStringParameters(this.cursValutarParams);
                break;
            case "8":
                this.url += "DepozitBancarReport?";
                this.url += this.ConvertToQueryStringParameters(this.depozitBancarParams);
                break;
            case "10":
                this.url += "DetaliuSoldReport?";
                this.detaliuSoldParams.dataStart = moment(this.dataStart).format('M.DD.YYYY');
                this.url += this.ConvertToQueryStringParameters(this.detaliuSoldParams);
                break;

        }
        window.open(this.url);
    }

    showFisaCont() {
        window.open(AppConsts.appBaseUrl + '/app/reporting/repAccountSheet', "_blank");
    }

    showDetaliiFacturi() {
        window.open(AppConsts.appBaseUrl + '/app/reporting/invoiceDetailsReport', "_blank");
    }

    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.thirdPartyId = thirdPartyId;
        this.thirdPartyName = thirdPartyName;
    }

    getThirdPartyName() {
        if (this.thirdPartyId === null || this.thirdPartyId === undefined)
            return '';

        return this.thirdParties.getThirdParty.find(t => t.personId == this.thirdPartyId)?.fullName;
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}