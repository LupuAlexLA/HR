import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FiledocSearchComponent } from '../shared/modal/filedoc-search/filedoc-search.component';
import { NotificareDto, NotificareServiceProxy, PaapServiceProxy } from "../../shared/service-proxies/service-proxies";

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  //changeDetection: ChangeDetectionStrategy.OnPush
})

export class HomeComponent extends AppComponentBase {

    selectedId: number;
    invoicesWithoutPaapCount: number;
    nefinalizat30zilePaapListCount: number;
    nefinalizatDepasitPaapListCount: number;
    notificariList: NotificareDto[] = [];

    constructor(injector: Injector,
        private _modalService: BsModalService,
        private _paapService: PaapServiceProxy,
        private _notificareService: NotificareServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Buget.PAAP.AlocareFacturi')) {
            this._paapService.getInvoiceDetailsWithoutPaapCount().subscribe(result => {
                this.invoicesWithoutPaapCount = result
            });
        }

        if (this.isGranted('Buget.PAAP.Super')) {
            this._paapService.nefinalizat30zilePaapListCount().subscribe(result => {
                this.nefinalizat30zilePaapListCount = result
            });
        }

        if (this.isGranted('Buget.PAAP.Super')) {
            this._paapService.nefinalizatDepasitPaapListCount().subscribe(result => {
                this.nefinalizatDepasitPaapListCount = result
            });
        }
        this.getNotificariList();
    }

    filedocsearch(): void {
        this.showFildocSearchDialog();
    }

    private showFildocSearchDialog(id?: number): void {
        let createOrEditUserDialog: BsModalRef;
        if (!id) {
            createOrEditUserDialog = this._modalService.show(
                FiledocSearchComponent,
                {
                    class: 'modal-lg',
                }
            );
        } else {
            createOrEditUserDialog = this._modalService.show(
                FiledocSearchComponent,
                {
                    class: 'modal-lg',
                    initialState: {
                        id: id,
                    },
                }
            );
        }

        createOrEditUserDialog.content.onSave.subscribe((id) => {
            this.refresh(id);
        });
    }
    refresh(id) {
        this.selectedId = id;
    }

    getNotificariList() {
        this._notificareService.getNotificareList().subscribe(result => {
            this.notificariList = result;
        });
    }

    updateStareNotificare(notificareId: number) {
        this._notificareService.updateStareNotificare(notificareId).subscribe(result => {
            this.getNotificariList();
        });
    }
}
