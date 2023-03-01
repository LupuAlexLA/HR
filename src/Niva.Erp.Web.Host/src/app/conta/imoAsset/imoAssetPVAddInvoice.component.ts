import { Component, Injector, OnInit } from '@angular/core';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    AccountServiceProxy, GetAccountOutput, GetAssetOutput, GetImoAssetStorageOutput, ImoAssetAddDto, ImoAssetAddInvoiceDetailDto, ImoAssetClassCodeEditDto, ImoAssetClassCodeListDDDto, ImoAssetClassCodeServiceProxy,
    ImoAssetServiceProxy, ImoAssetStorageServiceProxy, InvoiceServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetPVAddInvoice.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetPVAddInvoiceComponent extends AppComponentBase implements OnInit {

    oper: ImoAssetAddDto = new ImoAssetAddDto();
    invDetails: ImoAssetAddInvoiceDetailDto[] = [];
    invoiceList: GetAssetOutput;
    storageList: GetImoAssetStorageOutput;
    classCode: ImoAssetClassCodeListDDDto[] = [];
    accounts: GetAccountOutput;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _imoAssetService: ImoAssetServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private _imoAssetClassCodeService: ImoAssetClassCodeServiceProxy,
        private _accountService: AccountServiceProxy
    ) {
        super(inject);
    }

    ngOnInit() {
        this._imoAssetService.addFromInvoiceInit().subscribe(result => {
            this.oper = result;
            this.showForm(1);
            this.searchInvoices();
            this.searchClassCode();
        });
    }

    searchInvoices() {
        this._invoiceService.invoicesForAsset().subscribe(result => {
            this.invoiceList = result;
        });
    }

    showInvoiceDetails() {
        this._imoAssetService.getInvoiceDetails(this.oper.invoiceId, this.oper.documentTypeId, this.oper.operationDate).subscribe(result => {
            this.oper.invoiceDetail = result;
            this.showForm(2);
            this.searchStorage();
        });
    }

    showForm(formNr: number) {
        this.oper.showForm1 = (formNr == 1);
        this.oper.showForm2 = (formNr == 2);
        this.oper.showForm3 = (formNr == 3);
    }

    searchStorage() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    searchClassCode() {
        this._imoAssetClassCodeService.imoAssetClassCodeListDD().subscribe(result => {
            this.classCode = result;
        });
    }

    searchAccounts() {
        this._accountService.imoAssetAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    getClassCode(index: number) {
        var classCode = new ImoAssetClassCodeEditDto();
        var classCodeId = this.oper.assets[index].assetClassCodesId;
        this._imoAssetClassCodeService.getClassCodeById(classCodeId).subscribe(result => {
            classCode = result;
            this.oper.assets[index].assetAccountId = classCode.assetAccountId;
            this.oper.assets[index].depreciationAccountId = classCode.depreciationAccountId;
            this.oper.assets[index].expenseAccountId = classCode.expenseAccountId;
            this.oper.assets[index].durationInMonths = classCode.durationMin;
        });
    }

    prepareAssets() {
        this._imoAssetService.prepareAssets(this.oper).subscribe(result => {
            this.oper = result;
            this.searchAccounts();
            this.showForm(3);
        });
    }

    getDurationMax(assetClassCodesId: number, index: number) {
        this._imoAssetClassCodeService.getClassCodeById(assetClassCodesId).subscribe(result => {
            this.oper.assets[index].durationInMonths = result.durationMin;
        });
    }

    saveAssets() {
        this.isLoading = true;
        this._imoAssetService.saveAssets(this.oper)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(result => {
            abp.notify.info('AddMessage');
            this.oper = result;
        });
    }
}