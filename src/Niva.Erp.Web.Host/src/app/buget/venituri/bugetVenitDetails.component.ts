import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetTitluriViewDto, BugetVenituriServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { BugetVenituriBVCDetailsDialogComponent } from "./bugetVenituriBVC-dialog/bugetVenituriBVCDetailsDialog.component";
import { BugetVenituriCFDetailsDialogComponent } from "./bugetVenituriCF-dialog/bugetVenituriCFDetailsDialog.component";

@Component({
    templateUrl: './bugetVenitDetails.component.html',
    animations: [appModuleAnimation()]
})
export class BugetVenitDetailsComponent extends AppComponentBase implements OnInit {

    formularBVCId: any;
    bugetVenituriList: BugetTitluriViewDto[] = [];

    constructor(inject: Injector,
        private _bugetVenituriService: BugetVenituriServiceProxy,
        private _modalService: BsModalService,
        private route: ActivatedRoute,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Venituri.Acces")) {
            this.formularBVCId = this.route.snapshot.queryParamMap.get('formularBVCId');
            this.getBugetVenituriList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getBugetVenituriList() {
        this._bugetVenituriService.titluriViewList(this.formularBVCId).subscribe(result => {
            this.bugetVenituriList = result;
        });
    }

    getCountVenituriDetails() {
        if (this.bugetVenituriList.length == 0) {
            return 0;
        } else {
            return this.bugetVenituriList.length;
        }
    }

    showBVCDetailsDialog(venitTitluId: number) {
        let showBVCDetailsDialog: BsModalRef;

        showBVCDetailsDialog = this._modalService.show(
            BugetVenituriBVCDetailsDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    venitTitluId: venitTitluId
                },
            });
    }

    showCFDetailsDialog(venitTitluId: number) {
        let showCFDetailsDialog: BsModalRef;

        showCFDetailsDialog = this._modalService.show(
            BugetVenituriCFDetailsDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    venitTitluId: venitTitluId
                },
            });
    }

    selectAll() {
        for (var i = 0; i < this.bugetVenituriList.length; i++) {
            this.bugetVenituriList[i].selectat = true;
        }
    }

    selectNone() {
        for (var i = 0; i < this.bugetVenituriList.length; i++) {
            this.bugetVenituriList[i].selectat = false;
        }
    }

    save() {
        this._bugetVenituriService.updateVenituriTitluCF(this.formularBVCId, this.bugetVenituriList).subscribe(result => {
            abp.notify.success("Sumele au fost reinvestite");
        });
    }
}