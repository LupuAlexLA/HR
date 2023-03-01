import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AccountServiceProxy, GetAccountOutput, GetAssetOutput, InvoiceServiceProxy, PrepaymentsAddDto, PrepaymentsServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsAddInvoice.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsAddInvoiceComponent extends AppComponentBase implements OnInit {

    prepaymentType: number;
    oper: PrepaymentsAddDto = new PrepaymentsAddDto();
    invoiceList: GetAssetOutput;
    modCalcul: number;
    accounts: GetAccountOutput;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentsService: PrepaymentsServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _accountService: AccountServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.CheltAvans.Chelt.Modificare')) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentsService.addFromInvoiceInit(this.prepaymentType).subscribe(result => {
                this.oper = result;
            });
            this._prepaymentsService.durationSetupDetails(this.prepaymentType).subscribe(result => {
                this.modCalcul = result.prepaymentDurationCalcId;
            });
            this.searchInvoice();
        } else if (this.isGranted('Conta.VenitAvans.Venituri.Modificare')) {
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentsService.addFromInvoiceInit(this.prepaymentType).subscribe(result => {
                this.oper = result;
            });
            this._prepaymentsService.durationSetupDetails(this.prepaymentType).subscribe(result => {
                this.modCalcul = result.prepaymentDurationCalcId;
            });
            this.searchInvoice();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    searchInvoice() {
        this._invoiceService.invoicesForPrepayments(this.prepaymentType).subscribe(result => {
            this.invoiceList = result;
        });
    }

    searchAccounts() {
        this._accountService.prepaymentsAccountList(this.prepaymentType).subscribe(result => {
            this.accounts = result;
        });
    }

    showInvoiceDetails() {
        this._prepaymentsService.invoiceDetails(this.oper).subscribe(result => {
            this.oper = result;
            this.searchAccounts();
        });
    }

    calcDurata(index: number) {
        this._prepaymentsService.calcDurata(this.oper.invoiceDetail[index].depreciationStartDate, this.oper.invoiceDetail[index].endDateChelt, this.modCalcul).subscribe(result => {
            this.oper.invoiceDetail[index].durationInMonths = result;
        });
    }

    showForm(formNr: number) {
        if (formNr == 1) {
            this.oper.showForm1 = true;
            this.oper.showForm2 = false;
        }

        if (formNr == 2) {
            this.oper.showForm1 = false;
            this.oper.showForm2 = true;
        }
    }

    savePrepayments() {
        this._prepaymentsService.savePrepayments(this.oper).subscribe(result => {
            abp.notify.info('AddMessage');
            this.oper = result;
        });
    }
}