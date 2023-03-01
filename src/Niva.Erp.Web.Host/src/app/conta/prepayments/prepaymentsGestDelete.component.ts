import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PrepaymentBalanceServiceProxy, PrepaymentsBalanceDelListDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsGestDelete.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsGestDeleteComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    gestList: PrepaymentsBalanceDelListDto = new PrepaymentsBalanceDelListDto();

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentBalanceService: PrepaymentBalanceServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

        this.initForm();
    }

    initForm() {
        this._prepaymentBalanceService.initFormDel(this.prepaymentType).subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._prepaymentBalanceService.serchDateGest(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    deleteDateGest(dateGest) {
        abp.message.confirm(
            'Gestiunea va fi stearsa pentru data selectata. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._prepaymentBalanceService.deleteDateGest(this.prepaymentType, dateGest).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.searchGest();
                    });
                }
            }
        );
    }
}