import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';

@Component({
    templateUrl: './reporting.component.html',
    animations: [appModuleAnimation()]
})
export class ReportingComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    selectedReport = 0;
    prepaymentParams: any = {};
    url: string = '';
    repDate: Date;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("Conta.CheltAvans.Rapoarte.Acces")) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');
            this.prepaymentParams = {
                prepaymentType: this.prepaymentType
            };
            this.repDate = new Date();
        } else if (this.isGranted("Conta.VenitAvans.Venituri.Rapoarte")) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');
            this.prepaymentParams = {
                prepaymentType: this.prepaymentType
            };
            this.repDate = new Date();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.url += "PrepaymentsRegReport";
        this.prepaymentParams.repDate = moment(this.repDate).format('M.DD.YYYY');
        this.url += '?' + this.ConvertToQueryStringParameters(this.prepaymentParams);

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}