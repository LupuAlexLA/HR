import { Component, Injector, OnInit } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { OperationServiceProxy, SoldOperationDto } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'soldDb-dialog.component.html'
})
export class SoldDbDialogComponent extends AppComponentBase implements OnInit{

    accountId: number;
    currencyId: number;
    operationDate: any;
    soldOperation: SoldOperationDto = new SoldOperationDto();

    constructor(injector: Injector,
        public bsModalRef: BsModalRef,
        private _operationService: OperationServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this._operationService.soldOperation(this.accountId, this.currencyId, moment(this.operationDate)).subscribe(result => {
            this.soldOperation = result;
        });
    }

}