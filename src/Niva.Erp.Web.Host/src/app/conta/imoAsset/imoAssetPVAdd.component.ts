import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './imoAssetPVAdd.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetPVAddComponent extends AppComponentBase{

    constructor(inject: Injector,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Intrari.Modificare') && this.isGranted('Conta.MF.Intrari.Acces')) {

        }
        else {
            this.router.navigate(['/app/home']);
        }
    }
}