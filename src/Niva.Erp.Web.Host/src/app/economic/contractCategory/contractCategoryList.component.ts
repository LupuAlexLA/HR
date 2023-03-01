import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ContractCategoryListDto, ContractsServiceProxy } from '../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './contractCategoryList.component.html',
    animations: [appModuleAnimation()]
})
export class ContractCategoryListComponent extends AppComponentBase implements OnInit {

    categoryContractList: ContractCategoryListDto[] = [];

    constructor(injector: Injector,
        private _contractService: ContractsServiceProxy,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (true) {
            this.getCategoryList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getCategoryList() {
        this._contractService.categoryList().subscribe(result => {
            this.categoryContractList = result;
        });
    }

    delete(categoryContractId: number) {
        abp.message.confirm(
            "Categoria de contract va fi stearsa. Sigur?",
            undefined,
            (result: boolean) => {
                if (result) {
                    this._contractService.deleteContractCategory(categoryContractId).subscribe(() => {
                        this.getCategoryList();
                        abp.notify.success(this.l('DeleteMessage'));
                    });
                }
            }
        );
    }

    getCategoryCount() {
        if (this.categoryContractList.length > 0) {
            return this.categoryContractList.length;
        } else {
            return 0;
        }
    }
}