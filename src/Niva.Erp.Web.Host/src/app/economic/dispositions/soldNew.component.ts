import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CurrencyDto, CurrencyServiceProxy, SoldInitialEditDto, SoldInitialServiceProxy, } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './soldNew.component.html',
    animations: [appModuleAnimation()]
})
export class SoldNewComponent extends AppComponentBase implements OnInit {

    currencies: CurrencyDto[] = [];
    sold: SoldInitialEditDto = new SoldInitialEditDto();
    soldId: number;

    constructor(inject: Injector,
        private router: Router,
        private route: ActivatedRoute,
        private _currencyService: CurrencyServiceProxy,
        private _soldInitialService: SoldInitialServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.soldId = +this.route.snapshot.queryParamMap.get('soldId');

        this._soldInitialService.getSold(this.soldId || 0).subscribe(result => {
            this.sold = result;

            this.getCurrencyList();
        });

    }

    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    saveSold() {
        this._soldInitialService.saveSold(this.sold).subscribe(() => {
            this.router.navigate(['/app/economic/dispositions/soldList']);
            abp.notify.info("SoldAddedMessage");
        });
    }

    backToList() {
        this.router.navigate(['/app/economic/dispositions/soldList']);
    }
}