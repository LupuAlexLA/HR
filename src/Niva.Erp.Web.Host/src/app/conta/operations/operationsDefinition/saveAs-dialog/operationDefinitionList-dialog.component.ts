import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../../shared/app-component-base";
import { AmanarePaapEditDto, OperationDefinitionDto, OperationDefinitionServiceProxy, PaapServiceProxy } from "../../../../../shared/service-proxies/service-proxies";
import { OperationDefinitionComponent } from "../operationDefinition.component";

@Component({
    templateUrl: 'operationDefinitionList-dialog.component.html'
})
export class OperationDefinitionListDialogComponent extends AppComponentBase implements OnInit {

    operationDefinitionList: OperationDefinitionDto[] = [];

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _operationDefinitionService: OperationDefinitionServiceProxy,
        private _modalService: BsModalService,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this._operationDefinitionService.operationDefinitionList().subscribe(result => {
            this.operationDefinitionList = result;
        });
    }

    pick(element) {
        this.bsModalRef.hide();
        this.onSave.emit(element);
    }

    delete(element) {
        this._operationDefinitionService.deleteOperationDefinition(element.id).subscribe(() => {

        this.operationDefinitionList.splice(this.operationDefinitionList.indexOf(element), 1);

            this.notify.info(this.l('Sablon Sters'));
        });
    }

    edit(element) {
        let editDialog: BsModalRef;
        console.log(element.id);
        editDialog = this._modalService.show(
            OperationDefinitionComponent,
            {
                class: 'modal-lg',
                initialState: {
                    operationId: element.id,
                },
            }
        );
    }
}