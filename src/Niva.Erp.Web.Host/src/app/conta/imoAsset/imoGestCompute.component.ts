import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoGestComputeListDto, ImoGestServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoGestCompute.component.html',
    animations: [appModuleAnimation()]
})
export class ImoGestComputeComponent extends AppComponentBase implements OnInit {

    gestList: ImoGestComputeListDto = new ImoGestComputeListDto();

    constructor(inject: Injector,
        private _imoGestService: ImoGestServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this._imoGestService.initFormCompute().subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._imoGestService.serchComputeOper(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    computeDateGest() {
        this._imoGestService.computeDateGest(this.gestList).subscribe(result => {
            abp.notify.info('ComputeMessage');
            this.gestList = result;
        });
    }
}