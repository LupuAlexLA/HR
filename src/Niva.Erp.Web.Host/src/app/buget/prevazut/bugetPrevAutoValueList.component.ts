import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetPrevAutoServiceProxy, BugetPrevAutoValueListDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevAutoValueList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevAutoValueListComponent extends AppComponentBase implements OnInit {

    bugetPrevAutoValueList: BugetPrevAutoValueListDto[] = [];

    constructor(inject: Injector,
        private _bugetPrevAutoService: BugetPrevAutoServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Prevazut.ConfigValori")) {
            this.getBugetPrevAutoValueList();
            this.getCountBugetPrevAutoValue();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getBugetPrevAutoValueList() {
        this._bugetPrevAutoService.bugetPrevAutoList().subscribe(result => {
            this.bugetPrevAutoValueList = result;
        });
    }

    getCountBugetPrevAutoValue() {
        if (this.bugetPrevAutoValueList.length == 0) {
            return 0;
        } else {
            return this.bugetPrevAutoValueList.length;
        }
    }

    delete(id: number) {
        abp.message.confirm('Valorile preluate automat vor fi sterse. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevAutoService.deleteBugetPrevAuto(id).subscribe(() => {
                        abp.notify.info('Valorile preluate automat au fost sterse');
                        this.getBugetPrevAutoValueList();
                    });
                }
            });
    }

}