import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { PaapRedistribuireDto, PaapRedistribuireServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'redistribuirePaapDetails-dialog.component.html'
})
export class RedistribuirePaapDetailsDialogComponent extends AppComponentBase implements OnInit {
    paapRedistribId: number;
    paapRedistrib: PaapRedistribuireDto = new PaapRedistribuireDto();
    constructor(injector: Injector,
        private _paapRedistribuireService: PaapRedistribuireServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit(): void {
        this._paapRedistribuireService.getPaapRedistribuire(this.paapRedistribId).subscribe(result => {
            this.paapRedistrib = result;
        });
    }

 

}