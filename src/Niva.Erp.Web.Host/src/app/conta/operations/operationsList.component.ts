import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { DocumentTypeServiceProxy, GetDocumentTypeOutput, OperationSearchDto, OperationServiceProxy, OperationTypesListDto, OperationTypesServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './operationsList.component.html',
    animations: [appModuleAnimation()]
})
export class OperationsListComponent extends AppComponentBase implements OnInit {

    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    searchOperation: OperationSearchDto = new OperationSearchDto();
    operationTypeList: OperationTypesListDto[] = [];
    isLoading: any;
    showButtonDeleteOperExterna: boolean = false;

    constructor(injector: Injector,
        private _operationService: OperationServiceProxy,
        private _operationTypeService: OperationTypesServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('Conta.OperContab.OperContab.Acces')) {
            this._operationService.initSearch().subscribe(result => {
                //this.searchOperation = result;

                this.searchOperation.dataStart = sessionStorage.getItem('dataStartOperations') ? moment(sessionStorage.getItem('dataStartOperations')) : moment(result.dataStart);
                this.searchOperation.dataEnd = sessionStorage.getItem('dataEndOperations') ? moment(sessionStorage.getItem('dataEndOperations')) : moment(result.dataEnd);
                this.searchOperation.account1 = sessionStorage.getItem('account1Operations') ? JSON.parse(sessionStorage.getItem('account1Operations')) : result.account1;
                this.searchOperation.account2 = sessionStorage.getItem('account2Operations') ? JSON.parse(sessionStorage.getItem('account2Operations')) : result.account2;
                this.searchOperation.documentTypeId = sessionStorage.getItem('documentTypeIdOperations') ? JSON.parse(sessionStorage.getItem('documentTypeIdOperations')) : result.documentTypeId;
                this.searchOperation.documentNumber = sessionStorage.getItem('documentNumberOperations') ? JSON.parse(sessionStorage.getItem('documentNumberOperations')) : result.documentNumber;
                this.searchOperation.value = sessionStorage.getItem('valueOperations') ? JSON.parse(sessionStorage.getItem('valueOperations')) : result.value;
                this.searchOperation.operationId = sessionStorage.getItem('idOperationOperations') ? JSON.parse(sessionStorage.getItem('idOperationOperations')) : result.operationId;
                this.searchOperation.explication = sessionStorage.getItem('explicationOperations') ? JSON.parse(sessionStorage.getItem('explicationOperations')) : result.explication;
                this.searchOperation.operationTypeId = sessionStorage.getItem('operationTypeIdOperations') ? JSON.parse(sessionStorage.getItem('operationTypeIdOperations')) : result.operationTypeId;

            });

            this._operationService.getSetupStergOperExterna().subscribe(result => {
                this.showButtonDeleteOperExterna = result;
            });

            this._documentTypeService.documentTypeList().subscribe(result => {
                this.documentTypeList = result;
            });

            this.getOperationTypeList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchOperations() {
        this.isLoading = true;
        this._operationService.searchOperations(this.searchOperation).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })).subscribe(result => {
            this.searchOperation = result;

            sessionStorage.setItem("dataStartOperations", this.searchOperation.dataStart.toString());
            sessionStorage.setItem('dataEndOperations', this.searchOperation.dataEnd.toString());
            sessionStorage.setItem('account1Operations', JSON.stringify(this.searchOperation.account1));
            sessionStorage.setItem('account2Operations', JSON.parse(this.searchOperation.account2));
            sessionStorage.setItem('documentTypeIdOperations', JSON.stringify(this.searchOperation.documentTypeId));
            sessionStorage.setItem('documentNumberOperations', JSON.stringify(this.searchOperation.documentNumber));
            sessionStorage.setItem('valueOperations', JSON.stringify(this.searchOperation.value));
            sessionStorage.setItem('idOperationOperations', JSON.stringify(this.searchOperation.operationId));
            sessionStorage.setItem('explicationOperations', JSON.stringify(this.searchOperation.explication));
            sessionStorage.setItem('operationTypeIdOperations', JSON.stringify(this.searchOperation.operationTypeId));
        });
    }

    getOperationTypeList() {
        this.isLoading = true;
        this._operationTypeService.operTypesList().pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })).subscribe(result => {
            this.operationTypeList = result;
        });
    }

    getOperationCount() {
        if (this.searchOperation.operations === undefined) {
            return 0;
        } else {
            return this.searchOperation.operations.length;
        }
    }

    //selectAll() {
    //    for (var i = 0; i < this.searchOperation.operations.length; i++) {
    //        this.searchOperation.operations[i].operationStatus = true;
    //    }
    //}

    //selectNone() {
    //    for (var i = 0; i < this.searchOperation.operations.length; i++) {
    //        this.searchOperation.operations[i].operationStatus = false;
    //    }
    //}

    //saveValidation() {
    //    this._operationService.saveValidation(this.searchOperation).subscribe(result => {
    //        this.searchOperation = result;
    //        abp.notify.info(this.l('ValidationMessage'));
    //    }, error => {
    //        abp.notify.error(error.data.message, error.data.details);
    //    });
    //}

    showDetail(index: number) {
        this.searchOperation.operations[index].showDetail = true;
    }

    deleteOperation(operationId: number) {
        abp.message.confirm('Nota contabila va fi stearsa.', 'Sigur?',
            (result: boolean) => {
                if (result) {
                    this._operationService.deleteOperation(operationId).subscribe(() => {
                        this.searchOperations();
                    });
                }
            });
    }

    hideDetail(index: number) {
        this.searchOperation.operations[index].showDetail = false;
    }

    save() {
        console.log(this.showButtonDeleteOperExterna);
        this._operationService.allowDeletionOperExterna(this.showButtonDeleteOperExterna).subscribe(result => {

        });
    }
}