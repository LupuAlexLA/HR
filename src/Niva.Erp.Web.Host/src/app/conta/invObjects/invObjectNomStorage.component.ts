import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetInvObjectStorageOutput, InvObjectStorageServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomStorage.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomStorageComponent extends AppComponentBase implements OnInit {

    storageList: GetInvObjectStorageOutput = new GetInvObjectStorageOutput();

    constructor(inject: Injector,
        private _storageService: InvObjectStorageServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getStorageList();
    }

    getInvObjectStorageListCount() {
        if (this.storageList.getInvObjectStorage == null) {
            return 0;
        } else {
            return this.storageList.getInvObjectStorage.length;
        }
    }

    getStorageList() {
        this._storageService.invObjectStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    deleteStorage(storageId: number) {
        abp.message.confirm("Gestiunea va fi stearsa. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._storageService.deleteStorage(storageId).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.getStorageList();
                    });
                }
            });
    }
}