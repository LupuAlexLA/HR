import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvObjectGestListDto, InvObjectGestServiceProxy, InvObjectListDto, InvObjectStorageDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectGestList.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectGestListComponent extends AppComponentBase implements OnInit {

    gestList: InvObjectGestListDto = new InvObjectGestListDto();
    invObjectList: InvObjectListDto[] = [];
    storageList: InvObjectStorageDto[] = [];


    constructor(inject: Injector,
        private _invObjectGestService: InvObjectGestServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Administrare.MF.CalculGestiune.Acces')) {
            this._invObjectGestService.initForm().subscribe(result => {
                this.gestList = result;
                this.searchInvObjects();
                this.searchStorage();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getGestListCount() {
        if (this.gestList.gestDetail == null) {
            return 0;
        } else {
            return this.gestList.gestDetail.length;
        }
    }

    searchGest() {
        const date = new Date(this.gestList.dataStart.toDate());
        this.gestList.dataStart = moment(date.toLocaleString());
        this._invObjectGestService.searchGest(this.gestList).subscribe(result => {
            this.gestList = result;
            this.searchInvObjects();
            this.searchStorage();
        });
    }

    searchInvObjects() {
        this._invObjectGestService.invObjectListDD(this.gestList.dataStart, this.gestList.dataEnd).subscribe(result => {
            this.invObjectList = result;
        });
    }

    searchStorage() {
        this._invObjectGestService.storageListDD(this.gestList.dataStart, this.gestList.dataEnd).subscribe(result => {
            this.storageList = result;
        });
    }

    showReserve(index: number) {
        this.gestList.gestDetail[index].showReserve = true;
    }

    hideReserve(index: number) {
        this.gestList.gestDetail[index].showReserve = false;
    }
}