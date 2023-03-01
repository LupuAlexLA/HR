import { Component, Injector, OnInit } from '@angular/core';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BankListDto, GetCurrencyOutput, GetThirdPartyOutput, InvoiceDTO, InvoiceListSelectableDto, InvoiceServiceProxy, PayementOrdersForm, PaymentOrdersServiceProxy, PersonServiceProxy, ThirdPartyAccListDto } from '../../../shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
    templateUrl: './paymentOrders.component.html',
    animations: [appModuleAnimation()],
    styles: ['./paymentOrders.component.css'],
})
export class PaymentOrdersComponent extends AppComponentBase implements OnInit {

    form: PayementOrdersForm;
    bankForPayer: BankListDto[] = [];
    thirdPartyList: GetThirdPartyOutput;
    bankAccountsForPayer: ThirdPartyAccListDto[] = [];
    bankForBenef: BankListDto[] = [];
    bankAccountsForBenef: ThirdPartyAccListDto[] = [];
    invoicesForBenef: InvoiceListSelectableDto[] = [];
    invoice: InvoiceDTO;
    currencies: GetCurrencyOutput;
    isLoading: boolean = false;
    isGridVisible: boolean = false;
    /*dataGridInstance: DataGrid;*/

    constructor(inject: Injector,
        private _paymentOrdersService: PaymentOrdersServiceProxy,
        private _personService: PersonServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private router: Router    ) {
        super(inject);
        this.paymentOrderDetail2 = this.paymentOrderDetail2.bind(this);
        this.paymentOrderDelete2 = this.paymentOrderDelete2.bind(this);
        this.isVisible = this.isVisible.bind(this);
        
    }

    ngOnInit() {
        if (this.isGranted('Casierie.OP.Acces')) {
            if (sessionStorage.getItem('formPaymentOrders')) {
                this.form = JSON.parse(sessionStorage.getItem('formPaymentOrders'));
                this.paymentOrdersList();
            }
            else {
                this._paymentOrdersService.initForm().subscribe(result => {
                    this.form = result;
                    this.searchCurrency();
                });
            }
        }
        else {
            this.router.navigate(['/app/home']);
        }
        
        
    }

    // interesant de utilizat verifica documentatia
    //saveGridInstance(e) {
    //    this.dataGridInstance = e.component;
        
    //}
    //getRows() {
    //    console.log(this.dataGridInstance.getVisibleRows())
    //}
    onCellPrepared(e) {
        if (e.rowType == "header" ) {
            e.cellElement.style.fontWeight = 'bold' ;
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor  = "#17a2b8";
        }
    }

    isVisible(e) {
        if (e.row.key.finalisedDb ) {
            return false;
        }
        else {
            if (this.isGranted('Casierie.OP.Modificare')) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    searchCurrency() {
        this._personService.currencyList().subscribe(result => {
            this.currencies = result;
        });
    }

    paymentOrdersList() {
        sessionStorage.setItem('formPaymentOrders', JSON.stringify(this.form));
        this._paymentOrdersService.paymentOrdersList(this.form).subscribe(result => {
            this.form = result;
        });
    }

    getPaymentOrderListCount() {
        if (this.form.opList == null) {
            return 0;
        } else {
            return this.form.opList.length;
        }
    }

    paymentOrderDetailAdd(orderId: any) {
        if (orderId !== "") {
            this.isLoading = true;
            this._paymentOrdersService.paymentOrderDetail(orderId, this.form).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            ).subscribe(result => {
                this.form = result;
                this.bankForPayerFnc();
                this.thirdPartyListFnc();
            });
        }
    }

    bankForPayerFnc() {
        this._personService.banksForThirdParty(this.form.payerId).subscribe(result => {
            this.bankForPayer = result;
        });
    }

    thirdPartyListFnc() {
        this._personService.thirdPartyList().subscribe(result => {
            this.thirdPartyList = result;
        });
    }

    selectAll() {
        for (var i = 0; i < this.form.opList.length; i++) {
            this.form.opList[i].finalised = true;
        }
    }

    selectNone() {
        for (var i = 0; i < this.form.opList.length; i++) {
            this.form.opList[i].finalised = false;
        }
    }

    saveValidation() {
        this._paymentOrdersService.saveValidation(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info(this.l('ValidationMessage'));
        });
    }

    paymentOrderDetail(orderId: any) {
        if (orderId !== "") {
            this._paymentOrdersService.paymentOrderDetail(orderId, this.form).subscribe(result => {
                this.form = result;
                this.bankForPayerFnc();
                this.bankAccountsForPayerFnc();
                this.thirdPartyListFnc();
                this.bankForBenefFnc();
                this.bankAccountsForBenefFnc();
            });
        }
    }

    paymentOrderDetail2(e) {
        
        if (e.row.key.id !== "") {
            this._paymentOrdersService.paymentOrderDetail(e.row.key.id, this.form).subscribe(result => {
                this.form = result;
                this.form.opDetail.beneficiaryName = this.getThirdPartyName(this.form.opDetail.beneficiaryId);
                this.bankForPayerFnc();
                this.bankAccountsForPayerFnc();
                this.thirdPartyListFnc();
                this.bankForBenefFnc();
                this.bankAccountsForBenefFnc();
            });
       }
    }

