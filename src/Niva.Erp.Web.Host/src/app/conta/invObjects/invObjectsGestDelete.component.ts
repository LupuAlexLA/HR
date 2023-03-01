import { Component, Injector, OnInit } from '@angular/core';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvObjectDelListDto, InvObjectGestServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectsGestDelete.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectsGestDeleteComponent extends AppComponentBase implements OnInit {

    gestList: InvObjectDelListDto = new InvObjectDelListDto();

    constructor(inject: Injector,
        private _invObjectGestService: InvObjectGestServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this._invObjectGestService.initFormDel().subscribe(result => {
            this.gestList = result;
        });
    }

    searchGest() {
        this._invObjectGestService.searchGestDel(this.gestList).subscribe(result => {
            this.gestList = result;
        });
    }

    deleteDateGest(gestDate: Date) {
        abp.message.confirm("Gestiunea va fi stearsa pentru data selectata. Sigur?",
            null,
            (result: boolean) => {
                if (result) {
                    this._invObjectGestService.deleteGest(moment(gestDate)).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.searchGest();
                    });
                }
            });
    }
}