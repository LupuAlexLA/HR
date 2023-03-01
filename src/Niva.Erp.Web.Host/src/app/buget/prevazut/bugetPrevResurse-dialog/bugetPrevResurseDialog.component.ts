import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetPrevDDDto, BugetPrevServiceProxy, EnumServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevResurseDialog.component.html'
})
export class BugetPrevResurseDialogComponent extends AppComponentBase implements OnInit {

    bugetPrevId: number;
    formularId: number;
    bvcTip: number;
    bugetPrevCashFlowId: number;
    bugetPrevCashFlowList: BugetPrevDDDto[] = [];
    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _enumService: EnumServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }


    ngOnInit() {
        this._bugetPrevService.bugetPrevDDList(this.formularId, this.bvcTip).subscribe(result => {
            this.bugetPrevCashFlowList = result;
        });
    }

    save() {
        this._bugetPrevService.calculResurse(this.bugetPrevId, this.bugetPrevCashFlowId, this.formularId, this.bvcTip).subscribe(result => {
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }

}