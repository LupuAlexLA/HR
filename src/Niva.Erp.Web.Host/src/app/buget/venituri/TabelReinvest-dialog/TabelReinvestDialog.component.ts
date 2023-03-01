import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetReinvestIncasari, BugetVenituriServiceProxy, BVC_VenitTitluCFReinvDto, GetCurrencyOutput, PersonServiceProxy, TabelIncasari } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './tabelReinvDialog.component.html',
    animations: [appModuleAnimation()]
})
export class TabelReinvDialogComponent extends AppComponentBase implements OnInit {

    bVC_VenitTitluCFId: number;
    formularBVCId: number;
    tableData: TabelIncasari[] = [];
    currenciesList: GetCurrencyOutput;

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.getCurrenciesList();

    }

    prepareDataTable() {
        
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    save() {
        this.bsModalRef.hide();
        this.onSave.emit();
    }

    getCurrenciesList() {
        this._personService.currencyList().subscribe(result => {
            this.currenciesList = result;
        });
    }


    
}