import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { result } from 'lodash';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { EnumServiceProxy, EnumTypeDto, PrepaymentsDecDeprecSetupDto, PrepaymentsDurationSetupDto, PrepaymentsServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsSetup.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsSetupComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    setup: PrepaymentsDurationSetupDto = new PrepaymentsDurationSetupDto();
    durationCalcType: EnumTypeDto[] = [];
    decDeprec: PrepaymentsDecDeprecSetupDto = new PrepaymentsDecDeprecSetupDto();

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentsService: PrepaymentsServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router    ) {
        super(inject);
    }
    
    ngOnInit() {
        if (this.isGranted('Administrare.CheltAvans.Setup.Acces') || this.isGranted('Administrare.VenitAvans.Setup.Acces')) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentsService.durationSetupDetails(this.prepaymentType).subscribe(result => {
                this.setup = result;
            });

            this._prepaymentsService.decDeprecSetupDetails(this.prepaymentType).subscribe(result => {
                this.decDeprec = result;
            });

            this.prepaymentDurationCalcList();
        }
        else {
            this.router.navigate(['/app/home']);
        }

    }

    prepaymentDurationCalcList() {
        this._enumService.prepaymentDurationCalcList().subscribe(result => {
            this.durationCalcType = result;
        });
    }

    saveSetup() {
        this._prepaymentsService.durationSetupSave(this.setup).subscribe(result => {
            abp.notify.info('Modificare OK');
            this.setup = result;
        });
    }

    decDeprecSetupSave() {
        this._prepaymentsService.decDeprecSetupSave(this.decDeprec).subscribe(result => {
            abp.notify.info('Modificare OK');
            this.decDeprec = result;
        });
    }
}