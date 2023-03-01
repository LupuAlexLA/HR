import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './preincomesInit.component.html',
    animations: [appModuleAnimation()]
})
export class PreincomesInitComponent extends AppComponentBase implements OnInit {

    constructor(inject: Injector,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.router.navigate(['/app/conta/prepayments/prepaymentsList'], { queryParams: { prepaymentType: 1 } });
    }
}