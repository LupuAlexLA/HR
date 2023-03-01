import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetTitluriCFViewList, BugetVenituriServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetVenituriCFDetailsDialog.component.html',
    animations: [appModuleAnimation()]
})
export class BugetVenituriCFDetailsDialogComponent extends AppComponentBase implements OnInit {
    venitTitluId: any;
    detailsCF: BugetTitluriCFViewList[] = [];

    constructor(inject: Injector,
        public bsModalRef: BsModalRef,
        private _bugetVenituriService: BugetVenituriServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getCFDetails();
    }

    getCFDetails() {
        this._bugetVenituriService.titluriViewCFList(this.venitTitluId).subscribe(result => {
            this.detailsCF = result;
        });
    }
}