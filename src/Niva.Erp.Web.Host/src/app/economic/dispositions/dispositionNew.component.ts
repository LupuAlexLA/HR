import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from "../../../shared/AppConsts";
import {
    CurrencyDto, CurrencyServiceProxy, DispositionEditDto, DispositionServiceProxy, DocumentTypeServiceProxy, GetDocumentTypeOutput, GetThirdPartyOutput, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceListSelectableDto,
    InvoiceServiceProxy, PersonEditDto, PersonServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './dispositionNew.component.html',
    animations: [appModuleAnimation()]
})
export class DispositionNewComponent extends AppComponentBase implements OnInit {

    disposition: DispositionEditDto = new DispositionEditDto();
    thirdParties: GetThirdPartyOutput;
    invoices: InvoiceListSelectableDto[] = [];
    filtredInvoices: InvoiceListSelectableDto[] = [];
    documentTypeList: GetDocumentTypeOutput = new GetDocumentTypeOutput();
    currencies: CurrencyDto[] = [];
    categoryElements: InvoiceElementsDetailsCategoryListDTO[] = [];
    elementsDetails: InvoiceElementsDetailsDTO[] = [];
    dispositionId: number;
    thirdPartyId: number;
    person: PersonEditDto = new PersonEditDto();
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _dispositionService: DispositionServiceProxy,
        private _personService: PersonServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _currencyService: CurrencyServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.dispositionId = +this.route.snapshot.queryParamMap.get('dispositionId');

