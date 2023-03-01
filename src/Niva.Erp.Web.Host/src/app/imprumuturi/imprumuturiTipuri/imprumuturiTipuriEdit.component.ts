import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ImprumuturiTipuriEditDto, ImprumuturiTipuriServiceProxy } from '../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './imprumuturiTipuriEdit.component.html',
    animations: [appModuleAnimation()]
})
/** imprumuturiTermenEdit component*/
export class ImprumuturiTipuriEditComponent extends AppComponentBase implements OnInit{
    /** imprumuturiTermenEdit ctor */
    imprumuturiTip: ImprumuturiTipuriEditDto = new ImprumuturiTipuriEditDto();
    imprumuturiTipId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumuturiTipuriService: ImprumuturiTipuriServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.imprumuturiTipId = + this.route.snapshot.queryParamMap.get('imprumuturiTipId');

        if (this.imprumuturiTipId !== 0) {
            this._imprumuturiTipuriService.getTipId(this.imprumuturiTipId).subscribe(result => {
                this.imprumuturiTip = result;
            });
        }
    }

    saveTip() {
        this._imprumuturiTipuriService.saveTip(this.imprumuturiTip).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/imprumuturi/imprumuturiTipuri']);
        });
    }
}