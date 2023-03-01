import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvObjectCategoryEditDto, InvObjectCategoryServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomCategoryEdit.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomCategoryEditComponent extends AppComponentBase implements OnInit {

    category: InvObjectCategoryEditDto = new InvObjectCategoryEditDto();
    categoryId: number;

    constructor(inject: Injector,
        private _invObjCategoryService: InvObjectCategoryServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.categoryId = + this.route.snapshot.queryParamMap.get('categoryId');

        if (this.categoryId !== null) {
            this._invObjCategoryService.getCategory(this.categoryId).subscribe(result => {
                this.category = result;
            });
        }
    }

    saveCategory() {
        this._invObjCategoryService.saveCategory(this.category).subscribe(() => {
            abp.notify.info(this.l('AddModifyMessage'));
            this.router.navigate(['/app/conta/invObjects/invObjectNomCategory']);
        });
    }
}