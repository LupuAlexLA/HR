import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { CurrencyDto, CurrencyServiceProxy, EnumServiceProxy, EnumTypeDto, ExchangeInitDto, ExchangeServiceProxy, GetCurrencyOutput } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './exchangeList.component.html',
    animations: [appModuleAnimation()]
})
export class ExchangeListComponent extends AppComponentBase implements OnInit {

    form: ExchangeInitDto = new ExchangeInitDto();
    exchangeOperTypeList: EnumTypeDto[] = [];
    currencies: CurrencyDto[] = [];

    constructor(inject: Injector,
        private _exchangeService: ExchangeServiceProxy,
        private _enumService: EnumServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private router: Router) {
        super(inject);
    }


    ngOnInit() {
        if (this.isGranted('Casierie.SchimbValutar.Acces')) {
            this.exchangeInit();
            this.getExchangeOperType();
            this.getCurrenciesList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    exchangeInit() {
        this._exchangeService.exchangeInit().subscribe(result => {
            // this.form = result;
            this.form.startDate = sessionStorage.getItem('startDate') ? moment(sessionStorage.getItem('startDate')) : result.startDate;
            this.form.endDate = sessionStorage.getItem('endDate') ? moment(sessionStorage.getItem('endDate')) : result.endDate;
            this.form.operationTypeId = JSON.parse(sessionStorage.getItem('operationTypeId')) ?? result.operationTypeId;
            this.form.currencyId = JSON.parse(sessionStorage.getItem('currencyId')) ?? result.currencyId;
            this.form.exchangeList = result.exchangeList;
         //   this.searchExchange();
        });
    }

    searchExchange() {
        this._exchangeService.exchangeList(this.form).subscribe(result => {
            this.form = result;
            sessionStorage.setItem('startDate', this.form.startDate.toString());
            sessionStorage.setItem('endDate', this.form.endDate.toString());
            sessionStorage.setItem('operationTypeId', JSON.stringify(this.form.operationTypeId));
            sessionStorage.setItem('currencyId', JSON.stringify(this.form.currencyId));
        });
    }

    getExchangeOperType() {
        this._enumService.exchangeOperTypeList().subscribe(result => {
            this.exchangeOperTypeList = result;
        });
    }

    getExchangeCount() {
        if (this.form.exchangeList?.length == null) {
            return 0;
        } else {
            return this.form.exchangeList?.length;
        }
    }

    getCurrenciesList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    delete(exchangeId: number) {
        abp.message.confirm(
            this.l('Schimbul valutar va fi sters. Sigur?'),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._exchangeService
                        .deleteExchange(exchangeId)
                        .subscribe(result => {
                            this.searchExchange();
                            abp.notify.info(this.l('Schimbul valutar a fost sters.'));
                        });
                }
            }
        )
    }

}