import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './detailElementsList.component.html',
    animations: [appModuleAnimation()]
})
export class DetailElementsListComponent extends AppComponentBase implements OnInit {

    detailElementsList: InvoiceElementsDetailsDTO[] = [];
    element: any;
    account: any;
    invoiceElementsDetailsCategory: any;
    invoiceElementsDetailsCategoryList: InvoiceElementsDetailsCategoryListDTO[] = [];

    constructor(inject: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private router: Router) {
        super(inject);
        this.delete = this.delete.bind(this);
        this.editDetailElement = this.editDetailElement.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.ElemFactura.Acces')) {
            // this.getInvoiceElementsDetails();
            this.searchInvoiceElementDetails();
            this.getInvoiceElementsCategories();

        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getInvoiceElementsCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.invoiceElementsDetailsCategoryList = result;
        });
    }

    getInvoiceElementsDetails() {
        this._invoiceService.getInvoiceElementsDetails().subscribe(result => {
            this.detailElementsList = result;
        });
    }

    getCount() {
        if (this.detailElementsList.length > 0) {
            return this.detailElementsList.length;
        } else {
            return 0;
        }
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    editDetailElement(e) {
        this.router.navigate(['/app/conta/nomenclatoare/categoryElements/detailElementsNew'], { queryParams: { id: e.row.key.id } });
    }

    delete(e) {
        abp.message.confirm('Elementul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invoiceService.deleteInvoiceElementsDetail(e.row.key.id)
                        .subscribe(() => {
                            this.getInvoiceElementsDetails();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });

    }

    searchInvoiceElementDetails() {
        this._invoiceService.searchInvoiceElementsDetails(this.element, this.account, this.invoiceElementsDetailsCategory).subscribe(result => {
            this.detailElementsList = result;
        });
    }
}