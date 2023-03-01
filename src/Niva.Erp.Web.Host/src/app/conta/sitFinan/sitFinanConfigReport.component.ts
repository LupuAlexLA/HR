import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { EnumServiceProxy, EnumTypeDto, SitFinanConfigServiceProxy, SitFinanReportForm } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './sitFinanConfigReport.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanConfigReportComponent extends AppComponentBase implements OnInit {

    form: SitFinanReportForm = new SitFinanReportForm();
    sitFinanRowModCalcList: EnumTypeDto[] = [];
    sitFinanId: number;

    constructor(inject: Injector,
        private _sitFinanConfigService: SitFinanConfigServiceProxy,
        
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.sitFinanId = +this.route.snapshot.queryParamMap.get('sitFinanId');
        
        this._sitFinanConfigService.sitFinanReportInit(this.sitFinanId).subscribe(result => {
            this.form = result;
        });
    }

    sitFinanReportSave(index: number) {
        console.log(this.form);
        this._sitFinanConfigService.sitFinanReportSave(index, this.form).subscribe(result => {
            this.form = result;
        });
    }

    
    sitFinanReportDelete(reportId: number) {
        abp.message.confirm(
            'Raportul va fi sters. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._sitFinanConfigService.sitFinanReportDelete(reportId, this.form)
                        .subscribe(result =>{
                            this.form = result;
                            abp.notify.info('DeleteMessage');
                        });
                }
            }
        );
    }
}