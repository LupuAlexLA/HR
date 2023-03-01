import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { DecontInitForm, DecontServiceProxy, DiurnaListDto, DiurnaServiceProxy, EnumServiceProxy, EnumTypeDto, GetThirdPartyOutput, PersonServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './decontList.component.html',
    animations: [appModuleAnimation()]
})
export class DecontListComponent extends AppComponentBase implements OnInit {
    decontInit: DecontInitForm = new DecontInitForm();
    diurnaList: DiurnaListDto[] = [];
    decontTypeList: EnumTypeDto[] = [];
    scopDeplasareList: EnumTypeDto[] = [];
    thirdParties: GetThirdPartyOutput;
    startDate: Date = null;
    endDate: Date = null;
    rangeDateDelegatie: any = [];

    constructor(inject: Injector,
        private _decontService: DecontServiceProxy,
        private _diurnaService: DiurnaServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router) {
        super(inject);
        this.editDecont = this.editDecont.bind(this);
        this.deleteDecont = this.deleteDecont.bind(this);
        this.isVisible = this.isVisible.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Deconturi.Acces')) {
            if (sessionStorage.getItem('delegatieStartDate')) {
                this.startDate = JSON.parse(sessionStorage.getItem('delegatieStartDate'));
                this.rangeDateDelegatie[0] = new Date(this.startDate);
            }

            if (JSON.parse(sessionStorage.getItem('delegatieEndDate'))) {
                this.endDate = JSON.parse(sessionStorage.getItem('delegatieEndDate'));
                this.rangeDateDelegatie[1] = new Date(this.endDate);
            }

            this._decontService.initForm().subscribe(result => {

                this.getDecontTypes();
                this.getScopDeplasareList();
                this.getCountryList();

                this.decontInit.thirdPartyId = sessionStorage.getItem('decontThirdPartyId') ? JSON.parse(sessionStorage.getItem('decontThirdPartyId')) : result.thirdPartyId;
                this.decontInit.thirdPartyName = sessionStorage.getItem('thirdPartyName') ? JSON.parse(sessionStorage.getItem('thirdPartyName')) : "";

                this.decontInit.decontStartDate = sessionStorage.getItem('decontStartDate') ? moment(sessionStorage.getItem('decontStartDate')) : result.decontStartDate;
                this.decontInit.decontEndDate = sessionStorage.getItem('decontEndDate') ? moment(sessionStorage.getItem('decontEndDate')) : result.decontEndDate;

                this.decontInit.diurnaLegalaId = sessionStorage.getItem('diurnaLegalaId') ? JSON.parse(sessionStorage.getItem('diurnaLegalaId')) : result.diurnaLegalaId;
                this.decontInit.decontTypeId = sessionStorage.getItem('decontTypeId') ? JSON.parse(sessionStorage.getItem('decontTypeId')) : result.decontTypeId;
                this.decontInit.scopDeplasareTypeId = sessionStorage.getItem('scopDeplasareTypeId') ? JSON.parse(sessionStorage.getItem('scopDeplasareTypeId')) : result.scopDeplasareTypeId;
                this.decontInit.delegatieEndDate = sessionStorage.getItem('delegatieEndDate') ? moment(sessionStorage.getItem('delegatieEndDate')) : result.delegatieEndDate;
                this.decontInit.delegatieStartDate = sessionStorage.getItem('delegatieStartDate') ? moment(sessionStorage.getItem('delegatieStartDate')) : result.delegatieStartDate;
                this.decontInit.documentType = sessionStorage.getItem('documentTypeName') ? sessionStorage.getItem('documentTypeName') : result.documentType;
                this.searchDecontList();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    getDataStartEnd(rowData) {
        if (rowData.dateStart != null && rowData.dateEnd != null) {
            return moment(rowData.dateStart).format('DD.M.YYYY') + " - " + moment(rowData.dateEnd).format('DD.M.YYYY');
        }
        return "";
        //return (rowData.scopDeplasareTypeName == "Alte deconturi") ? "" : moment(rowData.dateStart).format('DD.M.YYYY') + " - " + moment(rowData.dateEnd).format('DD.M.YYYY');
    }

    searchDecontList() {
        if (this.rangeDateDelegatie?.length > 0) {
            sessionStorage.setItem('delegatieStartDate', JSON.stringify(this.rangeDateDelegatie[0]));
            sessionStorage.setItem('delegatieEndDate', JSON.stringify(this.rangeDateDelegatie[1]));
            this.decontInit.delegatieEndDate = moment(this.rangeDateDelegatie[1])
            this.decontInit.delegatieStartDate = moment(this.rangeDateDelegatie[0]);

        } else {
            sessionStorage.removeItem('delegatieStartDate');
            sessionStorage.removeItem('delegatieEndDate');
            this.decontInit.delegatieEndDate = null;
            this.decontInit.delegatieStartDate = null;
        }

        this._decontService.searchDecont(this.decontInit).subscribe(result => {
            this.decontInit.decontList = result;

            sessionStorage.setItem('decontThirdPartyId', JSON.stringify(this.decontInit.thirdPartyId));
            sessionStorage.setItem('decontStartDate', this.decontInit.decontStartDate.toString());
            sessionStorage.setItem('decontEndDate', this.decontInit.decontEndDate.toString());
            sessionStorage.setItem('diurnaLegalaId', JSON.stringify(this.decontInit.diurnaLegalaId));
            sessionStorage.setItem('decontTypeId', JSON.stringify(this.decontInit.decontTypeId));
            sessionStorage.setItem('scopDeplasareTypeId', JSON.stringify(this.decontInit.scopDeplasareTypeId));
            sessionStorage.setItem('documentTypeName', this.decontInit.documentType);
        });
    }

    getDecontCount() {
        if (this.decontInit.decontList?.length == null) {
            return 0;
        } else {
            return this.decontInit.decontList.length;
        }
    }

    getDecontTypes() {
        this._enumService.decontTypeList().subscribe(result => {
            this.decontTypeList = result;
        });
    }

    getScopDeplasareList() {
        this._enumService.scopeDeplasareList().subscribe(result => {
            this.scopDeplasareList = result;
        });
    }

    getCountryList() {
        this._diurnaService.diurnaLegalaList().subscribe(result => {
            this.diurnaList = result;
        });
    }

    searchThirdParty(thirdPartyName: any) {
        this._personService.thirdPartySearch(thirdPartyName.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    selectedThirdParty(id: number, thirdPartyName: string) {
        this.decontInit.thirdPartyId = id;
        this.decontInit.thirdPartyName = thirdPartyName;

        sessionStorage.setItem('thirdPartyName', JSON.stringify(thirdPartyName));
        this.searchDecontList();
        this.getThirdPartyName();
    }

    getThirdPartyName() {
        return (this.decontInit.thirdPartyId === null || this.decontInit.thirdPartyId === undefined) ? "" : this.decontInit.thirdPartyName;
    }

    deleteDecont(e) {
        abp.message.confirm('Decontul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._decontService.deleteDecont(e.row.key.id)
                        .subscribe(() => {
                            this.searchDecontList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }

    editDecont(e) {
        this.router.navigate(['/app/decontAdd'], { queryParams: { decontId: e.row.key.id } });
    }

    isVisible(e) {
        if (this.isGranted('Conta.Deconturi.Modificare')) {
            return true;
        }
        else {
            return false;
        }
    }
}