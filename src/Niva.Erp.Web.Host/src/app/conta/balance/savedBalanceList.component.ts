import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { BalanceDDDto, BalanceServiceProxy, CurrencyDto, CurrencyServiceProxy, SavedBalanceDto, SavedBalanceServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './savedBalanceList.component.html',
    animations: [appModuleAnimation()]
})
export class SavedBalanceListComponent extends AppComponentBase implements OnInit{

    balanceInit: SavedBalanceDto = new SavedBalanceDto();
    balanceDD: BalanceDDDto[] = [];
    url: string = "";
    savedBalanceParams: any = {};
    balanceType: string = '';
    savedBalanceId: number;
    nivelMax: any;
    currencyId: any = 0;
    currencyList: CurrencyDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _savedBalanceService: SavedBalanceServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _balanceService: BalanceServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Balanta.BalanteSalvate.Acces')) {
            this._savedBalanceService.initBalanceForm().subscribe(result => {
                this.balanceInit = result;
                this.balanceDDList();
                this.getCurrencyList();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }

        if (e.rowType == "data") {
            e.cellElement.style.fontWeight = e.row.data.totalSum == true ? 'bold' : '';
        }
    }

    balanceDDList() {
        this._balanceService.balanceDDList().subscribe(result => {
            this.balanceDD = result;
        });
    }

    balanceList() {
        this._savedBalanceService.savedBalanceList(this.balanceInit/*, this.balanceInit.externalSave*/).subscribe(result => {
            this.balanceInit = result;
        });
    }

    showSaveFormFnc() {
        this.balanceInit.showSaveForm = true;
    }

    setDailyBalanceDate() {
        if (this.balanceInit.savedBalanceForm.isDaily) {

            var currentDate = new Date();
            var prevDate = new Date(currentDate);
            this.balanceInit.savedBalanceForm.dailyBalanceDate = moment(prevDate.setDate(prevDate.getDate() -1));
        }
    }

    hideSaveFormFnc() {
        this.balanceInit.showSaveForm = false;
    }

    saveBalance() { // trebuie adaugat spinner
        this.isLoading = true;
        this._savedBalanceService.saveBalance(this.balanceInit).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            }))
            .subscribe(result => {
                abp.notify.info('ComputeMessage');
                this.balanceInit = result;
        });
    }

    getBalanceListCount() {
        if (this.balanceInit.balanceList == null) {
            return 0;
        } else {
            return this.balanceInit.balanceList.length;
        }
    }

    balanceDetailsInit(id: number) {
        this._savedBalanceService.balanceDetailsInit(id, this.currencyId, this.balanceInit).subscribe(result => {
            this.balanceInit = result;
            this.nivelMax = Math.max.apply(Math, result.viewBalanceDetail.balanceDetail.map(f => { return f.nivelRand; }));
            this.balanceInit.viewBalanceDetail.nivelRand = this.nivelMax;
            this.savedBalanceId = id;     
        });
    }

    deleteSavedBalance(balanceId: number) {
        abp.message.confirm(
            'Balanta salvata va fi stearsa. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._savedBalanceService.deleteBalance(balanceId).subscribe(() => {
                        this.balanceList();
                        abp.notify.info('DeleteMessage');
                    });
                }
            }
        );
    }

    hideBalanceDetails() {
        this.balanceInit.showBalanceDetails = false;
    }

    balanceDetails() {
        this.balanceInit.viewBalanceDetail.currencyId = this.currencyId;
        this.balanceInit.viewBalanceDetail.nivelRand = this.nivelMax;
        this._savedBalanceService.balanceDetails(this.balanceInit).subscribe(result => {
            this.balanceInit = result;
        });
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        this.savedBalanceParams.savedBalanceId = this.savedBalanceId;
        this.savedBalanceParams.searchAccount = this.balanceInit.viewBalanceDetail.searchAccount;
        this.savedBalanceParams.nivelRand = this.balanceInit.viewBalanceDetail.nivelRand;
        this.savedBalanceParams.currencyId = this.currencyId;

       // this.url += "SavedBalanceReport";
        this.url += "SavedBalanceReportV2";
        this.url += '?' + this.ConvertToQueryStringParameters(this.savedBalanceParams);
        window.open(this.url);
    }

    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencyList = result;
        });
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}