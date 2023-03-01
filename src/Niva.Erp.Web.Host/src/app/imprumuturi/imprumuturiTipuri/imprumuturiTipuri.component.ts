import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ImprumuturiTipuriDto, ImprumuturiTipuriServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imprumuturiTipuri.component.html',
    animations: [appModuleAnimation()]
})
/** imprumuturiTipuri component*/
export class ImprumuturiTipuriComponent extends AppComponentBase implements OnInit {
    imprumuturiTipuriList: ImprumuturiTipuriDto[] = [];

    constructor(injector: Injector,
        private _tipService: ImprumuturiTipuriServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.getTipuriList();
    }

    getTipuriList() {
        this._tipService.imprumuturiTipuriList().subscribe(result => {
            this.imprumuturiTipuriList = result;
        });
    }

    delete(TipId: number) {
        this._tipService.deleteTip(TipId).subscribe(() => {
            this.getTipuriList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getTipuriCount() {
        if (this.imprumuturiTipuriList.length > 0) {
            return this.imprumuturiTipuriList.length;
        } else {
            return 0;
        }
    }
}