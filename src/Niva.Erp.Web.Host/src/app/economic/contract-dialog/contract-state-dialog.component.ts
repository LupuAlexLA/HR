import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ChangeContractStateDto, ContractsServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './contract-state-dialog.component.html',
})
/** contractState-dialog component*/
export class ContractStateDialogComponent extends AppComponentBase implements OnInit {
    /** contractState-dialog ctor */
    contractId: number;
    dataEnd: Date;
    changeContract: ChangeContractStateDto = new ChangeContractStateDto();

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _contractService: ContractsServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.changeContract.dataEnd = moment(new Date());
        this.changeContract.contractId = this.contractId;
    }

    save() {
        this._contractService.changeContractsState(this.changeContract).subscribe(() => {
            this.notify.info(this.l('Statutul contractului a fost schimbat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }


}