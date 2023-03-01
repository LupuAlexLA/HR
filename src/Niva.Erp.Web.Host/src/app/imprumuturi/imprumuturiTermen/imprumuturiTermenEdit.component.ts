import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ImprumuturiTermenDto, ImprumuturiTermenServiceProxy, ImprumuturiTermenEditDto } from '../../../shared/service-proxies/service-proxies';

import { AppComponent } from '../../app.component';

@Component({
    templateUrl: './imprumuturiTermenEdit.component.html',
    animations: [appModuleAnimation()]
})
/** imprumuturiTermenEdit component*/
export class ImprumuturiTermenEditComponent extends AppComponentBase implements OnInit{
    /** imprumuturiTermenEdit ctor */
    imprumuturiTermen: ImprumuturiTermenEditDto = new ImprumuturiTermenEditDto();
    imprumuturiTermenId: any;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumuturiTermenService: ImprumuturiTermenServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.imprumuturiTermenId = + this.route.snapshot.queryParamMap.get('imprumuturiTermenId');

        if (this.imprumuturiTermenId !== 0) {
            this._imprumuturiTermenService.getTermenId(this.imprumuturiTermenId).subscribe(result => {
                this.imprumuturiTermen = result;
            });
        }
    }

    saveTermen() {
        this._imprumuturiTermenService.saveTermen(this.imprumuturiTermen).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/imprumuturi/imprumuturiTermen']);
        });
    }
}