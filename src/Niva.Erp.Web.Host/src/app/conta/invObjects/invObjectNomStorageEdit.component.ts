import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvObjectStorageDto, InvObjectStorageServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomStorageEdit.component.html',
    animations: [appModuleAnimation()]
})

export class InvObjectNomStorageEditComponent extends AppComponentBase implements OnInit {

    storage: InvObjectStorageDto = new InvObjectStorageDto();
    storageId: number;

    constructor(inject: Injector,
        private _storageService: InvObjectStorageServiceProxy,
        private router: Router,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.storageId = +this.route.snapshot.queryParamMap.get('storageId');

        if (this.storageId != 0) {
            this._storageService.getStorageById(this.storageId).subscribe(result => {
                this.storage = result;
            });
        }
    }

    saveStorage() {
        this._storageService.saveStorage(this.storage).subscribe(() => {
            abp.notify.info(this.l('AddModifyMessage'));
            this.router.navigate(['/app/conta/invObjects/invObjectNomStorage']);
        });
    }
}