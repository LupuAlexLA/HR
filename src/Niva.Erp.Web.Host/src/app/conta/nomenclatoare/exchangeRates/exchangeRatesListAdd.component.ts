import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { CurrencyListDto, ExchangeRateDto, ExchangeRateForecastServiceProxy, PersonServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './exchangeRatesListAdd.component.html',
    animations: [appModuleAnimation()],
})
/** exchangeRateForecastAdd component*/
export class ExchangeRatesListAddComponent extends AppComponentBase implements OnInit {
    /** exchangeRateForecastAdd ctor */
    currencyList: CurrencyListDto[] = [];
    exchangeRate: ExchangeRateDto = new ExchangeRateDto();
    exchangeRateId;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _exchangeRateForecastService: ExchangeRateForecastServiceProxy,
        private _personService: PersonServiceProxy    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('General.CursValutar.Modificare')) {
            this.exchangeRateId = + this.route.snapshot.queryParamMap.get('exchangeRateId');

            if (this.exchangeRateId) {
                this._personService.exchangeRateId(this.exchangeRateId).subscribe(result => {
                    this.exchangeRate = result;
                    this.getCurrencyList();
                });
            }
            else {
                this.getCurrencyList();
            }
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getCurrencyList() {
        this._personService.currencyList().subscribe(result => {
            this.currencyList = result.getCurrency;
        });
    }

    saveExchangeRate() {
        this._personService.exchangeRateAdd(this.exchangeRate).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/conta/nomenclatoare/exchangeRates/exchangeRatesList'] );
        });
    }
}