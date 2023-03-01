import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { CupiuriDetailsDto, CupiuriForm, CupiuriListDto, CupiuriServiceProxy, GetCurrencyOutput, PersonServiceProxy, SoldInitialServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './cupiuriAdd.component.html',
    animations: [appModuleAnimation()]
})
export class CupiuriAddComponent extends AppComponentBase implements OnInit {

    form: CupiuriForm = new CupiuriForm();
    currencies: GetCurrencyOutput = new GetCurrencyOutput();
    cupiuriId: number;

    constructor(inject: Injector,
        private _personService: PersonServiceProxy,
        private _cupiuriService: CupiuriServiceProxy,
        private _soldInitialService: SoldInitialServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.cupiuriId = + this.route.snapshot.queryParamMap.get('cupiuriId');
        this._cupiuriService.cupiuriFormInit(this.cupiuriId).subscribe(result => {
            this.form = result;
        });

        this.getCurrencyList();
    }

    getCurrencyList() {
        this._personService.currencyList().subscribe(result => {
            this.currencies = result;
        });
    }

    deleteCupiuriDetRow(index: number, indexDet: number) {
        this.form.cupiuri[index].cupiuriDetails.splice(indexDet, 1);
        this.updateTotalValue(index);
    }

    addRow() {
        this._cupiuriService.addRow(this.form).subscribe(result => {
            this.form = result;
        });
    }

    addDetailsRow(index: number) {
        this._cupiuriService.addDetailsRow(index, this.form).subscribe(result => {
            this.form = result;
            this.updateTotalValue(index);
        });
    }

    deleteCupiuriRow(index: number) {
        this.form.cupiuri.splice(index, 1);
    }

    cupiuriDetailsSave() {
        this._cupiuriService.save(this.form).subscribe(result => {
            abp.notify.info("Cupiurile au fost inregistrate");
        });
    }

    updateTotalValue(rowIndex: number) {
        this._cupiuriService.updateTotal(rowIndex, this.form).subscribe(result => {
            this.form = result;
        });
    }

    getSoldForCurrency(currencyId: number, index: number) {
        this._cupiuriService.soldForCurrencyId(currencyId, this.form.operationDate).subscribe(result => {
            this.form.cupiuri[index].sold = result;
        });
    }
}