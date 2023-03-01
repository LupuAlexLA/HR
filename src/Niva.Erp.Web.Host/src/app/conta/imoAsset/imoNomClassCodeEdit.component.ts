import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AccountServiceProxy, GetAccountOutput, GetImoAssetClassCodeOutput, ImoAssetClassCodeEditDto, ImoAssetClassCodeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoNomClassCodeEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImoNomClassCodeEditComponent extends AppComponentBase implements OnInit {

    classCode: ImoAssetClassCodeEditDto = new ImoAssetClassCodeEditDto();
    classCodeParrent: GetImoAssetClassCodeOutput;
    accounts: GetAccountOutput;
    classCodeId: any;

    constructor(inject: Injector,
        private _imoAssetClassCodeService: ImoAssetClassCodeServiceProxy,
        private _accountService: AccountServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.classCodeId = this.route.snapshot.queryParamMap.get('classCodeId');

        if (this.classCodeId !== null) {
            this._imoAssetClassCodeService.getClassCodeById(this.classCodeId).subscribe(result => {
                this.classCode = result;
            });
        }

        this.searchAccount();
        this.searchClassCodeParrent();
    }

    searchAccount() {
        this._accountService.imoAssetAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    searchClassCodeParrent() {
        this._imoAssetClassCodeService.imoAssetClassCodeList().subscribe(result => {
            this.classCodeParrent = result;
        });
    }

    saveClassCodeFnc() {
        this._imoAssetClassCodeService.saveClassCode(this.classCode).subscribe(() => {
            abp.notify.info(this.l('InvClassCodeAddMessage'));
        });
    }
}