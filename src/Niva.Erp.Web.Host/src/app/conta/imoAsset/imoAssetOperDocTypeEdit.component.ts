import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DocumentTypeServiceProxy, EnumServiceProxy, GetDocumentTypeOutput, GetImoAssetOperTypeOutput, ImoAssetOperDocTypeEditDto, ImoAssetOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetOperDocTypeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetOperDocTypeEditComponent extends AppComponentBase implements OnInit {

    operDocType: ImoAssetOperDocTypeEditDto = new ImoAssetOperDocTypeEditDto();
    operType: GetImoAssetOperTypeOutput;
    operDocTypeId: any;
    documentType: GetDocumentTypeOutput;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _imoAssetOperDocTypeService: ImoAssetOperDocTypeServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.operDocTypeId = this.route.snapshot.queryParamMap.get('operDocTypeId');

        if (this.operDocTypeId !== null) {
            this._imoAssetOperDocTypeService.getOperDocTypeById(this.operDocTypeId).subscribe(result => {
                this.operDocType = result;
            })
        }

        this.searchDocumentType();
        this.searchOperType();
    }

    searchDocumentType() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentType = result;
        });
    }

    searchOperType() {
        this._enumService.imoAssetOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    saveOperDocTypeFnc() {
        this.isLoading = true;
        this._imoAssetOperDocTypeService.saveOperDocType(this.operDocType).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                abp.notify.info(this.l('ImoAssetOperDocTypeAddMessage'));
            })
        ).subscribe(() => { });
    }
}