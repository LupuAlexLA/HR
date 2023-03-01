import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { CupiuriListDto, CupiuriServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './cupiuriList.component.html',
    animations: [appModuleAnimation()]
})
export class CupiuriListComponent extends AppComponentBase implements OnInit {

    cupiuriList: CupiuriListDto[] = [];
    dataStart: Date;

    constructor(inject: Injector,
        private _cupiuriService: CupiuriServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Casierie.Numerar.Cupiuri.Acces')) {
            if (sessionStorage.getItem("dataStartCupiuriList")) {
                this.dataStart = JSON.parse(sessionStorage.getItem("dataStartCupiuriList"));
            }
            else {
                this.dataStart = new Date();
            }

            this.initForm();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    initForm() {
        this._cupiuriService.initList().subscribe(result => {
            this.cupiuriList = result;
        });
    }

    getCupiuriCount() {
        if (this.cupiuriList.length == 0) {
            return 0;
        } else {
            return this.cupiuriList.length;
        }
    }

    getCupiuriList() {
        sessionStorage.setItem("dataStartCupiuriList", JSON.stringify(this.dataStart));
        this._cupiuriService.searchCupiuri(moment(this.dataStart)).subscribe(result => {
            this.cupiuriList = result;
        });
    }

    delete(cupiuriId: number) {
        abp.message.confirm("Cupiurile inregistrate vor fi sterse. Sigur?",
            null,
            (response: boolean) => {
                if (response) {
                    this._cupiuriService.delete(cupiuriId).subscribe(result => {
                        this.getCupiuriList();
                        abp.notify.info("Inregistrarea a fost stearsa");
                    });
                }
            });
    }
}