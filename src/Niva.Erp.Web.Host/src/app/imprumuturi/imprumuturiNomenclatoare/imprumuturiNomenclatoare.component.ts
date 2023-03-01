import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './imprumuturiNomenclatoare.component.html',
    animations: [appModuleAnimation()]
})
/** imprumuturiNomenclatoare component*/
export class ImprumuturiNomenclatoareComponent extends AppComponentBase {
    /** imprumuturiNomenclatoare ctor */
    constructor(inject: Injector,) {
        super(inject);
    }
}