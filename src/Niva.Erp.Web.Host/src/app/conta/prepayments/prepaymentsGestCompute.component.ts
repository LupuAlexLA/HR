import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PrepaymentBalanceServiceProxy, PrepaymentsBalanceComputeListDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsGestCompute.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsGestComputeComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    gestList: PrepaymentsBalanceComputeListDto = new PrepaymentsBalanceComputeListDto();

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentBalanceService: PrepaymentBalanceServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

        this._prepaymentBalanceService.initFormCompute(this.prepaymentType).subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._prepaymentBalanceService.serchComputeOper(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    computeDateGest() {
        this._prepaymentBalanceService.computeDateGest(this.gestList).subscribe(result => {
            abp.notify.info('ComputeMessage');
            this.gestList = result;
        });
    }
}