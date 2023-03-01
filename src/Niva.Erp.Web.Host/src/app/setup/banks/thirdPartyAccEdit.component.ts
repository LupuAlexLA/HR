import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { GetBankOutput, GetCurrencyOutput, PersonServiceProxy, ThirdPartyAccEditDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './thirdPartyAccEdit.component.html',
    animations: [appModuleAnimation()],
})
export class ThirdPartyAccEditComponent extends AppComponentBase implements OnInit {

    bankId: any;
    thirdPartyId: any;

    thirdPartyAccEdit: ThirdPartyAccEditDto = new ThirdPartyAccEditDto();
    banks: GetBankOutput = new GetBankOutput();
    currencies: GetCurrencyOutput = new GetCurrencyOutput();

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('General.ListaConturi.Modificare')) {
            this.bankId = this.route.snapshot.queryParamMap.get('id');
            this.thirdPartyId = this.route.snapshot.queryParamMap.get('thirdPartyId');

            if (this.bankId != null) {
                this._personService.getBankAccSetupById(this.bankId).subscribe(result => {
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
        this._personService.saveThirdPartySetupAcc(this.thirdPartyAccEdit).pipe(finalize(() => {
            this.notify.info(this.l('SavedSuccessfully'));

        })).subscribe(() => {

            this.router.navigate(['app/setup/banks/thirdPartyAcc']);
        });
    }
}