import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { ExchangeRateModelDto, PersonServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './exchangeRatesList.component.html',
    animations: [appModuleAnimation()]
})
export class ExchangeRatesListComponent extends AppComponentBase implements OnInit {

    exchangeRate: ExchangeRateModelDto = new ExchangeRateModelDto();
    

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private router: Router    ) {
        super(injector);
        this.updateItem = this.updateItem.bind(this);
        this.deleteItem = this.deleteItem.bind(this);
    }

    ngOnInit() {
        if (this.isGranted('General.CursValutar.Acces')) {
            this._personService.exchangeRateInit().subscribe(result => {
                this.exchangeRate = result;
                this.searchExchangeRates();
            });
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

    updateItem(e) {
        if (e.row.key.id !== "") {
            this.router.navigate(['/app/conta/nomenclatoare/exchangeRates/exchangeRatesListAdd'], { queryParams: { exchangeRateId : e.row.key.id }});
        }
    }

    deleteItem(e) {
        abp.message.confirm(
            'Cursul valutar va fi sters. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    if (e.row.key.id !== "") {
                        this._personService.exchangeRateDetele(parseInt(e.row.key.id)).subscribe(() => { this.searchExchangeRates(), abp.notify.info('OKStergere'); });
                    }
                }
            }
        );
        
    }

    searchExchangeRates() {
        this._personService.searchExchangeRates(this.exchangeRate).subscribe(result => {
            this.exchangeRate = result;
        });
    }

}