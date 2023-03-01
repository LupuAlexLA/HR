import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetConfigDto, BugetConfigServiceProxy, BugetPrevGenerateDto, BugetPrevServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevAdd.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevAddComponent extends AppComponentBase implements OnInit {
    anBugetList: BugetConfigDto[] = [];
    bugetPrev: BugetPrevGenerateDto = new BugetPrevGenerateDto();
    bvcTipList: EnumTypeDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Prevazut.Modificare')) {
            this.getAnBugetList();
            this._bugetPrevService.bVC_PrevGenerateInit().subscribe(result => {
                this.bugetPrev = result;
            });
            this.getBvcTipList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getAnBugetList() {
        this._bugetConfigService.bugetConfigList().subscribe(result => {
            this.anBugetList = result;
        });
    }

    getBvcTipList() {
        this._enumService.bvcTipList().subscribe(result => {
            this.bvcTipList = result;
        });
    }

    generate() {
        this.isLoading = true;
        this._bugetPrevService.generateBugetPrevV2(this.bugetPrev).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.router.navigate(['/app/buget/prevazut/bugetPrevDetails'], { queryParams: { bugetPrevId: result.id } });
            abp.notify.info("Bugetul prevazut a fost generat");
        });
    }
}