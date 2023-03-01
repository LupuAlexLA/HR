import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { CurrencyDto, CurrencyServiceProxy, DiurnaEditDto, DiurnaServiceProxy, EnumServiceProxy, EnumTypeDto, GetCountryOutput, PersonServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './diurnaEdit.component.html',
    animations: [appModuleAnimation()]
})
export class DiurnaEditComponent extends AppComponentBase implements OnInit {

    diurna: DiurnaEditDto = new DiurnaEditDto();
    currencies: CurrencyDto[] = [];
    countries: GetCountryOutput;
    diurnaId: number;
    diurnaTypes: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _diurnaService: DiurnaServiceProxy,
        private _personService: PersonServiceProxy,
        private _currencyService: CurrencyServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.diurnaId = + this.route.snapshot.queryParamMap.get('diurnaId');

        this._diurnaService.getDiurnaLegalaById(this.diurnaId || 0).subscribe(result => {
            this.diurna = result;
            this.currencyList();
            this.countryList();
            this.diurnaTypeList();
        });
    }

    currencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    countryList() {
        this._personService.countryList().subscribe(result => {
            this.countries = result;
        });
    }

    diurnaTypeList() {
        this._enumService.diurnaTypeList().subscribe(result => {
            this.diurnaTypes = result;
        });
    }

    save() {
        this._diurnaService.saveDiurnaLegala(this.diurna).subscribe(result => {
            this.notify.success("OKMessage");
            this.router.navigate(['/app/conta/nomenclatoare/diurna/diurnaList']);
        });

    }

}