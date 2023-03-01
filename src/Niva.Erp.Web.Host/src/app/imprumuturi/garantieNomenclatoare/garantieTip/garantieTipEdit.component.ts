import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { GarantieTipEditDto, GarantieTipServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './garantieTipEdit.component.html',
    animations: [appModuleAnimation()],

})
/** garantieTipEdit component*/
export class GarantieTipEditComponent extends AppComponentBase implements OnInit{
    /** garantieTipEdit ctor */
    garantieTip: GarantieTipEditDto = new GarantieTipEditDto();
    garantieTipId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _garantieTipService: GarantieTipServiceProxy) {
        super(injector);
    }
    ngOnInit() {
        this.garantieTipId = + this.route.snapshot.queryParamMap.get('garantieTipId');

        if (this.garantieTipId !== 0) {
            this._garantieTipService.getGarantieTipId(this.garantieTipId).subscribe(result => {
                this.garantieTip = result;
            });
        }
    }

    saveTip() {
        this._garantieTipService.saveGarantieTip(this.garantieTip).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/imprumuturi/garantieTip']);
        });
    }
}