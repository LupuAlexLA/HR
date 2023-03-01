import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetReinvest, BugetReinvestIncasari, BugetReinvestPlati, BugetTitluriCFViewCurrenciesList, BugetVenituriServiceProxy, GetCurrencyOutput, PersonServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { BugetReinvDialogComponent } from "./bugetReinvest-dialog/bugetReinvDialog.component";
import { TabelReinvDialogComponent } from "./TabelReinvest-dialog/TabelReinvestDialog.component";


@Component({
    templateUrl: './bugetReinvest.component.html',
    animations: [appModuleAnimation()]
})
export class BugetReinvestComponent extends AppComponentBase implements OnInit {

    bugetReinvestStart: BugetReinvest = new BugetReinvest();
    bugetReinvestIncasari: BugetReinvestIncasari[] = [];
    bugetReinvestPlati: BugetReinvestPlati[] = [];
    isLoading: boolean = false;
    formularBVCId: any;
    currenciesList: GetCurrencyOutput;
    sumarBugetReinvestit: BugetTitluriCFViewCurrenciesList[] = [];
    sumarTotalValoareIncasataLei: any;
    sumarTotalvaloareReinvestitaLei: any;
    sumarTotalPlasamentLei: any;
    

    constructor(inject: Injector,
        private _bugetVenituriService: BugetVenituriServiceProxy,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private _modalService: BsModalService,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Buget.BVC.Venituri.Acces")) {
            this.formularBVCId = +this.route.snapshot.queryParamMap.get('formularBVCId');
            this.BugetReinvestStart();
        } else {
            this.router.navigate(['app/home']);
        }
    }


    BugetReinvestStart() {
        this._bugetVenituriService.bugetReinvestStart(this.formularBVCId).subscribe(result => {
            this.bugetReinvestStart = result;

        });
    }

    getBugetReinvestIncasari(startDate, endDate) {
        this.isLoading = true;
        this._bugetVenituriService
            .reinvestIncas(this.formularBVCId, moment(startDate, "dd/MM/yyyy"), moment(endDate))
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(result => {
                this.bugetReinvestIncasari = result;
                this.getCurrenciesList();
            });

    }

    getCurrenciesList() {
        this._personService.currencyList().subscribe(result => {
            this.currenciesList = result;
        });
    }

    deleteReinv(i, j) {
        this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[j].delete = true;
        //this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[0].sumaReinvestita += Number(this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[j].sumaReinvestita);
        this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[0].sumaIncasata += Number(this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[j].sumaIncasata);
        this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[0].sumaReinvestita += Number(this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[j].sumaIncasata);
    }

    addReinv(bVC_VenitTitluCFId, i) {
        let addReinvDialog: BsModalRef;

        addReinvDialog = this._modalService.show(
            BugetReinvDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    bVC_VenitTitluCFId: bVC_VenitTitluCFId,
                    formularBVCId: this.formularBVCId
                },
            }
        );

        addReinvDialog.content.onSave.subscribe(result => {
            this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[0].sumaIncasata -= result.sumaIncasata;
            this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[0].sumaReinvestita -= result.sumaIncasata;
            this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv.push(result);
        });
    }

    tabelReinv() {



        this._bugetVenituriService
            .tableDate(this.formularBVCId, moment(this.bugetReinvestStart.startDate, "dd/MM/yyyy"), moment(this.bugetReinvestStart.endDate))

            .subscribe(result => {
                let tabelReinvDialog: BsModalRef;

                tabelReinvDialog = this._modalService.show(
                    TabelReinvDialogComponent,
                    {
                        class: 'modal-lg',
                        initialState: {
                            tableData: result,
                        },
                    }
                );

            });



    }

    getBugetReinvestPlati(startDate, endDate) {
        this.isLoading = true;
        this._bugetVenituriService
            .reinvestPlati(this.formularBVCId, moment(startDate), moment(endDate))
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(result => {
                this.bugetReinvestPlati = result;
            });
    }

    search() {
        this.getBugetReinvestIncasari(this.bugetReinvestStart.startDate, this.bugetReinvestStart.endDate);
        this.getBugetReinvestPlati(this.bugetReinvestStart.startDate, this.bugetReinvestStart.endDate);
        this.sumarVenituri();
    }

    saveReinvest() {
        this.isLoading = true;
        this._bugetVenituriService
            .reinvestSave(this.bugetReinvestIncasari)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            ).subscribe(() => {
                abp.notify.success("Sumele au fost reinvestite");
                this.search();
            });
    }

    changeSumReinvest(incasareId: number, reinvId: number) {
        var venitReinv = this.bugetReinvestIncasari[incasareId].bvC_VenitTitluCFReinv[reinvId];
        this._bugetVenituriService.changeSumReinvest(this.formularBVCId, venitReinv).subscribe(result => {
            this.bugetReinvestIncasari[incasareId].bvC_VenitTitluCFReinv[reinvId].sumaReinvestita = result.sumaReinvestita;
            this.bugetReinvestIncasari[incasareId].bvC_VenitTitluCFReinv[reinvId].cursValutar = result.cursValutar;
        });
    }

    computeCurs(i, j) {
        this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[i].sumaReinvestita = this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[i].sumaIncasata *
            this.bugetReinvestIncasari[i].bvC_VenitTitluCFReinv[i].cursValutar;
    }

    sumarVenituri() {
        this._bugetVenituriService.titluriViewCurrencyList(this.formularBVCId, moment(this.bugetReinvestStart.startDate), moment(this.bugetReinvestStart.endDate)).subscribe(result => {
            this.sumarBugetReinvestit = result;
            this.sumarTotalValoareIncasataLei = result.reduce((total, { valoareIncasataLei }) => total + valoareIncasataLei, 0);
            this.sumarTotalvaloareReinvestitaLei = result.reduce((total, { valoareReinvestitaLei }) => total + valoareReinvestitaLei, 0);
            this.sumarTotalPlasamentLei = result.reduce((total, { valoarePlasamentLei }) => total + valoarePlasamentLei, 0);
            console.log()
        });
    }
}