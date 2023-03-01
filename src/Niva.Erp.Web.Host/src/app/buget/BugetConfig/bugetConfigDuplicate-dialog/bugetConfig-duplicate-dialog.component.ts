import { Component, EventEmitter, Injector, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetConfigServiceProxy, BugetPrevServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetConfig-duplicate-dialog.component.html'
})
export class BugetConfigDuplicateDialogComponent extends AppComponentBase {

    bugetConfigId: number;
    an: number;
    @Output() onSave = new EventEmitter<any>();

    constructor(inject: Injector,
        private _bugetConfigService: BugetConfigServiceProxy,
        public bsModalRef: BsModalRef    ) {
        super(inject);
    }

    save() {
        this._bugetConfigService.bugetClonare(this.bugetConfigId, this.an).subscribe(() => {
            this.notify.info(this.l('Bugetul configurat a fost duplicat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}