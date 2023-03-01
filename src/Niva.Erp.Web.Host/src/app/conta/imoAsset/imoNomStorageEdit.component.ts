import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoAssetStorageDto, ImoAssetStorageServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoNomStorageEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImoNomStorageEditComponent extends AppComponentBase implements OnInit {

    storage: ImoAssetStorageDto = new ImoAssetStorageDto();
    storageId: any;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.storageId = this.route.snapshot.queryParamMap.get('storageId');

        if (this.storageId !== null) {
            this._imoAssetStorageService.getStorageById(this.storageId).subscribe(result => {
                this.storage = result;
            });
        }
    }

    saveStorageFnc() {
        this.isLoading = true;
        this._imoAssetStorageService.saveStorage(this.storage).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                abp.notify.info(this.l('AddModifyMessage'));
            })
        ).subscribe(() => { });
    }
}