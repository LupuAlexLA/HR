import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { InvoiceDetailPAAPWithInvoiceElementsDto, PaapServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './paapAlocareFacturi.component.html',
    animations: [appModuleAnimation()]
})
export class PaapAlocareFacturiComponent extends AppComponentBase implements OnInit {

    facturiList: InvoiceDetailPAAPWithInvoiceElementsDto[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _paapService: PaapServiceProxy    ) {
        super(inject);
    }

    ngOnInit() {
        this._paapService.getInvoiceDetailsWithoutPaap().subscribe(result => {
            this.facturiList = result
        });
    }

    approveInvoices(invoiceId, paapId) {
        console.log(invoiceId);
        console.log(paapId);
        this._paapService.approveInvoicesPentruAlocareFacturi(invoiceId, paapId).subscribe(async result => {
            if (result) {  
                abp.notify.info("Facturile au fost alocate");
                window.location.reload();
            }
            else {
                abp.message.error('Valoarea facturilor selectate depaseste valoarea achizitiei')
                await new Promise(f => setTimeout(f, 3000));
                window.location.reload();
            }
        });
    }

    
}