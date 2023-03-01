import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { EnumServiceProxy, EnumTypeDto, InvoiceElementsDetailsCategoryEditDTO, InvoiceServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './categoryDetailElementsNew.component.html',
    animations: [appModuleAnimation()]
})
export class CategoryDetailElementsNewComponent extends AppComponentBase implements OnInit {

    elementDetailsCategory: InvoiceElementsDetailsCategoryEditDTO = new InvoiceElementsDetailsCategoryEditDTO();
    categoryTypeList: EnumTypeDto[] = [];
    elementId: any;
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: ActivatedRoute,
        private route: Router) {
        super(injector);
    }

    ngOnInit() {
        this.elementId = this.router.snapshot.queryParamMap.get('id');

        if (this.elementId != null) {
            this._invoiceService.getInvoiceElementsDetailsCategory(this.elementId).subscribe(result => {
                this.elementDetailsCategory = result;               
            });
        }
        this.getCategoryTypes();
    }

    getCategoryTypes() {
        this._enumService.categoryTypeList().subscribe(result => {
            this.categoryTypeList = result;
        });
    }

    saveInvoiceCategoryElement() {
        this.isLoading = true;
        this._invoiceService.saveInvoiceCategoryElement(this.elementDetailsCategory).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                abp.notify.info(this.l('ElementAddedMessage'));
            })
        ).subscribe(() => { });
    }
}