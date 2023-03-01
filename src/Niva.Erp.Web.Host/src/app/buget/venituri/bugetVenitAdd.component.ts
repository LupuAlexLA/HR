import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetPrevDDDto, BugetPrevServiceProxy, BugetTitluriDDDto, BugetVenituriServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetVenitAdd.component.html',
    animations: [appModuleAnimation()]
})
export class BugetVenitAddComponent extends AppComponentBase implements OnInit {
    anBugetList: BugetTitluriDDDto[] = [];
    formularBVCId: number = null;
    isLoading: boolean = false;
    monthStart: number = null;
    monthEnd: number = null;
    bugetPrevList: BugetPrevDDDto[] = [];
    bugetPreliminatId: number = null;

    constructor(inject: Injector,
        private _bugetVenitService: BugetVenituriServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Venituri.Modificare')) {
            this.getAnBugetList();
            this.monthStart = 1;
            this.monthEnd = 12;
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getAnBugetList() {
        this._bugetVenitService.dispoTitluriAdd().subscribe(result => {
            this.anBugetList = result;
        });
    }

    addTitluri() {
        this.isLoading = true;
        this._bugetVenitService.addTitluri(this.formularBVCId, this.monthStart, this.monthEnd, this.bugetPreliminatId)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(() => {
                this.router.navigate(['/app/buget/venituri/bugetVenitList']);
                abp.notify.success("Titlurile au fost adaugate");
            });
    }

    getBugetPreliminat() {
        this._bugetPrevService.bugetPreliminatCFLastYear(this.formularBVCId).subscribe(result => {
            this.bugetPrevList = result;
        });
    }

}