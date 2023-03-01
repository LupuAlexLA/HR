import { Component, EventEmitter, Injector, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetPrevServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrev-duplicate-dialog.component.html'
})
export class BugetPrevDuplicateDialogComponent extends AppComponentBase {

    bugetPrevId: number;
    dataBuget: Date;
    descriere: string;
    @Output() onSave = new EventEmitter<any>();

    constructor(inject: Injector,
        private _bugetPrevService: BugetPrevServiceProxy,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    save() {
        this._bugetPrevService.duplicateBugetPrev(this.bugetPrevId, moment(this.dataBuget), this.descriere).subscribe(() => {
            this.notify.info(this.l('Bugetul prevazut a fost duplicat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}