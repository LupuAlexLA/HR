import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { RepConfigEditDto, ReportConfigServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './reportConfigEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ReportConfigEditComponent extends AppComponentBase implements OnInit {

    reportEdit: RepConfigEditDto = new RepConfigEditDto();
    repConfigId: number;

    constructor(inject: Injector,
        private _repConfigService: ReportConfigServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.repConfigId = +this.route.snapshot.queryParamMap.get('repConfigId');

        this._repConfigService.editRep(this.repConfigId || 0).subscribe(result => {
            this.reportEdit = result;
        });
    }

    backToRepList() {
        this.router.navigate(['/app/conta/nomenclatoare/configRapoarte/rapConfigList']);

    }

    save() {
        this._repConfigService.saveReport(this.reportEdit).subscribe(() => {
            abp.notify.success("SuccessfullyAddedMessage");
            this.backToRepList();
        });
    }
}