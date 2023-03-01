import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AutoOperationDto, AutoOperationServiceProxy, EnumServiceProxy, EnumTypeDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './autoOperations.component.html',
    animations: [appModuleAnimation()]
})
export class AutoOperationsComponent extends AppComponentBase implements OnInit {

    form: AutoOperationDto = new AutoOperationDto();
    autoOperType: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _autoOperationsService: AutoOperationServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.ContariAutomate.Acces')) {
            this._autoOperationsService.initForm().subscribe(result => {
                this.form = result;
                this.autoOperTypeSearch();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    autoOperTypeSearch() {
        this._enumService.autoOperationTypeList().subscribe(result => {
            this.autoOperType = result;
        });
    }

    searchAutoOper() {
        this._autoOperationsService.searchAutoOper(this.form).subscribe(result => {
            this.form = result;
            console.log(this.form);
        });
    }

    prepareAddHeader() {
        if (this.form.autoOperType === null) {
            abp.notify.error("Eroare", "Nu ati selectat categoria de obiecte!");
        } else {
            this.form.showSearchForm = false;
            this.form.showSummary = false;
            this.form.showResults = false;
            this.form.showCompute = true;
            this.form.showComputeDetails = false;
        }
    }

    showDetail(index: number) {
        this.form.summary[index].showDetail = true;
    }

    hideDetail(index: number) {
        this.form.summary[index].showDetail = false;
    }

    deleteOperations(detailId: number) {
        abp.message.confirm(
            'Notele contabile vor fi sterse. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._autoOperationsService.deleteOperations(detailId).subscribe(() => {
                        this.searchAutoOper();
                        abp.notify.info('OKStergere');
                    });
                }
            }
        );
    }

    showSearch() {
        this.form.showSearchForm = true;
        this.form.showSummary = false;
        this.form.showResults = false;
        this.form.showCompute = false;
        this.form.showComputeDetails = false;
    }

    prepareAdd() {
        this._autoOperationsService.prepareAdd(this.form).subscribe(result => {
            this.form = result;
        });
    }

    operationsAdd() {
        this._autoOperationsService.operationsAdd(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info('ContareAutomataOK');
        })
    }
}