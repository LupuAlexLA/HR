import { Component, Injector, OnInit } from "@angular/core";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../shared/animations/routerTransition";
import { AppComponentBase } from "../../shared/app-component-base";
import { AccountListDDDto, AccountServiceProxy, InvoiceDetailsReportDto, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceServiceProxy, ReportsServiceProxy } from "../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './invoiceDetailsReport.component.html',
    animations: [appModuleAnimation()]

})
export class InvoiceDetailsReportComponent extends AppComponentBase implements OnInit {

    invoiceDetails: InvoiceDetailsReportDto = new InvoiceDetailsReportDto();
    invoiceElementsDetailsCategoryList: InvoiceElementsDetailsCategoryListDTO[] = [];
    invoiceElementsDetails: InvoiceElementsDetailsDTO[] = [];
    accountList: AccountListDDDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _reportService: ReportsServiceProxy,
        private _invoiceService: InvoiceServiceProxy, 
        private _accountService: AccountServiceProxy) {
        super(inject);
    }

    ngOnInit(): void {
        this.isLoading = true;
        this._reportService.invoiceDetailsInit()
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                 this.invoiceDetails = result;
                 this.getInvoiceElementsCategories();
            });
    }


    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    getInvoiceElementsCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.invoiceElementsDetailsCategoryList = result;
        });
    }

    getInvoiceElementDetailsByCategoryId(invoiceElementsDetailsCategoryId: number) {
        this._invoiceService.getInvoiceElementsDetailsByCategoryId(invoiceElementsDetailsCategoryId).subscribe(result => {
            this.invoiceElementsDetails = result;
        });
    }

    getAccountList(search: any) {
        this._accountService.accountListAll(search.target.value).subscribe(result => {
            this.accountList = result;
        });
    }

    getAccountName(accountId: number) {
        if (!accountId)
            return '';
        return this.invoiceDetails.contCheltuiala;
    }

    selectedInput(itemName: string, itemId: number) {
        this.invoiceDetails.contCheltuialaId = itemId;
        this.invoiceDetails.contCheltuiala = itemName;
    }

    search() {
        this._reportService.searchInvoiceDetails(this.invoiceDetails).subscribe(result => {
            this.invoiceDetails = result;
        });
    }

    closeTab() {
        window.close();
    }

}