import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../shared/animations/routerTransition";
import { AppComponentBase } from "../../shared/app-component-base";
import { BNR_SectorCalcServiceProxy, BNR_SectorDetailDto, BNR_SectorListDto, BNR_SectorServiceProxy } from "../../shared/service-proxies/service-proxies";
@Component({
    templateUrl: './calculBnrDetail-dialog.component.html',
    animations: [appModuleAnimation()]
})
export class CalculBnrDetailDialogComponent extends AppComponentBase implements OnInit {

    sectorList: BNR_SectorListDto[] = [];
    bnrConturiList: BNR_SectorDetailDto[] = [];
    anexaDetailId: number;
    anexaId: number;
    sectorId: any;
    savedBalanceId: any;
    @Output() onSave = new EventEmitter<any>();

    constructor(inject: Injector,
        private _bnrSectorCalcService: BNR_SectorCalcServiceProxy,
        private _sectorService: BNR_SectorServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this.getSectorList();
        this.getBNRConturi();
    }

    getSectorList() {
        this._sectorService.getBNRSectorList().subscribe(result => {
            this.sectorList = result;
        });
    }

    getBNRConturi() {
        this._bnrSectorCalcService.getBNRConturiList(this.anexaDetailId, this.sectorId, this.savedBalanceId).subscribe(result => {
            this.bnrConturiList = result;
        });
    }

    save() {
        this._bnrSectorCalcService.saveBNRConturi(this.savedBalanceId, this.bnrConturiList).subscribe(() => {
      /*      this.getBNRConturi();*/
            abp.notify.success("Modificarile au fost salvate");
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }

}