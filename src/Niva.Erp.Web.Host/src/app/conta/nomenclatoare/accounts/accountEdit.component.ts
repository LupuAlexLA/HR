import { Component, Injector, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import {
    AccountEditDto,
    CurrencyDto,
    AccountServiceProxy,
    CurrencyServiceProxy,
    ThirdPartyListDDDto,
    PersonServiceProxy,
    EnumTypeDto,
    EnumServiceProxy,
    ThirdPartyAccListDDDto, ActivityTypeDto, ActivityTypeServiceProxy, BNR_SectorListDto, BNR_SectorServiceProxy
} from '../../../../shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';


@Component({
    templateUrl: './accountEdit.component.html',
    animations: [appModuleAnimation()]
})
export class AccountEditComponent extends AppComponentBase implements OnInit {

    account: AccountEditDto = new AccountEditDto();
    currency: CurrencyDto[];
    thirdParties: ThirdPartyListDDDto[] = [];
    accountTypes: EnumTypeDto[];
    accountFuncTypes: EnumTypeDto[];
    taxStatusList: EnumTypeDto[];
    bankAccounts: ThirdPartyAccListDDDto[];
    activityTypes: ActivityTypeDto[] = [];
    accountId: any;
    accountSearch: string;
    isLoading: boolean = false;
    sectorBnrList: BNR_SectorListDto[] = [];

    constructor(injector: Injector,
        private _accountService: AccountServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private _bnrSectorService: BNR_SectorServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit(): void {

        this.accountId = this.route.snapshot.queryParamMap.get('account');
        this.accountSearch = this.route.snapshot.queryParamMap.get('accountSearch');

        this.accountTypeList();
        this.appClientBankAccountList();
        this.accountFuncTypeList();
        this.accountTaxStatusList();
        this.thirdPartyDDList();
        this.currencyDDList();
        this.getActivityTypeList();

        this._accountService.accountEditInit(this.accountId || 0).subscribe(result => {
            this.account = result;
            this.getSectorBnrList();
        });
    }

    accountTypeList() {
        this._enumService.accountTypeList().subscribe(result => {
            this.accountTypes = result;
        });
    }

    getActivityTypeList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypes = result;
        })
    };

    appClientBankAccountList() {
        this._personService.appClientBankAccountList().subscribe(result => {
            this.bankAccounts = result;
        });
    }

    accountFuncTypeList() {
        this._enumService.accountFuncTypeList().subscribe(result => {
            this.accountFuncTypes = result;
        });
    }

    accountTaxStatusList() {
        this._enumService.accountTaxStatusList().subscribe(result => {
            this.taxStatusList = result;
        });
    }

    thirdPartyDDList() {

        this._personService.thirdPartyDDList().subscribe(result => {
            this.thirdParties = result;
        });
    }

    currencyDDList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currency = result;
        });
    }

    getSectorBnrList() {
        this._bnrSectorService.getBNRSectorList().subscribe(result => {
            this.sectorBnrList = result;
        });
    }

    saveAccount() {
        this._accountService.saveAccount(this.account).subscribe(response => {
            abp.notify.info(this.l('Modificarea a fost salvata cu succes'));
            this.account = response;
        });
    }
}