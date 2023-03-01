import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ActivityTypeServiceProxy, BankAccountDto, BankListDto, CurrencyDto, CurrencyServiceProxy, EnumServiceProxy, EnumTypeDto, ExchangeEditDto, ExchangeServiceProxy, PersonServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './exchangeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ExchangeEditComponent extends AppComponentBase implements OnInit {

    exchange: ExchangeEditDto = new ExchangeEditDto();
    currencies: CurrencyDto[] = [];
    activityTypeList: EnumTypeDto[] = [];
    exchangeOperTypeList: EnumTypeDto[] = [];
    exchangeTypeList: EnumTypeDto[] = [];
    bankAccountsLeiList: BankAccountDto[] = [];
    bankAccountsValutaList: BankAccountDto[] = [];
    bankLeiList: BankListDto[] = [];
    bankValutaList: BankListDto[] = [];
    exchangeId: number;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _exchangeService: ExchangeServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.exchangeId = +this.route.snapshot.queryParamMap.get('exchangeId');

        this.isLoading = true;
        this._exchangeService.getExchangeById(this.exchangeId).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.exchange = result;

            this.getBanksValuta();
            this.getBanksLei();
            this.getCurrencies();
            this.getActivityType();
            this.getExchangeOperType();
            this.getExchangeType();
        });

    }

    getCurrencies() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    getActivityType() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypeList = result;
        });
    }

    getExchangeOperType() {
        this._enumService.exchangeOperTypeList().subscribe(result => {
            this.exchangeOperTypeList = result;
        });
    }

    getExchangeType() {
        this._enumService.exchangeTypeList().subscribe(result => {
            this.exchangeTypeList = result;
        });
    }

    getBanksLei() {
        this._personService.bankForExchangeLei().subscribe(result => {
            this.bankLeiList = result;
            if (this.exchange.bankLeiId !== null) {
                this.getBankAccountsLei();
            }
        });

    }

    getBankAccountsLei() {
        this._personService.bankAccountsForExchangeLei(this.exchange.bankLeiId).subscribe(result => {
            this.bankAccountsLeiList = result;
        });
    }

    getBanksValuta() {
        this._personService.bankForExchangeValuta(this.exchange.currencyId).subscribe(result => {
            this.bankValutaList = result;

            if (this.exchange.bankValutaId !== null) {
                this.getBankAccountsValuta();
            }
        });
    
    }

    getBankAccountsValuta() {
        this._personService.bankAccountsForExchangeValuta(this.exchange.bankValutaId, this.exchange.currencyId).subscribe(result => {
            this.bankAccountsValutaList = result;
        });
    }

    // afisez label pentru moneda dupa selectarea Tipului de operatie
    changeOperTypeCurrencyCode() {
      
        if (this.exchange.exchangeOperType == 1) { //CumparValuta
            this.exchange.currencyCode = 'RON';
            this.exchange.localCurrencyCode = '';
        } else {
            this.exchange.localCurrencyCode = 'RON';
            this.exchange.currencyCode = '';
        }

        this.resetValues();
    }

    resetValues() {
        this.exchange.value = 0;
        this.exchange.exchangeRate = 0;
        this.exchange.exchangedValue = 0;
        this.exchange.currencyId = null;
        this.exchange.bankLeiId = null;
        this.exchange.bankValutaId = null;
        this.exchange.bankAccountLeiId = null;
        this.exchange.bankAccountValutaId = null;
        this.exchange.operationType = null;
        this.exchange.activityTypeId = null;
    }

    changeCurrencyCode() {
        if (this.exchange.exchangeOperType == 1) { // CumparValuta
            this.exchange.localCurrencyCode = this.currencies.find(f => f.id == this.exchange.currencyId).name;
        } else { // CumparLei
            this.exchange.currencyCode = this.currencies.find(f => f.id == this.exchange.currencyId).name;
        }
        this.getBanksValuta();
    }

    exchangeValue() {
        if (this.exchange.value !== 0 && (this.exchange.exchangeRate !== 0 || this.exchange.exchangeRate !== null)) {
            this._exchangeService.calculateExchangeValue(this.exchange.value, this.exchange.exchangeRate, this.exchange.exchangeOperType).subscribe(result => {
                this.exchange.exchangedValue = result;
            });
        }
    }

    save() {
        this._exchangeService.saveExchange(this.exchange).subscribe(result => {
            abp.notify.info('Schimbul valutar a fost inregistrat');
            this.router.navigate(['/app/economic/exchange/exchangeList']);
        });
    }
}