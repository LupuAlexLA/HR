import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { result } from 'lodash';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { DataTragereDto, TragereServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './calculatorDobanda.component.html',
    animations: [appModuleAnimation()]
})
/** ImprumuturiTermen component*/
export class CalculatorDobandaComponent extends AppComponentBase implements OnInit  {
    dateList: DataTragereDto[] = [];
    newDate;
    isLoading: boolean = false;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _tragereService: TragereServiceProxy   ) {
        super(injector);
    }

    ngOnInit() {
        
        this.getDateList();
       
    }

    getDateList() {
        this._tragereService.dateTragereList().subscribe(result => {
            this.dateList = result;
        });
    }

    saveDate() {
        this.isLoading = true;
        this._tragereService.calculatorDobanda(this.newDate).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.dateList = result;
        });
    }

    refreshDate(value) {
        this.isLoading = true;
        this._tragereService.calculatorDobanda(value).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.dateList = result;
        });
    }

    deleteDate(date) {
        this.isLoading = true;
        this._tragereService.deleteDobandaByDate(date).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.dateList = result;
        })
    }

}