import { Component } from '@angular/core';
import { Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    AccountServiceProxy, GetAccountOutput, GetAssetOutput, GetInvCategoryOutput, GetInvObjectStorageOutput, InvObjectAddDto, InvObjectAddInvoiceDetailDto,
    InvObjectCategoryServiceProxy, InvObjectServiceProxy, InvObjectStorageServiceProxy, InvoiceServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectAddInvoice.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectAddInvoiceComponent extends AppComponentBase implements OnInit {
    oper: InvObjectAddDto = new InvObjectAddDto();
    invoiceDetails: InvObjectAddInvoiceDetailDto[] = [];
    invoiceList: GetAssetOutput;
    storageList: GetInvObjectStorageOutput;
    categoryList: GetInvCategoryOutput;
    accounts: GetAccountOutput;

    constructor(inject: Injector,
        private _invObjectService: InvObjectServiceProxy,
        private _accountService: AccountServiceProxy,
        private _invCategService: InvObjectCategoryServiceProxy,
        private _invObjectStorageService: InvObjectStorageServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Intrari.Modificare')) {
            this._invObjectService.addFromInvoiceInit().subscribe(result => {
                this.oper = result;
                this.searchInvoices();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    searchInvoices() {
        this._invoiceService.invoicesForInvObjects().subscribe(result => {
            this.invoiceList = result;
        });
    }

    showInvoiceDetails() {
        this._invObjectService.getInvoiceDetails(this.oper.invoiceId, this.oper.documentTypeId, moment(this.oper.operationDate)).subscribe(result => {
            this.oper.invoiceDetail = result;
            this.showForm(2);
            this.searchStorage();
        });
    }

    searchStorage() {
        this._invObjectStorageService.invObjectStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    prepareInvObjects() {
        this._invObjectService.prepareInvObjects(this.oper).subscribe(result => {
            this.oper = result;
            this.searchAccounts();
            this.searchCategory();
            this.showForm(3);
        })
    }

    searchAccounts() {
        this._accountService.invObjAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    searchCategory() {
        this._invCategService.categoryList().subscribe(result => {
            this.categoryList = result;
        });
    }

    saveInvObjects() {
        this._invObjectService.saveObject(this.oper).subscribe(result => {
            abp.notify.info('AddMessage');
            if (result.finishAdd) {
                this.router.navigate(['/app/conta/invObjects/invObject']);
            }
        });
    }

    showForm(formNr: number) {
        this.oper.showForm1 = (formNr == 1);
        this.oper.showForm2 = (formNr == 2);
        this.oper.showForm3 = (formNr == 3);
    }

}