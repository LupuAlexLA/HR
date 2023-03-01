import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { GetInvObjectStorageOutput, InventoryDDList, InvObjectStorageServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './reporting.component.html',
    animations: [appModuleAnimation()]
})
export class ReportingInvObjectComponent extends AppComponentBase implements OnInit {

    selectedReport = "0";
    repDate = new Date();
    storageId: any;
    invObjectId: any = null;
    url: string = '';
    inventoryType: any;
    storageList: GetInvObjectStorageOutput = new GetInvObjectStorageOutput();
    invObjectReportParams: any = {};
    invObjectList: InventoryDDList[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _invObjectStorage: InvObjectStorageServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Rapoarte.Acces')) {
            this.inventoryType = this.route.snapshot.queryParamMap.get('inventoryType');
            this.storageListFnc();
            // this.invObjectDateList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    storageListFnc() {
        this._invObjectStorage.invObjectStorageList().subscribe(result => {
            this.storageList = result;
            this.storageId = null;
        });
    }

    //invObjectDateList() {
    //    this._invObjectInventariereService.invDateList().subscribe(result => {
    //        this.invObjectList = result;
    //    });
    //}

    showReport() {
        console.log(this.selectedReport);
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        var date = moment(this.repDate).format('M.DD.YYYY');
        
        switch (this.selectedReport) {
            case "0":
                this.invObjectReportParams.repDate = date;
                this.invObjectReportParams.storageId = this.storageId;
                this.url += "InvObjectReport?";
                this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);
                break;
                //this.invObjectReportParams.repDate = date;
                //this.invObjectReportParams.storage = this.storageId;
                //this.invObjectReportParams.inventoryType = this.inventoryType;
                //this.url += "InvObjectAssetReport?";
                //this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);
                //break;
        }

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}