import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImoAssetsDDDto, ImoAssetStorageDDDto, ImoGestListDto, ImoGestServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoGestList.component.html',
    animations: [appModuleAnimation()]
})
export class ImoGestListComponent extends AppComponentBase implements OnInit {

    gestList: ImoGestListDto = new ImoGestListDto();
    storageList: ImoAssetStorageDDDto[] = [];
    assetList: ImoAssetsDDDto[] = [];

    constructor(inject: Injector,
        private _imoGestService: ImoGestServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Administrare.MF.CalculGestiune.Acces')) {
            this._imoGestService.initForm().subscribe(result => {
                //  this.gestList = result;

                this.gestList.dataStart = sessionStorage.getItem('dataStart') ? moment(sessionStorage.getItem('dataStart')) : result.dataStart;
                this.gestList.dataEnd = sessionStorage.getItem('dataEnd') ? moment(sessionStorage.getItem('dataEnd')) : result.dataEnd;
                this.gestList.assetId = JSON.parse(sessionStorage.getItem('assetId')) ?? result.assetId;
                this.gestList.storageId = JSON.parse(sessionStorage.getItem('storageId')) ?? result.storageId;
                this.gestList.gestDetail = result.gestDetail;
                this.searchAssets();
                this.searchStorage();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    searchGest() {
        const date = new Date(this.gestList.dataStart.toDate());
        this.gestList.dataStart = moment(date.toLocaleString());
        this._imoGestService.serchGest(this.gestList).subscribe(result => {
            this.gestList = result;

            sessionStorage.setItem('dataStart', this.gestList.dataStart.toString());
            sessionStorage.setItem('dataEnd', this.gestList.dataEnd.toString());
            sessionStorage.setItem('storageId', JSON.stringify(this.gestList.storageId));
            sessionStorage.setItem('assetId', JSON.stringify(this.gestList.assetId));

            this.searchAssets();
            this.searchStorage();
        });
    }

    searchAssets() {
        this._imoGestService.assetListDD(this.gestList.dataStart, this.gestList.dataEnd).subscribe(result => {
            this.assetList = result;
        });
    }

    searchStorage() {
        this._imoGestService.storageListDD(this.gestList.dataStart, this.gestList.dataEnd).subscribe(result => {
            this.storageList = result;
        });
    }

    getGestListCount() {
        if (this.gestList.gestDetail == null) {
            return 0;
        } else {
            return this.gestList.gestDetail.length;        
        }
    }

    showReserve(index: number) {
        this.gestList.gestDetail[index].showReserve = true;
    }

    hideReserve(index: number) {
        this.gestList.gestDetail[index].showReserve = false;
    }

    showModerniz(index: number) {
        this.gestList.gestDetail[index].showModerniz = true;
    }

    hideModerniz(index: number) {
        this.gestList.gestDetail[index].showModerniz = false;
    }
}