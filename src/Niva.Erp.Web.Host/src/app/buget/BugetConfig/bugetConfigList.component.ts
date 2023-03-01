import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BugetConfigDto, BugetConfigServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { BugetConfigDuplicateDialogComponent } from './bugetConfigDuplicate-dialog/bugetConfig-duplicate-dialog.component';

@Component({
    
    templateUrl: './bugetConfigList.component.html',
    animations: [appModuleAnimation()],

})
/** bugetConfigList component*/
export class BugetConfigListComponent extends AppComponentBase implements OnInit {
    /** bugetConfigList ctor */
    bugetConfigList: BugetConfigDto[] = [];
    //bugetPrev: BugetPrevListDto//

    constructor(injector: Injector,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _modalService: BsModalService,
        private router: Router) {
        super(injector);
    }
    ngOnInit() {
        this.getBugetConfigList();
    }

    getBugetConfigList() {
        if (this.isGranted("Buget.BVC.Configurare")) {
            this._bugetConfigService.bugetConfigList().subscribe(result => {
                this.bugetConfigList = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    delete(BugetId: number) {
        this._bugetConfigService.deleteBugetConfig(BugetId).subscribe(() => {
            this.getBugetConfigList();
            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    getBugetCount() {
        if (this.bugetConfigList.length > 0) {
            return this.bugetConfigList.length;
        } else {
            return 0;
        }
    }

    duplicate(bugetConfigId: number) {
        let showDuplicateDialog: BsModalRef;

        showDuplicateDialog = this._modalService.show(
            BugetConfigDuplicateDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    bugetConfigId: bugetConfigId
                }
            }
        );

        showDuplicateDialog.content.onSave.subscribe(() => {
            this.getBugetConfigList();
        });
    }
}