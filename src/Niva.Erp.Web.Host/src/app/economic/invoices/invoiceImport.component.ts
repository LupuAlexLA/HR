import { Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '../../../shared/app-component-base';
import { FiledocSearchComponent } from '../../shared/modal/filedoc-search/filedoc-search.component';

@Component({ 
    templateUrl: './invoiceImport.component.html'    
})
/** invoiceImport component*/
export class InvoiceImportComponent extends AppComponentBase implements OnInit {
    /** invoiceImport ctor */
     

    selectedId: number;

    constructor(injector: Injector,
        private _modalService: BsModalService) {
        super(injector);
    }
    ngOnInit(): void {
        //throw new Error('Method not implemented.');
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
}