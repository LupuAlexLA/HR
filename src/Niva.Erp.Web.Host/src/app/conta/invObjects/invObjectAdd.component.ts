import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetInvObjectOutput } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectAdd.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectAddComponent extends AppComponentBase implements OnInit {

    invObjectList: GetInvObjectOutput;

    constructor(inject: Injector,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (!this.isGranted('Conta.ObInventar.Intrari.Modificare')) {
            this.router.navigate(['app/home']);
        }
    }
}