import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { DispositionListDto, DispositionServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './dispositionList.component.html',
    animations: [appModuleAnimation()]
})
export class DispositionListComponent extends AppComponentBase implements OnInit {

    dispositionList: DispositionListDto[] = [];
    dataStart: Date;
    dataEnd: Date;
    dispState: any;
    dispositionReportParams: any = {};
    url: string = '';

    constructor(inject: Injector,
        private router: Router,
        private _dispositionService: DispositionServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Casierie.Numerar.Dispozitii.Acces')) {
            if (sessionStorage.getItem("dataStartDispositionList")) {
                this.dataStart = JSON.parse(sessionStorage.getItem("dataStartDispositionList"));
            }
            else {
                this.dataStart = new Date();
                this.dataStart = new Date(this.dataStart.setMonth(this.dataStart.getMonth() - 1));
            }

            if (sessionStorage.getItem("dataEndDispositionList")) {
                this.dataEnd = JSON.parse(sessionStorage.getItem("dataEndDispositionList"));
            }
            else {
                this.dataEnd = new Date();
            }

            if (sessionStorage.getItem("dispState")) {
                this.dispState = JSON.parse(sessionStorage.getItem("dispState"));
            } else {
                this.dispState = "";
            }

            this.getDispositionList();
        }
   else {
            this.router.navigate(['/app/home']);
        }
        

    }

    getCount() {
        if (this.dispositionList.length == 0) {
            return 0;
        } else {
            return this.dispositionList.length;
        }
    }


    getDispositionList() {
        sessionStorage.setItem("dataStartDispositionList", JSON.stringify(this.dataStart));
        sessionStorage.setItem("dataEndDispositionList", JSON.stringify(this.dataEnd));
        sessionStorage.setItem("dispState", this.dispState);
        this._dispositionService.dispositionList(moment(this.dataStart), moment(this.dataEnd), this.dispState).subscribe(result => {
            this.dispositionList = result;
        });
    }

    delete(dispositionId: number) {
        abp.message.confirm('Operatiunea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._dispositionService.deleteDisposition(dispositionId)
                        .subscribe(() => {
                            this.getDispositionList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }

    showReport(dispositionId: number, operTypeId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.dispositionReportParams.dispositionId = dispositionId;
        this.dispositionReportParams.operationTypeId = operTypeId;

        this.url += "DispositionReport?";
        this.url += this.ConvertToQueryStringParameters(this.dispositionReportParams);

        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }

}