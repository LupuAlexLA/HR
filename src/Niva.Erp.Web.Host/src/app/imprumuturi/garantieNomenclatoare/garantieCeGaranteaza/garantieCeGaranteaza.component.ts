import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { GarantieCeGaranteazaDto, GarantieCeGaranteazaServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './garantieCeGaranteaza.component.html',
    animations: [appModuleAnimation()],
})
/** garantieCeGaranteaza component*/
export class GarantieCeGaranteazaComponent extends AppComponentBase implements OnInit {
    /** garantieCeGaranteaza ctor */
    garantieCeGaranteazaList: GarantieCeGaranteazaDto[] = [];
    constructor(injector: Injector,
        private _garantieService: GarantieCeGaranteazaServiceProxy ) {
        super(injector);
    }
    ngOnInit(): void {
        this.getGarantieCeGaranteazaList()
    }

    getGarantieCeGaranteazaList() {
        this._garantieService.garantieCeGaranteazaList().subscribe(result => {
            this.garantieCeGaranteazaList = result;
        });
    }

    delete(garantieCeGaranteazaId: number) {
        this._garantieService.deleteGarantieCeGaranteaza(garantieCeGaranteazaId).subscribe(() => {
            this.getGarantieCeGaranteazaList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getGarantieCeGaranteazaCount() {
        if (this.garantieCeGaranteazaList.length > 0) {
            return this.garantieCeGaranteazaList.length;
        } else {
            return 0;
        }
    }
}