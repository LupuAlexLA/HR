import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';

@Component({
    templateUrl: './prepaymentsAdd.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsAddComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');
        if (this.prepaymentType === 0 && this.isGranted('Conta.CheltAvans.Chelt.Modificare')) {
        } else if (this.prepaymentType === 1 && this.isGranted("Conta.VenitAvans.Venituri.Modificare")) {
        } else {
            this.router.navigate(['app/home']);
        }
    }
}