import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetConfigDto, BugetConfigServiceProxy, BugetPrevListDto, BugetPrevServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { BugetPrevDuplicateDialogComponent } from "./bugetPrevDuplicate-dialog/bugetPrev-duplicate-dialog.component";
import { BugetPrevResurseDialogComponent } from "./bugetPrevResurse-dialog/bugetPrevResurseDialog.component";
import { BugetPrevStateDialogComponent } from "./bugetPrevState-dialog/bugetPrev-state-dialog.component";

@Component({
    templateUrl: './bugetPrevList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevListComponent extends AppComponentBase implements OnInit {

    anBugetList: BugetConfigDto[] = [];
    bugetPrevList: BugetPrevListDto[] = [];
    anBugetId: number = null;
    dataBuget: any = null;

    constructor(inject: Injector,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _modalService: BsModalService,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Prevazut.Acces")) {
            this.getAnBugetList();
            this.getBugetPrevList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getAnBugetList() {
        this._bugetConfigService.bugetConfigList().subscribe(result => {
            this.anBugetList = result;
        });
    }

    getBugetPrevList() {
        this._bugetPrevService.bugetPrevList().subscribe(result => {
            this.bugetPrevList = result;
        });
    }

    search() {
        this._bugetPrevService.searchBugetPrev(this.anBugetId).subscribe(result => {
            this.bugetPrevList = result;
        });
    }

    getCountBugetPrevazut() {
        if (this.bugetPrevList.length == 0) {
            return 0;
        } else {
            return this.bugetPrevList.length;
        }
    }

    delete(bugetPrevId: number) {
        abp.message.confirm('Bugetul prevazut va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevService.delete(bugetPrevId).subscribe(() => {
                        abp.notify.info('Bugetul prevazut a fost sters');
                        this.getBugetPrevList();
                    });
                }
            });
    }

    changeState(bugetPrevId: number) {
        this.showChangeStateDialog(bugetPrevId);
    }

    showChangeStateDialog(bugetPrevId: number) {
        let showChangeStateDialog: BsModalRef;

        showChangeStateDialog = this._modalService.show(
            BugetPrevStateDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    bugetPrevId: bugetPrevId
                },
            }
        );

        showChangeStateDialog.content.onSave.subscribe(() => {
            this.getBugetPrevList();
        });
    }

    duplicate(bugetPrevId: number) {
        let showDuplicateDialog: BsModalRef;

        showDuplicateDialog = this._modalService.show(
            BugetPrevDuplicateDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    bugetPrevId: bugetPrevId
                }
            }
        );

        showDuplicateDialog.content.onSave.subscribe(() => {
            this.getBugetPrevList();
        });
    }

    openDialog(bugetPrevId: number, formularId: number, bvcTip: number) {
        let showComputeDialog: BsModalRef;

        showComputeDialog = this._modalService.show(
            BugetPrevResurseDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    bugetPrevId: bugetPrevId,
                    formularId: formularId,
                    tip_BVC: bvcTip
                }
            }
        );

        showComputeDialog.content.onSave.subscribe(() => {
            abp.notify.info('Calculul resurselor a fost efectuat');
        });
    }
    
}