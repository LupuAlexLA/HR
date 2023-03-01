import { OnInit } from '@angular/core';
import { Injector } from '@angular/core';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '../../../shared/app-component-base';
import { InvoiceDetailsForPAAPDto, PaapDto, PaapReferatDto, PaapReferatServiceProxy, PaapServiceProxy, PaapTranseListDto } from '../../../shared/service-proxies/service-proxies';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AddReferatDialogComponent } from '../referat/add-referat-dialog.component';

@Component({
    animations: [appModuleAnimation()],
    templateUrl: './paapAlocareCheltuieli.component.html',
    
})
/** paapAlocareCheltuieli component*/
export class PaapAlocareCheltuieliComponent extends AppComponentBase implements OnInit {

    paapInvoices: InvoiceDetailsForPAAPDto = new InvoiceDetailsForPAAPDto();
    paap: PaapDto = new PaapDto();
    referatList: PaapReferatDto[] = [];
    paapId: number;
    paapUsedSum: number = 0;
    paapSelectedSum: number = 0;
    paapRest: number = 0;
    showTranse: boolean = false;
    transeList: PaapTranseListDto[] = [];


    constructor(inject: Injector,
        private _paapService: PaapServiceProxy,
        private _paapReferatService: PaapReferatServiceProxy,
        private _modalService: BsModalService,
        private route: ActivatedRoute,
        private router: Router,
            ) {
        super(inject);
    }
    ngOnInit(): void {
        this.paapId = + this.route.snapshot.queryParamMap.get('paapId');

        this.getPaapById(this.paapId);
        this.getReferatList();

    }

    getSumaUtilizata() {
        this.paapUsedSum = this.paapInvoices.invoiceDetailsAllocatedForPAAP.reduce((acc, paap) => acc + paap.detailValueLocalCurr, 0);
        return this.paapUsedSum;
    }

    getSumaSelectata() {
        this.paapSelectedSum = this.paapInvoices.invoiceDetailsAvailableForPAAP.filter(x => x.selected == true).reduce((acc, paap) => acc + paap.detailValueLocalCurr, 0);
        if (this.paapSelectedSum == 0) {
            this.getRestPAAP();
        } else {
            this.paapRest = this.paap.value - this.paapSelectedSum;
        }
        return this.paapSelectedSum;
    }

    getRestPAAP() {
        this.paapRest = this.paap.value - this.paapUsedSum;
        return this.paapRest;
    }

    getPaapById(paapId: number) {
        this._paapService.getSinglePaap(paapId).subscribe(result => {
            this.paap = result;
            this.getTranseList(paapId);
            this.getInvoiceList(paapId);
        });
    }

    getInvoiceList(paapId: number) {
        this._paapService.getInvoiceDetails(paapId).subscribe(result => {
            this.paapInvoices = result;
            this.getSumaUtilizata();
            this.getSumaSelectata();
            this.getRestPAAP();
        });
    }

    deleteAllocatedInvoice(allocatedInvoiceId: number) {
        abp.message.confirm("Factura alocata va fi stearsa. Sigur?", null,
            (result: boolean) => {
                if (result) {
                    this._paapService.deleteAllocatedInvoice(this.paapId, allocatedInvoiceId).subscribe(result => {
                        abp.notify.info("Factura alocata a fost stearsa");
                        this.getPaapById(this.paapId);
                    });
                }
            });
    }


    approveInvoices() {
        this._paapService.approveInvoices(this.paapId, this.paapInvoices).subscribe(result => {
            this.getPaapById(this.paapId);
            abp.notify.info("Facturile au fost alocate");

        });
    }

    finalizeaza() {
        this._paapService.finalizePAAP(this.paapId).subscribe(result => {
            this.getPaapById(this.paapId);
            abp.notify.info("Alocarea cheltuielilor a fost finalizata");
        });
    }

    anuleazaFinalizarea() {
        abp.message.confirm("Alocarea cheltuielilor va fi anulata. Sigur?", null,
            (result: boolean) => {
                if (result) {
                    this._paapService.cancelFinalizePAAP(this.paapId).subscribe(result => {
                        abp.notify.info("Alocarea cheltuielilor a fost anulata");
                        this.getPaapById(this.paapId);
                    });
                }
            });
    }


    getReferatList() {
        this._paapReferatService.getPaapReferatList(this.paapId).subscribe(result => {
            this.referatList = result;
        });
    }

    addReferat() {
        this.showAddReferatDialog(this.paapId, 0);
    }

    showAddReferatDialog(paapId: number, referatId: number) {
        let showAddReferatDialog: BsModalRef;

        showAddReferatDialog = this._modalService.show(
            AddReferatDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    paapId: paapId,
                    referatId: referatId
                },
            }
        );

        showAddReferatDialog.content.onSave.subscribe(() => {
            this.getReferatList();
            this.getPaapById(this.paapId);
        });
    }

    editareReferat(referatId: number) {
        this.showAddReferatDialog(this.paapId, referatId);
    }

    deleteReferat(referatId: number) {
        abp.message.confirm("Referatul va fi sters. Sigur?", null,
            (result: boolean) => {
                if (result) {
                    this._paapReferatService.deletePappReferat(referatId).subscribe(result => {
                        abp.notify.info("Referatul a fost sters");
                        this.getPaapById(this.paapId);
                        this.getReferatList();
                    });
                }
            });
    }

    viewTranse() {
        this.showTranse = true;
    }

    hideTranse() {
        this.showTranse = false;
    }

    getTranseList(paapId: number) {
        this._paapService.getTranseByPaapId(paapId).subscribe(result => {
            this.transeList = result;
        });
    }
}