import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './prepaymentsSetupInit.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsSetupInitComponent extends AppComponentBase implements OnInit {

    constructor(inject: Injector,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.router.navigate(['/app/conta/prepayments/prepaymentsSetup'], { queryParams: { prepaymentType: 0 } });
    }
}