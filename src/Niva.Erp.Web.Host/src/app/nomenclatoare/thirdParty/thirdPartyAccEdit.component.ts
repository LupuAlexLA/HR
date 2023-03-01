import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetBankOutput, GetCurrencyOutput, PersonServiceProxy, ThirdPartyAccEditDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './thirdPartyAccEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ThirdPartyAccountEditComponent extends AppComponentBase implements OnInit {

    bankId: number;
    thirdPartyId: number;
    thirdPartyAccEdit: ThirdPartyAccEditDto = new ThirdPartyAccEditDto();
    banks: GetBankOutput;
    currencies: GetCurrencyOutput;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted("General.Persoane.Acces")) {
            this.bankId = +this.route.snapshot.queryParamMap.get('id');
            this.thirdPartyId = +this.route.snapshot.queryParamMap.get('thirdPartyId');
            this.thirdPartyAccEdit.thirdPartyId = this.thirdPartyId;

            if (this.bankId != 0) {
                this._personService.getBankAccById(this.bankId).subscribe(result => {
                    this.thirdPartyAccEdit = result;
                });
            }

            this._personService.bankList().subscribe(result => {
                this.banks = result;
            });

            this._personService.currencyList().subscribe(result => {
                this.currencies = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    saveBankFnc() {
        this.isLoading = true;
        this._personService.saveThirdPartyAcc(this.thirdPartyAccEdit).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
                this.notify.info(this.l('BankAddMessage'));
            })
        ).subscribe(() => { });
    }
}