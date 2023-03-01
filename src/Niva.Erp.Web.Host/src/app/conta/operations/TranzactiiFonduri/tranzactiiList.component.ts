import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Session } from "inspector";
import * as moment from "moment";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { DocumentTypeServiceProxy, GetDocumentTypeOutput, OperationSearchDto, OperationServiceProxy, OperationTypesListDto, OperationTypesServiceProxy, TranzactiiFonduriDto, TranzactiiFonduriServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './tranzactiiList.component.html',
    animations: [appModuleAnimation()]
})
export class TrazactiiListComponent extends AppComponentBase implements OnInit {

    dataStart: any;
    dataEnd: any;
    debit: any;
    credit: any;
    explicatie: any;
    tranzactiiList: TranzactiiFonduriDto[] = [];
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _tranzactiiService: TranzactiiFonduriServiceProxy,
        
        private router: Router    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.initialize();
        this.refreshTranzactii();
    }

    initialize() {
        this.dataStart = sessionStorage.getItem('dataStart') ? moment(sessionStorage.getItem('dataStart')) : new Date(new Date().setMonth(new Date().getMonth() - 1));
        this.dataEnd = sessionStorage.getItem('dataEnd') ? moment(sessionStorage.getItem('dataEnd')) : new Date();
        
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    refreshTranzactii() {
        this.isLoading = true;
    this._tranzactiiService.tranzactiiFonduriList(moment(this.dataEnd), moment(this.dataStart), this.debit, this.credit, this.explicatie).pipe(
        delay(1000),
        finalize(() => {
            this.isLoading = false;
        })
    ).subscribe(result => {
            this.tranzactiiList = result;
            
            sessionStorage.setItem('dataStart', this.dataStart.toString());
            sessionStorage.setItem('dataEnd', this.dataEnd.toString());
            
        });
    }
}