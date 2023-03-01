import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './invObjectNomenclatoare.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomenclatoareComponent extends AppComponentBase implements OnInit {

    constructor(inject: Injector) {
        super(inject);
    }

    ngOnInit() {  
    }
}