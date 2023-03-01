import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { ReportConfigFormulaForm, ReportConfigServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './repConfigFormula.component.html',
    animations: [appModuleAnimation()]
})
export class RepConfigFormulaComponent extends AppComponentBase implements OnInit {

    repConfigId: number;
    reportId: number;
    form: ReportConfigFormulaForm = new ReportConfigFormulaForm();

    constructor(inject: Injector,
        private _repConfigService: ReportConfigServiceProxy,
        private route: ActivatedRoute    ) {
        super(inject);
    }

    ngOnInit() {
        this.repConfigId = + this.route.snapshot.queryParamMap.get('repConfigId');
        this.reportId = + this.route.snapshot.queryParamMap.get('reportId');
        this._repConfigService.formulaInit(this.repConfigId, this.reportId).subscribe(result => {
            this.form = result;
        });
    }

    repFormulaDelRow(index: number) {
        this.form.configFormulaList.splice(index, 1);
    }

    repFormulaAddRow() {
        this._repConfigService.repFormulaAddRow(this.form).subscribe(result => {
            this.form = result;
        });
    }

    repFormulaSave() {
        this._repConfigService.repFormulaSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.success("AddUpdateMessage");
        });
    }


}