import { ChangeDetectorRef, ChangeDetectionStrategy, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { EnumServiceProxy, EnumTypeDto, ImprumutServiceProxy, GarantieServiceProxy, OperatieGarantieDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './operatieGarantie.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.Default
})
/** ImprumuturiTermen component*/
export class OperatieGarantieComponent extends AppComponentBase implements OnInit  {
    operatieGarantieList: OperatieGarantieDto[] = [];
    newOperatie: OperatieGarantieDto = new OperatieGarantieDto();
    operatieGarantieTip: EnumTypeDto[] = [];
    garantieId: number;
    imprumutId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private changeDetectorRef: ChangeDetectorRef,
        private _garantieService: GarantieServiceProxy,
        private _imprumutService: ImprumutServiceProxy,
        private _enumSerivce: EnumServiceProxy,   ) {
        super(injector);
    }

    ngOnInit() {

        this.garantieId = + this.route.snapshot.queryParamMap.get('garantieId');
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.getOperatieGarantieList();
        this.getOperatieGarantieTip();

    }

    getOperatieGarantieTip() {
        this._enumSerivce.operatieGarantieTipEnum().subscribe(result => this.operatieGarantieTip = result);
    }

  
    getOperatieGarantieList() {
        this._garantieService.getOperatieGarantieList(this.garantieId).subscribe(result => {
            this.operatieGarantieList = result;
            
        });
    }

     saveOperatie() {

        this.newOperatie.garantieId = this.garantieId;
         this._garantieService.saveOperatieGarantie(this.newOperatie).subscribe(result => {
            this.operatieGarantieList = result;
            
        });

    }

     operationDelete(id) {
        this._garantieService.deleteOperatieGarantie(id).subscribe(result => {
            this.operatieGarantieList = result;
           
        });
    }

}