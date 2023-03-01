import { Component, Injector, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PaapDepartamentListDto, PaapDto, PaapServiceProxy, PaapTranseListDto } from '../../../shared/service-proxies/service-proxies';
import { delay, finalize } from 'rxjs/operators';

@Component({

    animations: [appModuleAnimation()],
    templateUrl: './paapValidator.component.html',
    styleUrls: ['paapValidator.component.css'],
})
/** paapValidator component*/
export class PaapValidatorComponent extends AppComponentBase implements OnInit {
    /** paapValidator ctor */
    paapListByYear: PaapDto[] = [];
    paapTranseList: PaapTranseListDto[] = [];
    cautareAni: number[] = [];
    cautareAn: number = new Date().getFullYear();;
    dataSource = new MatTableDataSource();
    displayedColumns = ['Categorie', 'Ianuarie', 'Februarie', 'Martie', 'Aprilie', 'Mai', 'Iunie', 'Iulie', 'August', 'Septembrie', 'Octombrie', 'Noiembrie', 'Decembrie'];
    dictionarTotal: Record<number, number> = {};
    paapByDepartament: PaapDepartamentListDto[] = [];
    isLoading: boolean = false;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _paapService: PaapServiceProxy) {
        super(injector);
    }
    ngOnInit(): void {
        if (this.isGranted('Buget.PAAP.Aprobare')) {
            this.getPaapListByYear(new Date().getFullYear());
            this.getPAAPYearList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    filterCategories(paapTranseList: PaapTranseListDto[]) {
        let categorieObiect = new Map();
        let filteredCategorieObj = [];


        for (const paap of this.paapTranseList.filter(f => f.statePAAP !== 'Anulat')) {
            console.log(paap);
            if (categorieObiect.has(paap.invoiceElementsDetailsCategoryName)) {
                let obj = categorieObiect.get(paap.invoiceElementsDetailsCategoryName)
                if (isNaN(obj[paap.dataTransa.month()])) {
                    obj[paap.dataTransa.month()] = paap.valoareLei;
                }
                else {

                    obj[paap.dataTransa.month()] += paap.valoareLei;
                }
                categorieObiect.set(paap.invoiceElementsDetailsCategoryName, obj);
            }
            else {
                let obj = {}
                obj[paap.dataTransa.month()] = paap.valoareLei;
                categorieObiect.set(paap.invoiceElementsDetailsCategoryName, obj);
            }
        }

        for (let categorie of categorieObiect) {
            let obj = {};
            obj = categorie[1];
            obj['categorie'] = categorie[0];


            filteredCategorieObj.push(obj);
        }
        return filteredCategorieObj;
    }

    getPAAPYearList() {
        this._paapService.getPAAPYearList().subscribe(result => {
            this.cautareAni = result;
        });
    }

    getPaapByDepartments(year: number) {
        this._paapService.getPAAPByDepartament(year).subscribe(result => {
            this.paapByDepartament = result;
        });
    }

    getPaapListByYear(year: number) {
        //this._paapService.getPAAPListByYear(year).subscribe(result => {
        //    this.paapListByYear = result;
        //    this.dataSource = new MatTableDataSource(this.filterCategories(this.paapListByYear));
        //});
        this._paapService.getPAAPListTranseByYear(year).subscribe(result => {
            this.paapTranseList = result;
            this.dataSource = new MatTableDataSource(this.filterCategories(this.paapTranseList));
        })
        //this.dataSource = new MatTableDataSource(this.paapListByYear);
        this.getPaapByDepartments(year);
    }

    calculateTotal(luna: number) {
        //    return this.paapListByYear.filter(t => t.dataEnd.month() == luna && t.statePAAP !== 'Anulat').map(t => t.value).reduce((acc, value) => acc + value, 0);
        return this.paapTranseList.filter(t => t.dataTransa.month() == luna && t.statePAAP !== 'Anulat').reduce((acc, paap) => acc + paap.valoareLei, 0);
    }

    approvePaap() {
        this.isLoading = true;
        this._paapService.approvePaapList(this.cautareAn, this.paapByDepartament)
            .pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(() => {
            abp.notify.info("Planul a fost aprobat");
            this.getPaapListByYear(this.cautareAn);
        });
    }

    // anulez aprobarea pe anul selectat => achizitiilr trec in stare 'Inregistrat'
    cancelAllApprovedPaap() {
        abp.message.confirm('Planificarea pentru anul ' + this.cautareAn + ' va fi anulata. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this.isLoading = true;
                    this._paapService.cancelAllApprovedPaap(this.cautareAn)
                        .pipe(
                            delay(1000),
                            finalize(() => {
                                this.isLoading = false;
                            }))
                        .subscribe(() => {
                            this.getPaapListByYear(this.cautareAn);
                            abp.notify.info(this.l('Aprobarea planului a fost anulata'));
                        });
                }
            });
    }
}