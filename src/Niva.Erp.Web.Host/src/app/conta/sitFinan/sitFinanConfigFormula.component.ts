import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { EnumServiceProxy, EnumTypeDto, SitFinanConfigServiceProxy, SitFinanFormuleForm } from '../../../shared/service-proxies/service-proxies';
@Component({
    templateUrl: './sitFinanConfigFormula.component.html',
    animations: [appModuleAnimation()]
})

export class SitFinanConfigFormulaComponent extends AppComponentBase implements OnInit {

    sitFinanId: number;
    reportId: number;
    form: SitFinanFormuleForm = new SitFinanFormuleForm();
    sitFinanRowModCalcList: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _sitFinanConfigService: SitFinanConfigServiceProxy,
        private _EnumService: EnumServiceProxy,    ) {
        super(inject);
    }

    ngOnInit() {
        this.sitFinanId = +this.route.snapshot.queryParamMap.get('sitFinanId');
        this.reportId = +this.route.snapshot.queryParamMap.get('reportId');
        this._EnumService.sitFinanRowModCalc().subscribe(result => {
            this.sitFinanRowModCalcList = result;
        });
        this._sitFinanConfigService.sitFinanFormulaInit(this.sitFinanId, this.reportId).subscribe(result => {
            this.form = result;
        });
    }

    sitFinanColSave() {
        this._sitFinanConfigService.sitFinanColSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info('AddUpdateMessage');
        });
    }

    sitFinanFormulaDelRow(index: number) {
        this.form.configFormulaList.splice(index, 1);
    }

    sitFinanFormulaAddRow() {
        this._sitFinanConfigService.sitFinanFormulaAddRow(this.form).subscribe(result => {
            this.form = result;
        });
    }

    sitFinanFormulaSave() {
        this._sitFinanConfigService.sitFinanFormulaSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info('AddUpdateMessage');
        });
    }
}