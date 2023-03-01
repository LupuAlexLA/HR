import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetPrevDDDto, BugetPrevServiceProxy, BugetVenituriServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetAplica.component.html',
    animations: [appModuleAnimation()]
})
export class BugetAplicaComponent extends AppComponentBase implements OnInit {

    formularBVCId: any;
    bugetPrevBVCList: BugetPrevDDDto[] = [];
    bugetPrevCashFlowList: BugetPrevDDDto[] = [];
    bugetPrevBVCId: number;
    bugetPrevCashFlowId: number;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _bugetVenituriService: BugetVenituriServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Venituri.Acces")) {
            this.formularBVCId = this.route.snapshot.queryParamMap.get('formularBVCId');
            this._bugetPrevService.bugetPrevDDList(this.formularBVCId, 0).subscribe(result => {
                this.bugetPrevBVCList = result;
            });
            this._bugetPrevService.bugetPrevDDList(this.formularBVCId, 1).subscribe(result => {
                this.bugetPrevCashFlowList = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    aplica() {
        this.isLoading = true;
        if (this.bugetPrevBVCId && this.bugetPrevCashFlowId) {
            this._bugetVenituriService.aplicaBVCsiCashFlow(this.bugetPrevBVCId, this.bugetPrevCashFlowId).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })).subscribe(() => { abp.notify.success("Actualizare Reusita"); });
        }
    }
}