import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { EnumServiceProxy, EnumTypeDto, ImprumutServiceProxy, ImprumutStateDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imprumuturiState.component.html',
    animations: [appModuleAnimation()]
})
/** ImprumuturiTermen component*/
export class ImprumuturiStateComponent extends AppComponentBase implements OnInit  {
    imprumuturiStateList: ImprumutStateDto[] = [];
    newImprumutState: ImprumutStateDto = new ImprumutStateDto();
    imprumutStareList: EnumTypeDto[] = [];
    imprumutId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumutService: ImprumutServiceProxy,
        private _enumSerivce: EnumServiceProxy,   ) {
        super(injector);
    }

    ngOnInit() {
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.getImprumutStateList();
        this.getEnums();

    }
    getEnums() {
        this._enumSerivce.imprumuturiStare().subscribe(result => this.imprumutStareList = result);
    }

    getImprumutStateList() {
        this._imprumutService.imprumutStateList(this.imprumutId).subscribe(result => { this.imprumuturiStateList = result, console.log(result) });
        
    }

    saveNewImprumutState() {
        this.newImprumutState.imprumutId = this.imprumutId;
        this._imprumutService.saveImprumutStateList(this.newImprumutState).subscribe(result => { this.imprumuturiStateList = result; });
        
    }

}