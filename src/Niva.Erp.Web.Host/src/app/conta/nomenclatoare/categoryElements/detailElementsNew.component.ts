import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import {
    AccountServiceProxy, EnumServiceProxy, EnumTypeDto, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO,
    InvoiceServiceProxy
} from '../../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './detailElementsNew.component.html',
    animations: [appModuleAnimation()]
})
export class DetailElementsNewComponent extends AppComponentBase implements OnInit {

    elementId: number;
    element: InvoiceElementsDetailsDTO = new InvoiceElementsDetailsDTO();
    elementDetailsCategoryList: InvoiceElementsDetailsCategoryListDTO[] = [];
    elementTypes: EnumTypeDto[] = [];
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _invoiceService: InvoiceServiceProxy,
        private _accountService: AccountServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        this.elementId = +this.route.snapshot.queryParamMap.get('id');

        if (this.elementId != 0) {
            this._invoiceService.getInvoiceElementsDetail(this.elementId).subscribe(result => {
                this.element = result;
            });
        } 

        this.elementTypeList();
        this.getElementsDetailCategory();
    }

    elementTypeList() {
        this._enumService.invoiceElementsTypeList().subscribe(result => {
            this.elementTypes = result;
        });
    }

    getElementsDetailCategory() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.elementDetailsCategoryList = result;
        });
    }

    saveElement() {
        this.isLoading = true;

        this._invoiceService.saveInvoiceElementDetail(this.element).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                abp.notify.info(this.l('ElementAddedMessage'));
            })
        ).subscribe(() => {  });
    }
}