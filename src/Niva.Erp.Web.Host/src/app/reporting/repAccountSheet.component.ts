import { Component, Injector, OnInit } from '@angular/core';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { AppConsts } from '../../shared/AppConsts';
import { AccountListDDDto, AccountServiceProxy, GetCurrencyOutput, PersonServiceProxy, ReportsServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './repAccountSheet.component.html',
    animations: [appModuleAnimation()]
})
export class RepAccountSheetComponent extends AppComponentBase implements OnInit {

    accountList: AccountListDDDto[] = [];
    currencies: GetCurrencyOutput = new GetCurrencyOutput();
    accountSheet: any = {
    corespAccountId: "0"};
    url: string;
    isLoading: boolean = false;
 
    constructor(inject: Injector,
        private _reportService: ReportsServiceProxy,
        private _accountService: AccountServiceProxy,
        private _personService: PersonServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this._reportService.fisaContInit().subscribe(result => {
            this.accountSheet = result;

            this.searchCurrency();
            this.accountListFnc();
        });
    }

    accountListFnc() {
        this._accountService.accountListAll(this.accountSheet.accountId).subscribe(result => {
            this.accountList = result;
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
        return this.accountSheet.accountId == accountId ? this.accountSheet.accountName : '';
    }

    selectedAccount(accountId: number, accountName: string) {
        this.accountSheet.accountId = accountId;
        this.accountSheet.accountName = accountName;
        this.accountSheet.corespAccountId = null;
        this.accountSheet.corespAccountList = null;

    }

    searchCurrency() {
        this._personService.currencyList().subscribe(result => {
            this.currencies = result;
        });
    }

    closeTab() {
        window.close();
    }

    fisaContView() {
        this.isLoading = true;

        this._reportService.fisaContView(this.accountSheet).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.accountSheet = result;
        });
    }

    fisaContChangeCoresp() {
        this.accountSheet.corespAccountId = this.accountSheet.corespAccountId === "0" ? null : this.accountSheet.corespAccountId;

        this.isLoading = true;
        this._reportService.fisaContChangeCoresp(this.accountSheet).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.accountSheet = result;
        });
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.url += "FisaCont?";

        this.url += "accountId=" + this.accountSheet.accountId;

        var startDate = moment(this.accountSheet.startDate).format('M.DD.YYYY');
        this.url += "&startDate=" + startDate;

        var endDate = moment(this.accountSheet.endDate).format('M.DD.YYYY');
        this.url += "&endDate=" + endDate;

        this.url += "&currencyId=" + this.accountSheet.currencyId;
        this.url += "&corespAccountId=" + this.accountSheet.corespAccountId;

        window.open(this.url);
    }
}