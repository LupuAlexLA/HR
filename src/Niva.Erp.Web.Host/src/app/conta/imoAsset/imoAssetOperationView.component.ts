import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoAssetOperEditDto, ImoAssetOperServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetOperationView.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetOperationViewComponent extends AppComponentBase implements OnInit {

    operationId: number;
    operation: ImoAssetOperEditDto = new ImoAssetOperEditDto();

    constructor(inject: Injector,
        private _imoAssetOperService: ImoAssetOperServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.operationId = +this.route.snapshot.queryParamMap.get('operationId');

        this._imoAssetOperService.viewOperation(this.operationId || 0).subscribe(result => {
            this.operation = result;
        });
    }
}