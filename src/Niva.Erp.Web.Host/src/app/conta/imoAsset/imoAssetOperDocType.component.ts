import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetImoAssetOperDocTypeOutput, ImoAssetOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetOperDocType.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetOperDocTypeComponent extends AppComponentBase implements OnInit {

    operDocTypeList: GetImoAssetOperDocTypeOutput;

    constructor(inject: Injector,
        private _imoAssetOperDocTypeService: ImoAssetOperDocTypeServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.operDocTypeFnc();
    }

    operDocTypeFnc() {
        this._imoAssetOperDocTypeService.operDocTypeList().subscribe(result => {
            this.operDocTypeList = result;
        });
    }

    getOperDocTypeListCount() {
        if (this.operDocTypeList?.getImoAssetOperDocType == null) {
            return 0;
        } else {
           return this.operDocTypeList?.getImoAssetOperDocType.length;
        }
    }

    deleteOperDocType(id: number) {
        abp.message.confirm("Documentul va fi sters. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._imoAssetOperDocTypeService.deleteOperDocType(id).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.operDocTypeFnc();
                    });
                }
            }
            )
    }
}