import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetImoAssetStorageOutput, ImoAssetStorageServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoNomStorage.component.html',
    animations: [appModuleAnimation()]
})
export class ImoNomStorageComponent extends AppComponentBase implements OnInit {

    storageList: GetImoAssetStorageOutput;

    constructor(inject: Injector,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.storageListFnc();
    }

    storageListFnc() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    getImoAssetStorageListCount() {
        if (this.storageList?.getImoAssetStorage == null) {
            return 0;
        } else {
            return this.storageList.getImoAssetStorage.length;
        }
    }

    deleteStorage(id: number) {
        abp.message.confirm("Gestiunea va fi stearsa. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._imoAssetStorageService.deleteStorage(id).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.storageListFnc();
                    });
                }
            });
    } 
}