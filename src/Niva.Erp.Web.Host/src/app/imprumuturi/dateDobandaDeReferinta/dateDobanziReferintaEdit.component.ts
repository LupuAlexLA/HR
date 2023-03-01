import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DateDobanziReferintaEditDto, DateDobanziReferintaServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './dateDobanziReferintaEdit.component.html',
    animations: [appModuleAnimation()]

})

export class DateDobanziReferintaEditComponent extends AppComponentBase implements OnInit {
    
    dateDobanziReferinta: DateDobanziReferintaEditDto = new DateDobanziReferintaEditDto();
    dobanziReferintaId: number; // new
    dateDobanziReferintaId: number; // edit

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _dateDobanziReferintaService: DateDobanziReferintaServiceProxy,
          ) {
        super(injector);
    }
    ngOnInit(): void {
        this.dobanziReferintaId = + this.route.snapshot.queryParamMap.get('dobanziReferintaId');
        this.dateDobanziReferintaId = + this.route.snapshot.queryParamMap.get('dateDobanziReferintaId');

        if (this.dobanziReferintaId !== 0) {
            this.dateDobanziReferinta.dobanziReferintaId = this.dobanziReferintaId;
        }

        if (this.dateDobanziReferintaId !== 0) {

            this._dateDobanziReferintaService.getDateDobanziReferintaId(this.dateDobanziReferintaId).subscribe(result => {
                this.dateDobanziReferinta = result;
            //    this.dobanziReferintaId = this.dateDobanziReferinta.dobanziReferintaId;
                
            });
        }
    }

    saveGarantie() {
        

        this._dateDobanziReferintaService.saveDateDobanziReferinta(this.dateDobanziReferinta).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));

            this.router.navigate(['/app/imprumuturi/dateDobandaDeReferinta'], { queryParams: { dobanziReferintaId: this.dateDobanziReferinta.dobanziReferintaId} });
        });
    }
}