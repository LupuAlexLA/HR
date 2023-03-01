import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { CotaTVAListDto, CotaTVAServiceProxy, PaapDto, PaapServiceProxy, DepartamentListDto, DepartamentServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { ShowAmanarePAAPDialogComponent } from "./amanare-paap/amanare-paap-dialog.component";
import { ShowCancelPAAPDialogComponent } from "./cancel-paap/cancel-paap-dialog.component";
import { ShowRelocarePAAPDialogComponent } from "./realocare/realocare-paap-dialog.component";

@Component({
    templateUrl: './paapList.component.html',
    animations: [appModuleAnimation()]
})
export class PaapListComponent extends AppComponentBase implements OnInit {
    paapListByYearFiltered: PaapDto[] = [];
    paapListByYear: PaapDto[] = [];
    cautareAni: number[] = [];
    cautareAn: number = new Date().getFullYear();
    departamentList: string[] = [];
    departament: string = '';
    categorieList: string[] = [];
    categorie: string = '';
    endDateRange: Date = null;
    startDateRange: Date = null;
    rangeDate: any = [];
    coteTVA: CotaTVAListDto[] = [];
    coteTvaPaapId: Record<number, CotaTVAListDto[]>;
    showTVA: boolean = false;
    json: JSON;
    achizitieFaraTranse: boolean = false;
    nefinalizat30zile: boolean = false;
    nefinalizatDepasit: boolean = false;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _paapService: PaapServiceProxy,
        private _modalService: BsModalService,
        private _cotaTVAService: CotaTVAServiceProxy,
        private route: ActivatedRoute,
        private _departamentService: DepartamentServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.PAAP.Acces')) {
            if (this.route.snapshot.queryParamMap.get('nefinalizat30zile') == 'true') {
                this.nefinalizat30zile = true;
            }

            if (this.route.snapshot.queryParamMap.get('nefinalizatDepasit') == 'true') {
                this.nefinalizatDepasit = true;
            }

            if (sessionStorage.getItem('cautareAnPaapList')) {
                this.cautareAn = JSON.parse(sessionStorage.getItem('cautareAnPaapList'));
            }
            if (sessionStorage.getItem('departamentPaapList')) {
                this.departament = JSON.parse(sessionStorage.getItem('departamentPaapList'));
            }
            if (sessionStorage.getItem('categoriePaapList')) {
                this.categorie = JSON.parse(sessionStorage.getItem('categoriePaapList'));
            }
            if (sessionStorage.getItem('startDatePaapList')) {
                this.startDateRange = JSON.parse(sessionStorage.getItem('startDatePaapList'));
                this.rangeDate[0] = new Date(this.startDateRange);
            }
            if (sessionStorage.getItem('endDatePaapList')) {
                this.endDateRange = JSON.parse(sessionStorage.getItem('endDatePaapList'));
                this.rangeDate[1] = new Date(this.endDateRange);
            }
            if (sessionStorage.getItem('achizitieFaraTranse')) {
                this.achizitieFaraTranse = JSON.parse(sessionStorage.getItem('achizitieFaraTranse'));
            }
            this.getDepartamentList();
            this.getUserDeptName();

            this.getPaapListByYearFnc(this.cautareAn);
            this.getPaapListByYear(this.cautareAn);
            this.getPAAPYearList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getPaapListByYear(year: number) {
        this._paapService.getPAAPListByYear(year).subscribe(result => {
            this.paapListByYear = result;
            //this.departamentList = Array.from(new Set(result.map(x => x.departamentName)));
            this.categorieList = Array.from(new Set(result.map(x => x.invoiceElementsDetailsCategoryName)));
        });
    }

    getPaapListByYearFnc(year: number) {
        sessionStorage.setItem('cautareAnPaapList', JSON.stringify(this.cautareAn));
        sessionStorage.setItem('departamentPaapList', JSON.stringify(this.departament));
        sessionStorage.setItem('categoriePaapList', JSON.stringify(this.categorie));
        sessionStorage.setItem('achizitieFaraTranse', JSON.stringify(this.achizitieFaraTranse));

        if (this.rangeDate?.length > 0) {
            sessionStorage.setItem('startDatePaapList', JSON.stringify(this.rangeDate[0]));
            sessionStorage.setItem('endDatePaapList', JSON.stringify(this.rangeDate[1]));
        } else {
            sessionStorage.removeItem('startDatePaapList');
            sessionStorage.removeItem('endDatePaapList');
        }

        if (this.showTVA) {
            this._paapService.getPAAPListByYear(year).subscribe(result => {
                //this.paapListByYear = result;
                this.paapListByYearFiltered = result.filter(option => option.invoiceElementsDetailsCategoryName.includes(this.categorie) && option.statePAAP === 'Inregistrat');
                if (this.departament != null) {
                    this.paapListByYearFiltered = this.paapListByYearFiltered.filter(option => option.departamentName.includes(this.departament));
                }

                if (this.rangeDate?.length > 0) {
                    this.startDateRange = this.rangeDate[0];
                    this.endDateRange = this.rangeDate[1];
                    this.paapListByYearFiltered = this.paapListByYearFiltered.filter(option => option.dataEnd.format("DD-MM-YYYY") >= moment(this.startDateRange).format("DD-MM-YYYY") &&
                        option.dataEnd.format("DD-MM-YYYY") <= moment(this.endDateRange).format("DD-MM-YYYY"));
                }

                this._cotaTVAService.tvaListsPaapList(this.paapListByYearFiltered).subscribe(result => {
                    this.coteTvaPaapId = result;
                });
            });
        }
        else {
            this.isLoading = true;
            this._paapService.getPAAPListByYear(year).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })).subscribe(result => {
                    //this.paapListByYear = result;
                    this.paapListByYearFiltered = result.filter(option => option.invoiceElementsDetailsCategoryName.includes(this.categorie));
                    if (this.departament != null) {
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(option => option.departamentName.includes(this.departament));
                    }

                    if (this.achizitieFaraTranse === true) {
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(f => f.transe.length === 0);
                    }
                    else {
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(f => f.transe.length > 0);
                    }

                    if (this.nefinalizat30zile) {
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(f =>
                            f.dataEnd.diff(moment(), 'days') < 30 && f.dataEnd.diff(moment(), 'days') > 0 && f.statePAAP != 'Finalizat'
                        );
                    }

                    if (this.nefinalizatDepasit) {
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(f =>
                            f.dataEnd.diff(moment(), 'days') < 0 && f.statePAAP != 'Finalizat'
                        );
                    }

                    if (this.rangeDate?.length > 0) {
                        this.startDateRange = this.rangeDate[0];
                        this.endDateRange = this.rangeDate[1];
                        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(option => option.dataEnd.format("DD-MM-YYYY") >= moment(this.startDateRange).format("DD-MM-YYYY") &&
                            option.dataEnd.format("DD-MM-YYYY") <= moment(this.endDateRange).format("DD-MM-YYYY"));
                    }
                    this.checkSum();
                });
        }
    }

    anChange() { // updateaza lista de departamente si categorii cand se selecteaza un an nou
        this._paapService.getPAAPListByYear(this.cautareAn).subscribe(result => {
            /*this.departament = '';*/
            this.categorie = '';
            this.rangeDate = null;
            //this.departamentList = Array.from(new Set(result.map(x => x.departamentName)));
            this.categorieList = Array.from(new Set(result.map(x => x.invoiceElementsDetailsCategoryName)));
            this.paapListByYear = result;
        });
    }

    departamentChange() {
        this.categorieList = Array.from(new Set(this.paapListByYear.filter(x => x.departamentName.includes(this.departament)).map(x => x.invoiceElementsDetailsCategoryName)));
    }

    categorieChange() {
        //this.departamentList = Array.from(new Set(this.paapListByYear.filter(x => x.invoiceElementsDetailsCategoryName.includes(this.categorie)).map(x => x.departamentName)));
    }

    getPAAPYearList() {
        this._paapService.getPAAPYearList().subscribe(result => {
            this.cautareAni = result;
        });
    }

    paapListCount() {
        if (this.paapListByYearFiltered.length > 0) {
            return this.paapListByYearFiltered.length;
        } else {
            return 0;
        }
    }

    delete(paapId: number) {
        abp.message.confirm('Achizitia va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._paapService.deletePaap(paapId)
                        .subscribe(() => {
                            this.getPaapListByYearFnc(this.cautareAn);
                            abp.notify.info(this.l('Achizitia a fost stearsa'));
                        });
                }
            });
    }

    // Anulare PAAP
    cancelPAAP(paapId: number) {
        this.showCancelPAAPDialog(paapId);
    }

    // Deschide modal pentru anularea PAAP-ului
    showCancelPAAPDialog(paapId: number) {
        let showCancelPAAPDialog: BsModalRef;

        showCancelPAAPDialog = this._modalService.show(
            ShowCancelPAAPDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    paapId: paapId
                },
            }
        );

        showCancelPAAPDialog.content.onSave.subscribe(() => {
            this.getPaapListByYearFnc(this.cautareAn);
        });
    }

    // Amanare PAAP
    amanarePAAP(paapId: number) {
        this.showAmanarePAAPDialog(paapId);
    }

    // Deschide modal pentru amanarea PAAP-ului
    showAmanarePAAPDialog(paapId: number) {
        let showAmanarePAAPDialog: BsModalRef;

        showAmanarePAAPDialog = this._modalService.show(
            ShowAmanarePAAPDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    paapId: paapId
                },
            }
        );

        showAmanarePAAPDialog.content.onSave.subscribe(() => {
            this.getPaapListByYearFnc(this.cautareAn);
        });
    }

    // Realocare PAAP
    realocarePAAP(paapId: number) {
        this.showRealocarePAAPDialog(paapId);
    }
    showRealocarePAAPDialog(paapId: number) {
        let showRealocarePAAPDialog: BsModalRef;

        showRealocarePAAPDialog = this._modalService.show(
            ShowRelocarePAAPDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    paapId: paapId
                },
            }
        );
        showRealocarePAAPDialog.content.onSave.subscribe(() => {
            this.getPaapListByYearFnc(this.cautareAn);
        });
    }

    showCategory(index: number) {
        this.paapListByYearFiltered[index].showCategory = true;
    }

    hideCategory(index: number) {
        this.paapListByYearFiltered[index].showCategory = false;
    }

    getCoteTVAList() {
        this._cotaTVAService.getTVAList().subscribe(result => {
            this.coteTVA = result;
        });
    }

    getCoteById(id: number) {
        if (this.coteTvaPaapId) {
            return this.coteTvaPaapId[id];
        }
        return [];
    }

    showCoteTVA() {
        this.showTVA = true;
        this.paapListByYearFiltered = this.paapListByYearFiltered.filter(f => f.statePAAP === 'Inregistrat');
        this._cotaTVAService.tvaListsPaapList(this.paapListByYearFiltered).subscribe(result => {
            this.coteTvaPaapId = result;
        });
    }

    hideCoteTVA() {
        this.showTVA = false;
        this.getPaapListByYearFnc(this.cautareAn);
        this.getPaapListByYear(this.cautareAn);
        this.getPAAPYearList();
    }

    saveCoteTVA() {
        this._cotaTVAService.saveCoteTVAForPaap(this.paapListByYearFiltered).subscribe(() => {
            abp.notify.info("Cotele au fost modificate");
            this.showTVA = false;
            this.getPaapListByYearFnc(this.cautareAn);
        });
    }

    calculateValuesForSelectedTVA(paapId: number, index: number) {
        this._cotaTVAService.calculateValueByTVA(paapId, this.paapListByYearFiltered[index].cotaTVA_Id).subscribe(result => {
            this.paapListByYearFiltered[index] = result;
        });
    }

    checkSum() {
        this._paapService.checkEqualSumPAAPTranse(this.paapListByYearFiltered).subscribe(result => {
            this.paapListByYearFiltered = result;
        });
    }

    getUserDeptName() {
        this._paapService.getUserDeptName().subscribe(result => {
            if (result != "") {
                this.departament = result;
            }
        });
    }

    getDepartamentList() {
        this._departamentService.getDepartamentNumeList().subscribe(result => {
            this.departamentList = result;
        });
    }
}