import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AccountRelationDto, AccountRelationServiceProxy } from '../../../../shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';


@Component({
    templateUrl: './accountRelationList.component.html',
    animations: [appModuleAnimation()]
})
export class AccountRelationListComponent extends AppComponentBase implements OnInit {

    relations: AccountRelationDto[];

    constructor(injector: Injector,
        private _accountRelationService: AccountRelationServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.getAccountRelationList();
    }

    getAccountRelationList() {
        this._accountRelationService.accountRelationList().subscribe(result => {
            this.relations = result;
        });
    }

    getRelationsCount() {
        if (this.relations == null) {
            return 0;
        } else {
            return this.relations.length;
        }
    }

    deleteAccountRelation(id: number) {
        abp.message.confirm(this.l('DeleteAccountRelationMessage', id),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._accountRelationService.deleteAccountRelation(id).pipe(finalize(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                    })).subscribe(() => {
                        this.getAccountRelationList();
                    });
                }
        });

    }
}