import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { AmanarePaapEditDto, BVC_PAAPTranseDto, PaapEditDto, PaapServiceProxy, RealocarePaapDto } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'realocare-paap-dialog.component.html'
})
export class ShowRelocarePAAPDialogComponent extends AppComponentBase implements OnInit {
    paapId: number;
    dataEnd: Date;
    realocarePaap: RealocarePaapDto = new RealocarePaapDto();
    paap: PaapEditDto = new PaapEditDto();
    transe: string[] = [];

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _paapService: PaapServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        this.realocarePaap.paapId = this.paapId;

        this._paapService.getPaap(this.paapId).subscribe(result => {
            this.paap = result;
            this.realocarePaap.valoareTotala = result.valoareTotalaLei;
            this.realocarePaap.dataEnd = result.dataEnd;
            this.paap.transe.forEach(x => this.transe.push(x.dataTransa.format("DD/MM/YYYY")));
        });
    }

    onChangeDateEnd() {
        let diff = this.realocarePaap.dataEnd.diff(this.paap.dataEnd, 'days')
        for (let i = 0; i < this.paap.transe.length; i++) {
            this.paap.transe[i].dataTransa = moment(this.paap.transe[i].dataTransa).add(diff, 'd');
        }
    }

    save() {
        this.realocarePaap.transe = this.paap.transe;
        this._paapService.realocarePaap(this.realocarePaap).subscribe(() => {
            this.notify.info(this.l('Achizitia a fost realocata'));
            this.bsModalRef.hide();
            this.onSave.emit();
        });
    }
}