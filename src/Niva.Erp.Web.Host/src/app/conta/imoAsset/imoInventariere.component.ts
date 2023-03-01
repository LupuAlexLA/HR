import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { ImoInventariereInitDto, ImoInventariereServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './imoInventariere.component.html',
    animations: [appModuleAnimation()]
})
export class ImoInventariereComponent extends AppComponentBase implements OnInit {

    imoInventariere: ImoInventariereInitDto = new ImoInventariereInitDto();
    invObjectReportParams: any = {};
    url: string = '';

    constructor(inject: Injector,
        private _imoInvService: ImoInventariereServiceProxy,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Inventar.Acces')) {
            this.initFormFnc();
        }
        else {
            this.router.navigate(['/app/home']);
        }
        
    }

    initFormFnc() {
        this._imoInvService.initForm().subscribe(result => {
           //this.imoInventariere = result;

            this.imoInventariere.dateStart = sessionStorage.getItem('dateStartImoInventariere') ? moment(sessionStorage.getItem('dateStartImoInventariere')) : result.dateStart;
            this.imoInventariere.dateEnd = sessionStorage.getItem('dateEndImoInventariere') ? moment(sessionStorage.getItem('dateEndImoInventariere')) : result.dateEnd;
            this.imoInventariere.imoInventariereList = result.imoInventariereList;
        });
    }

    getImoInventariereCount() {
        if (this.imoInventariere.imoInventariereList == null) {
            return 0;
        } else {
            return this.imoInventariere.imoInventariereList.length;
        }
    }

    search() {
        this._imoInvService.searchImoInventariere(moment(this.imoInventariere.dateStart), moment(this.imoInventariere.dateEnd)).subscribe(result => {
            this.imoInventariere.imoInventariereList = result;

            sessionStorage.setItem('dateStartImoInventariere', this.imoInventariere.dateStart.toString());
            sessionStorage.setItem('dateEndImoInventariere', this.imoInventariere.dateEnd.toString());
        });
    }

    delete(imoInvId: number) {
        abp.message.confirm('Inregistrarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._imoInvService.deleteImoInventariere(imoInvId).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.search();
                    });
                }
            });
    }


    showReport(invOperId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.invObjectReportParams.invOperId = invOperId;
        this.invObjectReportParams.inventoryType = 0;
        this.url += "InvObjectAssetReport?";
        this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);

        window.open(this.url);
    }


    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}