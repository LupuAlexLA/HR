import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { AppConsts } from '../../shared/AppConsts';
import { BNR_Detalii, BNR_RaportareServiceProxy, GetThirdPartyOutput, InvoiceDTO, InvoiceServiceProxy, PersonServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './detaliiBNR.component.html',
    animations: [appModuleAnimation()]
})
export class DetaliiBNRComponent extends AppComponentBase implements OnInit {

    bnrDetaliiList: BNR_Detalii[] = [];
    savedBalanceId : number ;

    constructor(injector: Injector,
        private _bnrService: BNR_RaportareServiceProxy,
        
        private route: ActivatedRoute,
        private router: Router    ) {
        super(injector);
        
    }

    ngOnInit() {
        this.savedBalanceId = + this.route.snapshot.queryParamMap.get('savedBalanceId');
        //this._bnrService.raportareDetailsList(this.savedBalanceId).subscribe(result => {
        //    this.bnrDetaliiList = result;
        //})
        
    }
    onCellPrepared(e) {
        console.log(e);

        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    

    

    

    
    

    

    
}