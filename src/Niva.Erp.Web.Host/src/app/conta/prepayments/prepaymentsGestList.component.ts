import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PrepaymentBalanceServiceProxy, PrepaymentsBalanceListDto, PrepaymentsDDDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsGestList.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsGestListComponent extends AppComponentBase implements OnInit {

    gestList: PrepaymentsBalanceListDto = new PrepaymentsBalanceListDto();
    prepaymentType: number;
    prepaymentList: PrepaymentsDDDto[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentBalanceService: PrepaymentBalanceServiceProxy,
        private router: Router ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Administrare.CheltAvans.TipuriDoc.Acces') || this.isGranted('Administrare.VenitAvans.CalculGestiune.Acces')) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentBalanceService.initForm(this.prepaymentType).subscribe(result => {
                // this.gestList = result;

                if (sessionStorage.getItem('dataStartPrepaymentsGestList')) {
                    this.gestList.dataStart = moment(sessionStorage.getItem('dataStartPrepaymentsGestList'));
                }
                else {
                    this.gestList.dataStart = result.dataStart;
                }
                if (sessionStorage.getItem('dataEndPrepaymentsGestList')) {
                    this.gestList.dataEnd = moment(sessionStorage.getItem('dataEndPrepaymentsGestList'));
                }
                else {
                    this.gestList.dataEnd = result.dataEnd;
                }
                if (JSON.parse(sessionStorage.getItem('prepaymentIdPrepaymentsGestList'))) {
                    this.gestList.prepaymentId = JSON.parse(sessionStorage.getItem('prepaymentIdPrepaymentsGestList'));
                }
                else {
                    this.gestList.prepaymentId = result.prepaymentId;
                }
                this.gestList.gestDetail = result.gestDetail;


                this.searchGest();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchPrepayments() {
        this._prepaymentBalanceService.prepaymentsListDD(this.gestList.dataStart, this.gestList.dataEnd, this.prepaymentType).subscribe(result => {
            this.prepaymentList = result;

            sessionStorage.setItem('dataStartPrepaymentsGestList', this.gestList.dataStart.toString());
            sessionStorage.setItem('dataEndPrepaymentsGestList', this.gestList.dataEnd.toString());
            sessionStorage.setItem('prepaymentIdPrepaymentsGestList', JSON.stringify(this.gestList.prepaymentId));
        });
    }

    

    searchGest() {
        this._prepaymentBalanceService.serchGest(this.gestList).subscribe(result => {
            this.gestList = result;
            this.searchPrepayments();
        });
    }

    getGestListCount() {
        if (this.gestList.gestDetail == null) {
            return 0;
        } else {
            return this.gestList.gestDetail.length;
        }
    }

}