import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AccountServiceProxy, GetAccountOutput, ImoAssetAddDirectDto, ImoAssetServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetAddInUse.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetAddInUseComponent extends AppComponentBase implements OnInit {

    assetId: any;
    oper: ImoAssetAddDirectDto = new ImoAssetAddDirectDto();
    accounts: GetAccountOutput;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _accountService: AccountServiceProxy,
        private _imoAssetService: ImoAssetServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.assetId = this.route.snapshot.queryParamMap.get('assetId');

        this._imoAssetService.addDirectInit(this.assetId).subscribe(result => {
            this.oper = result;
            this.searchAccounts();
        })
    }

    searchAccounts() {
        this._accountService.imoAssetAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    setDepreciationDate() {
        this._imoAssetService.addInUseChangeDate(this.oper).subscribe(result => {
            this.oper = result;
        });
    }

    saveAsset() {
        this.isLoading = true;
        this._imoAssetService.saveAssetInUse(this.oper)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false
                }))
            .subscribe(() => {
                abp.notify.info('AddMessage');
                this.router.navigate(['/app/conta/imoAsset/imoAssetPV']);
            });
    }
}