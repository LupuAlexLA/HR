import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { BNR_AnexaDetailDto, BNR_AnexaDetailServiceProxy, BNR_AnexaDto, BNR_AnexaServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({

    templateUrl: './anexaBnr.component.html',
    animations: [appModuleAnimation()],

})
/** anexaBnr component*/
export class AnexaBnrComponent extends AppComponentBase implements OnInit {
    /** anexaBnr ctor */
    anexaDetails: BNR_AnexaDetailDto[] = [];
    anexaApiCall: BNR_AnexaDetailDto[] = [];
    anexaList: BNR_AnexaDto[] = [];
    anexaId: number;
    constructor(inject: Injector,
        private _anexaDetailService: BNR_AnexaDetailServiceProxy,
        private _anexaService: BNR_AnexaServiceProxy,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit(): void {

        if (this.isGranted('Conta.BNR.Configurare.Acces')) {
            this.getAnexaParentList();
            this.getAnexaList();
        }
        else {
            this.router.navigate(['/app/home']);
        }

    }

    onChangeAnexaParent(event) {
        this.anexaId = event;
        this.anexaDetails = this.anexaApiCall.filter(e => e.anexaId == this.anexaId);
    }

    getAnexaParentList() {
        this._anexaService.anexaList().subscribe(result => {
            this.anexaList = result;
            this.anexaId = this.anexaList.find(x => true).id;
        });
    }

    getAnexaList() {
        this._anexaDetailService.anexaBnrList().subscribe(result => {
            this.anexaApiCall = result;
            this.anexaDetails = result.filter(f => f.anexaId == this.anexaId);
        });
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    editRowFormula(e) {
        if (e.parentType === "dataRow") {
            if (e.dataField === "formulaConta") {
                e.editorOptions.disabled = e.row.data && e.row.data.eDinConta === false;
                if (e.editorOptions.disabled === false) {
                    e.editorOptions.onValueChanged = args => {
                        e.row.data.formulaConta = args.value;
                    }
                }
            }

            if (e.dataField === "formulaCresteri") {
                e.editorOptions.onValueChanged = args => {
                    e.row.data.formulaCresteri = args.value;
                }
            }
            if (e.dataField === "formulaReduceri") {
                e.editorOptions.onValueChanged = args => {
                    e.row.data.formulaReduceri = args.value;
                }
            }
            if (e.dataField === "formulaTotal") {
                e.editorOptions.onValueChanged = args => {
                    e.row.data.formulaTotal = args.value;
                }
            }
        }

    }

    save() {
        this._anexaDetailService.updateAnexaBnr(this.anexaDetails).subscribe(result => {
            abp.notify.success("Modificarile au fost salvate");
            this.getAnexaList();
        });
    }
}