    /**
 * Populeaza lista tertilor 
 * @param thidPartyName
 */
    searchThirdParty(thidPartyName: string) {
        this._personService.thirdPartySearch(thidPartyName).subscribe(result => {
            this.thirdPartyList = result;
        });
    }

    /**
     * Populare lista tertilor dupa numele cautat
     * @param search
     */
    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdPartyList = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.form.opDetail.beneficiaryId = thirdPartyId;
        this.form.opDetail.beneficiaryName = thirdPartyName;
        this.bankForBenefFnc();
    }

    // Returneaza numele tertului daca thirdPartyId != 0
    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        return this.form.opDetail.beneficiaryName;
    }

    bankAccountsForPayerFnc() {
        this._personService.bankAccountsForThirdParty(this.form.payerId, this.form.opDetail.payerBankId, this.form.opDetail.currencyId).subscribe(result => {
            this.bankAccountsForPayer = result;
        });
    }

    bankForBenefFnc() {
        this._personService.banksForThirdParty(this.form.opDetail.beneficiaryId).subscribe(result => {
            this.bankForBenef = result;
            this.unpayedInvoicesForThirdParty();
            this.bankAccountsForBenefFnc();
           // this.form.opDetail.benefBankId = null;
        });
    }

    unpayedInvoicesForThirdParty() {
        this.form.opDetail.invoicesList = [];
        this._invoiceService.getInvoicesForPaymentOrderByThirdPartyId(this.form.opDetail.beneficiaryId, this.form.opDetail.id).subscribe(result => {
            this.invoicesForBenef = result;
            if (this.form.opDetail.currencyId !== null) {
                this.form.opDetail.invoicesList.push(...(this.invoicesForBenef.filter(f => f.currencyId === this.form.opDetail.currencyId)));
            } else {
                this.form.opDetail.invoicesList.push(...this.invoicesForBenef);
            }
        });
    }

    getInvoicesCount() {
        if (this.form !== undefined) {
            if (this.form?.opDetail.invoicesList.length == 0) {
                return 0;
            } else {
                return this.form.opDetail.invoicesList.length;
            }
        }

    }

    bankAccountsForBenefFnc() {
        this._personService.bankAccountsForThirdParty(this.form.opDetail.beneficiaryId, this.form.opDetail.benefBankId, this.form.opDetail.currencyId).subscribe(result => {
            this.bankAccountsForBenef = result;

        });
    }

    paymentOrderDelete(orderId: number) {
        abp.message.confirm(
            this.l('Ordinul de plata va fi sters. Sigur?'),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._paymentOrdersService
                        .paymentOrderDelete(orderId, this.form)
                        .pipe(
                            finalize(() => {
                                abp.notify.info(this.l('Ordinul de plata a fost sters'));
                            })
                        )
                        .subscribe(result => this.form = result);
                }
            }
        )
    }

    paymentOrderDelete2(e) {
        abp.message.confirm(
            this.l('Ordinul de plata va fi sters. Sigur?'),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._paymentOrdersService
                        .paymentOrderDelete(e.row.key.id, this.form)
                        .pipe(
                            finalize(() => {
                                abp.notify.info(this.l('Ordinul de plata a fost sters'));
                            })
                        )
                        .subscribe(result => this.form = result);
                }
            }
        )
    }

    getInvoice(invoiceId) {
        if (invoiceId !== null) {
            this._invoiceService.getInvoice(invoiceId).subscribe(result => {
                this.invoice = result;
                this.form.opDetail.value = this.invoice.restPlata;
                this.form.opDetail.currencyId = this.invoice.currencyId;
                // var dateFormat = new Date(Date.parse(this.invoice.invoiceDate)).toLocaleDateString('ro');
                this.form.opDetail.paymentDetails = "FF " + (this.invoice.invoiceSeries !== null ? this.invoice.invoiceSeries : '') + " " + this.invoice.invoiceNumber + " / " + moment(this.invoice.invoiceDate).format('DD.MM.YYYY');
            });
        }
    }

    paymentOrderSave() {
        this._paymentOrdersService.paymentOrderSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info(this.l('AddModifyMessage'));
        });
    }

    backToList() {
        this.form.showList = true;
        this.form.showEditForm = false;
    }

    calculateInvoiceTotal(index: number) {
     
        var rest = this.form.opDetail.value - this.form.opDetail.invoicesList.filter(f => f.selected == true).reduce((sum, current) => sum + Number(current.payedValue), 0);
        
        if (this.form.opDetail.invoicesList[index].selected == true) {
            this._invoiceService.calculatePayedInvoice(rest, this.form.opDetail.invoicesList[index]).subscribe(result => {
                this.form.opDetail.invoicesList[index] = result;
            });
        } else {
            if (this.form.opDetail.invoicesList[index].remainingValue !== this.form.opDetail.invoicesList[index].rest) {
                this.form.opDetail.invoicesList[index].rest += Number(this.form.opDetail.invoicesList[index].payedValue);

            }
            this.form.opDetail.invoicesList[index].payedValue = 0;
        }
    }

    filterInvoicesListByCurrency(currencyId: number) {
        this.form.opDetail.invoicesList = this.invoicesForBenef.filter(f => f.currencyId == currencyId);
    }

}

