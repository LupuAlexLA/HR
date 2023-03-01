import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DocumentTypeServiceProxy, EnumServiceProxy, GetDocumentTypeOutput, GetPrepaymentsOperDocTypeOutput, PrepaymentOperationTypeDto, PrepaymentsOperDocTypeEditDto, PrepaymentsOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsOperDocTypeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsOperDocTypeEditComponent extends AppComponentBase implements OnInit {

    operDocType: PrepaymentsOperDocTypeEditDto = new PrepaymentsOperDocTypeEditDto();
    operType: PrepaymentOperationTypeDto[] = [];
    operDocTypeId: number;
    documentType: GetDocumentTypeOutput;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentsOperDocTypeService: PrepaymentsOperDocTypeServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _enumService: EnumServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.operDocTypeId = +this.route.snapshot.queryParamMap.get('operDocTypeId');

        if (this.operDocTypeId != 0) {
            this._prepaymentsOperDocTypeService.getOperDocTypeById(this.operDocTypeId).subscribe(result => {
                this.operDocType = result;
            });
        }

        this.searchOperType();
        this.searchDocumentType();
    }

    searchOperType() {
        this._enumService.prepaymentsOperationTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    searchDocumentType() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentType = result;
        });
    }

    saveOperDocTypeFnc() {
        this.isLoading = true;
        this._prepaymentsOperDocTypeService.saveOperDocType(this.operDocType).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                abp.notify.info('OKAddModifyMessage');
            })
        ).subscribe(() => { });
    }
}