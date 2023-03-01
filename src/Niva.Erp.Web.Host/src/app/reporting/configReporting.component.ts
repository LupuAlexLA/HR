import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../shared/animations/routerTransition";
import { AppComponentBase } from "../../shared/app-component-base";
import { AppConsts } from "../../shared/AppConsts";
import { CalcRapListDto, ReportConfigServiceProxy } from "../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './configReporting.component.html',
    animations: [appModuleAnimation()]
})
export class ConfigReportingComponent extends AppComponentBase implements OnInit {
    reportParams: any = {};
    reportId: any = 0;
    reportList: CalcRapListDto[] = [];
    isDateRange: boolean = false;
    startDate: any;
    endDate: any;
    currentDate: Date;
    url: string;
    rulaj: boolean = false;
    convertToLocalCurrency: boolean = true;
    constructor(inject: Injector,
        private _repConfigService: ReportConfigServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Rapoarte.RapoarteConfigurabile.Acces')) {
            this._repConfigService.calcReportList().subscribe(result => {
                this.reportList = result;
                this.reportParams.reportId = 0;
                this.endDate = moment(this.currentDate);
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }
    showDateRange() {
        this.currentDate = new Date();
        if (this.isDateRange === false) {
            this.endDate = moment(this.currentDate);
           
            this.rulaj = false;
        } else {
            this.endDate = moment(this.currentDate);
            this.startDate = moment(this.currentDate.setMonth(this.currentDate.getMonth() - 1));
            this.rulaj = true;
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        this.url += "ConfigReport";
        if (this.isDateRange === true) {
            this.reportParams.dataStart = moment(this.startDate).format('M.DD.YYYY');
            this.reportParams.dataEnd = moment(this.endDate).format('M.DD.YYYY');
            this.reportParams.isDateRange = this.isDateRange;
            this.reportParams.rulaj = this.rulaj;
            this.reportParams.reportId = this.reportId;
            this.reportParams.convertToLocalCurrency = this.convertToLocalCurrency;
            this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);

        } else {
            this.reportParams.isDateRange = this.isDateRange;
            this.reportParams.dataEnd = moment(this.endDate).format('M.DD.YYYY');
            this.reportParams.rulaj = this.rulaj;
            this.reportParams.reportId = this.reportId;
            this.reportParams.convertToLocalCurrency = this.convertToLocalCurrency;
            this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
        }

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }

}