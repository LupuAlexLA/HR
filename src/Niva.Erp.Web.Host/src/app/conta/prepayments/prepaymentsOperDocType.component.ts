import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetPrepaymentsOperDocTypeOutput, PrepaymentsOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsOperDocType.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsOperDocTypeComponent extends AppComponentBase implements OnInit {

    operDocTypeList: GetPrepaymentsOperDocTypeOutput;

    constructor(inject: Injector,
        private _prepaymentsOperDocTypeService: PrepaymentsOperDocTypeServiceProxy,
        private router: Router       ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Administrare.CheltAvans.TipuriDoc.Acces') || this.isGranted('Administrare.VenitAvans.TipuriDoc.Acces')) {
            this.operDocTypeFnc();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    operDocTypeFnc() {
        this._prepaymentsOperDocTypeService.operDocTypeList().subscribe(result => {
            this.operDocTypeList = result;
        });
    }

    getOperDocTypeListCount() {
        if (this.operDocTypeList?.getPrepaymentsOperDocType == null) {
            return 0;
        } else {
            return this.operDocTypeList.getPrepaymentsOperDocType.length;
        }
    }

    deleteOperDocType(operDocTypeId: number) {
        abp.message.confirm(
            'Documentul va fi sters. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._prepaymentsOperDocTypeService.deleteOperDocType(operDocTypeId).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.operDocTypeFnc();
                    });
                }
            }
        )
    }
}