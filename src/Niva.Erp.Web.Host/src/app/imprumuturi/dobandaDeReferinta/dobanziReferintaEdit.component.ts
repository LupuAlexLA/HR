import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DobanziReferintaEditDto, DobanziReferintaServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './dobanziReferintaEdit.component.html',
    animations: [appModuleAnimation()],
})
/** dobanziReferintaEdit component*/
export class DobanziReferintaEditComponent extends AppComponentBase implements OnInit {
    /** dobanziReferintaEdit ctor */
    dobanziReferinta: DobanziReferintaEditDto = new DobanziReferintaEditDto();
    dobanziReferintaId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _dobanziReferintaService: DobanziReferintaServiceProxy) {
        super(injector);
    }
    ngOnInit(): void {
        this.dobanziReferintaId = + this.route.snapshot.queryParamMap.get('dobanziReferintaId');

        if (this.dobanziReferintaId !== 0) {
            this._dobanziReferintaService.getDobandaReferintaId(this.dobanziReferintaId).subscribe(result => {
                this.dobanziReferinta = result;
            });
        }
    }

    saveDobanda() {
        this._dobanziReferintaService.saveDobandaReferinta(this.dobanziReferinta).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/imprumuturi/dobandaDeReferinta']);
        });
    }
}