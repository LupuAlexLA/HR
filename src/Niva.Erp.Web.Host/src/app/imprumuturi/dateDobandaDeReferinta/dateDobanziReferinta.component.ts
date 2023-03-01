import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DateDobanziReferintaServiceProxy, DateDobanziReferintaDto, ImprumutServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './dateDobanziReferinta.component.html',
    animations: [appModuleAnimation()]
})
/** dateDobanziReferinta component*/
export class DateDobanziReferintaComponent extends AppComponentBase implements OnInit {
    dobanziReferintaId: number;
    dobanziReferinta: DateDobanziReferintaDto[] = [];
    /** dateDobanziReferinta ctor */
    constructor(injector: Injector,
        private route: ActivatedRoute,
        private _dobRefService: DateDobanziReferintaServiceProxy,
        private _imprumutService: ImprumutServiceProxy    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.dobanziReferintaId = + this.route.snapshot.queryParamMap.get('dobanziReferintaId');
        this.getDobanziReferintaListId(this.dobanziReferintaId);
    }

    
    getDobanziReferintaListId(dobanziReferintaId: number) {
        this._dobRefService.dateDobanziReferintaListId(dobanziReferintaId).subscribe(result => this.dobanziReferinta = result);
    }

    getDobanziReferintaCount() {
        if (this.dobanziReferinta.length > 0) {
            return this.dobanziReferinta.length;
        } else {
            return 0;
        }
    }
    updateRoborImprumuturi() {
        this._imprumutService.updateRateVariabile().subscribe();
        abp.notify.success(this.l('Imprumuturi Actualizate'));
    }

    delete(datedobanziReferintaId: number) {
        this._dobRefService.deleteDateDobanziReferinta(datedobanziReferintaId).subscribe(() => {

            this.getDobanziReferintaListId(this.dobanziReferintaId);

            abp.notify.success(this.l('DeleteMessage'));
        })
    }
}