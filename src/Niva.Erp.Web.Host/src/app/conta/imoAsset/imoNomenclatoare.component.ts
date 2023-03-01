import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './imoNomenclatoare.component.html',
    animations: [appModuleAnimation()]
})
export class ImoNomenclatoareComponent extends AppComponentBase {

    constructor(inject: Injector,) {
        super(inject);
    }
}