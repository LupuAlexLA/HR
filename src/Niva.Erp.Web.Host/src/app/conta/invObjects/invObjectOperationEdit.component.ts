import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    DocumentTypeListDto, GetAssetOutput, GetInvObjectStorageOutput, GetThirdPartyOutput, InvObjectOperEditDto, InvObjectOperServiceProxy, InvObjectsDto, InvObjectStorageServiceProxy, InvoiceDTO, InvoiceListDto,
    InvoiceListForInvObjectDto, InvoiceServiceProxy, OperTypeListDto, PersonServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectOperationEdit.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectOperationEditComponent extends AppComponentBase implements OnInit {

    operationId: number;
    storeId: number;
    operTypeList: OperTypeListDto[] = [];
    documentTypeList: DocumentTypeListDto[] = [];
    invoiceList: GetAssetOutput;
    operation: InvObjectOperEditDto = new InvObjectOperEditDto();
    storageInList: GetInvObjectStorageOutput;
    storageOutList: GetInvObjectStorageOutput;
    invoice: InvoiceDTO = new InvoiceDTO();
    invoiceDetails: InvoiceListDto[] = [];
    invoiceDetailsForInvObject: InvoiceListForInvObjectDto[] = [];
    invObjects: InvObjectsDto[] = [];
    isLoading: boolean = false;
    thirdPartiesStoreIn: GetThirdPartyOutput = new GetThirdPartyOutput();
    thirdPartiesStoreOut: GetThirdPartyOutput = new GetThirdPartyOutput();

    constructor(inject: Injector,
        private _invObjectOperService: InvObjectOperServiceProxy,
        private _invObjectStorageService: InvObjectStorageServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Operatii.Acces')) {
            this.operationId = +this.route.snapshot.queryParamMap.get('operationId');
            this._invObjectOperService.initOperation(this.operationId || 0).subscribe(result => {
                this.operation = result;
                this.searchInvoice(this.operation.invoiceId);
                if ((this.operation.operationTypeId != null) || (this.operation.operationTypeId != -1)) {
                    this.changeOperation();
                    this.searchPersonStoreOutByInput(this.operation.personStoreOutName);
                    this.searchPersonStoreInByInput(this.operation.personStoreInName);
                }
            });

            this.searchOperType();
            this.searchStorageIn();
            this.searchStorageOut();
        } else {
            this.router.navigate(['app/home']);
        }

    }

    initDetails() {
        this._invObjectOperService.initDetails(this.operation).subscribe(result => {
            this.operation = result;

            this.storeId = this.operation.invObjectsStoreInId || 0;
            this.getInvoiceDetails(this.operation.invoiceId || 0);
            for (var i = 0; i < this.operation.details.length; i++) {
                this.getInvoiceDetailsforInvObject(this.operation.details[i].invObjectItemId || 0);
            }
            this.invObjectsDDList();
        });
    }

    invObjectsDDList() {
        this._invObjectOperService.invObjectsDtoList( this.operation.operationTypeId, this.operation).subscribe(result => {
            this.invObjects = result;
        });
    }

    searchOperType() {
        this._invObjectOperService.operTypeList().subscribe(result => {
            this.operTypeList = result;
            this.searchDocumentType();
        });
    }

    changeOperation() {
        this._invObjectOperService.changeOperation(this.operation).subscribe(result => {
            this.operation = result;
            if (this.operation.operationTypeId != null) {
                this.searchDocumentType();
            }
        });
    }

    getNextDocNumber() {
        if (this.operation.documentTypeId == null) {
            this.operation.documentNr = 0;
        } else {
            this._invObjectOperService.getNextDocumentNumber(this.operation.documentTypeId).subscribe(result => {
                this.operation.documentNr = result;
            });
        }
    }

    getInvoice(invoiceId: number) {
        this._invoiceService.getInvoice(invoiceId).subscribe(result => {
            this.invoice = result;
            this.operation.documentNr = +this.invoice.invoiceNumber;
            this.operation.operationDate = this.invoice.invoiceDate;
        });
    }

    searchInvoice(invoiceId: number) {
        this._invoiceService.invoicesForInvObject(invoiceId || 0).subscribe(result => {
            this.invoiceList = result;
        });
    }

    getInvoiceDetails(invoiceId: number) {
        this._invoiceService.getInvoiceDetails(invoiceId).subscribe(result => {
            this.invoiceDetails = result;
        });
    }

    searchDocumentType() {
        this._invObjectOperService.documentTypeList(this.operation.operationTypeId).subscribe(result => {
            this.documentTypeList = result;
            this.searchStorageIn();
            this.searchStorageOut();
        });
    }

    searchStorageIn() {
        this._invObjectStorageService.invObjectStorageList().subscribe(result => {
            this.storageInList = result;
        });
    }
    searchStorageOut() {
        this._invObjectStorageService.invObjectStorageList().subscribe(result => {
            this.storageOutList = result;
        });
    }

    detailChangeInvObject(idOrd: number) {
        this._invObjectOperService.detailChangeInvObject(idOrd, this.operation).subscribe(result => {
            this.operation = result;
        this.getInvoiceDetailsforInvObject(this.operation.details[idOrd-1].invObjectItemId || 0);
        });
    }

    getInvoiceDetailsforInvObject(invObjectItemId: number) {
        this._invoiceService.getInvoiceDetailsForInvObject(invObjectItemId, this.operationId).subscribe(result => {
            this.invoiceDetailsForInvObject = result;

        });
    }

    addInvoiceDetails() {
        for (var i = 0; i < this.invoiceDetailsForInvObject.length; i++) {
            this.operation.details[i].invValueModif = this.invoiceDetailsForInvObject[i].invValue;
        }
    }

    addRow() {
        this._invObjectOperService.addRow(this.operation).subscribe(result => {
            this.operation = result;
            for (var i = 0; i < this.operation.details.length; i++) {
                for (var j = 0; j < this.invObjects.length; j++) {
                    if (this.invObjects[j].id === this.operation.details[i].invObjectItemId) {
                        this.operation.details[i].isSelectedInvObjectItem = true;
                        this.operation.details[i].selectedInvObjectItemId = this.invObjects[j].id;
                        this.operation.details[i].selectedInvObjectItem = this.invObjects[j].name;
                        console.log(this.operation.details[i]);
                        this.invObjects.splice(j, 1);
                    }
                }
            }
        });
    }

    deleteRow(index: number) {
        this.operation.details.splice(index, 1);
    }

    summary() {
        this._invObjectOperService.summary(this.operation).subscribe(result => {
            this.operation = result;
        });
    }

    showFormOper(formNr: number) {
        this._invObjectOperService.showFormOper(formNr, this.operation).subscribe(result => {
            this.operation = result;
        })
    }
    
    saveOperation() {
        this.isLoading = true;
        this._invObjectOperService.saveOperation(this.operation).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.operation = result;
            abp.notify.info('AddUpdateMessage');
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
}