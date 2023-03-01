import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetPrevServiceProxy, BugetPrevStatusDto, EnumServiceProxy, EnumTypeDto } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrev-state-dialog.component.html'
})
export class BugetPrevStateDialogComponent extends AppComponentBase implements OnInit {

    bugetPrevId: number;
    bvcStatusList: EnumTypeDto[] = [];
    bugetStatus: BugetPrevStatusDto = new BugetPrevStatusDto();

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _enumService: EnumServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.bugetStatus.bugetPrevId = this.bugetPrevId;
        this.getBVC_StatusList();
    }

    getBVC_StatusList() {
        this._enumService.bVC_StatusList().subscribe(result => {
            this.bvcStatusList = result;
        });
    }

    save() {
        this._bugetPrevService.changeBugetPrevState(this.bugetStatus).subscribe(() => {
            this.notify.info(this.l('Statusul a fost actualizat'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}