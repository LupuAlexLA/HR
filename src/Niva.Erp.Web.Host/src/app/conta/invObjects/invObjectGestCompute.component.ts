import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvObjectGestComputeListDto, InvObjectGestServiceProxy } from '../../../shared/service-proxies/service-proxies';
@Component({
    templateUrl: './invObjectGestCompute.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectGestComputeComponent extends AppComponentBase implements OnInit {

    gestList: InvObjectGestComputeListDto = new InvObjectGestComputeListDto();

    constructor(inject: Injector,
        private _invObjectGestService: InvObjectGestServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this._invObjectGestService.initFormCompute().subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._invObjectGestService.searchCompute(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    computeDateGest() {
        this._invObjectGestService.computeGest(this.gestList).subscribe(result => {
            abp.notify.info('ComputeMessage');
            this.gestList = result;
        });
    }
}