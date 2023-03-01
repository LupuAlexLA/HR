import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../../shared/app-component-base";
import { AmanarePaapEditDto, OperationDefinitionDto, OperationDefinitionServiceProxy, PaapServiceProxy } from "../../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'saveAs-dialog.component.html'
})
export class SaveAsDialogComponent extends AppComponentBase implements OnInit {

   
    operationDefinition: OperationDefinitionDto = new OperationDefinitionDto();

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _operationDefinitionService: OperationDefinitionServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
    }

    save() {
        this._operationDefinitionService.saveOperationDefinition(this.operationDefinition).subscribe(() => {
            this.notify.info(this.l('Sablon Salvat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}