import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { PaapEditDto, PaapServiceProxy, PaapStateEditDto } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'cancel-paap-dialog.component.html'
})
export class ShowCancelPAAPDialogComponent extends AppComponentBase implements OnInit {

    paapId: number;
    paapState: PaapStateEditDto = new PaapStateEditDto();

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _paapService: PaapServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this._paapService.getPaapStateByPaapId(this.paapId).subscribe(result => {
            this.paapState = result;
        });
    }

    save() {
        this._paapService.cancelApprovedPaap(this.paapState).subscribe(() => {
            this.notify.info(this.l('Achizitia a fost anulata'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}