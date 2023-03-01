import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetInvObjOperDocTypeOutput, InvObjectOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomOperDocType.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomOperDocTypeComponent extends AppComponentBase implements OnInit {

    operDocTypeList: GetInvObjOperDocTypeOutput = new GetInvObjOperDocTypeOutput();

    constructor(inject: Injector,
        private _invObjOperDocTypeService: InvObjectOperDocTypeServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getInvObjectOperDocTypeList();
    }

    getInvObjectOperDocTypeList() {
        this._invObjOperDocTypeService.invObjOperDocTypeList().subscribe(result => {
            this.operDocTypeList = result;
        });
    }

    getOperDocTypeListCount() {
        if (this.operDocTypeList.getInvObjOperDocType == null) {
            return 0;
        } else {
            return this.operDocTypeList.getInvObjOperDocType.length;
        }
    }

    deleteOperDocType(operDocTypeId: number) {
        abp.message.confirm("Documentul va fi sters. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invObjOperDocTypeService.deleteInvObjOperDocType(operDocTypeId).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.getInvObjectOperDocTypeList();
                    });
                }
            });
    }
}