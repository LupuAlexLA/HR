import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { SitFinanCalcBalanceListDto, SitFinanCalcRapListDto, SitFinanRapServiceProxy, SitFinanReportColumn } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './sitFinanRap.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanRapComponent extends AppComponentBase implements OnInit {

    sitFinanParams: any = {};
    sitFinanRapParams: any = {};
    balanceId: any = null;
    raportId: any = null;
    balanceList: SitFinanCalcBalanceListDto[] = [];
    raportList: SitFinanCalcRapListDto[] = [];
    url: string = "";
    colNr: number = 0;
    isDailyBalance: boolean = false;
    isDateRange: boolean = false;
    startDate: Date;
    endDate: Date;
    columnList: SitFinanReportColumn[] = [];

    constructor(inject: Injector,
        private _sitFinanRapService: SitFinanRapServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.startDate = new Date();
        this.endDate = new Date();

        if (this.isGranted('Conta.SitFinan.Rapoarte.Acces')) {
            this.getCalculatedBalanceList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getCalculatedBalanceList() {
        this._sitFinanRapService.calcBalanceList(this.isDailyBalance).subscribe(result => {
            this.balanceList = result;
        });

    }

    getCalcRaportList(saveBalanceId: number) {
        this._sitFinanRapService.calcRaportList(saveBalanceId).subscribe(result => {
            this.raportList = result;
        });
    }

    getCalcRaportListByDateRange() {
        this._sitFinanRapService.getCalcRaportListByDateRange(moment(this.startDate), moment(this.endDate), this.isDailyBalance).subscribe(result => {
            this.raportList = result;
        });
    }

    showDateRange() {
        this.endDate = new Date();
        this.startDate = new Date(this.endDate.getFullYear(), 0, 1, 12);
        this.getCalcRaportListByDateRange();

        this.balanceId = (this.isDailyBalance === false && this.isDateRange === false) ? null : this.balanceId;
    }

    columnsList(raportId: number) {
        this._sitFinanRapService.getReportColumnList(raportId).subscribe(result => {
            this.columnList = result;
        });
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.sitFinanParams.raportId = this.raportId;

        if (this.isDailyBalance === false && this.isDateRange === false) {
            this.sitFinanParams.balanceId = this.balanceId;
            this._sitFinanRapService.raportColNumber(this.sitFinanParams.raportId).subscribe(result => {
                switch (result) {
                    case 2:
                        this.url += "SitFinanRapC2?";
                        this.url += this.ConvertToQueryStringParameters(this.sitFinanParams);
                        break;
                    case 3:
                        this.url += "SitFinanRapC3?";
                        this.url += this.ConvertToQueryStringParameters(this.sitFinanParams);
                        break;
                    case 4:
                        this.url += "SitFinanRapC4?";
                        this.url += this.ConvertToQueryStringParameters(this.sitFinanParams);
                        break;
                    case 5:
                        this.url += "SitFinanRapC5?";
                        this.url += this.ConvertToQueryStringParameters(this.sitFinanParams);
                        break;
                    case 6:
                        this.url += "SitFinanRapC6?";
                        this.url += this.ConvertToQueryStringParameters(this.sitFinanParams);
                        break;
                    default:
                }
                window.open(this.url);
            });
        } else {
            this.sitFinanRapParams.raportId = this.raportId;
            this.sitFinanRapParams.startDate = moment(this.startDate).format('M.DD.YYYY');
            this.sitFinanRapParams.endDate = moment(this.endDate).format('M.DD.YYYY');
            this.sitFinanRapParams.colNumber = this.colNr;

            if (this.isDailyBalance) {
                this.sitFinanRapParams.balanceId = this.balanceId;
            }

            this.sitFinanRapParams.isDailyBalance = this.isDailyBalance ?? false;
            this.sitFinanRapParams.isDateRange = this.isDateRange ?? false;

            this.url += "SitFinanRap?";
            this.url += this.ConvertToQueryStringParameters(this.sitFinanRapParams);
            window.open(this.url);
        }
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}