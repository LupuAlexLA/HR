import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './preincomesGestInit.component.html',
    animations: [appModuleAnimation()]
})
export class PreincomesGestInitComponent extends AppComponentBase implements OnInit {

    constructor(inject: Injector,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.router.navigate(['/app/conta/prepayments/prepaymentsGestList'], { queryParams: { prepaymentType: 1 } });
    }
}