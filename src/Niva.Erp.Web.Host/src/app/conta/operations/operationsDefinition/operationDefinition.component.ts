import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { AppConsts } from "../../../../shared/AppConsts";
import {
    AccountListDDDto, AccountServiceProxy, CurrencyDto, CurrencyServiceProxy, DocumentTypeServiceProxy, GetDocumentTypeOutput, OperationDefinitionDetailsDto, OperationDefinitionDto, OperationDefinitionServiceProxy, OperationEditDetailsDto, OperationEditDto, OperationServiceProxy, OperationTypesListDto,
    OperationTypesServiceProxy
} from '../../../../shared/service-proxies/service-proxies';
import { SoldDbDialogComponent } from '../sold-dialog/soldDb-dialog.component';

@Component({
    templateUrl: './operationDefinition.component.html',
    animations: [appModuleAnimation()]
})

export class OperationDefinitionComponent extends AppComponentBase implements OnInit {

    operation: OperationDefinitionDto ;
    operationId: number;
    currency: CurrencyDto[];
    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    accountsDb: AccountListDDDto[] = [];
    accountsCr: AccountListDDDto[] = [];
    showAccountsDb: any[] = [];
    showAccountsCr: any[] = [];
    operationTypeList: OperationTypesListDto[] = [];
    creditName;
    debitName;

    constructor(injector: Injector,
        private _currencyService: CurrencyServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _operationService: OperationServiceProxy,
        private _operationDefinition : OperationDefinitionServiceProxy,
        private _accountService: AccountServiceProxy,
        private _operationTypeService: OperationTypesServiceProxy,
        private _modalService: BsModalService,
        public bsModalRef: BsModalRef,
        private route: ActivatedRoute,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Conta.OperContab.OperContab.Acces')) {
          //  this.operationId = + this.route.snapshot.queryParamMap.get('operationId');
            console.log(this.operationId);
            this._operationDefinition.initOperation(0 || this.operationId).subscribe(result => {
                this.operation = result;
                console.log(result);
                console.log(this.showAccountsDb);
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
        console.log(this.operation.currencyId)
        this._accountService.accountListComputing(search.target.value, this.operation.currencyId).subscribe(result => {
            this.accountsCr = result;
            this.showAccountsCr[index] = true;
        });
    }


    deleteOperationDetail(index: number) {
        this.operation.operationDetails.splice(index, 1);
    }

    addOperationDetail() {
        
        this.operation.operationDetails.push(new OperationDefinitionDetailsDto());

        let index = this.operation.operationDetails.length - 1;

        this.showAccountsDb[index] = false;
        this.showAccountsCr[index] = false;

    }

    saveOperation() {
        console.log(this.operation);
        this._operationDefinition.saveOperationDefinition(this.operation).subscribe(() => { });
        this.notify.info(this.l('Modificare realizata'));
        this.bsModalRef.hide();
    }

    back() {
        this.bsModalRef.hide();
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