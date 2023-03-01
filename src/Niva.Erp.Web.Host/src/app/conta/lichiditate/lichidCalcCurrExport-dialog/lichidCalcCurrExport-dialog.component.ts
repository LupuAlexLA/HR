import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { LichidCalcCurrListDto, LichidCalcCurrServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './lichidCalcCurrExport-dialog.component.html'
})
export class LichidCalcCurrExportComponent extends AppComponentBase implements OnInit {

    savedBalanceId: any;
    lichidCalcCurrList: LichidCalcCurrListDto[] = [];

    constructor(inject: Injector,
        private _lichidCalcCurrService: LichidCalcCurrServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this._lichidCalcCurrService.getLichidCalcCurrList(this.savedBalanceId).subscribe(result => {
            this.lichidCalcCurrList = result;
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