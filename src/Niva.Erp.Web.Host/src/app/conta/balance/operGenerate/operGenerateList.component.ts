import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { OperationDTO, OperationServiceProxy, OperGenerateListDto, OperGenerateServiceServiceProxy } from '../../../../shared/service-proxies/service-proxies';


@Component({
    templateUrl: './operGenerateList.component.html',
    animations: [appModuleAnimation()]
})
export class OperGenerateListComponent extends AppComponentBase implements OnInit{

    dataStart: Date;
    dataEnd: Date;
    operGenerateList: OperGenerateListDto[] = [];
    searchOperationList: OperationDTO[][] = [];
    searchOperation: OperationDTO[];

    constructor(injector: Injector,
        private _operGenerateService: OperGenerateServiceServiceProxy,
        private _operationService: OperationServiceProxy,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Balanta.SfarsitLuna.Acces')) {
            if (sessionStorage.getItem("dataStartOperGenerateList")) {
                this.dataStart = JSON.parse(sessionStorage.getItem("dataStartOperGenerateList"));
            }
            else {
                this.dataStart = new Date();
                this.dataStart = new Date(this.dataStart.setMonth(this.dataStart.getMonth() - 1));
            }

            if (sessionStorage.getItem("dataEndOperGenerateList")) {
                this.dataEnd = JSON.parse(sessionStorage.getItem("dataEndOperGenerateList"));
            }
            else {
                this.dataEnd = new Date();
            }

            this.generateOperList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    generateOperList() {
        sessionStorage.setItem("dataStartOperGenerateList", JSON.stringify(this.dataStart));
        sessionStorage.setItem("dataEndOperGenerateList", JSON.stringify(this.dataEnd));
        this._operGenerateService.operGenerateList(moment(this.dataStart), moment(this.dataEnd)).subscribe(result => {
            this.operGenerateList = result;
        });

        this.searchOperationList.length = this.operGenerateList.length;
    }

    countOper() {
        if (this.operGenerateList.length !== null) {
            return this.operGenerateList.length;
        } else {
            return 0;
        }
    }

    deleteOperation(operationId: number) {
        abp.message.confirm('Operatia va fi stearsa.', 'Sigur?',
            (result: boolean) => {
                if (result) {
                    this._operGenerateService.operGenerateDelete(operationId).subscribe(() => {
                        this.generateOperList();
                    });
                }
            });
    }

    showDetail(operId: number, index: number) {
        this._operationService.searchOperationsByOperGenerateId(operId).subscribe(result => {
            this.searchOperation = result;
            this.searchOperationList[index] = this.searchOperation;
        });

        this.operGenerateList[index].showDetail = true;
    }

    hideDetail(index: number) {
        this.operGenerateList[index].showDetail = false;
    }

    showOperDetail(operGenIndex: number, index: number) {
        this.searchOperationList[operGenIndex][index].showDetail = true;
    }

    hideOperDetail(operGenIndex: number, index: number) {
        this.searchOperationList[operGenIndex][index].showDetail = false;
    }
}
