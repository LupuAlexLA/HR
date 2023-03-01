import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CurrencyDto, CurrencyServiceProxy, DepositEditDto, DepositServiceProxy, GetBankOutput, GetThirdPartyAccOutput, PersonServiceProxy, ThirdPartyAccListDto } from '../../../shared/service-proxies/service-proxies';
@Component({
    templateUrl: './depositNew.component.html',
    animations: [appModuleAnimation()]
})
export class DepositNewComponent extends AppComponentBase implements OnInit {

    deposit: DepositEditDto = new DepositEditDto();
    thirdPartyAccList: GetThirdPartyAccOutput = new GetThirdPartyAccOutput();
    currencies: CurrencyDto[] = [];
    bankAccounts: ThirdPartyAccListDto[] = [];
    banks: GetBankOutput = new GetBankOutput();
    depositId: number;

    constructor(inject: Injector,
        private _depositService: DepositServiceProxy,
        private _personService: PersonServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.depositId = +this.route.snapshot.queryParamMap.get('depositId');

        this._depositService.getDeposit(this.depositId || 0).subscribe(result => {
            this.deposit = result;
            this.getBankList();
            this.getBankAccountsList(this.deposit.bankId);
            this.getCurrencyList();
        });
    }

    /** 
     *  Lista bancilor
     *  */
    getBankList() {
        this._personService.bankList().subscribe(result => {
            this.banks = result;
        });
    }

    /**
     * Lista conturilor corespunzatoarea bancii selectate
     * @param bankId
     */
    getBankAccountsList(bankId: number) {
        this._depositService.getBankAccountsByBankId(bankId).subscribe(result => {
            this.bankAccounts = result;
        });
    }

    /**
     * Lista monedelor
     * */
    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    /**
     * Setez moneda in functie de moneda contului selectat
     * @param bankAccountId
     */
    getAccountCurrency(bankAccountId: number) {
        this._personService.getBankAccById(bankAccountId).subscribe(result => {
            this.deposit.currencyId = result.currencyId;
        });
    }

    saveDeposit() {
        this._depositService.saveDeposit(this.deposit).subscribe(() => {
            abp.notify.info("DepositAddedMessage");
            this.router.navigate(['/app/economic/dispositions/depositList']);
        });
    }

    backToList() {
        this.router.navigate(['/app/economic/dispositions/depositList']);
    }
}