import { Component, ChangeDetectionStrategy, OnInit, Injector, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { DocumentTypeServiceProxy, DocumentTypeEditDto } from '../../../../shared/service-proxies/service-proxies';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { Subscription } from 'rxjs';

@Component({
    templateUrl: './docTypeListEdit.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.Default
})
export class DocTypeListEditComponent extends AppComponentBase implements OnInit, OnDestroy {

    docId: any;
    document: DocumentTypeEditDto = new DocumentTypeEditDto();
    private routeSub: Subscription;
    saving = false;

    constructor(injector: Injector,
        private _documentTypeService: DocumentTypeServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit(): void {
        this.routeSub = this.route.queryParams.subscribe(params => {
            this.docId = params['docId'];
        });

        if (this.docId != undefined) {
            this._documentTypeService.getDocTypeById(this.docId).subscribe((result: DocumentTypeEditDto) => {
                this.document = result;
            });
        }
    }

    ngOnDestroy(): void {
        this.routeSub.unsubscribe();
    }

    save() {
        this.saving = true;
        this._documentTypeService.saveDocumentType(this.document).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.router.navigate(['app/conta/nomenclatoare/document/docType']);
        });
    }
}