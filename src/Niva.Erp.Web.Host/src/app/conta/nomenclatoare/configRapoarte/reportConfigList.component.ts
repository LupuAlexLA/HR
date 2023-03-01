import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { RepConfigInitDto, ReportConfigServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './reportConfigList.component.html',
    animations: [appModuleAnimation()]
})
export class ReportConfigListComponent extends AppComponentBase implements OnInit {

    reportList: RepConfigInitDto[] = [];

    constructor(inject: Injector,
        private _repConfigService: ReportConfigServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.ConfigRapoarte.Acces')) {
            this.getReportList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }


    getReportList() {
        this._repConfigService.reportInitList().subscribe(result => {
            this.reportList = result;
        });
    }

    repConfigDelete(repId: number) {
        abp.message.confirm(
            'Raportarea va fi stearsa. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._repConfigService.delete(repId).subscribe(result => {
                        this.getReportList();
                        abp.notify.info('DeleteMessage');
                    });
                }
            }
        );
    }
}