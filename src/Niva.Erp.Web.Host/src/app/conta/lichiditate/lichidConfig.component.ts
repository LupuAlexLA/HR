import { Component, Injector, OnInit } from "@angular/core";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { LichidBenziDto, LichidConfigDto, LichidConfigServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './lichidConfig.component.html',
    animations: [appModuleAnimation()]
})
export class LichidConfigComponent extends AppComponentBase implements OnInit {

    lichidConfigList: LichidConfigDto[] = [];
    lichidBenziList: LichidBenziDto[] =[];
 
    constructor(inject: Injector,
        private _lichidConfigService: LichidConfigServiceProxy) {
        super(inject);
    }

    ngOnInit(){
        this.getLichidConfigList();
        this.getLichidBenziList();
    }

    getLichidConfigList() {
        this._lichidConfigService.lichidConfigList().subscribe(result => {
            this.lichidConfigList = result;
        });
    }

    getLichidBenziList() {
        this._lichidConfigService.lichidBenziList().subscribe(result => {
            this.lichidBenziList = result;
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

            if (e.dataField === "lichidBenziId") {
                e.editorOptions.disabled = e.row.data && e.row.data.eDinConta === false;
                e.editorOptions.onValueChanged = args => {
                    e.row.data.lichidBenziId = args.value;
                }
            }
        }
    }

    save() {
        this._lichidConfigService.saveLichidConfig(this.lichidConfigList).subscribe(result => {
            abp.notify.success("Modificarile au fost salvate");
            this.getLichidConfigList();
        });
    }
}