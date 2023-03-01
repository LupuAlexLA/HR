import { Component, Injector, OnInit} from '@angular/core';
import { AppComponentBase } from "../../../shared/app-component-base";
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ContractDto, ContractSearchParametersDTO, ContractsServiceProxy, EnumServiceProxy, EnumTypeDto } from '../../../shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ContractStateDialogComponent } from '../contract-dialog/contract-state-dialog.component';
import { ContractStateListDialogComponent } from '../contract-dialog/contract-state-list-dialog.component';
import { Router } from '@angular/router';

@Component({
    templateUrl: './contractsList.component.html',
    animations: [appModuleAnimation()]
})
export class ContractsListComponent extends AppComponentBase implements OnInit {

    contractsList: ContractDto[] = [];
    contractSearchParam: ContractSearchParametersDTO = new ContractSearchParametersDTO();
    contractsStatus: EnumTypeDto[] = [];

    constructor(injector: Injector,
                private _contractService: ContractsServiceProxy,
                private _modalService: BsModalService,
                private _enumService: EnumServiceProxy,
                private router: Router) {
        super(injector);
        this.editContract = this.editContract.bind(this);
        this.delete = this.delete.bind(this);
        this.changeStateDialog = this.changeStateDialog.bind(this);
        this.stateListDialog = this.stateListDialog.bind(this);
    }   

    ngOnInit() {
        if (this.isGranted("General.Contracte.Acces")) {
            this.extendPeriod();
            this.initSearchContract();
            this.getContractsStatus();
        } else {
            this.router.navigate(['app/home']);
        }

    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    getNrAndData(rowData) {
        return rowData.contractNr + " / " + moment(rowData.contractDate).format("DD.M.YYYY");
    }

    getValueAndCurrency(rowData) {
        return ((rowData.value === null) ? "" : rowData.value.toLocaleString()) + " / " + ((rowData.currencyName === null) ? "" : rowData.currencyName);
    }

    editContract(e) {
        this.router.navigate(['/app/economic/contracts/contractNew'], { queryParams: { contractId: e.row.key.id } });
    }

    changeStateDialog(e) {
        let ContractStateDialog: BsModalRef;

        ContractStateDialog = this._modalService.show(
            ContractStateDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    contractId: e.row.key.id
                },
            }
        );

        ContractStateDialog.content.onSave.subscribe(() => {
            this.initSearchContract();
        });
    }

    stateListDialog(e) {
        let ContractStateDialog: BsModalRef;

        ContractStateDialog = this._modalService.show(
            ContractStateListDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    contractId: e.row.key.id
                },
            }
        );

        //ContractStateDialog.content.onSave.subscribe(() => {
        //    this.initSearchContract();
        //});
    }

    initSearchContract() {
        this._contractService.initSearch().subscribe(result => {
            this.contractSearchParam.dataEnd = sessionStorage.getItem('dataEndContracts') ? moment(sessionStorage.getItem('dataEndContracts')) : result.dataEnd;
            this.contractSearchParam.dataStart = sessionStorage.getItem('dataStartContracts') ? moment(sessionStorage.getItem('dataStartContracts')) : result.dataStart;
            this.contractSearchParam.thirdPartyQuality = sessionStorage.getItem('thirdPartyQualityContracts') ?
                JSON.parse(sessionStorage.getItem('thirdPartyQualityContracts')) : result.thirdPartyQuality;
            this.contractSearchParam.thirdParty = sessionStorage.getItem('thirdPartyContracts') ? sessionStorage.getItem('thirdPartyContracts') : result.thirdParty;
            this.contractSearchParam.state = sessionStorage.getItem('stateContracts') ? JSON.parse(sessionStorage.getItem('stateContracts')) : result.state;

            this.contractSearch();   
        });
    }

    contractSearch() {
        this._contractService.search(this.contractSearchParam).subscribe(result => {
            this.contractsList = result;
            sessionStorage.setItem('dataStartContracts', this.contractSearchParam.dataStart.toString());
            sessionStorage.setItem('dataEndContracts', this.contractSearchParam.dataEnd.toString());
            sessionStorage.setItem('thirdPartyQualityContracts', JSON.stringify(this.contractSearchParam.thirdPartyQuality));
            sessionStorage.setItem('thirdPartyContracts', this.contractSearchParam.thirdParty ?? "");
            sessionStorage.setItem('stateContracts', JSON.stringify(this.contractSearchParam.state));
        });
    }

    getContractsStatus() {
        this._enumService.contractsStatusList().subscribe(result => {
            this.contractsStatus = result;
        });
    }

    getFacturiCountText() {
        return this.contractsList.length;
    }

    extendPeriod() {
        var currentDate = new Date();
        this._contractService.extendContractPeriod(moment(currentDate)).subscribe(() => { });
    }

    delete(e) {
        abp.message.confirm('Contractul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._contractService.deleteContract(e.row.key.id)
                        .subscribe(() => {
                            this.initSearchContract();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }
}