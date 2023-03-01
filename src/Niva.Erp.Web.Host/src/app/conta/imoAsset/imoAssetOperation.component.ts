import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { ImoAssetOperListDto, ImoAssetOperServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetOperation.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetOperationComponent extends AppComponentBase implements OnInit {

    operList: ImoAssetOperListDto = new ImoAssetOperListDto();
    imoAssetReportParams: any = {};
    url: string = '';

    constructor(inject: Injector,
        private _imoAssetOperService: ImoAssetOperServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Operatii.Acces')) {
            this._imoAssetOperService.initOperList().subscribe(result => {
                //this.operList = result;
                this.operList.dataStart = sessionStorage.getItem('dataStartImoAssetOperation') ? moment(sessionStorage.getItem('dataStartImoAssetOperation')) : result.dataStart;
                this.operList.dataEnd = sessionStorage.getItem('dataEndImoAssetOperation') ? moment(sessionStorage.getItem('dataEndImoAssetOperation')) : result.dataEnd;

                this.searchOper();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchOper() {
        this._imoAssetOperService.operList(this.operList).subscribe(result => {
            this.operList = result;

            sessionStorage.setItem('dataStartImoAssetOperation', this.operList.dataStart.toString());
            sessionStorage.setItem('dataEndImoAssetOperation', this.operList.dataEnd.toString());
        });
    }



    operListCount() {
        if (this.operList.listDetail == null) {
            return 0;
        } else {
            return this.operList.listDetail.length;
        }
    }

    deleteOperationFnc(detailId: number) {
        abp.message.confirm('Operatia va fi stearsa. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._imoAssetOperService.deleteOperation(detailId).subscribe
                        (() => {
                            abp.notify.info('DeleteMessage');
                            this.searchOper();
                        })
                }
            });
    }

    generareRaport(operationId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.imoAssetReportParams.inventoryType = 0;
        this.imoAssetReportParams.operationId = operationId;
        this.url += "BonTransferReport?";
        this.url += this.ConvertToQueryStringParameters(this.imoAssetReportParams);

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}
