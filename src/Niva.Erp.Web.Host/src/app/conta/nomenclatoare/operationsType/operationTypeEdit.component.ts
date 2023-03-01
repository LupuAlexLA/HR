import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { OperationTypesEditDto, OperationTypesServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './operationTypeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class OperationTypeEditComponent extends AppComponentBase implements OnInit {

    operType: OperationTypesEditDto = new OperationTypesEditDto();
    operTypeId: any;

    constructor(injector: Injector,
        private _operTypeService: OperationTypesServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        this.operTypeId = this.route.snapshot.queryParamMap.get('operTypeId');

        if (this.operTypeId !== null) {
            this._operTypeService.getOperTypeById(this.operTypeId).subscribe(result => {
                this.operType = result;
            });
        }
    }

    save() {
        this._operTypeService.saveOperType(this.operType).subscribe(() => {
            this.router.navigate(['/app/conta/nomenclatoare/operationsType/operationType']);
        });
    }


}