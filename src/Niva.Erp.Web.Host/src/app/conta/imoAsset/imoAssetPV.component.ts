import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BalanceServiceProxy, GetImoAssetOutput, ImoAssetServiceProxy } from '../../../shared/service-proxies/service-proxies';
import * as moment from "moment";
import { Router } from '@angular/router';

@Component({
    templateUrl: './imoAssetPV.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetPVComponent extends AppComponentBase implements OnInit {

    imoAssetList: GetImoAssetOutput;
    dataStart: Date;
    dataEnd: Date;

    constructor(inject: Injector,
        private _imoAssetService: ImoAssetServiceProxy,
        private _balanceService: BalanceServiceProxy,
        private router: Router) {
        super(inject);
        this.edit = this.edit.bind(this);
        this.dareInFolosinta = this.dareInFolosinta.bind(this);
        this.deleteAsset = this.deleteAsset.bind(this);
        this.showDeleteButton = this.showDeleteButton.bind(this); 
        this.showButtons = this.showButtons.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Intrari.Acces')) {
            this.initialize();
        }
        else {
            this.router.navigate(['/app/home']);
        }
        console.log()
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    initialize() {
        this._balanceService.getBalanceDateNextDay().subscribe(result => {
            this.dataStart = sessionStorage.getItem('dataStartImoAssetPv') ? moment(sessionStorage.getItem('dataStartImoAssetPv')).toDate() : result.toDate();
            this.dataEnd = sessionStorage.getItem('dataEndImoAssetPv') ? moment(sessionStorage.getItem('dataEndImoAssetPv')).toDate() : new Date();
            this.imoAssetListFnc();
        });
    }

    imoAssetListFnc() {
        this._imoAssetService.imoAssetsEntryList(moment(this.dataStart), moment(this.dataEnd)).subscribe(result => {
            this.imoAssetList = result;
            sessionStorage.setItem('dataStartImoAssetPv', this.dataStart.toString());
            sessionStorage.setItem('dataEndImoAssetPv', this.dataEnd.toString());
        })
    }

    getImoAssetListCount() {
        if (this.imoAssetList?.getImoAssets == null) {
            return 0;
        } else {
            return this.imoAssetList?.getImoAssets.length;
        }
    }

    deleteAsset(e) {
        abp.message.confirm('Inregistrarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._imoAssetService.deleteAsset(e.row.key.id).subscribe(() => {
                        abp.notify.info('DeleteMessage');
                        this.imoAssetListFnc();
                    });
                }
            });
    }

    edit(e) {
        this.router.navigate(['/app/conta/imoAsset/imoAssetPVAddDirect'], { queryParams: { assetId: e.row.key.id } });
    }

    dareInFolosinta(e) {
        this.router.navigate(['/app/conta/imoAsset/imoAssetAddInUse'], { queryParams: { assetId: e.row.key.id }});
    }

    showDeleteButton(e) {
        if (e.row.key.processedIn || e.row.key.processedInUse) {
            return false;
        } else {
            if (this.isGranted('Conta.MF.Intrari.Modificare')) {
                return true;
            }
        }
    }

    showButtons(e) {
        if (this.isGranted('Conta.MF.Intrari.Modificare')) {
            return true;
        }
        else {
            return false;
        }
    }
}