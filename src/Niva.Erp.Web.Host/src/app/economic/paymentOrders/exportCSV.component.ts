import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { foreColor } from "devexpress-reporting/scopes/reporting-designer-controls-metadata";
import { forEach } from "lodash";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { GetBankOutput, PaymentOrderExportList, PaymentOrdersServiceProxy, PersonServiceProxy } from "../../../shared/service-proxies/service-proxies";
import { saveAs } from 'file-saver';
@Component({
    templateUrl: './exportCSV.component.html',
    animations: [appModuleAnimation()]
})
export class ExportCSVComponent extends AppComponentBase implements OnInit {

    bankForBenefId: any;
    bankForBenef: GetBankOutput = new GetBankOutput();
    opList: PaymentOrderExportList[] = [];
    
    constructor(inject: Injector,
        private _paymentOrdersService: PaymentOrdersServiceProxy,
        private _personService: PersonServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.bankForBenefFnc();
    }

    bankForBenefFnc() {
        this._personService.bankList().subscribe(result => {
            this.bankForBenef = result;
        });
    }

    searchOP() {
        this._paymentOrdersService.searchPaymentOrderByBenefBankId(this.bankForBenefId).subscribe(result => {
            this.opList = result;
        });
    }

    getOPCount() {
        if (this.opList.length == 0) {
            return 0;
        } else {
            return this.opList.length;
        }
    }

    exportToCSV() {
        let selectedOpIds: Array<number> = new Array<number>();
        this.opList.forEach(function (item) {
            if (item.selected)
                selectedOpIds.push(item.id);
        });
        this._paymentOrdersService.exportOPToCSVBin(selectedOpIds).subscribe(result => {
            const byteCharacters = atob(result);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: 'application/binary' });
            //var blob = new Blob(result.u, { type: "text/plain;charset=utf-8" });
            saveAs(blob, "Export.csv");
            abp.notify.success("Fisierul a fost exportat");
            this.finalizeExportedOP(selectedOpIds);
        });

    }

    finalizeExportedOP(selectedOPIds: number[]) {
        this._paymentOrdersService.finalizeExportedOp(selectedOPIds).subscribe(() => {
            abp.notify.success("OP-urile au fost finalizate");
            this.router.navigate(['/app/economic/paymentOrders/paymentOrders']);
        });
    }


}