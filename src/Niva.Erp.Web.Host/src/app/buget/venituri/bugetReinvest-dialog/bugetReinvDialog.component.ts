import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { BugetVenituriServiceProxy, BVC_VenitTitluCFReinvDto, GetCurrencyOutput, PersonServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetReinvDialog.component.html',
    animations: [appModuleAnimation()]
})
export class BugetReinvDialogComponent extends AppComponentBase implements OnInit {

    bVC_VenitTitluCFId: number;
    formularBVCId: number;
    Reinv: BVC_VenitTitluCFReinvDto = new BVC_VenitTitluCFReinvDto();
    currenciesList: GetCurrencyOutput;

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private _bugetVenituriService: BugetVenituriServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.Reinv.id = 0;
        this.Reinv.bvC_VenitTitluCFId = this.bVC_VenitTitluCFId;
        this.Reinv.currencyId = null;
        this.Reinv.delete = false;
        this.getCurrenciesList();
    }

    save() {
        this.bsModalRef.hide();
        this.onSave.emit(this.Reinv);
    }

    getCurrenciesList() {
        this._personService.currencyList().subscribe(result => {
            this.currenciesList = result;
        });
    }

    getCursValutar() {
        this._bugetVenituriService.exchangeForecastByCurrency(this.formularBVCId, this.Reinv).subscribe(result => {
            this.Reinv = result;
        });
    }
}