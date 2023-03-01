import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetRealizatRowDto, BugetRealizatServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { BugetRealizatRowDetailDialogComponent } from "./realizatRowDetail-dialog/bugetRealizatRowDetailDialog.component";

@Component({
    templateUrl: './bugetRealizatRandList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRealizatRandListComponent extends AppComponentBase implements OnInit {

    bugetRealizatRandList: BugetRealizatRowDto[] = [];
    realizatId: number;

    constructor(inject: Injector,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        private _modalService: BsModalService,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Realizat.Acces")) {
            this.realizatId = +this.route.snapshot.queryParamMap.get('realizatId');
            this.getBugetRealizatRanduri();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getBugetRealizatRanduri() {
        this._bugetRealizatService.realizatRows(this.realizatId).subscribe(result => {
            this.bugetRealizatRandList = result;
        });
    }

    getCountRealizatRanduri() {
        if (this.bugetRealizatRandList.length == 0) {
            return 0;
        } else {
            return this.bugetRealizatRandList.length;
        }
    }

    showRowDetailDialog(rowId: number) {
        let showRowDetailDialog: BsModalRef;

        showRowDetailDialog = this._modalService.show(
            BugetRealizatRowDetailDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    rowId: rowId,
                },
            }
        );

    }

}