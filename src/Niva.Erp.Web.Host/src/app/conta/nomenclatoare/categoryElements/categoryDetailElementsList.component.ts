import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { InvoiceElementsDetailsCategoryListDTO, InvoiceServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './categoryDetailElementsList.component.html',
    animations: [appModuleAnimation()]
})
export class CategoryDetailElementsListComponent extends AppComponentBase implements OnInit {

    elementDetailsCategoryList: InvoiceElementsDetailsCategoryListDTO[] = [];

    constructor(injector: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private router: Router     ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.CategElemFactura.Acces')) {
            this.getInvoiceElementsDetailsCategories();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getInvoiceElementsDetailsCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.elementDetailsCategoryList = result;
        });
    }

    getCount() {
        if (this.elementDetailsCategoryList.length == null) {
            return 0;
        } else {
           return  this.elementDetailsCategoryList.length;
        }
    }

    delete(elementId: number) {
        abp.message.confirm('Categoria va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invoiceService.deleteInvoiceElementsDetailsCategory(elementId)
                        .subscribe(() => {
                            this.getInvoiceElementsDetailsCategories();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });

    }

}