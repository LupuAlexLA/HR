import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import * as moment from "moment";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { AmanarePaapEditDto, BVC_PAAPTranseDto, PaapEditDto, PaapServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: 'amanare-paap-dialog.component.html'
})
export class ShowAmanarePAAPDialogComponent extends AppComponentBase implements OnInit {

    paapId: number;
    dataEnd: Date;
    amanarePaap: AmanarePaapEditDto = new AmanarePaapEditDto();
    paap: PaapEditDto = new PaapEditDto();
    transe: string[] = [];

    @Output() onSave = new EventEmitter<any>();

    constructor(injector: Injector,
        private _paapService: PaapServiceProxy,
        public bsModalRef: BsModalRef) {
        super(injector);
    }

    ngOnInit() {
        
        this.amanarePaap.paapId = this.paapId;

        this._paapService.getPaap(this.paapId).subscribe(result => {
            this.paap = result;
            this.paap.transe.forEach(x => this.transe.push(x.dataTransa.format("DD/MM/YYYY")));
        });

    }

    onChangeDateAmanare() {
        let diff = this.amanarePaap.dataEnd.diff(this.paap.dataEnd,'days')
        
        for (let i = 0; i < this.paap.transe.length; i++) {

            this.paap.transe[i].dataTransa = moment(this.paap.transe[i].dataTransa).add(diff, 'd');
            
        }
        
    }

    save() {
        

        this._paapService.paapSave(this.paap).subscribe(() => {
            this._paapService.amanarePAAP(this.amanarePaap).subscribe(() => {
                this.notify.info(this.l('Achizitia a fost amanata'));
                this.bsModalRef.hide();
                this.onSave.emit();
            });
        })
        
    }
}