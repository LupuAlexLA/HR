import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BNR_SectorListDto, BNR_SectorServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bnrSectorList.component.html',
    animations: [appModuleAnimation()]
})
export class BnrSectorListComponent extends AppComponentBase implements OnInit {

    bnrSectorList: BNR_SectorListDto[] = [];

    constructor(injector: Injector,
        private _bnrSectorService: BNR_SectorServiceProxy,
        private router: Router      ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.SectoareBNR.Acces')) {
            this.getBnrSectorList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
   }

    getBnrSectorList() {
        this._bnrSectorService.getBNRSectorList().subscribe(result => {
            this.bnrSectorList = result;
        });
    }

    getCountBnrSector() {
        if (this.bnrSectorList.length == 0) {
            return 0;
        } else {
            return this.bnrSectorList.length;
        }
    }

}