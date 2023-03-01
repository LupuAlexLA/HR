import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { InvObjectOperListDto, InvObjectOperServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectOperation.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectOperationComponent extends AppComponentBase implements OnInit {

    operList: InvObjectOperListDto = new InvObjectOperListDto();
    invObjectReportParams: any = {};
    url: string = '';

    constructor(inject: Injector,
        private _invObjectOperService: InvObjectOperServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Operatii.Acces')) {
            if (sessionStorage.getItem('operListInvObjectOperation')) {
                this.operList = JSON.parse(sessionStorage.getItem('operListInvObjectOperation'));
                this.searchOper();
            }
            else {
                this._invObjectOperService.initOperList().subscribe(result => {
                    this.operList = result;
                    this.searchOper();
                });
            }
        } else {
            this.router.navigate(['app/home']);
        }
        
    }

    searchOper() {
        sessionStorage.setItem('operListInvObjectOperation', JSON.stringify(this.operList));
        this._invObjectOperService.operList(this.operList).subscribe(result => {
            this.operList = result;
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
                    this._invObjectOperService.deleteOperation(detailId).subscribe
                        (() => {
                            abp.notify.info('DeleteMessage');
                            this.searchOper();
                        })
                }
            });
    }

    generareRaport(operationId: number, operationTypeId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.invObjectReportParams.inventoryType = 1;
        if (operationTypeId === 9) {
            this.invObjectReportParams.operationId = operationId;
            this.url += "BonConsumReport?";
            this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);
        } else {
            this.invObjectReportParams.operationId = operationId;
            this.url += "BonTransferReport?";
            this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);
        }

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
    
}