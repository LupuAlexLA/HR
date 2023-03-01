import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetTitluriBVCViewList, BugetVenituriServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetVenituriBVCDetailsDialog.component.html',
    animations: [appModuleAnimation()]
})
export class BugetVenituriBVCDetailsDialogComponent extends AppComponentBase implements OnInit {

    venitTitluId: any;
    detailsBVC: BugetTitluriBVCViewList[] = [];

    constructor(inject: Injector,
        public bsModalRef: BsModalRef,
        private _bugetVenituriService: BugetVenituriServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getBVCDetails();
    }

    getBVCDetails() {
        this._bugetVenituriService.titluriViewBVCList(this.venitTitluId).subscribe(result => {
            this.detailsBVC = result;
        });
    }
}