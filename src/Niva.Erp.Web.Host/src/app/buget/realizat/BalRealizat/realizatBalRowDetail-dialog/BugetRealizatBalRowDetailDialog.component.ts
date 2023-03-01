import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../../shared/app-component-base";
import { BugetBalRealizatRowDetailDto, BugetRealizatRowDetailDto, BugetRealizatServiceProxy } from "../../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetRealizatBalRowDetailDialog.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRealizatBalRowDetailDialogComponent extends AppComponentBase implements OnInit {

    bugetBalRealizatRowDetList: BugetBalRealizatRowDetailDto[] = [];
    rowId: any;

    constructor(inject: Injector,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    ngOnInit() {
        this.getRowDetails();
    }

    getRowDetails() {
        this._bugetRealizatService.balRealizatRowDetails(this.rowId).subscribe(result => {
            this.bugetBalRealizatRowDetList = result;
        });
    }

}