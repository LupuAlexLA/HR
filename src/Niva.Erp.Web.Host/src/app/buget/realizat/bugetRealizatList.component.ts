import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetConfigDto, BugetConfigServiceProxy, BugetRealizatDto, BugetRealizatServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetRealizatList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRealizatListComponent extends AppComponentBase implements OnInit {

    bugetRealizatList: BugetRealizatDto[] = [];
    tipBvcList: EnumTypeDto[] = [];
    anBugetList: BugetConfigDto[] = [];
    formularId: number;
    tip_BVCId: number;

    constructor(inject: Injector,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Realizat.Acces")) {
            this.getBvcTipList();
            this.getAnBugetList();
            this.search();
        } else {
            this.router.navigate(['app/home']);
        }
        
    }

    getBvcTipList() {
        this._enumService.bvcTipList().subscribe(result => {
            this.tipBvcList = result;
            this.tip_BVCId = this.tipBvcList[0].id;
        });
    }

    getAnBugetList() {
        this._bugetConfigService.bugetConfigList().subscribe(result => {
            this.anBugetList = result;
            this.formularId = this.anBugetList[0].id;
        });
    }


    search() {
        this._bugetRealizatService.bugetRealizatList(this.formularId, this.tip_BVCId).subscribe(result => {
            this.bugetRealizatList = result;
            console.log(this.bugetRealizatList)
        });
    }

    getCountBugetRealizat() {
        if (this.bugetRealizatList.length == 0) {
            return 0;
        } else {
            return this.bugetRealizatList.length;
        }
    }

    delete(bugetRealizatId: number, bugetBalRealizatId: number) {
        abp.message.confirm('Calculul bugetului realizat va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetRealizatService.bugetRealizatDelete(bugetRealizatId, bugetBalRealizatId).subscribe(() => {
                        abp.notify.info('Calculul bugetului a fost sters');
                        this.search();
                    });
                }
            });
    }

    getBalRealizatDetails(bvC_BalRealizatId: number) {
        if (bvC_BalRealizatId !== null) {
            this.router.navigate(['/app/buget/realizat/bugetBalRealizatRandList'], { queryParams: { realizatId: bvC_BalRealizatId } });

        } else {
            abp.message.warn(`Nu a fost identificat Bugetul realizat la data selectata`);
        }
    }
}