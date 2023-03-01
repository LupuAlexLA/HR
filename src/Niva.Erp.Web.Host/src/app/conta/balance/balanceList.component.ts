import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from "../../../shared/app-component-base";
import { BalanceCompSummaryDto, BalanceCompValidDto, BalanceInitDto, BalanceServiceProxy, CurrencyDto, CurrencyServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './balanceList.component.html',
    animations: [appModuleAnimation()]
})
export class BalanceListComponent extends AppComponentBase implements OnInit {

    balanceInit: BalanceInitDto = new BalanceInitDto();
    balanceSummary: BalanceCompSummaryDto[] = [];
    balanceValid: BalanceCompValidDto[] = [];
    currencies: CurrencyDto[];
    showCalcButton: true;

    constructor(injector: Injector,
        private _balanceService: BalanceServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted("Conta.Balanta.Balante.Acces")) {
            if (sessionStorage.getItem('balanceInitBalanceList')) {
                this.balanceInit = JSON.parse(sessionStorage.getItem('balanceInitBalanceList'));
                this.balanceList();
            }
            else {
                this.initBalance();
            }
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
    }

    initBalance() {
        this._balanceService.initBalanceForm().subscribe(result => {
            this.balanceInit = result;
        });
    }

    showComputeFormFnc() {
        this.balanceInit.showComputeForm = true;
    }

    hideComputeFormFnc() {
        this.balanceInit.showComputeForm = false;
    }

    hideBalanceDetails() {
        this.balanceInit.showForm = 1;
    }

    hideSummary() {
        this.balanceInit.showForm = 1;
    }

    hideValidation() {
        this.balanceInit.showForm = 1;
    }

    computeBalance() {
        this._balanceService.computeBalance(this.balanceInit).subscribe(result => {
            abp.notify.info(this.l('Calcul OK'));
            this.balanceInit = result;
            this.balanceInit.showForm = 1;
        });
    }

    getBalanceListCount() {
        if (this.balanceInit.balanceList == null) {
            return 0;
        } else {
            return this.balanceInit.balanceList.length;
        }
    }

    balanceDetailsInit(id: number, balanceType: string) {
        this._balanceService.balanceDetailsInit(id, balanceType, this.balanceInit).subscribe(result => {
            this.balanceInit = result;
            this.currencyList();
        });
    }

    balanceList() {
        sessionStorage.setItem('balanceInitBalanceList', JSON.stringify(this.balanceInit));
        this._balanceService.balanceList(this.balanceInit).subscribe(result => {
            this.balanceInit = result;
        });
    }

    balanceDetails() {
        this._balanceService.balanceDetails(this.balanceInit).subscribe(result => {
            this.balanceInit = result;

        });
    }

    deleteBalance(balanceId: number) {
        abp.message.confirm(
            'Balanta va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                this._balanceService.deleteBalance(balanceId).subscribe(() => {
                    abp.notify.info(this.l('DeleteMessage'));
                    this.initBalance();
                });
            }
        );
    }

    balanceSummaryList() {
        this._balanceService.balanceSummaryList(this.balanceInit.calcDate).subscribe(result => {
            this.balanceSummary = result;
            this.balanceInit.showForm = 3;
            this.showCalcButton = true;
        });
    }

    balanceValidList(balanceId: number) {
        this._balanceService.balanceValidList(balanceId).subscribe(result => {
            this.balanceValid = result;
            this.balanceInit.showForm = 4;
        });
    }

    //currency list 
    currencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }
}