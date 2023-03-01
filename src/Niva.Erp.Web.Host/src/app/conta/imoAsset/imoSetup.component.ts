import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoAssetSetupDto, ImoAssetSetupServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoSetup.component.html',
    animations: [appModuleAnimation()]
})
export class ImoSetupComponent extends AppComponentBase implements OnInit {

    setup: ImoAssetSetupDto;

    constructor(inject: Injector,
        private _imoAssetSetupService: ImoAssetSetupServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Administrare.MF.Setup.Acces')) {
            this._imoAssetSetupService.initForm().subscribe(result => {
                this.setup = result;
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    saveSetup() {
        this._imoAssetSetupService.saveSetup(this.setup).subscribe(result => {
            abp.notify.info(this.l('AddMessage'));
            this.setup = result;
        });
    }
}