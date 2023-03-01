import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DobanziReferintaDto, DobanziReferintaServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './dobanziReferinta.component.html',
    animations: [appModuleAnimation()]
})
/** dobandaDeReferinta component*/
export class DobanziReferintaComponent extends AppComponentBase implements OnInit{
    /** dobandaDeReferinta ctor */
    dobanziReferintaList: DobanziReferintaDto[] = [];

    constructor(injector: Injector,
        private _dobRefService: DobanziReferintaServiceProxy) {
        super(injector);
    }
    ngOnInit(): void {
        this.getDobanziReferintaList();
    }

    getDobanziReferintaList() {
        this._dobRefService.dobanziReferintaList().subscribe(result => {
            this.dobanziReferintaList = result;
        });
    }

    delete(DobRefId: number) {
        this._dobRefService.deleteDobandaReferinta(DobRefId).subscribe(() => {
            this.getDobanziReferintaList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getDobanziReferintaCount() {
        if (this.dobanziReferintaList.length > 0) {
            return this.dobanziReferintaList.length;
        } else {
            return 0;
        }
    }
}