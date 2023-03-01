import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';

@Component({
    templateUrl: './prepaymentsInit.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsInitComponent implements OnInit {

    constructor(private router: Router) {

    }

    ngOnInit() {
        this.router.navigate(['/app/conta/prepayments/prepaymentsList'], { queryParams: {prepaymentType: 0}});
    }

}