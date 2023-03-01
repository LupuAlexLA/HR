import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { DataComisionDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './rateList-dialog.component.html',
    animations: [appModuleAnimation()]
})
export class RateListDialogComponent extends AppComponentBase implements OnInit {

    comisionList: DataComisionDto = new DataComisionDto();

    @Output() onSave = new EventEmitter<any>();


    constructor(inject: Injector,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    ngOnInit() {
        console.log(this.comisionList);
    }
}