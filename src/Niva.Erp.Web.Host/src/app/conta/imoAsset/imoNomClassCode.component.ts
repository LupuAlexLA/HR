import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetImoAssetClassCodeOutput, ImoAssetClassCodeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoNomClassCode.component.html',
    animations: [appModuleAnimation()]
})
export class ImoNomClassCodeComponent extends AppComponentBase implements OnInit {

    classCodeList: GetImoAssetClassCodeOutput;

    constructor(inject: Injector,
        private _imoAssetClassCodeService: ImoAssetClassCodeServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.classCodeListFnc();
    }

    classCodeListFnc() {
        this._imoAssetClassCodeService.imoAssetClassCodeList().subscribe(result => {
            this.classCodeList = result;
        });
    }

    getImoAssetClassCodeListCount() {
        if (this.classCodeList?.getImoAssetClassCode == null) {
            return 0;
        } else {
           return this.classCodeList.getImoAssetClassCode.length;
        }
    }

    deleteClassCode(id: number) {
        abp.message.confirm(
            "Codul de clasificarre va fi sters. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._imoAssetClassCodeService.deleteClassCode(id).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.classCodeListFnc();
                    });
                }
            });
    }
}