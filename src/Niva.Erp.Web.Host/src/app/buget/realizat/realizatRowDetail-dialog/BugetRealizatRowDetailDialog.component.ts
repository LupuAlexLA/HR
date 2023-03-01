import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetRealizatRowDetailDto, BugetRealizatServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetRealizatRowDetailDialog.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRealizatRowDetailDialogComponent extends AppComponentBase implements OnInit {

    bugetRealizatRowDetList: BugetRealizatRowDetailDto[] = [];
    rowId: any;
   /* @Output() onSave = new EventEmitter<any>();*/

    constructor(inject: Injector,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    ngOnInit() {
        this.getRowDetails();
    }

    getRowDetails() {
        this._bugetRealizatService.realizatRowDetails(this.rowId).subscribe(result => {
            this.bugetRealizatRowDetList = result;
        });
    }

}