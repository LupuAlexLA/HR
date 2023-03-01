import { Component, inject, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BNR_Detalii, BNR_RaportareServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'raportareBNRDetail-dialog.component.html'
})
export class RaportareBNRDetailDialogComponent extends AppComponentBase implements OnInit {
    anexaId: number;
    savedBalanceId: number;
    bnrDetaliiList: BNR_Detalii[] = [];

    constructor(inject: Injector,
        private _bnrRaportareService: BNR_RaportareServiceProxy,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    ngOnInit(): void {
        this._bnrRaportareService.raportareDetailsList(this.savedBalanceId, this.anexaId).subscribe(result => {
            this.bnrDetaliiList = result;
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