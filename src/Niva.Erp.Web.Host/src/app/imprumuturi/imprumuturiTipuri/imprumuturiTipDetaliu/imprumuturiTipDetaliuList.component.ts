import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { ImprumutTipDetaliuDto, ImprumuturiTipuriServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './imprumuturiTipDetaliuList.component.html',
    animations: [appModuleAnimation()]
})
export class ImprumuturiTipDetaliuListComponent extends AppComponentBase implements OnInit {

    imprumutTipId: number;
    imprumutTipDetaliuList: ImprumutTipDetaliuDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _imprumutTipService: ImprumuturiTipuriServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.imprumutTipId = +this.route.snapshot.queryParamMap.get("imprumuturiTipId");
        this.getImprumuturiTipDetaliuList();
    }

    getImprumuturiTipDetaliuList() {
        this._imprumutTipService.imprumutTipDetaliiList(this.imprumutTipId).subscribe(result => {
            this.imprumutTipDetaliuList = result;
        });

    }

    getDetaliiCount() {
        if (this.imprumutTipDetaliuList.length > 0) {
            return this.imprumutTipDetaliuList.length;
        } else {
            return 0;
        }
    }

    delete(detaliuId: number) {
        abp.message.confirm('Detaliul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this.isLoading = true;
                    this._imprumutTipService.deleteImprumutTipDetaliu(detaliuId).pipe(
                        delay(1000),
                        finalize(() => {
                            this.isLoading = false;
                        })
                    ).subscribe(() => {
                        this.getImprumuturiTipDetaliuList();
                        abp.notify.success(this.l('DeleteMessage'));
                    });
                }
            });

    }
}