import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { SavedBalanceServiceProxy, LichidSavedBalanceListDto, LichidCalcServiceProxy, LichidCalcInitDto, LichidCalcListDetDto, LichidCalcCurrServiceProxy, LichidCalcCurrListDto, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";
import { LichidCalcCurrDetDialogComponent } from "./lichidCalcCurrDet-dialog/lichidCalcCurrDet-dialog.component";
import { LichidCalcCurrExportComponent } from "./lichidCalcCurrExport-dialog/lichidCalcCurrExport-dialog.component";
import { LichidCalcDetDialogComponent } from "./lichidCalcDet-dialog/lichidCalcDet-dialog.component";
import { LichidCalcExportComponent } from "./lichidCalcExport-dialog/lichidCalcExport-dialog.component";

@Component({
    templateUrl: './lichidCalcul.component.html',
    animations: [appModuleAnimation()]
})
export class LichidCalculComponent extends AppComponentBase implements OnInit {
    computeSavedBalanceId: number = 0;
    savedBalanceList: LichidSavedBalanceListDto[] = [];
    lichidCalcInit: LichidCalcInitDto = new LichidCalcInitDto();
    lichidCalcDetList: LichidCalcListDetDto[] = [];
    lichidCalcCurrList: LichidCalcCurrListDto[] = [];
    lichidTypeList: EnumTypeDto[] = [];
    isLoading: boolean = false;
    savedBalanceId: any;
    savedBalanceDescription: any;
    url: string = '';
    reportParams: any = {};

    constructor(inject: Injector,
        private _savedBalanceService: SavedBalanceServiceProxy,
        private _lichidCalcService: LichidCalcServiceProxy,
        private _lichidCalcCurrService: LichidCalcCurrServiceProxy,
        private _enumService: EnumServiceProxy,
        private _modalService: BsModalService) {
        super(inject);
    }

    ngOnInit() {
        this.getSavedBalanceList();
        this.getLichidCalcList();
    }

    getSavedBalanceList() {
        this._savedBalanceService.lichidSavedBalanceList().subscribe(result => {
            this.savedBalanceList = result;
        });
    }

    getLichidCalcList() {
        this.isLoading = true;
        this._lichidCalcService.getLichidCalcList()
            .pipe(
                delay(1000),
                finalize(() => { this.isLoading = false; })
            )
            .subscribe(result => {
                this.lichidCalcInit = result;
            });
    }

    compute() {
        this.isLoading = true;
        this._lichidCalcService.lichidCalc(this.computeSavedBalanceId).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            }))
            .subscribe(result => {
                this.getSavedBalanceList();
                this.getLichidCalcList();
                this.lichidCalcInit.showLichidCalcSavedBalanceList = true;
                abp.notify.success("Lichiditatile au fost calculate");
            });
    }

    getLichidCalcCount() {
        return this.lichidCalcInit.lichidCalcSavedBalanceList?.length ?? 0;
    }

    showLichidType(savedBalanceId: number, index: number) {
        this.lichidCalcInit.showLichidType = true;
        this.savedBalanceId = savedBalanceId;
        this.savedBalanceDescription = this.lichidCalcInit.lichidCalcSavedBalanceList[index].savedBalanceDesc + " - " + this.lichidCalcInit.lichidCalcSavedBalanceList[index].savedBalanceDate.format('DD/MM/YYYY');
        this.getLichidTypeList();
    }

    getLichidTypeList() {
        this._enumService.lichidCalcTypeList().subscribe(result => {
            this.lichidTypeList = result;
        });
    }

    showDetails(index: number) {
        if (index == 0) { // lichiditate pe benzi
            this._lichidCalcService.getLichidCalcDetList(this.savedBalanceId).subscribe(result => {
                this.lichidCalcDetList = result;
                this.lichidCalcInit.showLichidCalcList = true;
                this.lichidCalcInit.showLichidCalcSavedBalanceList = false;
                this.lichidCalcInit.showLichidType = false;        
            });
        }
        if (index == 1) { // lichiditate pe monede
            this._lichidCalcCurrService.getLichidCalcCurrList(this.savedBalanceId).subscribe(result => {
                this.lichidCalcCurrList = result;
                this.lichidCalcInit.showLichidCalcCurrList = true;
                this.lichidCalcInit.showLichidCalcSavedBalanceList = false;
                this.lichidCalcInit.showLichidType = false;
            });
        }

    }

    showLichidCalcList() {
        this.lichidCalcInit.showLichidCalcList = false;
        this.lichidCalcInit.showLichidCalcCurrList = false;
        this.lichidCalcInit.showLichidCalcSavedBalanceList = true;
    }

    delete(savedBalanceId: number, index: number) {
        abp.message.confirm('Calculul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this.isLoading = true;
                    this._lichidCalcService.deleteLichidCalc(savedBalanceId)
                        .pipe(
                            delay(1000),
                            finalize(() => { this.isLoading = false }))
                        .subscribe(() => {
                            this.getSavedBalanceList();
                            this.getLichidCalcList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }

    showColumnDetails(columnId: number, lichidConfigId: number) {
        let showLichidCalcDetails: BsModalRef;

        showLichidCalcDetails = this._modalService.show(
            LichidCalcDetDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    columnId: columnId,
                    savedBalanceId: this.savedBalanceId,
                    lichidConfigId: lichidConfigId,
                },
            }
        );
    }

    showColumnCurrDetails(columnId: number, lichidConfigId: number) {
        let showLichidCalcCurrDetails: BsModalRef;

        showLichidCalcCurrDetails = this._modalService.show(
            LichidCalcCurrDetDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    columnId: columnId,
                    savedBalanceId: this.savedBalanceId,
                    lichidConfigId: lichidConfigId,
                },
            }
        );
    }

    computeTotalActualiz(index: number) {
        this.lichidCalcDetList[index].totalActualiz = Number(this.lichidCalcDetList[index].valoareBanda1) + Number(this.lichidCalcDetList[index].valoareBanda2) +
            Number(this.lichidCalcDetList[index].valoareBanda3) +
            Number(this.lichidCalcDetList[index].valoareBanda4) + Number(this.lichidCalcDetList[index].valoareBanda5);
    }

    computeTotalActualizCurr(index: number) {
        this.lichidCalcCurrList[index].totalActualiz = Number(this.lichidCalcCurrList[index].valoareRON) + Number(this.lichidCalcCurrList[index].valoareUSD) +
            Number(this.lichidCalcCurrList[index].valoareEUR) +
            Number(this.lichidCalcCurrList[index].valoareGBP) + Number(this.lichidCalcCurrList[index].valoareAlteMonede);
    }

    save() {
        this.isLoading = true;
        this._lichidCalcService.updateLichidCalc(this.savedBalanceId, this.lichidCalcDetList)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                this.lichidCalcDetList = result;
                abp.notify.success(this.l('Valorile au fost modificate'));
            });
    }

    saveLichidCalcCurr() {
        this.isLoading = true;
        this._lichidCalcCurrService.updateLichidCalcCurr(this.savedBalanceId, this.lichidCalcCurrList)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                this.lichidCalcCurrList = result;
                abp.notify.success(this.l('Valorile au fost modificate'));
            });
    }

    showReport(index: number) {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        switch (index) {
            case 0:
                this.reportParams.savedBalanceId = this.savedBalanceId;
                this.reportParams.lichidType = index;
                this.url += 'LichidCalcReport';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case 1:
                this.reportParams.savedBalanceId = this.savedBalanceId;
                this.reportParams.lichidType = index;
                this.url += 'LichidCalcCurrReport';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
        }
        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }

    calcLichiditate(savedBalanceId: number) {
        abp.message.confirm('Lichiditatile vor fi recalculate. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this.isLoading = true;
                    this._lichidCalcService.recalculLichiditate(savedBalanceId).pipe(
                        delay(1000),
                        finalize(() => {
                            this.isLoading = false;
                        }))
                        .subscribe(result => {
                            abp.notify.info('Calculul s-a realizat cu succes!');
                           
                        });

                }
            }
        );
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    openDetails() {
        let showLichidCalcDetExport: BsModalRef;

        showLichidCalcDetExport = this._modalService.show(
            LichidCalcExportComponent,
            {
                class: 'modal-lg',
                initialState: {
                    savedBalanceId: this.savedBalanceId
                },
            }
        );
    }

    openDetailsLichidCurr() {
        let showLichidCalcDetCurrExport: BsModalRef;

        showLichidCalcDetCurrExport = this._modalService.show(
            LichidCalcCurrExportComponent,
            {
                class: 'modal-lg',
                initialState: {
                    savedBalanceId: this.savedBalanceId
                },
            }
        );
    }
}