import { Component, ChangeDetectionStrategy, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { DocumentTypeServiceProxy, GetDocumentTypeOutput } from '../../../../shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
    templateUrl: './docType.component.html',
    animations: [appModuleAnimation()]
})
export class DocTypeComponent extends AppComponentBase implements OnInit {

    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();


    constructor(injector: Injector,
        private _documentTypeService: DocumentTypeServiceProxy,
        private router: Router) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('Admin.Conta.TipDoc.Acces')) {
            this.getDocumentTypeList();
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

     docTypeListCount() {
        if (this.documentTypeList.getDocumentType == null) {
            return 0;
        } else {
            return this.documentTypeList.getDocumentType.length;
        }
    }

    delete(id: number) {
        abp.message.confirm(
            this.l('DocumentTypeDeleteWarningMessage', id),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._documentTypeService.deleteDocumentType(id).pipe(finalize(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));

                    })).subscribe(() => {
                        this.getDocumentTypeList();
                    });
                }
            }
        );

    }
}