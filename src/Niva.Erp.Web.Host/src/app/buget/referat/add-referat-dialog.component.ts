import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../shared/app-component-base";
import { PaapReferatEditDto, PaapReferatServiceProxy } from "../../../shared/service-proxies/service-proxies";


@Component({
    templateUrl: 'add-referat-dialog.component.html'
})
export class AddReferatDialogComponent extends AppComponentBase implements OnInit {

    paapId: number;
    referatId: number;
    suma: number;
    paapReferat: PaapReferatEditDto = new PaapReferatEditDto();

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _paapReferatService: PaapReferatServiceProxy,
        private route: ActivatedRoute,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this._paapReferatService.getPaapReferatEdit(this.referatId, this.paapId).subscribe(result => {
            this.paapReferat = result;
        });
    }

    save() {
        this._paapReferatService.savePaapReferat(this.paapReferat).subscribe(() => {
            this.notify.info(this.l('Referatul a fost adaugat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}