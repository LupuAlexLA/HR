import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { GarantieTipDto, GarantieTipServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './garantieTip.component.html',
    animations: [appModuleAnimation()]

})
/** garantieTip component*/
export class GarantieTipComponent extends AppComponentBase implements OnInit {
    /** garantieTip ctor */
    garantieTipList: GarantieTipDto[] = [];

    constructor(injector: Injector,
        private _garantieService: GarantieTipServiceProxy    ) {
        super(injector);
    }
    ngOnInit() {
        this.getGarantieTipList();
    }

    getGarantieTipList() {
        this._garantieService.garantieTipList().subscribe(result => {
            this.garantieTipList = result;
        });
    }

    delete(garantieTipId: number) {
        this._garantieService.deleteGarantieTip(garantieTipId).subscribe(() => {
            this.getGarantieTipList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getGarantieTipuriCount() {
        if (this.garantieTipList.length > 0) {
            return this.garantieTipList.length;
        } else {
            return 0;
        }
    }
}