import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { AppComponentBase } from '../../shared/app-component-base';
import { AppConsts } from '../../shared/AppConsts';
import { CurrencyDto, CurrencyServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './registruCasa.component.html'
})
export class RegistruCasaComponent extends AppComponentBase implements OnInit {
    currencies: CurrencyDto[] = [];
    registruCasaParams: any = {};
    url: string;
    currentDate: Date;
    isDateRange = false;
    startDate: any;
    endDate: any;

    constructor(inject: Injector,
        private _currencyService: CurrencyServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Casierie.Rapoarte.Acces')) {
            this.currencyList();
            this.endDate = new Date();
            this.registruCasaParams.currencyId = 0;
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    currencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    showDateRange() {
        this.currentDate = new Date();
        if (this.isDateRange === false) {
            this.endDate = moment(this.currentDate);
        } else {
            this.endDate = moment(this.currentDate);
            this.startDate = moment(this.currentDate.setMonth(this.currentDate.getMonth() - 1));
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        if (this.isDateRange === true) {
            this.url += "RegistruCasaRangeReport";
            this.registruCasaParams.dataStart = moment(this.startDate).format('M.DD.YYYY');
            this.registruCasaParams.dataEnd = moment(this.endDate).format('M.DD.YYYY');
            this.url += '?' + this.ConvertToQueryStringParameters(this.registruCasaParams);
        } else {
            this.url += 'RegistruCasaReport';

            this.registruCasaParams.dataEnd = moment(this.endDate).format('M.DD.YYYY');

            this.url += '?' + this.ConvertToQueryStringParameters(this.registruCasaParams);
        }
        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}