import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { InvObjectInventariereInitDto, InvObjectInventariereServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './invObjectInventariere.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectInventariereComponent extends AppComponentBase implements OnInit {

    invObject: InvObjectInventariereInitDto = new InvObjectInventariereInitDto();
    invObjectReportParams: any = {};
    url: string = '';

    constructor(inject: Injector,
        private _invObjectInventariereService: InvObjectInventariereServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Inventar.Acces')) {
            if (sessionStorage.getItem('invObjectInvObjectInventariere')) {
                this.invObject = JSON.parse(sessionStorage.getItem('invObjectInvObjectInventariere'));
                this.search();
            }
            else {
                this.initFormFnc();
            }
        } else {
            this.router.navigate(['app/home']);
        }
    }

    initFormFnc() {
        this._invObjectInventariereService.initForm().subscribe(result => {
            this.invObject = result;
        });
    }

    getInvObjectInventariereCount() {
        if (this.invObject.invObjectInventariereList == null) {
            return 0;
        } else {
            return this.invObject.invObjectInventariereList.length;
        }
    }

    search() {
        sessionStorage.setItem('invObjectInvObjectInventariere', JSON.stringify(this.invObject));
        this._invObjectInventariereService.searchInvObjects(this.invObject).subscribe(result => {
            this.invObject = result;
        });
    }

    delete(invObjectId: number) {
        abp.message.confirm('Inregistrarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invObjectInventariereService.deleteInvObjectInventariere(invObjectId).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.search();
                    });
                }
            });
    }

    showReport(invOperId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.invObjectReportParams.invOperId = invOperId;
        this.invObjectReportParams.inventoryType = 1;
        this.url += "InvObjectAssetReport?";
        this.url += this.ConvertToQueryStringParameters(this.invObjectReportParams);

        window.open(this.url);
    }


    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}