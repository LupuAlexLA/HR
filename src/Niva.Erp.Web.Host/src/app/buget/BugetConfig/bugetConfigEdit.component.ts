import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BugetConfigEditDto, BugetConfigServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './bugetConfigEdit.component.html',
    animations: [appModuleAnimation()]

})
/** bugetConfigEdit component*/
export class BugetConfigEditComponent extends AppComponentBase implements OnInit{
    /** bugetConfigEdit ctor */

    bugetConfig: BugetConfigEditDto = new BugetConfigEditDto();
    bugetId: any;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumuturiTermenService: BugetConfigServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Configurare")) {
            this.bugetId = + this.route.snapshot.queryParamMap.get('bugetId');

            if (this.bugetId !== 0) {
                this._imprumuturiTermenService.getBugetConfigId(this.bugetId).subscribe(result => {
                    this.bugetConfig = result;
                });
            }
        } else {
            this.router.navigate(['app/home']);
        }
    }

    saveBuget() {
        this._imprumuturiTermenService.saveBugetConfig(this.bugetConfig).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/buget/bugetConfig/bugetConfigList']);
        });
    }

}