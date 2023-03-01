import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { FileDocErrorDto, FileDocExternServiceProxy, IFileDocErrorDto } from '../../../shared/service-proxies/service-proxies';

@Component({
     
    templateUrl: './file-doc-errors.component.html',
    animations: [appModuleAnimation()],
})
/** fileDocErrors component*/
export class FileDocErrorsComponent extends AppComponentBase implements OnInit {
    errors: FileDocErrorDto[];
    constructor(inject: Injector,
        private fileDocErrService: FileDocExternServiceProxy,
        private router: Router) {
        super(inject);
    }
    ngOnInit(): void {
        this.getErrors()
    }

    getErrors(): void {
        this.fileDocErrService.getImportErrors().subscribe(result => {
            this.errors = result;
        })
    }

    getCount(): number {
        return this.errors?.length;
    }
}