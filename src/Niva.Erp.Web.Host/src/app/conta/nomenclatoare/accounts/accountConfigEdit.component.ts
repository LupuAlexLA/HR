import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { AccountConfigDto, AccountConfigServiceProxy, ActivityTypeDto, ActivityTypeServiceProxy, EnumServiceProxy, EnumTypeDto, GetImoAssetStorageOutput, ImoAssetStorageServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './accountConfigEdit.component.html',
    animations: [appModuleAnimation()]
})
export class AccountConfigEditComponent extends AppComponentBase implements OnInit {

    accountId: any;
    account: AccountConfigDto = new AccountConfigDto();
    storageList: GetImoAssetStorageOutput;
    diurnaTypeList: EnumTypeDto[] = [];
    scopDeplasareList: EnumTypeDto[] = [];
    activityTypes: ActivityTypeDto[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _accountConfigService: AccountConfigServiceProxy,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _enumService: EnumServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.accountId = this.route.snapshot.queryParamMap.get('account');

        if (this.accountId != null) {
            this._accountConfigService.getAccountConfigById(this.accountId).subscribe(result => {
                this.account = result;
            });
        } else {
            var data = new Date();
            this.account.valabilityDate = moment(data);
        }
        this.searchStorage();
        this.getDiurnaList();
        this.getScopeList();
        this.getActivityType();
    }

    searchStorage() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    getDiurnaList() {
        this._enumService.diurnaTypeList().subscribe(result => {
            this.diurnaTypeList = result;
        });
    }

    getScopeList() {
        this._enumService.scopeDeplasareList().subscribe(result => {
            this.scopDeplasareList = result;
        });
    }

    getActivityType() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypes = result;
        });
    }

    saveAccountConfig() {
        this._accountConfigService.saveAccountConfig(this.account).subscribe(result => {
            abp.notify.info('AddModifyMessage');
            this.router.navigate(['/app/conta/nomenclatoare/accounts/accountConfig']);
        });
    }
}