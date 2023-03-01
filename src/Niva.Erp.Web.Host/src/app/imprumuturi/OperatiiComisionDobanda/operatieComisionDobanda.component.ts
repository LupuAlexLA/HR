import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { EnumServiceProxy, EnumTypeDto, ImprumutServiceProxy, OperatieDobandaComisionDtoList, OperatieComisionDobandaServiceProxy, OperatieDobandaComisionDto, ImprumutDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './operatieComisionDobanda.component.html',
    animations: [appModuleAnimation()]
})
/** ImprumuturiTermen component*/
export class OperatieComisionDobandaComponent extends AppComponentBase implements OnInit  {
    operatieList: OperatieDobandaComisionDtoList = new OperatieDobandaComisionDtoList();
    operatiiListForTable: OperatieDobandaComisionDto[] = [];
    newOperatie: OperatieDobandaComisionDto = new OperatieDobandaComisionDto();
    imprumutStareList: EnumTypeDto[] = [];
    imprumutId: number;
    imprumut: ImprumutDto = new ImprumutDto();

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _operatieComisionDobanda: OperatieComisionDobandaServiceProxy,
        private _imprumutService: ImprumutServiceProxy,
        private _enumSerivce: EnumServiceProxy,   ) {
        super(injector);
    }

    ngOnInit() {

        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        if (this.imprumutId) {
            this._imprumutService.imprumutId(this.imprumutId).subscribe(result => this.imprumut = result);
        }
        this.getOperatieList();

    }

  
    getOperatieList() {
        this._operatieComisionDobanda.operatieComisionDobandaByImprumutIdList(Number(this.route.snapshot.queryParamMap.get('imprumutId'))).subscribe(result => {
            this.operatieList = result
            console.log("1")
        });
        
    }

    saveOperatieComisionDobanda() {
        this.newOperatie.imprumutId = this.imprumutId;


        if (this.newOperatie.tipOperatieDobandaComision == 0) {

            if (this.operatieList.operatieComision.length) {
                this.newOperatie.dataStart = this.operatieList.operatieComision[this.operatieList.operatieComision.length - 1].dataEnd;
            }
            else {
                this.newOperatie.dataStart = this.imprumut.documentDate;
                
            }
            
            this._operatieComisionDobanda.operatieComisionDobandaSave(this.newOperatie).subscribe(() =>
                this.getOperatieList());
        }

        else if (this.newOperatie.tipOperatieDobandaComision == 1) {

            if (this.operatieList.operatieDobanda.length) {
                this.newOperatie.dataStart = this.operatieList.operatieDobanda[this.operatieList.operatieDobanda.length - 1].dataEnd;
            }
            else {
                this.newOperatie.dataStart = this.imprumut.documentDate;
                
            }

            this._operatieComisionDobanda.operatieComisionDobandaSave(this.newOperatie).subscribe(() =>
                this.getOperatieList());
        }


        else if (this.newOperatie.tipOperatieDobandaComision == 2) {
            this.newOperatie.tipOperatieDobandaComision = 0;
            if (this.operatieList.operatieComision.length) {
                this.newOperatie.dataStart = this.operatieList.operatieComision[this.operatieList.operatieComision.length - 1].dataEnd;
            }
            else {
                this.newOperatie.dataStart = this.imprumut.documentDate;
                
            }

            this._operatieComisionDobanda.operatieComisionDobandaSave(this.newOperatie).subscribe(() => {
                this.newOperatie.tipOperatieDobandaComision = 1;
                if (this.operatieList.operatieComision.length) {
                    this.newOperatie.dataStart = this.operatieList.operatieDobanda[this.operatieList.operatieDobanda.length - 1].dataEnd;
                }
                else {
                    this.newOperatie.dataStart = this.imprumut.documentDate;
                   
                }
                this._operatieComisionDobanda.operatieComisionDobandaSave(this.newOperatie).subscribe(() =>
                    this.getOperatieList())
            });
            this.newOperatie.tipOperatieDobandaComision = 2;
        }
        
        
        
    }

    operationDelete(id) {
        this._operatieComisionDobanda.operatieDeleteId(id).subscribe(() => this.getOperatieList());
    }

}