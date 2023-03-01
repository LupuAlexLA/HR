import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AutoInvForm, InvoiceAutoOperationServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './autoInvoices.component.html',
    animations: [appModuleAnimation()]
})
export class AutoInvoicesComponent extends AppComponentBase implements OnInit {

    form: AutoInvForm = new AutoInvForm();

    constructor(inject: Injector,
        private _invoiceAutoOperationService: InvoiceAutoOperationServiceProxy,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.Facturi.Acces')) {
            this.form.showProcessedForm = true;
            this._invoiceAutoOperationService.autoInvInit().subscribe(result => {
                this.form = result;
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    processedList() {
        this._invoiceAutoOperationService.processedList(this.form).subscribe(result => {
            this.form = result;
        });
    }

    startContare() {
        this.form.showProcessedForm = false;
        this.form.showNotProcessedForm = true;
        this.form.showDeleteForm = false;
    }

    startDelete() {
        this.form.showProcessedForm = false;
        this.form.showNotProcessedForm = false;
        this.form.showDeleteForm = true;
    }

    backToList() {
        this.form.showProcessedForm = true;
        this.form.showNotProcessedForm = false;
        this.form.showDeleteForm = false;
        this.form.invoices = null;
    }

    notProcessedList() {
        this._invoiceAutoOperationService.notProcessedList(this.form).subscribe(result => {
            this.form = result;
        });
    }

    saveToConta() {
        this._invoiceAutoOperationService.saveToConta(this.form).subscribe(result => {
            this.form = result;
        });
    }

    deleteList() {
        this._invoiceAutoOperationService.deleteList(moment(this.form.dataStart), moment(this.form.dataEnd)).subscribe(result => {
            this.form = result;
        });
    }

    deleteOperations() {
        abp.message.confirm(
            'Notele contabile vor fi sterse. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._invoiceAutoOperationService.eraseOperations(this.form).subscribe(result => {
                        this.form = result;
                        abp.notify.info('OKStergere');
                    });      
                }
            }
        );
    }
}