import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { DocumentTypeServiceProxy, EnumServiceProxy, GetDocumentTypeOutput, GetInvOperationTypeOutput, InvObjectOperDocTypeEditDto, InvObjectOperDocTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomOperDocTypeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomOperDocTypeEditComponent extends AppComponentBase implements OnInit {

    operDocType: InvObjectOperDocTypeEditDto = new InvObjectOperDocTypeEditDto();
    operType: GetInvOperationTypeOutput;
    operDocTypeId: number;
    documentType: GetDocumentTypeOutput;

    constructor(inject: Injector,
        private _invObjectOperDocTypeService: InvObjectOperDocTypeServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.operDocTypeId = +this.route.snapshot.queryParamMap.get('operDocTypeId');

        this._invObjectOperDocTypeService.getInvObjOperDocTypeById(this.operDocTypeId || 0).subscribe(result => {
            this.operDocType = result;
        });

        this.searchDocumentType();
        this.searchOperType();
    }

    searchDocumentType() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentType = result;
        });
    }

    searchOperType() {
        this._enumService.invOperationTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    saveOperDocType() {
        this._invObjectOperDocTypeService.saveInvObjOperDocType(this.operDocType).subscribe(() => {
            abp.notify.info(this.l('InvObjectOperDocTypeAddMessage'));
            this.router.navigate(['/app/conta/invObjects/invObjectNomOperDocType']);
        });
    }
}