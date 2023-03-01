import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from "../../../shared/AppConsts";
import {
    AccountListDDDto, AccountServiceProxy, CurrencyDto, CurrencyServiceProxy, DocumentTypeServiceProxy, GetDocumentTypeOutput, OperationEditDetailsDto, OperationEditDto, OperationServiceProxy, OperationTypesListDto,
    OperationTypesServiceProxy
} from '../../../shared/service-proxies/service-proxies';
import { SoldDbDialogComponent } from './sold-dialog/soldDb-dialog.component';

@Component({
    templateUrl: './operationEdit.component.html',
    animations: [appModuleAnimation()]
})
export class OperationEditComponent extends AppComponentBase implements OnInit {

    operation: OperationEditDto;
    operationId: number;
    currency: CurrencyDto[];
    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    accountsDb: AccountListDDDto[] = [];
    accountsCr: AccountListDDDto[] = [];
    showAccountsDb: any[] = [];
    showAccountsCr: any[] = [];
    operationTypeList: OperationTypesListDto[] = [];

    constructor(injector: Injector,
        private _currencyService: CurrencyServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _operationService: OperationServiceProxy,
        private _accountService: AccountServiceProxy,
        private _operationTypeService: OperationTypesServiceProxy,
        private _modalService: BsModalService,
        private route: ActivatedRoute,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Conta.OperContab.OperContab.Acces')) {
            this.operationId = + this.route.snapshot.queryParamMap.get('operationId');

            this._operationService.initOperation(this.operationId || 0).subscribe(result => {
                this.operation = result;
                this.getDocumentTypeList();
                this.getCurrencyList();
                this.getOperationTypeList();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getDocumentTypeList() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result;
        });
    }

    getOperationTypeList() {
        this._operationTypeService.operTypesList().subscribe(result => {
            this.operationTypeList = result;
        });
    }

    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currency = result;
        });
    }

    getAccountListComputingDebit(search: any, index: number) {
        this._accountService.accountListComputing(search.target.value, this.operation.currencyId).subscribe(result => {
            this.accountsDb = result;
            this.showAccountsDb[index] = true;
        });
    }

    getAccountListComputingCredit(search: any, index: number) {
        this._accountService.accountListComputing(search.target.value, this.operation.currencyId).subscribe(result => {
            this.accountsCr = result;
            this.showAccountsCr[index] = true;
        });
    }


    deleteOperationDetail(index: number) {
        this.operation.operationDetails.splice(index, 1);
    }

    addOperationDetail() {
        this.operation.operationDetails.push(new OperationEditDetailsDto());

        let index = this.operation.operationDetails.length - 1;

        this.showAccountsDb[index] = false;
        this.showAccountsCr[index] = false;

    }

    saveOperation() {
        this.operation.operationDate = moment(this.operation.operationDate, "YYYY-MM-DDTHH:mm:ss.SS");
        this.operation.documentDate = moment(this.operation.documentDate, "YYYY-MM-DDTHH:mm:ss.SS");

        this._operationService.saveOperation(this.operation).subscribe(result => {
            abp.notify.info(this.l('addUpdateMessage'));
            this.operation = result;
        });
    }

    selectedInputDb(itemName: string, itemId: number, index: number) {
        this.operation.operationDetails[index].debitId = itemId;
        this.operation.operationDetails[index].debitName = itemName;
    }

    selectedInputCr(itemName: string, itemId: number, index: number) {
        this.operation.operationDetails[index].creditId = itemId;
        this.operation.operationDetails[index].creditName = itemName;
    }

    changeCurrency() {
        this.operation.operationDetails.forEach(item => {
            item.debitId = 0;
            item.creditId = 0;
            item.debitName = '';
            item.creditName = '';
        });
        this.accountsDb = null;
        this.accountsCr = null;
    }

    /**
     * Afisez numele creditului selectat la editarea unei operatii contabile
     */
    getCreditName(creditId: number) {
        if (!creditId)
            return '';
        return this.operation.operationDetails.find(x => x.creditId == creditId).creditName;
    }

    /**
     * Afisez  numele debitului selectat la editarea unei operatii contabile
     */
    getDebitName(debitId: number) {
        if (!debitId)
            return '';
        return this.operation.operationDetails.find(x => x.debitId === debitId).debitName;
    }

    newAccount() {
        window.open(AppConsts.appBaseUrl + '/app/conta/nomenclatoare/accounts/accountEdit', '_blank');
    }

    showCrSold(creditId: number) {

    }

    showDbSold(accountId: number) {
        let showDbSoldDialog: BsModalRef;

        showDbSoldDialog = this._modalService.show(
            SoldDbDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    accountId: accountId,
                    currencyId: this.operation.currencyId
                },
            }
        );

    }

  
}