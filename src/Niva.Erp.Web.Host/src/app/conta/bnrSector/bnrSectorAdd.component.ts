import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BNR_SectorEditDto, BNR_SectorServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bnrSectorAdd.component.html',
    animations: [appModuleAnimation()]
})
export class BnrSectorAddComponent extends AppComponentBase implements OnInit {

    bnrSector: BNR_SectorEditDto = new BNR_SectorEditDto();

    constructor(inject: Injector,
        private _bnrSectorService: BNR_SectorServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        
    }

    save() {
        this._bnrSectorService.save(this.bnrSector).subscribe(() => {
            this.router.navigate(['/app/conta/bnrSector/bnrSectorList']);
            abp.notify.success("Inregistrarea a fost salvata");
        });
    }
}