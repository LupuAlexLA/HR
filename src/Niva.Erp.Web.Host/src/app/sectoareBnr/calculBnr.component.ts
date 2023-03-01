import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import {
    BNR_AnexaDto, BNR_AnexaServiceProxy, BNR_BalanceListDto, BNR_RaportareServiceProxy, BNR_SectorCalcServiceProxy, BNR_SectorCalculatListDto,
    BNR_SectorRowCalcListDto, BNR_SectorRowCalValDto, SavedBalanceServiceProxy
} from '../../shared/service-proxies/service-proxies';
import { CalculBnrDetailDialogComponent } from './calculBnrDetail-dialog.component';

@Component({

    templateUrl: './calculBnr.component.html',
    animations: [appModuleAnimation()],
    styles: [ 'table tr.highlight { background-color: #e9e9e9 !important; }table tr.unsectored { background-color: #dc3545 !important; color: white;}' ]
})
/** calculBnr.cs component*/
export class CalculBnrComponent extends AppComponentBase implements OnInit {
    /** calculBnr.cs ctor */
    sectorBalanceList: BNR_SectorCalculatListDto[] = [];
    sectorDetails: BNR_SectorRowCalcListDto[] = [];
    balanceList: BNR_BalanceListDto[] = [];
    sectorList: BNR_SectorRowCalValDto[] = [];
    balanceId: number = 0;
    balanceIdToCompute: number = 0;
    isLoading: boolean = false;
    showRows: boolean = false;
    showRowDetail: boolean = false;
    anexaList: BNR_AnexaDto[] = [];
    anexaId: number;
    selectedBalanceRow: any;
    selectedRow: any = null;
    sectoredRows: boolean = false;


    constructor(inject: Injector,
        private _savedBalanceService: SavedBalanceServiceProxy,
        private _bnrSectorCalcService: BNR_SectorCalcServiceProxy,
        private _bnrRaportareService: BNR_RaportareServiceProxy,
        private _anexaService: BNR_AnexaServiceProxy,
        private _modalService: BsModalService,
        private router: Router) {
        super(inject);
    }
    ngOnInit() {
        if (this.isGranted('Conta.BNR.Conturi.Acces')) {
            this.getSavedBalance();
            this.getCalculatedSector();
            this.getAnexaList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getAnexaList() {
        this._anexaService.anexaList().subscribe(result => {
            this.anexaList = result;
            this.anexaId = this.anexaList[0].id;
        });
    }

    getSavedBalance() {
        this._savedBalanceService.balanceListForBNR().subscribe(result => {
            this.balanceList = result;
        });
    }

    getCalculatedSector() {
        this._bnrSectorCalcService.getSectorBalanceList().subscribe(result => {
            this.sectorBalanceList = result;
        });
    }

    compute() {
        this.isLoading = true;
        this._bnrSectorCalcService.bNR_SectorConturiCalc(this.balanceIdToCompute).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })).subscribe(() => {
                this.getCalculatedSector();
                this.getSavedBalance();
                abp.notify.success("Sectoarele au fost calculate");
            });
    }

    showRowsList(balanceId: number) {

        if (this.selectedBalanceRow !== balanceId) {
            this.showRowDetail = false;
            this.showRows = false;
            this.selectedRow = null;

        }
        this.selectedBalanceRow = balanceId;

        this.balanceId = balanceId;

        this._bnrSectorCalcService.sectorBnrDetailList(this.balanceId).subscribe(result => {
            this.sectorDetails = result.filter(f => f.anexaId === this.anexaId);
            this.showRows = true;

            //this.sectoredRows = result.every(f => f.isSectored === true && f.balanceId === balanceId);
            //this.sectorBalanceList.find(f => f.balanceId === balanceId).allowCompute = this.sectoredRows;

            if (this.selectedRow != null) {
                this.showRowDetails(this.selectedRow, balanceId);
            }

            //this.sectorBalanceList.find(f => f.balanceId === this.balanceId).bnR_Rows = this.sectorDetails;
        });
    }

    onChangeShowDetails(anexaId: number) {
        this.anexaId = anexaId;
        this.showRowsList(this.balanceId);
        this.showRowDetail = false;
        this.selectedRow = null;
    }

    showRowDetails(anexaDetailId: number, savedBalanceId: number) {
        this.selectedRow = anexaDetailId;
        this.sectorList = this.sectorDetails.find(f => f.balanceId === savedBalanceId && f.anexaDetailId === anexaDetailId).bnR_RowDetails;
        this.showRowDetail = true;
    }

    delete(balanceId: number) {
        abp.message.confirm('Calculul va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bnrSectorCalcService.deleteCalculBNR(balanceId)
                        .subscribe(() => {
                            this.getCalculatedSector();
                            this.getSavedBalance();

                            this.showRows = false;
                            this.showRowDetail = false;
                            this.showRowDetails(0, balanceId);
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }


    showDetailDialog(anexaDetailId: number, sectorId: number) {
        let showDetailDialog: BsModalRef;

        showDetailDialog = this._modalService.show(
            CalculBnrDetailDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    anexaDetailId: anexaDetailId,
                    anexaId: this.anexaId,
                    sectorId: sectorId,
                    savedBalanceId: this.balanceId
                },
            }
        );

        showDetailDialog.content.onSave.subscribe(() => {
            this.showRowsList(this.balanceId);
            //this.showRowDetails(this.selectedRow, this.balanceId);
        });
    }

    computeRaportare(savedBalanceId: number) {
        this.isLoading = true;
        this._bnrRaportareService
            .computeRaportare(savedBalanceId)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                }))
            .subscribe(() => {
                this.notify.success("Raportarea a fost calculata");
                this.router.navigate(['/app/sectoareBnr/raportareBNR']);
        });
    } 
}