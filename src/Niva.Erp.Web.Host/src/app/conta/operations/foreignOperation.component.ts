import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import {
    AccountListDDDto, AccountServiceProxy, DocumentTypeListDDDto, DocumentTypeServiceProxy, FileUploadDto, ForeignOperationDto,
    ForeignOperationServiceProxy, PaymentOrderForForeignOperationDto, PersonServiceProxy, ThirdPartyAccListDto
} from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './foreignOperation.component.html',
    animations: [appModuleAnimation()]
})
export class ForeignOperationComponent extends AppComponentBase implements OnInit {

    form: ForeignOperationDto = new ForeignOperationDto();
    formData = new FormData();
    bankAccountList: ThirdPartyAccListDto[] = [];
    documentTypeList: DocumentTypeListDDDto[] = [];
    documentTypeStr: DocumentTypeListDDDto[] = [];
    accountsDb: AccountListDDDto[] = [];
    accountsCr: AccountListDDDto[] = [];

    isLoading: boolean = false;
    removedOP: Array<PaymentOrderForForeignOperationDto> = [];
    //currentPage: number = 1;
    //pageSize: number = 10;

    constructor(inject: Injector,
        private _foreignOperationService: ForeignOperationServiceProxy,
        private _personService: PersonServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _accountService: AccountServiceProxy,
        private router: Router
    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.OperContab.Extras.Acces')) {
            this._foreignOperationService.initForm().subscribe(result => {
                this.form = result;

                this.bankAccountListFnc();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    bankAccountListFnc() {
        this._personService.thirdPartyAccList(this.form.thirdPartyId).subscribe(result => {
            this.bankAccountList = result.getThirdPartyAcc;
        });
    }

    operSearch() {
        this.isLoading = true;
        this._foreignOperationService.operSearch(this.form).pipe(
            delay(1000),
            finalize(() => { this.isLoading = false }))
            .subscribe(result => {
                this.form = result;
            });
    }

    uploadFileStart() {
        this._foreignOperationService.uploadFileStart(this.form).subscribe(result => {
            this.form = result;
            this.documentTypeListFnc();
        });
    }

    documentTypeListFnc() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result.getDocumentType;
        });
    }

    documentTypeListByStr(search: any) {
        this._documentTypeService.documentTypeListByStr(search.target.value).subscribe(result => {
            this.documentTypeStr = result.getDocumentType;
        });
    }

    selectedDocumentType(documentId: number, docTypeName: string, index) {
        this.form.operList[index].documentTypeId = documentId;
        this.form.operList[index].documentTypeStr = docTypeName;
    }

    getDocTypeName(docTypeId: number) {
        if (docTypeId === null) {
            return '';
        }

        var documentTypeStr = this.form.operList.find(f => f.documentTypeId === docTypeId).documentTypeStr;
        return documentTypeStr;
    }

    getAccountListComputingDebit(search: any, index: number) {
        this._accountService.accountListComputing(search.target.value, this.form.operList[index].currencyId).subscribe(result => {
            this.accountsDb = result;
        });
    }

    getAccountListComputingCredit(search: any, index: number) {
        this._accountService.accountListComputing(search.target.value, this.form.operList[index].currencyId).subscribe(result => {
            this.accountsCr = result;
        });
    }

    selectedInputDb(itemName: string, itemId: number, index: number) {
        this.form.operList[index].accountingList[0].debitAccountId = itemId;
        this.form.operList[index].accountingList[0].debitAccount = itemName;
    }

    selectedInputCr(itemName: string, itemId: number, index: number) {
        this.form.operList[index].accountingList[0].creditAccountId = itemId;
        this.form.operList[index].accountingList[0].creditAccount = itemName;
    }

    getCreditName(creditId: number) {
        if (!creditId)
            return '';
        var operAccountingList = this.form.operList.find(f => f.accountingList.length > 0 && f.accountingList.find(g => g.creditAccountId == creditId)).accountingList;
        return operAccountingList.find(x => x.creditAccountId === creditId).creditAccount;
    }

    getDebitName(debitId: number) {
        if (!debitId)
            return '';

        var operAccountingList = this.form.operList.find(f => f.accountingList.length > 0 && f.accountingList.find(g => g.debitAccountId == debitId)).accountingList;

        return operAccountingList.find(x => x.debitAccountId === debitId).debitAccount;
    }

    backToList() {
        this.form.showList = true;
        this.form.showUploadForm = false;
        this.form.showDeleteForm = false;
        this.operSearch();
    }

