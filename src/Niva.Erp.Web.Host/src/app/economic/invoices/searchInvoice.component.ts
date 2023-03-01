import { Route } from "@angular/compiler/src/core";
import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { InvoiceForDecontDTO, InvoiceServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './searchInvoice.component.html',
    animations: [appModuleAnimation()]
})
export class SearchInvoiceComponent extends AppComponentBase implements OnInit {

    invoicesList: InvoiceForDecontDTO[] = [];
    decontId: any;
    currencyId: any;

    constructor(inject: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private router: ActivatedRoute,
        private route: Router) {
        super(inject);
    }

    ngOnInit() {
        this.decontId = +this.router.snapshot.queryParamMap.get('decontId');
        this.currencyId = +this.router.snapshot.queryParamMap.get('currencyId');
        this._invoiceService.getInvoicesForDecont(this.decontId, this.currencyId).subscribe(result => {
            this.invoicesList = result;
        });
    }

    saveInvoice() {
        let selectedInvoiceIds: Array<number> = new Array<number>();
        this.invoicesList.forEach(function (item) {
            if (item.selected)
                selectedInvoiceIds.push(item.id);
        });
        this._invoiceService.saveInvoicesFromDecont(this.decontId, selectedInvoiceIds).subscribe(() => {
            this.route.navigate(['/app/decontAdd'], { queryParams: { decontId: this.decontId } });
            abp.notify.success("Facturile au inregistrate");
        });


    }

}