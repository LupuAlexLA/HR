import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { GarantieCeGaranteazaEditDto, GarantieCeGaranteazaServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './garantieCeGaranteazaEdit.component.html',
    animations: [appModuleAnimation()],

})
/** garantieCeGaranteazaEdit component*/
export class GarantieCeGaranteazaEditComponent extends AppComponentBase implements OnInit{
    /** garantieCeGaranteazaEdit ctor */
    garantieCeGaranteaza: GarantieCeGaranteazaEditDto = new GarantieCeGaranteazaEditDto();
    garantieCeGaranteazaId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _garantieCeGaranteaza: GarantieCeGaranteazaServiceProxy) {
        super(injector);
    }
    ngOnInit(): void {
        this.garantieCeGaranteazaId = + this.route.snapshot.queryParamMap.get('garantieCeGaranteazaId');

        if (this.garantieCeGaranteazaId !== 0) {
            this._garantieCeGaranteaza.getGarantieCeGaranteazaId(this.garantieCeGaranteazaId).subscribe(result => {
                this.garantieCeGaranteaza = result;
            });
        }
    }

    saveGarantieCeGaranteaza() {
        this._garantieCeGaranteaza.saveGarantieCeGaranteaza(this.garantieCeGaranteaza).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/imprumuturi/garantieCeGaranteaza']);
        });
    }
}