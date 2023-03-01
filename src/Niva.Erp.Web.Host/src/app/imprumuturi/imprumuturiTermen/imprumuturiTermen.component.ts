import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import {  ImprumuturiTermenDto, ImprumuturiTermenServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imprumuturiTermen.component.html',
    animations: [appModuleAnimation()]
})
/** ImprumuturiTermen component*/
export class ImprumuturiTermenComponent extends AppComponentBase implements OnInit  {
    imprumuturiTermenList: ImprumuturiTermenDto[] = [];

    constructor(injector: Injector,
        private _termenService: ImprumuturiTermenServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.getTermenList();
    }

    getTermenList() {
        this._termenService.imprumuturiTermenList().subscribe(result => {
            this.imprumuturiTermenList = result;
        });
    }

    delete(TermenId: number) {
        this._termenService.deleteTermen(TermenId).subscribe(() => {
            this.getTermenList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getTermenCount() {
        if (this.imprumuturiTermenList.length > 0) {
            return this.imprumuturiTermenList.length;
        } else {
            return 0;
        }
    }
}