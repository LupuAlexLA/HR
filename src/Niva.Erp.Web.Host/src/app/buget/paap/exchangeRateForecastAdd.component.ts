import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CurrencyListDto, ExchangeRateForecastEditDto, ExchangeRateForecastServiceProxy, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './exchangeRateForecastAdd.component.html',
    animations: [appModuleAnimation()],

})
/** exchangeRateForecastAdd component*/
export class ExchangeRateForecastAddComponent extends AppComponentBase implements OnInit {
    /** exchangeRateForecastAdd ctor */

    exchangeRateForecast: ExchangeRateForecastEditDto = new ExchangeRateForecastEditDto();
    currencyList: CurrencyListDto[] = [];
    exchangeRateForecastId: any;
    year: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _exchangeRateForecastService: ExchangeRateForecastServiceProxy,
        private _currencyService: PersonServiceProxy    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('Buget.CursValutarEstimat.Modificare')) {
            this.exchangeRateForecastId = + this.route.snapshot.queryParamMap.get('exchangeRateForecastId');
            this.year = + this.route.snapshot.queryParamMap.get('year');


            this._exchangeRateForecastService.getExchangeRateForecastId(this.exchangeRateForecastId).subscribe(result => {
                this.exchangeRateForecast = result;
                this.getCurrencyList();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getCurrencyList() {
        this._currencyService.currencyList().subscribe(result => {
            this.currencyList = result.getCurrency;
        });
    }

    saveExchangeRateForecast() {
        this._exchangeRateForecastService.saveExchangeRateForecast(this.exchangeRateForecast).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/buget/paap/exchangeRateForecast'] );
        });
    }
}