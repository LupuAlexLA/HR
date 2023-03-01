import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { SitFinanCalcDetail, SitFinanCalcServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'sitFinanCalcDetailsDialog.component.html'
})
export class SitFinanCalcDetailsDialogComponent extends AppComponentBase implements OnInit {

    columnId: number;
    reportId: number;
    balanceId: number;
    detailList: SitFinanCalcDetail[] = [];

    constructor(inject: Injector,
        private _sitFinanCalcService: SitFinanCalcServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit(): void {
        this._sitFinanCalcService.showReportDetails(this.columnId, this.reportId, this.balanceId).subscribe(result => {
            this.detailList = result;
        });
    }


    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }
}