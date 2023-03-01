import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetRealizatServiceProxy, RealizatAddDispoDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetRealizatadd.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRealizatAddComponent extends AppComponentBase implements OnInit {

    savedBalanceList: RealizatAddDispoDto[] = [];
    savedBalanceId: number = 0;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Realizat.Modificare')) {
            this.getBalanceList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getBalanceList() {
        this._bugetRealizatService.realizatAddDisponibil().subscribe(result => {
            this.savedBalanceList = result;
        });
    }

    generate() {
        this.isLoading = true;
        this._bugetRealizatService.bugetRealizatCalcul(this.savedBalanceId).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(() => {
            abp.notify.success("Bugetul realizat a fost calculat");
            this.router.navigate(['/app/buget/realizat/bugetRealizatList']);
        });
    }

}