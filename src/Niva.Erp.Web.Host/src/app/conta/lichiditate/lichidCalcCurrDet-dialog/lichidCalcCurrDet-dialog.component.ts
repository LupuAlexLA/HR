import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { LichidCalcCurrDetDto, LichidCalcCurrServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './lichidCalcCurrDet-dialog.component.html'
})
export class LichidCalcCurrDetDialogComponent extends AppComponentBase implements OnInit {

    columnId: any;
    savedBalanceId: any;
    lichidConfigId: any;
    lichidCalcCurrFormulaDetlist: LichidCalcCurrDetDto[] = [];

    constructor(inject: Injector,
        private _lichidCalcCurrService: LichidCalcCurrServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this._lichidCalcCurrService.getLichidCalcCurrFormulaDet(this.columnId, this.savedBalanceId, this.lichidConfigId).subscribe(result => {
            this.lichidCalcCurrFormulaDetlist = result;
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