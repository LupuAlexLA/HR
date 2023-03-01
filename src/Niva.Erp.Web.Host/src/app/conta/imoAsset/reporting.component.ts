import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { BalanceDDDto, BalanceServiceProxy, GetImoAssetStorageOutput, ImoAssetStorageServiceProxy, ImoInventariereServiceProxy, InventoryDDList, ReportsServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './reporting.component.html',
    animations: [appModuleAnimation()]
})
export class ReportingImoAssetComponent extends AppComponentBase implements OnInit {

    exportType: {};
    selectedReport = "0";
    repDate = new Date();
    balanceId: any;
    inventoryNr: any;
    storageId: any;
    url: string = '';
    existInvNr: number;
    imoAssetReportParams: any = {};
    inventoryType: any;

    balanceDD: BalanceDDDto[] = [];
    storageList: GetImoAssetStorageOutput = new GetImoAssetStorageOutput();
    storageInList: GetImoAssetStorageOutput;
    storageOutList: GetImoAssetStorageOutput;
    imoAssetList: InventoryDDList[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _balanceService: BalanceServiceProxy,
        private _imoAssetStorage: ImoAssetStorageServiceProxy,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private _reportService: ReportsServiceProxy,
        private _imoInventariereService: ImoInventariereServiceProxy,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Rapoarte.Acces')) {
            this.inventoryType = this.route.snapshot.queryParamMap.get('inventoryType');
            this.balanceList();
            this.storageListFnc();
            this.storageInFnc();
            this.storageOutFnc();
            this.invObjectDateList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    invObjectDateList() {
        this._imoInventariereService.imoOperDateList().subscribe(result => {
            this.imoAssetList = result;
        });
    }

    balanceList() {
        this._balanceService.balanceDDList().subscribe(result => {
            this.balanceDD = result;
            if (this.balanceDD.length != 0) {
                this.balanceId = this.balanceDD[0].id;
            }
        });
    }

    storageListFnc() {
        this._imoAssetStorage.imoAssetStorageList().subscribe(result => {
            this.storageList = result;
            this.storageId = null;

        });
    }

    storageInFnc() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageInList = result;
        });
    }

    storageOutFnc() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageOutList = result;
        });
    }

    
    inventoryNrExists(inventoryNr: number) {
        this._reportService.inventoryNrExists(inventoryNr).subscribe(result => {
            this.existInvNr = result;
        });
        return this.existInvNr;
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        var date = moment(this.repDate).format('M.DD.YYYY');

        switch (this.selectedReport) {
            case "0":
                this.imoAssetReportParams = {};

                this.imoAssetReportParams.repDate = date;
                this.imoAssetReportParams.storage = this.storageId;
                this.url += "AssetRegReport?";
                this.url += this.ConvertToQueryStringParameters(this.imoAssetReportParams);
                break;
            case "1":
                this.imoAssetReportParams = {};
                var existInvNr = this.inventoryNrExists(this.inventoryNr);

                if (existInvNr == 1) {
                    abp.notify.error("Eroare", "Nu exista numarul de inventar introdus");
                    return;
                }

                this.imoAssetReportParams.repDate = date;
                this.imoAssetReportParams.inventoryNr = this.inventoryNr;
                this.url += "AssetFisaReport?";
                this.url += this.ConvertToQueryStringParameters(this.imoAssetReportParams);
                break;
            case "2":
                this.imoAssetReportParams = {};
                this.imoAssetReportParams.storage = this.storageId;
                this.imoAssetReportParams.inventoryType = this.inventoryType;
                this.url += "InvObjectAssetReport?";
                this.url += this.ConvertToQueryStringParameters(this.imoAssetReportParams);
                break;
            case "3":
                this.imoAssetReportParams = {};
                this.imoAssetReportParams.repDate = date;
                this.imoAssetReportParams.storage = this.storageId;
                this.url += "AssetRegReportV2?";
                this.url += this.ConvertToQueryStringParameters(this.imoAssetReportParams);
                break;
            default:
        }

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}