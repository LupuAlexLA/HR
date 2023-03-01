import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PrepaymentsExitDto, PrepaymentsServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsExit.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsExitComponent extends AppComponentBase implements OnInit {

    prepaymentId: number;
    prepaymentType: number;
    oper: PrepaymentsExitDto = new PrepaymentsExitDto();
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentsService: PrepaymentsServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.prepaymentId = +this.route.snapshot.queryParamMap.get('prepaymentId');
        this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

        this._prepaymentsService.prepaymentsExitInit(this.prepaymentId, this.prepaymentType).subscribe(result => {
            this.oper = result;
        });
    }

    savePrepaymentExit() {
        this.isLoading = true;
        this._prepaymentsService.savePrepaymentExit(this.oper).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.oper = result;
            abp.notify.info("Operatiunea a fost inregistrata");
        });
    }
}