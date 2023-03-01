import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { DiurnaListDto, DiurnaServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({

    templateUrl: './diurnaList.component.html',
    animations: [appModuleAnimation()]
})
export class DiurnaListComponent extends AppComponentBase implements OnInit {

    diurnaList: DiurnaListDto[] = [];

    constructor(inject: Injector,
        private _diurnaService: DiurnaServiceProxy,
        private router: Router       ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.DiurnaLegala.Acces')) {
            this.getDiurnaList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getDiurnaList() {
        this._diurnaService.diurnaLegalaList().subscribe(result => {
            this.diurnaList = result;
        });
    }

    getDiurnaCount() {
        if (this.diurnaList.length === null) {
            return 0;
        } else {
            return this.diurnaList.length;
        }
    }

    delete(diurnaId: number) {
        abp.message.confirm('Tara de deplasare va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._diurnaService.deleteDiurnaLegala(diurnaId)
                        .subscribe(() => {
                            this.getDiurnaList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }
}