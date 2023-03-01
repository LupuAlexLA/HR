import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BalanceServiceProxy, PrepaymentsListDto, PrepaymentsServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './prepaymentsList.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsListComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    dataStart: Date;
    dataEnd: Date;
    prepaymentsList: PrepaymentsListDto[] = [];
    modCalcul: number;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentService: PrepaymentsServiceProxy,
        private _balanceService: BalanceServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Conta.CheltAvans.Chelt.Acces")) {
            this.prepaymentType = + this.route.snapshot.queryParamMap.get('prepaymentType');
            this.initialize();
            this._prepaymentService.durationSetupDetails(this.prepaymentType).subscribe(result => {
                this.modCalcul = result.prepaymentDurationCalcId;
            });
        } else if (this.isGranted("Conta.VenitAvans.Venituri.Acces")) {
            this.prepaymentType = + this.route.snapshot.queryParamMap.get('prepaymentType');
            this.initialize();
            this._prepaymentService.durationSetupDetails(this.prepaymentType).subscribe(result => {
                this.modCalcul = result.prepaymentDurationCalcId;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    initialize() {
        this._balanceService.getBalanceDateNextDay().subscribe(result => {
            this.dataStart = sessionStorage.getItem('dataStartPrepaymentsList') ? moment(sessionStorage.getItem('dataStartPrepaymentsList')).toDate() : result.toDate();
            this.dataEnd = sessionStorage.getItem('dataEndPrepaymentsList') ? moment(sessionStorage.getItem('dataEndPrepaymentsList')).toDate() : new Date();
            this.prepaymentsListFnc();
        })
    }

    prepaymentsListFnc() {
        this._prepaymentService.prepaymentsEntryList(this.prepaymentType, moment(this.dataStart), moment(this.dataEnd)).subscribe(result => {
            this.prepaymentsList = result;

            sessionStorage.setItem('dataStartPrepaymentsList', this.dataStart.toString());
            sessionStorage.setItem('dataEndPrepaymentsList', this.dataEnd.toString());
        });
    }

    getPrepaymentsListCount() {
        if (this.prepaymentsList.length == null) {
            return 0;
        } else {
            return this.prepaymentsList.length;
        }
    }

    deletePrepayment(prepaymentId: number) {
        abp.message.confirm(
            'Inregistrarea va fi stearsa. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._prepaymentService.deletePrepayment(prepaymentId).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.prepaymentsListFnc();
                    });
                }
            }
        )
    }
}