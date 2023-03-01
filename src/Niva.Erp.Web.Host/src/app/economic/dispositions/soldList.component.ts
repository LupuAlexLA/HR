import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SoldInitialDto, SoldInitialServiceProxy } from '../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './soldList.component.html',
    animations: [appModuleAnimation()]
})
export class SoldListComponent extends AppComponentBase implements OnInit {
    dataStart: Date;
    dataEnd: Date;
    soldList: SoldInitialDto[] = [];

    constructor(inject: Injector,
        private _soldInitialService: SoldInitialServiceProxy,
        private router: Router
    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Casierie.Numerar.SoldInitial.Acces')) {
            if (sessionStorage.getItem("dataStartSoldList")) {
                this.dataStart = JSON.parse(sessionStorage.getItem("dataStartSoldList"));
            }
            else {
                this.dataStart = new Date();
                this.dataStart = new Date(this.dataStart.setMonth(this.dataStart.getMonth() - 1));
            }

            if (sessionStorage.getItem("dataEndSoldList")) {
                this.dataEnd = JSON.parse(sessionStorage.getItem("dataEndSoldList"));
            }
            else {
                this.dataEnd = new Date();
            }

            this.getSoldList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getSoldList() {
        sessionStorage.setItem("dataStartSoldList", JSON.stringify(this.dataStart));
        sessionStorage.setItem("dataEndSoldList", JSON.stringify(this.dataEnd));
        this._soldInitialService.soldList(moment(this.dataStart), moment(this.dataEnd)).subscribe(result => {
            this.soldList = result;
        });
    }

    getSoldCount() {
        if (this.soldList.length == 0) {
            return 0;
        } else {
            return this.soldList.length;
        }
    }

    delete(soldId: number) {
        abp.message.confirm('Soldul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._soldInitialService.deleteSold(soldId)
                        .subscribe(() => {
                            this.getSoldList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }
}