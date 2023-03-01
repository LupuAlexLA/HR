import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SitFinanCalcForm, SitFinanCalcServiceProxy } from '../../../shared/service-proxies/service-proxies';
@Component({
    templateUrl: './sitFinanCalc.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanCalcComponent extends AppComponentBase implements OnInit {

    form: SitFinanCalcForm = new SitFinanCalcForm();
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _sitFinanCalService: SitFinanCalcServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.SitFinan.Calcul.Acces')) {
            if (sessionStorage.getItem('startDate') && sessionStorage.getItem('endDate') && sessionStorage.getItem('isDailyBalance')) {

                this.form.startDate = sessionStorage.getItem('startDate') ? moment(sessionStorage.getItem('startDate')) : this.form.startDate;
                this.form.endDate = sessionStorage.getItem('endDate') ? moment(sessionStorage.getItem('endDate')) : this.form.endDate;
                this.form.isDailyBalance = sessionStorage.getItem('isDailyBalance') ? JSON.parse(sessionStorage.getItem('isDailyBalance')) : this.form.isDailyBalance;
                this.calcList();
            } else {

                this._sitFinanCalService.calcInit().subscribe(result => {
                    this.form = result;

                });
            }
        } else {
            this.router.navigate(['app/home']);
        }
    }

    calcRapoarte(balantaId: number) {

        abp.message.confirm('Rapoartele vor fi calculate / recalculate. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this.isLoading = true;
                    this._sitFinanCalService.calcRapoarte(balantaId, this.form).pipe(
                        delay(1000),
                        finalize(() => {
                            this.isLoading = false;
                        }))
                        .subscribe(result => {
                        abp.notify.info('Calculul s-a realizat cu succes!');
                        this.form = result;
                    });

                }
            }
        );
    }

    deleteCalcRapoarte(balantaId: number) {
        abp.message.confirm('Calculul va fi sters. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._sitFinanCalService.deleteCalcRapoarte(balantaId).subscribe(result => {
                        abp.notify.info('DeleteMessage');
                        this.calcList();
                    });

                }
            }
        );
    }

    calcList() {
        this._sitFinanCalService.calcList(this.form).subscribe(result => {
            this.form = result;
            sessionStorage.setItem('startDate', this.form.startDate.toString());
            sessionStorage.setItem('endDate', this.form.endDate.toString());
            sessionStorage.setItem('isDailyBalance', JSON.stringify(this.form.isDailyBalance));
        });
    }
}