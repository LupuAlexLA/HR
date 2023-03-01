import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { OperationTypesListDto, OperationTypesServiceProxy } from '../../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './operationType.component.html',
    animations: [appModuleAnimation()]
})
export class OperationTypeComponent extends AppComponentBase implements OnInit {

    operTypesList: OperationTypesListDto[] = [];

    constructor(injector: Injector,
        private _operTypeService: OperationTypesServiceProxy,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.TipOperContab.Acces')) {
            this.getOperationTypeList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getOperationTypeList() {
        this._operTypeService.operTypesList().subscribe(result => {
            this.operTypesList = result;
        });
    }

    operTypeListCount() {
        if (this.operTypesList != null) {
            return this.operTypesList.length;
        } else {
            return 0;
        }
    }

    delete(operTypeId: number) {
        abp.message.confirm(
            this.l('Tipul operatiei contabile va fi sters. Sigur?', operTypeId),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._operTypeService.delete(operTypeId).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.getOperationTypeList();
                    });
                }
            }
        );
    }
}