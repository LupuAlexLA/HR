import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    
    templateUrl: './garantieNomenclatoare.component.html',
    animations: [appModuleAnimation()],

})
/** garantieNomenclatoare component*/
export class GarantieNomenclatoareComponent extends AppComponentBase  {
    /** garantieNomenclatoare ctor */
    constructor(inject: Injector,) {
        super(inject);
    }
}