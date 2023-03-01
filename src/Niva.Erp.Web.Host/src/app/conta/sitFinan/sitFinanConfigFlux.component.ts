import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SitFinanConfigServiceProxy, SitFinanRapConfigNoteDto, SitFinanRapFluxConfigDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './sitFinanConfigFlux.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanConfigFluxComponent extends AppComponentBase implements OnInit {

    sitFinanId: number;
    reportId: number;
    flux: SitFinanRapFluxConfigDto[] = [];

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _sitFinanConfigService: SitFinanConfigServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.sitFinanId = +this.route.snapshot.queryParamMap.get('sitFinanId');
        this.reportId = +this.route.snapshot.queryParamMap.get('reportId');

        this._sitFinanConfigService.sitFinanRapFluxConfigList(this.reportId).subscribe(result => {
            this.flux = result;
        });

    }

    addFlux0() {
        let newFlux = new SitFinanRapFluxConfigDto;
        newFlux.id = 0;
        newFlux.sitFinanFluxRowType = 0;
        newFlux.sitFinanRapId = this.reportId;
        console.log(newFlux);
        this.flux.push(newFlux);
        console.log(this.flux);
    }

    addFlux1() {
        let newFlux = new SitFinanRapFluxConfigDto;
        newFlux.id = 0;
        newFlux.sitFinanFluxRowType = 1;
        newFlux.sitFinanRapId = this.reportId;
        console.log(newFlux);
        this.flux.push(newFlux);
        console.log(this.flux);
    }

    deleteFlux(key) {
        const index = this.flux.indexOf(key, 0);
        if (index > -1) {
            this.flux.splice(index, 1);
        }
    }

    sitFinanFluxSave() {
        this._sitFinanConfigService.sitFinanFluxSave(this.reportId ,this.flux).subscribe(result => {
            this.flux = result;
            abp.notify.info('AddUpdateMessage');
        });
    }
}