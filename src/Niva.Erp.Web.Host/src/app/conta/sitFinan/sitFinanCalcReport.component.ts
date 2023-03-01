import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SitFinanCalcReportForm, SitFinanCalcServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { SitFinanCalcDetailsDialogComponent } from './sitFinanCalc-dialog/sitFinanCalcDetailsDialog.component';

@Component({
    templateUrl: './sitFinanCalcReport.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanCalcReportComponent extends AppComponentBase implements OnInit {

    balanceId: number;
    reportId: number;
    form: SitFinanCalcReportForm = new SitFinanCalcReportForm();
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _sitFinanCalcService: SitFinanCalcServiceProxy,
        private _modalService: BsModalService) {
        super(inject);
    }

    ngOnInit() {
        this.balanceId = +this.route.snapshot.queryParamMap.get('balanceId');

        this._sitFinanCalcService.calcDetInit(this.balanceId).subscribe(result => {
            this.form = result;
        });
    }

    calcDetRap(reportId: number) {
        this._sitFinanCalcService.calcDetRap(reportId, this.form).subscribe(result => {
            this.form = result;
            this.reportId = reportId;
        });
    }

    showNotaDetail(reportId: number) {
        this._sitFinanCalcService.showNotaDetail(reportId, this.form).subscribe(result => {
            this.form = result;
        });
    }

    showValueDetails(rowId: number, columnId: number) {
        this._sitFinanCalcService.showValueDetails(rowId, columnId, this.form).subscribe(result => {
            this.form = result;
        });
    }

    saveValuesRecalc() {
        abp.message.confirm(
            'Modificarile vor fi salvate si totalurile vor fi recalculate. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._sitFinanCalcService.saveValuesRecalc(this.form)
                        .subscribe(result => {
                            abp.notify.info('S-au salvat modificarile!');
                            this.form = result;
                       });
                }
            }
        );
    }

    calcRaport(balanceId: number, raportId: number) {
        abp.message.confirm(
            'Raportul va fi recalculat. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this.isLoading = true;
                    this._sitFinanCalcService.calcRaport(balanceId, raportId, this.form).pipe(
                        delay(1000),
                        finalize(() => {
                            this.isLoading = false;
                        })
                    )
                        .subscribe(result => {
                            abp.notify.info('Raportul a fost recalculat cu succes!');
                            this.form = result;
                        });

                }
            }
        );
    }

    valueDetailsBack() {
        this.form.showReport = true;
        this.form.showValueDetail = false;
    }

    saveNota() {
        abp.message.confirm(
            'Modificarile vor fi salvate. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._sitFinanCalcService.saveNota(this.form)
                        .subscribe(result => {
                            abp.notify.info('S-au salvat modificarile!');
                            this.form = result;
                        });                 
                }
            }
        );
    }

    recalcNota() {
        abp.message.confirm(
            'Nota va fi recalculata. Sigur?',
            null,
            (isConfirmed: boolean) =>  {
                if (isConfirmed) {
                    this._sitFinanCalcService.recalcNota(this.form)
                        .subscribe(result => {
                            abp.notify.info('Nota a fost recalculata cu succes!');
                            this.form = result;
                        });
                }
            }
        );
    }

    showDetails(columnId: number) {
        let showSitFinanCalcDetails: BsModalRef;

        showSitFinanCalcDetails = this._modalService.show(
            SitFinanCalcDetailsDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    columnId: columnId,
                    reportId: this.reportId,
                    balanceId: this.balanceId,
                },
            }
        );
    }

}