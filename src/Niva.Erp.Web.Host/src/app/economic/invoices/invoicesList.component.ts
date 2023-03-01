import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from '../../../shared/AppConsts';
import { GetThirdPartyOutput, InvoiceDTO, InvoiceServiceProxy, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invoicesList.component.html',
    animations: [appModuleAnimation()]
})
export class InvoicesListComponent extends AppComponentBase implements OnInit {

    invoicesList: InvoiceDTO[] = [];
    thirdParties: GetThirdPartyOutput;
    dataStart: any;
    dataEnd: any;
    thirdPartyId: number;
    documentType: any;
    suma: number;
    url: string;
    thirdPartyName: any;
    facturiIncomplete: boolean = false;
    dateType: any = "ID";


    constructor(injector: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
        this.deleteInvoice2 = this.deleteInvoice2.bind(this);
        this.editInvoice = this.editInvoice.bind(this);
        this.showReport2 = this.showReport2.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Documente.Acces')) {
            this.initialize();
            this.refreshInvoices();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    onCellPrepared(e) {

        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    isVisibleButtonsRegistration(e) {
        if (e.row.key.documentTypeShortName === 'FF' || (e.row.key.documentTypeShortName !== 'BF' && e.row.key.documentTypeShortName !== 'FF')) {
            return true;
        }
        else {
            return false;
        }
    }

    isVisibleButtons(e) {
        if (!e.row.key.contaOperationStatus && !e.row.key.hasDecont/* && this.isGranted('Conta.Documente.Modificare')*/) {
            return true;
        }
        else {
            return false;
        }
    }

    isVisibleShowReport(e) {
        if (e.row.key.thirdPartyQualityStr === 'Client' && e.row.key.documentTypeShortName === 'FF') {
            return true;
        }
        else {
            return false;
        }
    }

    initialize() {
        this.dataStart = sessionStorage.getItem('dataStart') ? moment(sessionStorage.getItem('dataStart')) : new Date(new Date().setMonth(new Date().getMonth() - 1));
        //this.dataStart = new Date(this.dataStart.setMonth(this.dataStart.getMonth() - 1));
        this.dataEnd = sessionStorage.getItem('dataEnd') ? moment(sessionStorage.getItem('dataEnd')) : new Date();
        this.documentType = sessionStorage.getItem('documentType') ? JSON.parse(sessionStorage.getItem('documentType')) : '';
        this.thirdPartyId = sessionStorage.getItem('thirdPartyId') ? JSON.parse(sessionStorage.getItem('thirdPartyId')) : "";
        this.thirdPartyName = sessionStorage.getItem('thirdPartyName') ? JSON.parse(sessionStorage.getItem('thirdPartyName')) : "";
        this.facturiIncomplete = sessionStorage.getItem('facturiIncomplete') ? JSON.parse(sessionStorage.getItem('facturiIncomplete')) : false;
    }

    getCount() {
        if (this.invoicesList.length > 0) {
            return this.invoicesList.length;
        } else {
            return 0;
        }
    }

    searchThirdParty(thirdPartyName: any) {
        this._personService.thirdPartySearch(thirdPartyName.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    selectedThirdParty(id: number, thirdPartyName: string) {
        this.thirdPartyId = id;
        sessionStorage.setItem('thirdPartyName', JSON.stringify(thirdPartyName));
        this.thirdPartyName = thirdPartyName;
        this.getThirdPartyName();
        this.searchThirdParty(thirdPartyName);
    }

    getThirdPartyName() {
        //if (this.thirdPartyId === null || this.thirdPartyId === undefined)
        //    return '';

        return (this.thirdPartyId === null || this.thirdPartyId === undefined) ? "" : this.thirdPartyName;
    }

    refreshInvoices() {
        this._invoiceService.getInvoices(moment(this.dataStart), moment(this.dataEnd), this.thirdPartyId, this.documentType, this.facturiIncomplete, this.suma, this.dateType).subscribe(result => {
            this.invoicesList = result;
            console.log(this.invoicesList)
            //  sessionStorage
            sessionStorage.setItem('dataStart', this.dataStart.toString());
            sessionStorage.setItem('dataEnd', this.dataEnd.toString());
            sessionStorage.setItem('thirdPartyId', JSON.stringify(this.thirdPartyId));
            sessionStorage.setItem('documentType', JSON.stringify(this.documentType));
            sessionStorage.setItem('facturiIncomplete', JSON.stringify(this.facturiIncomplete));


        });
    }

    showReport(invoiceId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.url += "InvoiceReport?invoiceId=" + invoiceId;

        window.open(this.url);
    }

    showReport2(e) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.url += "InvoiceReport?invoiceId=" + e.row.key.id;

        window.open(this.url);
    }

    deleteInvoice(invoiceId: number) {
        abp.message.confirm('Factura va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invoiceService.deleteInvoice(invoiceId)
                        .subscribe(() => {
                            this.refreshInvoices();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }

    deleteInvoice2(e) {
        console.log(e);
        abp.message.confirm('Factura va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invoiceService.deleteInvoice(e.row.key.id)
                        .subscribe(() => {
                            this.refreshInvoices();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }

    editInvoice(e) {
        this.router.navigate(['/app/economic/invoices/invoiceNew'], { queryParams: { facturaId: e.row.key.id } });
    }
}