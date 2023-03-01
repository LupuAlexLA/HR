import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ComisioaneServiceProxy, ComisionDto, DataComisionDto, DataComisionEditDto, DataComisionServiceProxy, ImprumutDto, ImprumutServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './DateComisionList.component.html',
    animations: [appModuleAnimation()],

})
/** DateComisionList component*/
export class DateComisionListComponent extends AppComponentBase implements OnInit {
    /** DateComisionList ctor */

    comision: ComisionDto = new ComisionDto();
    dateComision : DataComisionEditDto[] = [];
    comisionId: number;
    imprumutId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _comisionService: ComisioaneServiceProxy,
        private _dataComisionService: DataComisionServiceProxy,) {
        super(injector);
    }
    ngOnInit(): void {
        this.comisionId = + this.route.snapshot.queryParamMap.get('comisionId');
        this.getDateComisionAndComisionId(this.comisionId);

        
    }

    getDateComisionAndComisionId(comisionId: number) {
        this._dataComisionService.dataComisionEditListId(comisionId).subscribe(result => {
            this.dateComision = result;
            this._comisionService.comisionId(comisionId).subscribe(result => {
                this.comision = result
                this.imprumutId = result.imprumutId;
            });

        });
    }

    getDateComisionListId(comisionId: number) {
        this._dataComisionService.dataComisionEditListId(comisionId).subscribe(result => {
            this.dateComision = result;
        });
    }

    getDateComisionCount() {
        if (this.dateComision.length > 0) {
            return this.dateComision.length;
        } else {
            return 0;
        }
    }

    validateDataComision(_dataComision: DataComisionEditDto) {


        abp.message.confirm('Datele Comisioanelor validate nu sunt editabile. Esti sigur/a?',
            undefined,
            (result: boolean) => {
                if (result) {
                    
                    _dataComision.isValid = true
                    _dataComision.comisionId = this.comisionId;
                    _dataComision.imprumutId = this.imprumutId;
                    console.log(_dataComision)
                    this._dataComisionService.saveDataComision(_dataComision).subscribe(() => {
                        abp.notify.info(this.l('UpdateMessage'));
                        this.getDateComisionListId(this.comisionId);
                       
                    });
                }
            });
    }

    saveDataComision(_dataComision: DataComisionEditDto) {
        //this.rata = Object.assign(this.rata,_rata);
        _dataComision.imprumutId = this.imprumutId;
        _dataComision.comisionId = this.comisionId;
        //this.rata.isValid = true;


            abp.message.confirm('Esti sigur?',
                undefined,
                (result: boolean) => {
                    if (result) {
                        console.log("validat");
                        this._dataComisionService.saveDataComision(_dataComision).subscribe(() => {
                            abp.notify.info(this.l('UpdateMessage'));
                            this.getDateComisionListId(this.comisionId);
                        });
                    }
                });
        
        
    }
    delete(dataComisionId: number) {
        this._dataComisionService.deleteDataComision(dataComisionId).subscribe(() => {

            this.getDateComisionListId(this.comisionId);

            abp.notify.success(this.l('DeleteMessage'));
        })
    }
}