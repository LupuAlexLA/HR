import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { LichidCalcListDetDto, LichidCalcServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './lichidCalcExport-dialog.component.html'
})
export class LichidCalcExportComponent extends AppComponentBase implements OnInit {

    savedBalanceId: any;
    lichidCalcDetList: LichidCalcListDetDto[] = [];

    constructor(inject: Injector,
        private _lichidCalcService: LichidCalcServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this._lichidCalcService.getLichidCalcDetList(this.savedBalanceId).subscribe(result => {
            this.lichidCalcDetList = result;
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