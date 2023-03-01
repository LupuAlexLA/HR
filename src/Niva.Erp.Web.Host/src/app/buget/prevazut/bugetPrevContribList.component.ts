import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetPrevContribListDto, BugetPrevContribServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevContribList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevContribListComponent extends AppComponentBase implements OnInit {

    contribList: BugetPrevContribListDto[] = [];
    tipIncasare: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _bugetPrevContribService: BugetPrevContribServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Prevazut.ContribAlteIncasari")) {
            this.getContribList();
            this.getEnums();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getEnums() {
        this._enumService.bVC_BugetPrevContributieTipIncasare().subscribe(result => {
            this.tipIncasare = result;
        });
    }

    getContribList() {
        this._bugetPrevContribService.getContribList().subscribe(result => {
            this.contribList = result;
        });

    }

    getCountBugetPrevazutContrib() {
        if (this.contribList.length == 0) {
            return 0;
        } else {
            return this.contribList.length;
        }

    }

    search() {

    }

    delete(contribId: number) {
        abp.message.confirm('Contributia va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevContribService.deleteContrib(contribId).subscribe(() => {
                        abp.notify.info('Contributia a fost stearsa');
                        this.getContribList();
                    });
                }
            });
    }
}