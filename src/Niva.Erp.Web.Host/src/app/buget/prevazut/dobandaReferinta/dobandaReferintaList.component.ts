import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetPrevDobandaReferintaListDto, BugetPrevDobandaRefServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './dobandaReferintaList.component.html',
    animations: [appModuleAnimation()]
})
export class DobandaReferintaListComponent extends AppComponentBase implements OnInit {

    dobandaReferintaList: BugetPrevDobandaReferintaListDto[] = [];

    constructor(inject: Injector,
        private _bugetDobandaRefService: BugetPrevDobandaRefServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Prevazut.DobanziReferinta')) {
            this.getDobandaRefList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getDobandaRefList() {
        this._bugetDobandaRefService.getDobandaRefList().subscribe(result => {
            this.dobandaReferintaList = result;
        });
    }

    getDobandaRefCount() {
        if (this.dobandaReferintaList.length == 0) {
            return 0;
        } else {
            return this.dobandaReferintaList.length;
        }
    }

    delete(dobandaRefId: number) {
        abp.message.confirm('Dobanda de referinta va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetDobandaRefService.deleteDobandaRef(dobandaRefId).subscribe(() => {
                        abp.notify.info('Dobanda de referinta a fost stearsa');
                        this.getDobandaRefList();
                    });
                }
            });
    }

}