    uploadOperationFile() {
        this._foreignOperationService.uploadOperationFile(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info("OKIncarcareFisier!");
        });
    }

    uploadFiles(e) {
        var file: File = e.target.files[0];
        var myReader: FileReader = new FileReader();

        myReader.readAsDataURL(file);

        myReader.onload = () => {

            this.form.uploadFile.fileUpld = new FileUploadDto({ content: myReader.result.toString(), fileName: file.name });
        }
    }

    saveSelection() {
        this._foreignOperationService.saveSelection(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info("Modificarile au fost salvate");
        });
    }

    generateConta() {

        this._foreignOperationService.generateConta(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info("OKGenerareNote");
        });
    }

    initDeleteForm() {
        this._foreignOperationService.initDeleteForm(this.form).subscribe(result => {
            this.form = result;
        });
    }

    addAccountingRow(detailId: number) {
        this._foreignOperationService.addAccountingRow(detailId, this.form).subscribe(result => {
            this.form = result;
        });
    }

    deleteAccountingRow(detailId: number, index: number) {
        this._foreignOperationService.accountingRowDelete(detailId, index, this.form).subscribe(result => {
            this.form = result;
        });
    }

    operDeleteSearch() {
        this._foreignOperationService.operDeleteSearch(this.form).subscribe(result => {
            this.form = result;
        });
    }

    deleteExtras(foreignOperId: number) {
        abp.message.confirm("Extrasul selectat va fi sters. Sigur?", null, (isConfirmed: boolean) => {
            if (isConfirmed) {
                this._foreignOperationService.operDeleteExtras(foreignOperId, this.form).subscribe(result => {
                    this.form = result;
                    abp.notify.info('Extrasul a fost sters');
                });
            }
        });
    }

    deleteNC(foreignOperId: number) {
        abp.message.confirm("Notele contabile generate vor fi sterse. Sigur?", null, (isConfirmed: boolean) => {
            if (isConfirmed) {
                this._foreignOperationService.operDeleteNC(foreignOperId, this.form).subscribe(result => {
                    this.form = result;
                    abp.notify.info('OKStergere');
                });
            }
        });
    }

    openDictionaryPage() {
        this.router.navigate([]).then(result => { this.router.navigate(['/app/conta/operations/dictionary']); });
    }

    checkPaymentOrder(paymentOrderId: number, index: number) {
        if (paymentOrderId == null) {
         
            this.form.operList[index].paymentOrderId = paymentOrderId;
            this.form.operList[index].selectedOP = false;
            this.form.operList[index].selectedPaymentOrderDetails = null;
            for (var i = 0; i < this.form.operList.length; i++) {
                this.form.operList[i].paymentOrdersList.push(...this.removedOP);

            }
        } else {
            for (var i = 0; i < this.form.operList.length; i++) {
                for (var j = 0; j < this.form.operList[i].paymentOrdersList.length; j++) {
                    if (paymentOrderId == this.form.operList[i].paymentOrdersList[j].id) {
                        if (i == index) {
                            this.form.operList[index].paymentOrderId = paymentOrderId;
                            this.form.operList[i].selectedOP = true;
                            this.form.operList[i].selectedPaymentOrderDetails = this.form.operList[i].paymentOrdersList[j].paymentDetails;
                            this.removedOP.push(this.form.operList[i].paymentOrdersList[j]);
                        
                        }
                        this.form.operList[i].paymentOrdersList.splice(j, 1);
                    }

                }
            }
        }

        //     this.form.operList[index].paymentOrderId = paymentOrderId;
        ////     this.form.operList.forEach(f => { f.paymentOrdersList.pop(paymentOrderId); });


        //     for (var i = 0; i < this.form.operList.length; i++) {
        //         for (var j = 0; j < this.form.operList[i].paymentOrdersList.length; j++) {
        //             if (this.form.operList[i].paymentOrdersList[j].id == paymentOrderId) {
        //                 if (i == index) {

        //                     this.form.operList[i].selectedOP = true;
        //                     this.form.operList[i].selectedPaymentOrderDetails = this.form.operList[i].paymentOrdersList[j].paymentDetails;                    
        //                 }
        //                 this.form.operList[i].paymentOrdersList.splice(j, 1)

        //             } else if (paymentOrderId == null) {
        //                 if (i == index) {
        //                     this.form.operList[index].paymentOrderId = null;
        //                     this.form.operList[i].selectedOP = false;
        //                     this.form.operList[i].selectedPaymentOrderDetails = null;

        //                 }
        //                 var opt = this.removedOP.find(f => f.id == this.form.operList[i].paymentOrdersList[j].id);
        //                 this.form.operList[i].paymentOrdersList.push(opt);
        //             }
        //         }

        //     }
        //this._foreignOperationService.checkPaymentOrderForFgnOperDetail(paymentOrderId, this.form).subscribe(result => {

        //});
    }
}