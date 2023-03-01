import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { AccountRelationDto, AccountRelationServiceProxy } from '../../../../shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './accountRelationEdit.component.html',
    animations: [appModuleAnimation()]
})
export class AccountRelationEditComponent extends AppComponentBase implements OnInit {

    accountRelation: AccountRelationDto = new AccountRelationDto();
    accountRelationId: number;

    constructor(injector: Injector,
        private _accountRelationService: AccountRelationServiceProxy,
        private router: ActivatedRoute) {
        super(injector);
    }

    ngOnInit(): void {
        this.accountRelationId = +this.router.snapshot.queryParamMap.get('accountRelationId');
        this._accountRelationService.accountRelationEditInit(this.accountRelationId || 0)
            .subscribe(result => {
                this.accountRelation = result;
            });
    }

    save() {
        this._accountRelationService.saveAccountRelation(this.accountRelation)
            .subscribe(result => {
                abp.notify.info(this.l("AddUpdateMessage"));
                this.accountRelation = result;
            });
    }
}