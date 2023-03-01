import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetRepartizatDto, BugetFormularListDto, BugetPreliminatListDto, BugetRepartizatServiceProxy, EnumServiceProxy, EnumTypeDto, BugetPrevServiceProxy, BugetPrevDDDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetRepartizat.component.html',
    animations: [appModuleAnimation()]
})
export class BugetRepartizatComponent extends AppComponentBase implements OnInit {


    formularBVCId: number = null;
    preliminatCalculTypeId: number = 0;
    preliminatBugetPrelimId: number = 0;
    preliminatId: number = 0;
    isLoading: boolean = false;
    bugetPreliminatNewList: BugetFormularListDto[] = [];
    bugetPreliminatList: BugetPreliminatListDto[] = [];
    preliminatCalculTypeList: EnumTypeDto[] = [];
    bugetAddManualList: BugetRepartizatDto[] = [];
    bugetAddBalantaList: BugetRepartizatDto[] = [];
    bugetPrelimList: BugetPrevDDDto[] = [];
    anSelectat: string
    showFormPrelimList: boolean = false;
    showFormStartAdd: boolean = false;
    showFormAddManual: boolean = false;
    showFormAddBalanta: boolean = false;
    showFormPrelimVenitStart: boolean = false;

    constructor(inject: Injector,
        private _bugetRepartizatService: BugetRepartizatServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _enumServiceProxy: EnumServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Preliminat")) {
            this.getBugetPreliminatList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    showHideForms(idAction) {
        this.showFormPrelimList = false;
        this.showFormStartAdd = false;
        this.showFormAddManual = false;
        this.showFormAddBalanta = false;
        this.showFormPrelimVenitStart = false;

        if (idAction == 1) {
            this.showFormPrelimList = true;
        }
        else if (idAction == 2) {
            this.showFormStartAdd = true;
        }
        else if (idAction == 3) {
            this.showFormAddManual = true;
        }
        else if (idAction == 4) {
            this.showFormAddBalanta = true;
        }
        else if (idAction == 5) {
            this.showFormPrelimVenitStart = true;
        }
    }

    getBugetPreliminatList() {
        this._bugetRepartizatService.bugetPreliminatList().subscribe(result => {
            this.bugetPreliminatList = result;
            this.showHideForms(1);
        });
    }

    getBugetPreliminatNewList() {
        this._bugetRepartizatService.bugetPreliminatNewList().subscribe(result => {
            this.bugetPreliminatNewList = result;
        });
    }

    getPreliminatCalculTypeList() {
        this._enumServiceProxy.preliminatCalculTypeList().subscribe(result => {
            this.preliminatCalculTypeList = result;
        });
    }

    getBugetRepartizatAddManual() {
        this._bugetRepartizatService.bugetRepartizatAddManual(this.formularBVCId).subscribe(result => {
            this.bugetAddManualList = result;
        });
    }

    addNewPreliminat() {
        this.preliminatCalculTypeId = 0;
        this.getBugetPreliminatNewList();
        this.getPreliminatCalculTypeList();
        this.showHideForms(2);
    }

    addNewPreliminatOption() {
        if (this.preliminatCalculTypeId == 0) {
            this.addNewPreliminatManual();
        }
        else if (this.preliminatCalculTypeId == 1) {
            this.bugetRepartizatAddBalanta();
        }
        else if (this.preliminatCalculTypeId == 2) {
            this.bugetRepartizatPrelimVenitStart();
        }
    }

    bugetRepartizatAddManualSave() {
        this.isLoading = true;
        this._bugetRepartizatService.bugetRepartizatAddManualSave(this.preliminatId, this.formularBVCId, this.bugetAddManualList)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(() => {
                this.getBugetPreliminatList();
                abp.notify.success("Inregistrarea a fost salvata cu succes");
            });
    }

    preliminatDetails(calculType, preliminatId, formularBVCId) {
        if (calculType == 0) { // adaugat manual
            this.formularBVCId = formularBVCId;
            this.preliminatId = preliminatId;
            this.bugetRepartizatManualDetails(preliminatId);
            this.showHideForms(3);
        }
        else if (calculType == 1) { // adaugat pornind de la ultima balanta
            this.formularBVCId = formularBVCId;
            this.preliminatId = preliminatId;
            this.bugetRepartizatBalantaDetails(preliminatId);
            this.showHideForms(4);
        }
        else if (calculType == 2) { // adaugat preliminand veniturile
            this.formularBVCId = formularBVCId;
            this.preliminatId = preliminatId;
            this.bugetRepartizatBalantaDetails(preliminatId);
            this.showHideForms(4);
        }
    }

    bugetRepartizatManualDetails(preliminatId) {
        this._bugetRepartizatService.bugetRepartizatManualDetails(preliminatId, this.formularBVCId).subscribe(result => {
            this.bugetAddManualList = result;
        });
    }

    bugetRepartizatDelete(preliminatId) {
        abp.message.confirm("Inregistrarea va fi stearsa. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this.isLoading = true;
                    this._bugetRepartizatService.bugetRepartizatDelete(preliminatId)
                        .pipe(
                            delay(1000),
                            finalize(() => {
                                this.isLoading = false;
                            }))
                        .subscribe(() => {
                            this.getBugetPreliminatList();
                            abp.notify.success("Inregistrarea a fost stearsa cu succes");
                        });
                }
            });
    }

    bugetRepartizatAddBalanta() {
        this.isLoading = true;
        this._bugetRepartizatService.bugetRepartizatAddBalanta(this.formularBVCId)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(result => {
                this.preliminatId = result;
                this.bugetRepartizatBalantaDetails(this.preliminatId);
                this.showHideForms(4);
                abp.notify.success("Inregistrarea a fost salvata cu succes");
            });
    }

    bugetRepartizatAddBalantaSave() {
        this.isLoading = true;
        this._bugetRepartizatService.bugetRepartizatAddBalantaSave(this.bugetAddBalantaList)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(() => {
                this.getBugetPreliminatList();
                abp.notify.success("Inregistrarea a fost salvata cu succes");
            });
    }

    bugetRepartizatBalantaDetails(preliminatId) {
        this._bugetRepartizatService.bugetRepartizatBalantaDetails(preliminatId, this.formularBVCId).subscribe(result => {
            this.bugetAddBalantaList = result;
        });
    }


    addNewPreliminatBack() {
        this.showHideForms(1);
    }

    addNewPreliminatManual() {
        this.preliminatId = 0;
        this.getBugetRepartizatAddManual();
        this.showHideForms(3);
    }

    addNewPreliminatManualBack() {
        if (this.preliminatId == 0)
            this.showHideForms(2);
        else
            this.showHideForms(1);
    }

    addNewPreliminatBalantaBack() {
        this.getBugetPreliminatList();
    }

    getBugetPreliminatLastYear() {
        this._bugetPrevService.bugetPreliminatLastYear(this.formularBVCId).subscribe(result => {
            this.bugetPrelimList = result;
        });
    }

    bugetRepartizatPrelimVenitStart() {
        this.isLoading = true;
        this.getBugetPreliminatLastYear();
        this.showHideForms(5);
    }

    bugetRepartizatPrelimVenitBack() {
        this.isLoading = true;
        this.showHideForms(2);
    }

    bugetRepartizatAddPrelimVenit() {
        this.isLoading = true;
        this._bugetRepartizatService.bugetRepartizatAddPrelimVenit(this.formularBVCId, this.preliminatBugetPrelimId)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(result => {
                this.preliminatId = result;
                this.bugetRepartizatBalantaDetails(this.preliminatId);
                this.showHideForms(4);
                abp.notify.success("Inregistrarea a fost salvata cu succes");
            });
    }

}