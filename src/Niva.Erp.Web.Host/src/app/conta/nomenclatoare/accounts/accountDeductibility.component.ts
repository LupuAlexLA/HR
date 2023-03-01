import { Component, Injector, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AccountDeductibilityForm, AccountListDDDto, AccountServiceProxy, EnumServiceProxy, EnumTypeDto } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './accountDeductibility.component.html',
    animations: [appModuleAnimation()]
})
export class AccountDeductibilityComponent extends AppComponentBase implements OnInit {

    accountDeductibility: AccountDeductibilityForm;
    accountSearch: string;
    accountId: number;
    currentDate: any;
    propertyTypes: EnumTypeDto[] = [];
    accountsNeded: AccountListDDDto[] = [];

    constructor(injector: Injector,
        private router: ActivatedRoute,
        private _accountService: AccountServiceProxy,
        private _enumService: EnumServiceProxy) {
        super(injector);
    }

    ngOnInit(): void {
        this.accountSearch = this.router.snapshot.queryParamMap.get('accountSearch');
        this.accountId = +this.router.snapshot.queryParamMap.get('accountId');


        this._accountService.accountDeductibilityInit(this.accountSearch, this.accountId).subscribe(result => {
            this.accountDeductibility = result;
        });
    }

    selectDate(event: Date) {
        const FORMAT = 'YYYY-MM-DDTHH:mm:ss';
        this.accountDeductibility.deductibilityEdit.propertyDate = moment(event.toISOString().replace('.000Z', " "), FORMAT);
    }

    accountDeductibilityEditInit(propertyId: number) {
        this._accountService.accountDeductibilityEditInit(propertyId, this.accountDeductibility).subscribe(result => {
            this.accountDeductibility = result;
            this.currentDate = moment(this.accountDeductibility.deductibilityEdit.propertyDate).toDate();
            this.propertyTypeList();
        });
    }

    accountDeductibilityEditBack() {
        this.accountDeductibility.showList = true;
        this.accountDeductibility.showEdit = false;
    }

    accountDeductibilityEditSave() {
        this._accountService.accountDeductibilityEditSave(this.accountDeductibility).subscribe(result => {
            abp.notify.info(this.l('OKMessage'));
            this.accountDeductibility = result;
        });
    }

    accountDeductibilityDelete(propertyId: number) {
        abp.message.confirm('Inregistrarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._accountService.accountDeductibilityDelete(propertyId, this.accountDeductibility)
                        .subscribe(response => {
                            this.accountDeductibility = response;
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }

    propertyTypeList() {
        this._enumService.taxProfitDeducType().subscribe(result => {
            this.propertyTypes = result;
        });
    }

    propertyTypeChange() {
        if (this.accountDeductibility.deductibilityEdit.propertyTypeId == 1) {
            this.accountDeductibility.deductibilityEdit.propertyValue = 100;
        }
    }

    getAccountsNeded(search: any) {
        this._accountService.accountListNeded(search.target.value).subscribe(result => {
            this.accountsNeded = result;
        });
    }

    getAccountNededName(accountNededId: number) {
        if (accountNededId == 0) {
            return '';
        }
        return this.accountDeductibility.deductibilityEdit.accountNeded;
    }

    selectedAccount(accountNededName: string, accountNededId: number) {
        this.accountDeductibility.deductibilityEdit.accountNededId = accountNededId;
        this.accountDeductibility.deductibilityEdit.accountNeded = accountNededName;
    }

}