import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { AccountConfigDto, AccountConfigServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './accountConfig.component.html',
    animations: [appModuleAnimation()]
})
export class AccountConfigComponent extends AppComponentBase implements OnInit {

    accountList: AccountConfigDto[] = [];

    constructor(inject: Injector,
        private _accountConfigService: AccountConfigServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.ConfigConturi.Acces')) {
            this.searchAccount();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchAccount() {
        this._accountConfigService.accountConfigList().subscribe(result => {
            this.accountList = result;
        });
    }

    accountListCount() {
        if (this.accountList.length == null) {
            return 0;
        } else {
            return this.accountList.length;
        }
    }

    deleteAccountConfig(id: number) {
        abp.message.confirm('Contul va fi sters. Sigur?',
            null,
            (result: boolean) => {
                if (result) {
                    this._accountConfigService.deleteAccountConfig(id).subscribe(result => {
                        abp.notify.info('DeleteMessage');
                        this.searchAccount();
                    })
                }
            })
    }
}