import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetTitluriDDDto, BugetVenituriServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetVenitList.component.html',
    animations: [appModuleAnimation()]
})
export class BugetVenitListComponent extends AppComponentBase implements OnInit {

    formularBVCId: number = null;
    bugetTitluriList: BugetTitluriDDDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _bugetVenitService: BugetVenituriServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Venituri.Acces')) {
            this.getBugetVenituriList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getBugetVenituriList() {
        this._bugetVenitService.titluriAnList().subscribe(result => {
            this.bugetTitluriList = result;
        });
    }

    getCountVenituri() {
        if (this.bugetTitluriList.length == 0) {
            return 0;
        } else {
            return this.bugetTitluriList.length;
        }
    }

    delete(formularBVCId: number) {
        abp.message.confirm('Titlurile vor fi sterse. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this.isLoading = true;
                    this._bugetVenitService.deleteTitluri(formularBVCId)
                        .pipe(
                            delay(1000),
                            finalize(() => { this.isLoading = false; })
                        )
                        .subscribe(() => {
                        abp.notify.info('Titlurile au fost sterse');
                        this.getBugetVenituriList();
                    });
                }
            });
    }
}