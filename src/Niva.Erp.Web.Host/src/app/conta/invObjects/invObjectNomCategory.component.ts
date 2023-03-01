import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetInvCategoryOutput, InvObjectCategoryServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectNomCategory.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectNomCategoryComponent extends AppComponentBase implements OnInit {

   categoryList: GetInvCategoryOutput = new GetInvCategoryOutput();

    constructor(inject: Injector,
        private _invObjCategoryService: InvObjectCategoryServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getCategoryList();
    }

    getCategoryList() {
        this._invObjCategoryService.categoryList().subscribe(result => {
            this.categoryList = result;
        });
    }

    deleteCategory(categoryId: number) {
        abp.message.confirm("Categoria va fi stearsa. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._invObjCategoryService.deleteCategory(categoryId).subscribe(() => {
                        abp.notify.info(this.l('DeleteMessage'));
                        this.getCategoryList();
                    });
                }
            });
    }

    getInvObjectCategoryListCount() {
        if (this.categoryList.getCategoryList == null) {
            return 0;
        } else {
            return this.categoryList.getCategoryList.length;
        }
    }
}