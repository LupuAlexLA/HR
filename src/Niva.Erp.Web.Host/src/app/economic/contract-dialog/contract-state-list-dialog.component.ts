import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ContractStateListDto, ContractsServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    
    templateUrl: './contract-state-list-dialog.component.html',
    
})
/** contract-state-list-dialog component*/
export class ContractStateListDialogComponent extends AppComponentBase implements OnInit {
    /** contractState-dialog ctor */
    
    contractList: ContractStateListDto[];
    contractId: number;
    

    constructor(injector: Injector,
        private _contractService: ContractsServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.getContractStateList();
    }

    getContractStateList() {
        console.log(this.contractId);
        this._contractService.getContractStateListById(this.contractId).subscribe(result => {
            this.contractList = result;
        });
    }

}