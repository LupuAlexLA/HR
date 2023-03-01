import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BalanceServiceProxy, GetInvObjectOutput, InvObjectServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObject.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectComponent extends AppComponentBase implements OnInit {

    invObjectList: GetInvObjectOutput = new GetInvObjectOutput();
    dataStart: Date;
    dataEnd: Date;

    constructor(inject: Injector,
        private _invObjectService: InvObjectServiceProxy,
        private _balanceService: BalanceServiceProxy,
        private router: Router) {
        super(inject);
        this.edit = this.edit.bind(this); 
        this.deleteInvObject = this.deleteInvObject.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Intrari.Acces')) {
            this.initialize();
            this.getInvObjectList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    initialize() {
        this._balanceService.getBalanceDateNextDay().subscribe(result => {
            this.dataStart = sessionStorage.getItem('dataStartInvObject') ? moment(sessionStorage.getItem('dataStartInvObject')).toDate() : result.toDate();
            this.dataEnd = sessionStorage.getItem('dataEndInvObject') ? moment(sessionStorage.getItem('dataEndInvObject')).toDate() : new Date();
            this.getInvObjectList();
        })
    }

    getInvObjectList() {
        this._invObjectService.invObjectsList(moment(this.dataStart), moment(this.dataEnd)).subscribe(result => {
            this.invObjectList = result;

            sessionStorage.setItem('dataStartInvObject', this.dataStart.toString());
            sessionStorage.setItem('dataEndInvObject', this.dataEnd.toString());
        });
    }

    getInvObjectListCount() {
        if (this.invObjectList.getInvObjects == null) {
            return 0;
        } else {
            return this.invObjectList.getInvObjects.length;
        }
    }

    deleteInvObject(e) {
        abp.message.confirm('Inregistrarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invObjectService.deleteInvObject(e.row.key.id).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.getInvObjectList();
                    });
                }
            });
    }

    edit(e) {
        this.router.navigate(['/app/conta/invObjects/invObjectAddDirect'], { queryParams: { invObjectId: e.row.key.id}});
    }

    showDeleteButton(e) {
        if (e.row.key.processed) {
            return false;
        } else {
            return true;
        }
    }
}