import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { LichidCalcDetDto, LichidCalcServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './lichidCalcDet-dialog.component.html'
})
export class LichidCalcDetDialogComponent extends AppComponentBase implements OnInit {

    columnId: any;
    savedBalanceId: any;
    lichidConfigId: any;
    lichidCalcFormulaDetlist: LichidCalcDetDto[] = [];

    constructor(inject: Injector,
        private _lichidCalcService: LichidCalcServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this._lichidCalcService.getLichidCalcFormulaDet(this.columnId, this.savedBalanceId, this.lichidConfigId).subscribe(result => {
            this.lichidCalcFormulaDetlist = result;
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