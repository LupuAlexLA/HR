import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { ReportConfigForm, ReportConfigServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './rapConfig.component.html',
    animations: [appModuleAnimation()]
})
export class RapConfigComponent extends AppComponentBase implements OnInit {

    reportForm: ReportConfigForm = new ReportConfigForm();
    repConfigId: number;

    constructor(inject: Injector,
        private _repConfigService: ReportConfigServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.repConfigId = + this.route.snapshot.queryParamMap.get('repConfigId');

        this._repConfigService.initForm(this.repConfigId).subscribe(result => {
            this.reportForm = result;
        });
    }

    reportSave(index: number) {
        this._repConfigService.save(index, this.reportForm).subscribe(result => {
            this.reportForm = result;
            abp.notify.success("SaveMessage");
        });
    }

    reportDelete(reportId: number) {
        abp.message.confirm(
            'Raportul va fi sters. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._repConfigService.reportDelete(reportId, this.reportForm)
                        .subscribe(result => {
                            this.reportForm = result;
                            abp.notify.info('DeleteMessage');
                        });
                }
            }
        );
    }
}