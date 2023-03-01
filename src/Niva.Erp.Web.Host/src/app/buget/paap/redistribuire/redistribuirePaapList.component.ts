import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Route, Router } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { PaapRedistribuireListDto, PaapRedistribuireServiceProxy } from "../../../../shared/service-proxies/service-proxies";
import { RedistribuirePaapDetailsDialogComponent } from "./redistribuirePaapDetails-dialog.component";
@Component({
    templateUrl: './redistribuirePaapList.component.html',
    animations: [appModuleAnimation()]
})
export class RedistribuirePaapListComponent extends AppComponentBase implements OnInit {

    paapRedistribuire: PaapRedistribuireListDto[] = [];
    anAchizitie: any;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _paapRedistribuireService: PaapRedistribuireServiceProxy,
        private _modalService: BsModalService,
        private route: ActivatedRoute
    ) {
        super(inject);
    }

    ngOnInit() {
        this.anAchizitie = this.route.snapshot.queryParamMap.get('an');
        this.getPaapRedistribuireList();
    }


    getPaapRedistribuireList() {
        this.isLoading = true;
        this._paapRedistribuireService.getPaapRedistribuireList(this.anAchizitie)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
            this.paapRedistribuire = result;
        });
    }

    paapRedistribCount() {
        if (this.paapRedistribuire.length == 0) {
            return 0;
        } else {
            return this.paapRedistribuire.length;
        }
    }

    delete(paapRedistribId: number) {
        abp.message.confirm('Redistribuirea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._paapRedistribuireService.deleteAchizitieRedistribuita(paapRedistribId)
                        .subscribe(() => {
                            this.getPaapRedistribuireList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }
    openDetails(paapRedistribId: number) {
        let showDetailsDialog: BsModalRef;

        showDetailsDialog = this._modalService.show(
            RedistribuirePaapDetailsDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    paapRedistribId: paapRedistribId
                },
            }
        );
    }
}