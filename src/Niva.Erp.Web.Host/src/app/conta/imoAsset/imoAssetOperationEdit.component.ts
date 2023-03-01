import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    AccountServiceProxy,
    DocumentTypeListDto, GetAccountOutput, GetAssetOutput, GetImoAssetStorageOutput, GetThirdPartyOutput, ImoAssetOperEditDto, ImoAssetOperServiceProxy, ImoAssetsDDDto, ImoAssetStorageServiceProxy, InvoiceDTO, InvoiceListDto,
    InvoiceListForImoAssetDto,
    InvoiceServiceProxy, OperTypeListDto, PersonServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetOperationEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetOperationEditComponent extends AppComponentBase implements OnInit {

    operationId: number;
    storeId: number;
    operTypeList: OperTypeListDto[] = [];
    documentTypeList: DocumentTypeListDto[] = [];
    operation: ImoAssetOperEditDto = new ImoAssetOperEditDto();
    invoiceList: GetAssetOutput;
    invoice: InvoiceDTO = new InvoiceDTO();
    invoiceDetails: InvoiceListDto[] = [];
    invoiceDetailsforImoAsset: InvoiceListForImoAssetDto[] = [];
    storageInList: GetImoAssetStorageOutput;
    storageOutList: GetImoAssetStorageOutput;
    assets: ImoAssetsDDDto[] = [];
    isLoading: boolean = false;
    thirdPartiesStoreIn: GetThirdPartyOutput = new GetThirdPartyOutput();
    thirdPartiesStoreOut: GetThirdPartyOutput = new GetThirdPartyOutput();
    accounts: GetAccountOutput;

    constructor(inject: Injector,
        private _imoAssetOperService: ImoAssetOperServiceProxy,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _personService: PersonServiceProxy,
        private _accountService: AccountServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Operatii.Modificare')) {
            this.operationId = + this.route.snapshot.queryParamMap.get('operationId');
            this.initOperation(this.operationId || 0);
            this.searchOperType();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    initOperation(operationId: number) {
        this._imoAssetOperService.initOperation(this.operationId || 0).subscribe(result => {
            this.operation = result;
            this.searchInvoice(this.operation.invoiceId);
            if ((this.operation.operationTypeId != null) || (this.operation.operationTypeId != -1)) {
                this.changeOperation();
            }

        });
    }

    changeOperation() {
        this._imoAssetOperService.changeOperation(this.operation).subscribe(result => {
            this.operation = result;
            if (this.operation.operationTypeId != null) {
                this.searchDocumentType();
                if (this.operation.operationTypeId === 5) {
                    this.searchInvoice(this.operation.invoiceId);
                }
            }
        });
    }

    searchOperType() {
        this._imoAssetOperService.operTypeList().subscribe(result => {
            this.operTypeList = result;
            this.searchDocumentType();
        });
    }

    searchDocumentType() {
        this._imoAssetOperService.documentTypeList(this.operation.operationTypeId).subscribe(result => {
            this.documentTypeList = result;
            this.searchStorageIn();
            this.searchStorageOut();
        });
    }

    searchStorageIn() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageInList = result;
        });
    }
    searchStorageOut() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageOutList = result;
        });
    }

    getNextDocNumber() {
        if (this.operation.documentTypeId == null) {
            this.operation.documentNr = 0;
        } else {
            this._imoAssetOperService.getNextDocumentNumber(this.operation.documentTypeId).subscribe(result => {
                this.operation.documentNr = result;
            });
        }
    }

    searchInvoice(invoiceId: number) {
        this._invoiceService.invoicesForAssetSale(invoiceId || 0).subscribe(result => {
            this.invoiceList = result;
        });
    }

    getInvoice(invoiceId: number) {
        this._invoiceService.getInvoice(invoiceId).subscribe(result => {
            this.invoice = result;
            this.operation.documentNr = +this.invoice.invoiceNumber;
            this.operation.documentDate = this.invoice.invoiceDate;
            for (var i = 0; i < this.operation.details.length; i++) {
                this.operation.details[i].invValueModif = this.invoice.invoiceDetails[i].value;
                this.operation.details[i].durationModif = this.invoice.invoiceDetails[i].durationInMonths;

            }
        });
    }

    getInvoiceDetails(invoiceId: number) {
        this._invoiceService.getInvoiceDetails(invoiceId).subscribe(result => {
            this.invoiceDetails = result;

        });
    }

    initDetails() {
        this._imoAssetOperService.initDetails(this.operation).subscribe(result => {
            this.operation = result;

            this.storeId = this.operation.assetsStoreInId || 0;
            this.getInvoiceDetails(this.operation.invoiceId || 0);

            for (var i = 0; i < this.operation.details?.length; i++) {
                this.getInvoiceDetailsforImoAsset(this.operation.details[i].imoAssetItemId || 0);
            }

            if (this.operation.assetAccountDetails?.length > 0) {
                this.searchAccounts();
            }


            //this.assetsDDList(this.storeId);
            this.assetsDDList();
        })
    }

    assetsDDList() {
        this._imoAssetOperService.assetsDDList(this.operation.operationTypeId, this.operation).subscribe(result => {
            this.assets = result;
        });
    }

    detailChangeAsset(idOrd: number) {
        this._imoAssetOperService.detailChangeAsset(idOrd, this.operation).subscribe(result => {
            this.operation = result;
            this.getInvoiceDetailsforImoAsset(this.operation.details[idOrd - 1].imoAssetItemId || 0);

        });
    }

    getInvoiceDetailsforImoAsset(imoAssetItemId: number) {
        this._invoiceService.getInvoiceDetailsForImoAsset(imoAssetItemId, this.operation.operationTypeId, this.operationId, this.operation.invoiceId).subscribe(result => {
            this.invoiceDetailsforImoAsset = result;

        });
    }

    addInvoiceDetails(rowIndex: number, invoiceDetailId: number) {
        //for (var i = 0; i < this.invoiceDetailsforImoAsset.length; i++) {
        //    this.operation.details[i].invValueModif = this.invoiceDetailsforImoAsset[i].invValue;
        //    this.operation.details[i].durationModif = this.invoiceDetailsforImoAsset[i].duration;
        //}

        this.operation.details[rowIndex].invValueModif = this.invoiceDetailsforImoAsset.find(f => f.id == invoiceDetailId).invValue;
        this.operation.details[rowIndex].fiscalValueModif = this.invoiceDetailsforImoAsset.find(f => f.id == invoiceDetailId).fiscalValue;

    }

    actModifInvValue(index: number) {
        this.operation.details[index].invValueModif = +(this.operation.details[index].invValueNew - this.operation.details[index].invValueOld).toFixed(2);
    }

    actModifFiscalValue(index: number) {
        this.operation.details[index].fiscalValueModif = +(this.operation.details[index].fiscalValueNew - this.operation.details[index].fiscalValueOld).toFixed(2);
    }

    addRow() {
        this._imoAssetOperService.addRow(this.operation).subscribe(result => {
            this.operation = result;
        });
    }

    deleteRow(idOrd: number) {
        this._imoAssetOperService.operDeleteRow(idOrd, this.operation).subscribe(result => {
            this.operation = result;
        });
    }

    summary() {
        this._imoAssetOperService.summary(this.operation).subscribe(result => {
            this.operation = result;
        });
    }

    showFormOper(formNr: number) {
        this._imoAssetOperService.showFormOper(formNr, this.operation).subscribe(result => {
            this.operation = result;
        })
    }

    saveOperation() {
        this.isLoading = true;

        this._imoAssetOperService.saveOperation(this.operation).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            abp.notify.info('AddUpdateMessage');
            this.operation = result;
        });
    }

    searchPersonStoreInByInput(search: any) {
        this._personService.searchPersonForInvObjectByInput(search.target.value).subscribe(result => {
            this.thirdPartiesStoreIn = result;
        });
    }

    getPersonStoreInName(thirdPartyStoreInId: number) {
        if (thirdPartyStoreInId == 0) {
            return '';
        }
        return this.operation.personStoreInName;
    }

    selectedThirdPartyStoreIn(thirdPartyId: number, thirdPartyName: string) {
        this.operation.personStoreInId = thirdPartyId;
        this.operation.personStoreInName = thirdPartyName;
    }

    searchPersonStoreOutByInput(search: any) {
        this._personService.searchPersonForInvObjectByInput(search.target.value).subscribe(result => {
            this.thirdPartiesStoreOut = result;
        });
    }

    getPersonStoreOutName(thirdPartyStoreOutId: number) {
        if (thirdPartyStoreOutId == 0) {
            return '';
        }
        return this.operation.personStoreOutName;
    }

    selectedThirdPartyStoreOut(thirdPartyId: number, thirdPartyName: string) {
        this.operation.personStoreOutId = thirdPartyId;
        this.operation.personStoreOutName = thirdPartyName;
    }

    detailAccountAsset(idOrd: number) {
        this._imoAssetOperService.detailAccountAsset(idOrd, this.operation).subscribe(result => {
            this.operation = result;
            this.searchAccounts();
        });
    }

    searchAccounts() {
        this._accountService.imoAssetAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    updateImoAssetAccounts() {
        this.isLoading = true;

        this._imoAssetOperService.upateImoAssetAccounts(this.operation).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            abp.notify.info('AddUpdateMessage');
            this.router.navigate(['/app/conta/imoAsset/imoAssetOperation']);
        });
    }
}