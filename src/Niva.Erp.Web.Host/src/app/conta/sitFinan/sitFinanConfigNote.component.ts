import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SitFinanConfigServiceProxy, SitFinanRapConfigNoteDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './sitFinanConfigNote.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanConfigNoteComponent extends AppComponentBase implements OnInit {

    sitFinanId: number;
    reportId: number;
    form: SitFinanRapConfigNoteDto = new SitFinanRapConfigNoteDto();

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _sitFinanConfigService: SitFinanConfigServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.sitFinanId = +this.route.snapshot.queryParamMap.get('sitFinanId');
        this.reportId = +this.route.snapshot.queryParamMap.get('reportId');

        this._sitFinanConfigService.sitFinanNoteInit(this.sitFinanId, this.reportId).subscribe(result => {
            this.form = result;
        });

    }

    sitFinanNoteSave() {
        this._sitFinanConfigService.sitFinanNoteSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info('AddUpdateMessage');
        });
    }
}