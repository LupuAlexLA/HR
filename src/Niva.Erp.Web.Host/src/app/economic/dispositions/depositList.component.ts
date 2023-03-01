import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DepositListDto, DepositServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './depositList.component.html',
    animations: [appModuleAnimation()]
})
export class DepositListComponent extends AppComponentBase implements OnInit {

    depositList: DepositListDto[] = [];
    dataStart: Date;
    dataEnd: Date;
    operationState: number ;

    constructor(inject: Injector,
        private _depositService: DepositServiceProxy,
        private router: Router,
    ) {
        super(inject);
    }

    ngOnInit() {
        
        if (this.isGranted('Casierie.Numerar.DepunRetrag.Acces')) {
            if (sessionStorage.getItem("dataStartDepositList")) {
                this.dataStart = JSON.parse(sessionStorage.getItem("dataStartDepositList"));
            }
            else {
                this.dataStart = new Date();
                this.dataStart = new Date(this.dataStart.setMonth(this.dataStart.getMonth() - 1));
            }

            if (sessionStorage.getItem("dataEndDepositList")) {
                this.dataEnd = JSON.parse(sessionStorage.getItem("dataEndDepositList"));
            }
            else {
                this.dataEnd = new Date();
            }

            if (sessionStorage.getItem("operationStateDepositList")) {
                this.operationState = JSON.parse(sessionStorage.getItem("operationStateDepositList"));
            }
            else {
                this.operationState = null;
            }

            this.getDepositList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
        

    }

    getDepositList() {
        console.log(this.operationState);
        sessionStorage.setItem("dataStartDepositList", JSON.stringify(this.dataStart));
        sessionStorage.setItem("dataEndDepositList", JSON.stringify(this.dataEnd));
        sessionStorage.setItem("operationStateDepositList", JSON.stringify( this.operationState));
        this._depositService.getDepositList(moment(this.dataStart), moment(this.dataEnd), this.operationState).subscribe(result => {
            this.depositList = result;
        });
    }

    getCount() {
        if (this.depositList.length == null) {
            return 0;
        } else {
            return this.depositList.length;
        }
    }

    delete(depositId: number) {
        abp.message.confirm('Operatiunea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._depositService.deleteDeposit(depositId)
                        .subscribe(() => {
                            this.getDepositList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }
}