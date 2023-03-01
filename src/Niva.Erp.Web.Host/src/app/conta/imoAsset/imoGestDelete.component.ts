import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoGestDelListDto, ImoGestServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoGestDelete.component.html',
    animations: [appModuleAnimation()]
})
export class ImoGestDeleteComponent extends AppComponentBase implements OnInit {

    gestList: ImoGestDelListDto = new ImoGestDelListDto();

    constructor(inject: Injector,
        private _imoGestService: ImoGestServiceProxy) {
        super(inject);
    }

    ngOnInit(): void {
        this._imoGestService.initFormDel().subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._imoGestService.serchDateGest(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    deleteDateGest(deleteDate) {
        abp.message.confirm("Gestiunea va fi stearsa pentru data selectata. Sigur?",
            null,
            (result: boolean) => {
                if (result) {
                    this._imoGestService.deleteDateGest(deleteDate).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.searchGest();
                    });
                }
            });
    }


}