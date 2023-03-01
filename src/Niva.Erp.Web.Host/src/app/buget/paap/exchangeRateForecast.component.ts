import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ExchangeRateForecastDto, ExchangeRateForecastServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './exchangeRateForecast.component.html',
    animations: [appModuleAnimation()],

})
/** exchangeRateForecast component*/
export class ExchangeRateForecastComponent extends AppComponentBase implements OnInit{
    /** exchangeRateForecast ctor */
    exchangeRateList: ExchangeRateForecastDto[] = [];
    year: number;
    years: number[] = [];

    constructor(injector: Injector,
        private _exchangeRateService: ExchangeRateForecastServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('Buget.CursValutarEstimat.Acces')) {
            this.getExchangeYearList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getExchangeRateList(year) {
        sessionStorage.setItem('yearExchangeRate', JSON.stringify(this.year));
        this._exchangeRateService.exchangeRateForecastDtoListYear(year).subscribe(result => {
            this.exchangeRateList = result;
        });
    }

    changeYear() {
        this.getExchangeRateList(this.year);
    }

    getExchangeYearList() {
        this._exchangeRateService.getExchangeRateYearList().subscribe(result => {
            this.years = result;
            if (sessionStorage.getItem('yearExchangeRate')) {
                this.year = JSON.parse(sessionStorage.getItem('yearExchangeRate'));
            }
            else {
                
                this.year = this.years[this.years.length - 1];
            }
            
            this.getExchangeRateList(this.year);
        });
    }

    delete(Id: number) {
        abp.message.confirm("Cursul valutar estimat va fi sters. Sigur?",
            null,
            (response: boolean) => {
                this._exchangeRateService.deleteExchangeRateForecast(Id).subscribe(() => {
                    this.getExchangeRateList(this.year);
                    abp.notify.success(this.l('DeleteMessage'));
                });
            });
    }

    getExchangeRateCount() {
        if (this.exchangeRateList.length > 0) {
            return this.exchangeRateList.length;
        } else {
            return 0;
        }
    }
}