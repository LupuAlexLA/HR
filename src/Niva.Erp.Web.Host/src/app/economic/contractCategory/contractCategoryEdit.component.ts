import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ContractCategoryEditDto, ContractsServiceProxy } from '../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './contractCategoryEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ContractCategoryEditComponent extends AppComponentBase implements OnInit {

    categoryContract: ContractCategoryEditDto = new ContractCategoryEditDto();
    contractCategoryId: any;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private __contractService: ContractsServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.contractCategoryId = + this.route.snapshot.queryParamMap.get('contractCategoryId');

        if (this.contractCategoryId !== 0) {
            this.__contractService.getContractCategory(this.contractCategoryId).subscribe(result => {
                this.categoryContract = result;
            });
        }
    }

    saveCategory() {
        this.__contractService.saveContractCategory(this.categoryContract).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            this.router.navigate(['/app/economic/contractCategory/contractCategoryList']);
        });
    }
}