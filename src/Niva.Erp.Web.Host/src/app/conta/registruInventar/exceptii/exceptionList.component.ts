import { Component, Injector, OnInit } from "@angular/core";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { AccountListDDDto, AccountServiceProxy, GetExceptElimRegInventarOutput, GetRegInventarOutput, RegInventarExceptiiEliminareServiceProxy, RegInventarExceptiiServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './exceptionList.component.html',
    animations: [appModuleAnimation()]
})
export class ExceptionListComponent extends AppComponentBase implements OnInit {

    exceptRegInventarList: GetRegInventarOutput = new GetRegInventarOutput();
    exceptEliminareRegInventarList: GetExceptElimRegInventarOutput = new GetExceptElimRegInventarOutput();
    accounts: AccountListDDDto[] = [];
    accountsExceptEliminare: AccountListDDDto[] = [];

    constructor(inject: Injector,
        private _regInventarExceptService: RegInventarExceptiiServiceProxy,
        private _regInventarExceptEliminareService: RegInventarExceptiiEliminareServiceProxy,
        private _accountService: AccountServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getExceptRegList();
        this.getExceptEliminRegList();

        this.getAccountList();
        this.getAccountExceptElimList();
    }

    // Lista conturilor pentru exceptie
    getExceptRegList() {
        this._regInventarExceptService.getRegInventarInit().subscribe(result => {
            this.exceptRegInventarList = result;
        });
    }

    // Lista conturilor exceptate de la eliminare
    getExceptEliminRegList() {
        this._regInventarExceptEliminareService.getRegInventarInit().subscribe(result => {
            this.exceptEliminareRegInventarList = result;
        });
    }


    getAccountList() {
        this._accountService.getAnalythicsForRegistruInventar().subscribe(result => {
            this.accounts = result;
        });
    }

    getAccountExceptElimList() {
        this._accountService.getAccountsRegInvExceptEliminare().subscribe(result => {
            this.accountsExceptEliminare = result;
        });
    }

    exceptRegInventarAddRow() {
        this._regInventarExceptService.exceptRegInventarAddRow(this.exceptRegInventarList.getRegInventar).subscribe(result => {
            this.exceptRegInventarList.getRegInventar = result;
        });
    }

    exceptRegInventarSave() {
        this._regInventarExceptService.saveExceptRegInventar(this.exceptRegInventarList.getRegInventar).subscribe(result => {
            this.exceptRegInventarList.getRegInventar = result;
            abp.notify.success("Exceptia a fost inregistrata");
        });
    }

    exceptRegInventarDelRow(index: number) {
        this.exceptRegInventarList.getRegInventar.splice(index, 1);
    }

    selectedInput(itemName: string, itemId: number, index: number) {
        this.exceptRegInventarList.getRegInventar[index].accountId = itemId;
        this.exceptRegInventarList.getRegInventar[index].accountName = itemName;
    }

    getAccountName(accountId: number) {
        if (!accountId) {
            return '';
        }
        return this.exceptRegInventarList.getRegInventar.find(f => f.accountId == accountId).accountName;
    }

    getAccountListComputing(search: any) {
        this._accountService.getAccountsList(search.target.value).subscribe(result => {
            this.accounts = result;
        });
    }

    getAccountExceptElimListComputing(search: any) {
        this._accountService.getAccountsExceptEliminareList(search.target.value).subscribe(result => {
            this.accountsExceptEliminare = result;
        });
    }

    exceptElimRegInventarDelRow(index: number) {
        this.exceptEliminareRegInventarList.getRegInventar.splice(index, 1);
    }

    exceptElimRegInventarAddRow() {
        this._regInventarExceptEliminareService.exceptEliminareRegInventarAddRow(this.exceptEliminareRegInventarList.getRegInventar).subscribe(result => {
            this.exceptEliminareRegInventarList.getRegInventar = result;
        });
    }

    exceptElimRegInventarSave() {

        this._regInventarExceptEliminareService.saveExceptEliminareRegInventar(this.exceptEliminareRegInventarList.getRegInventar).subscribe(result => {
            this.exceptEliminareRegInventarList.getRegInventar = result;
            abp.notify.success("Exceptia de la eliminare a fost inregistrata");
        });
    }

    selectedExceptElimInput(itemName: string, itemId: number, index: number) {
        this.exceptEliminareRegInventarList.getRegInventar[index].accountId = itemId;
        this.exceptEliminareRegInventarList.getRegInventar[index].accountName = itemName;
    }

    getExceptElimAccountName(accountId: number) {
        if (!accountId) {
            return '';
        }
        return this.exceptEliminareRegInventarList.getRegInventar.find(f => f.accountId == accountId).accountName;
    }
}