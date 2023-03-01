import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "../../shared/animations/routerTransition";
import { AppComponentBase } from "../../shared/app-component-base";
import { AppConsts } from "../../shared/AppConsts";
import { BNR_AnexaDto, BNR_AnexaServiceProxy, BNR_RaportareDto, BNR_RaportareRowDetailsDto, BNR_RaportareRowDto, BNR_RaportareServiceProxy } from "../../shared/service-proxies/service-proxies";
import { RaportareBNRDetailDialogComponent } from "./raportareBNR-dialog/raportareBNRDetail-dialog.component";

@Component({
    templateUrl: './raportareBNR.component.html',
    animations: [appModuleAnimation()],
    styles: ['table tr.highlight { background-color: #e9e9e9 !important;}']
})
export class RaportareBNRComponent extends AppComponentBase implements OnInit {

    raportareBNR: BNR_RaportareDto[] = [];
    raportareRows: BNR_RaportareRowDto[] = [];
    raportareRowDetails: BNR_RaportareRowDetailsDto[] = [];
    anexaList: BNR_AnexaDto[] = [];
    anexaId: number;
    showRow: boolean = false;
    showRowDetail: boolean = false;
    savedBalanceId: number;

    selectedRaportareRow: any;
    selectedRow: any;
    reportParams: any = {};
    url: string;

    constructor(inject: Injector,
        private _bnrRaportareService: BNR_RaportareServiceProxy,
        private _anexaService: BNR_AnexaServiceProxy,
        private _modalService: BsModalService,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.BNR.Raportare.Acces')) {
            this.getRaportareList();
            this.getAnexaList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    showRaportareRand(savedBalanceId: number) {
        this.selectedRaportareRow = savedBalanceId;

        this.savedBalanceId = savedBalanceId;
        this.showRow = true;
        this._bnrRaportareService.raportareRows(savedBalanceId, this.anexaId).subscribe(result => {
            this.raportareRows = result.sort((x, y) => { return x.orderView - y.orderView });
        });
    }

    getAnexaList() {
        this._anexaService.anexaList().subscribe(result => {
            this.anexaList = result;
            this.anexaId = this.anexaList[0].id;
        });
    }

    onChangeShowDetails(anexaId: number) {
        this.anexaId = anexaId;
        this.showRaportareRand(this.savedBalanceId);
        this.showRowDetail = false;
    }

    getRaportareList() {
        this._bnrRaportareService.raportareList().subscribe(result => {
            this.raportareBNR = result;
        })
    }

    getCountRaportareBNR() {
        if (this.raportareBNR.length == 0) {
            return 0;
        } else {
            return this.raportareBNR.length;
        }
    }

    delete(savedBalanceId: number) {
        abp.message.confirm('Raportarea va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bnrRaportareService.deleteRaportare(savedBalanceId).subscribe(() => {
                        abp.notify.info('Raportarea a fost stearsa');
                        this.getRaportareList();
                        this.showRow = false;
                        this.showRowDetail = false;
                    });
                }
            });
    }

    computeRaportare(savedBalanceId: number) {
        abp.message.confirm('Raportarea va fi recalculata. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bnrRaportareService.computeRaportare(savedBalanceId).subscribe(() => {
                        abp.notify.info('Raportarea a fost recalculata');
                        this.getRaportareList();
                        this.showRow = false;
                        this.showRowDetail = false;
                    });
                }
            });
    }


    showRaportareRowDetails(raportareId: number) {
        this.selectedRow = raportareId;
        this.showRowDetail = true;
        this._bnrRaportareService.raportareRowDetails(raportareId).subscribe(result => {
            this.raportareRowDetails = result;
        });
    }

    showReport(savedBalanceId: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        this.url += "AnexeReport";
        this.reportParams.savedBalanceId = savedBalanceId;
        this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }

    // Deschide modal pentru afisarea detaliilor
    showRaportareBNRDetailDialog() {
        let showRaportareBNRDetailDialog: BsModalRef;

        showRaportareBNRDetailDialog = this._modalService.show(
            RaportareBNRDetailDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    savedBalanceId: this.selectedRaportareRow,
                    anexaId: this.anexaId
                },
            }
        );
    }

}