        this.isLoading = true;
        this._dispositionService.getDisposition(this.dispositionId || 0).pipe(
            delay(1000),
            finalize(() => this.isLoading = false)
        ).subscribe(result => {
            this.disposition = result;
            this.searchThirdParty(this.disposition.thirdPartyName);
            this.getInvoicesByThirdPartyId(this.disposition.thirdPartyId);
            this.getDocumentTypeList();
            this.getCurrencyList();
            this.getInvoiceCategoryElements();
         
            if (this.disposition.categoryElementId !== null) {
                this.getInvoiceElementDetailsByCategoryId(this.disposition.categoryElementId);
            }
        });
    }

    /**
     * Populeaza lista tertilor 
     * @param thidPartyName
     */
    searchThirdParty(thidPartyName: string) {
        this._personService.thirdPartySearch(thidPartyName).subscribe(result => {
            this.thirdParties = result;
        });
    }

    /**
     * Populare lista tertilor dupa numele cautat
     * @param search
     */
    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.disposition.thirdPartyId = thirdPartyId;
        this.disposition.thirdPartyName = thirdPartyName;
        this.getInvoicesByThirdPartyId(thirdPartyId);
        this.getPersonById(thirdPartyId);
    }

    // Returneaza numele tertului daca thirdPartyId != 0
    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        return this.disposition.thirdPartyName;
    }

    getInvoicesByThirdPartyId(thirdPartyId: number) {
        if (thirdPartyId !== 0) {
            this._invoiceService.getInvoicesForDispositionByThirdPartyId(thirdPartyId, this.disposition.id).subscribe(result => {
                this.invoices = result;
                this.disposition.invoiceList = this.disposition.currencyId !== null ? this.invoices.filter(f => f.currencyId == this.disposition.currencyId) : this.invoices;
            });
        }

    }

    getInvoicesCount() {
        if (this.disposition !== undefined && this.disposition.invoiceList !== undefined) {
            if (this.disposition.invoiceList?.length == 0) {
                return 0;
            } else {
                return this.disposition.invoiceList.length;
            }
        }
    }

    getPersonById(thirdPartyId: number) {
        this._personService.getPersonById(thirdPartyId).subscribe(result => {
            if (result.isNaturalPerson) {
                this.disposition.numePrenume = result.lastName + " " + result.firstName;
                this.disposition.actIdentitate = result.id2;
            }
        });
    }

    selectedInvoice(invoiceId: number) {
        this._invoiceService.getInvoice(invoiceId).subscribe(result => {
            this.disposition.currencyId = result.currencyId;
            this.disposition.currencyName = result.currencyName;
            this.disposition.value = result.restPlata;
            this.disposition.documentDate = result.invoiceDate;
            this.disposition.documentNumber = result.invoiceNumber;
            this.disposition.documentTypeId = result.documentTypeId;
        });
    }

    getDocumentTypeList() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result;
        });
    }

    getCurrencyList() {
        this._currencyService.currencyDDList().subscribe(result => {
            this.currencies = result;
        });
    }

    getInvoiceCategoryElements() {
        this._invoiceService.getInvoiceElementsDetailsCategoriesforThirdPartyQuality(2).subscribe(result => {
            this.categoryElements = result;
            this.disposition.categoryElementId = result[0].id;
        });
    }

    getInvoiceElementDetailsByCategoryId(categoryId: number) {
        this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).subscribe(result => {
            this.elementsDetails = result;
        });
    }

    checkDispNumber(event) {
        this._dispositionService.getNextNumber(moment(event.target.value)).subscribe(result => {
            this.disposition.dispositionNumber = result;
        });
    }

    checkNrChitanta() {
        if (this.disposition.operationType == 1) {
            this._dispositionService.getNextNumberForChitanta(moment(this.disposition.dispositionDate)).subscribe(result => {
                this.disposition.nrChitanta = result;
            });
        }
    }

    backToList() {
        this.router.navigate(['/app/economic/dispositions/dispositionList']);
    }

    resetList() {
      /*  this.disposition.invoiceId = null;*/
        this.disposition.currencyId = null;
        this.disposition.currencyName = null;
        this.disposition.value = null;
        this.disposition.documentDate = null;
        this.disposition.documentNumber = null;
        this.disposition.documentTypeId = null;
        this.getInvoicesByThirdPartyId(this.disposition.thirdPartyId);
    }

    openNewTab() {
        //this.router.navigate([]).then(response => {
        //    window.open('/app/economic/invoices/invoiceNew', '_blank');
        //});
        window.open(AppConsts.appBaseUrl + '/app/economic/invoices/invoiceNew', "_blank");

    }

    addThirdPartyNewTab() {
        window.open(AppConsts.appBaseUrl + '/app/nomenclatoare/person/personNew', "_blank");
        //this.router.navigate([]).then(response => {
        //    window.open("/app/nomenclatoare/person/personNew", '_blank');
        //});
    }

    calculateInvoiceTotal(index: number) {
        var rest = this.disposition.value - this.disposition.invoiceList.filter(f => f.selected == true).reduce((sum, current) => sum + Number(current.payedValue), 0);

        if (this.disposition.invoiceList[index].selected == true) {
            this._invoiceService.calculatePayedInvoice(rest, this.disposition.invoiceList[index]).subscribe(result => {
                this.disposition.invoiceList[index] = result;
            });
        } else {
            if (this.disposition.invoiceList[index].rest !== this.disposition.invoiceList[index].remainingValue) {
                this.disposition.invoiceList[index].rest += Number(this.disposition.invoiceList[index].payedValue);
            }
            this.disposition.invoiceList[index].payedValue = 0;
        }
    }

    getInvoicesByCurrencyId(currencyId: number) {
        this.disposition.invoiceList = this.invoices.filter(f => f.currencyId == currencyId);
    }

    saveDisposition() {
        this._dispositionService.saveDisposition(this.disposition).subscribe(result => {
            abp.notify.info("Dispozitia a fost salvata");
            this.router.navigate(['/app/economic/dispositions/dispositionList'] );
        });
    }

    checkAutoNumberForDoc(documentTypeId: number) {
        this._documentTypeService.getDocTypeById(documentTypeId).subscribe(result => {
            if (result.autoNumber === true) {
                this._documentTypeService.nextDocumentNumber(moment(this.disposition.dispositionDate), result).subscribe(result => {
                    this.disposition.documentNumber = result;
                });
            } else {
                this.disposition.documentNumber = "";
            }
        });
    }

}