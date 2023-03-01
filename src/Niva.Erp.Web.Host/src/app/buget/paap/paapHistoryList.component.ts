import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { PaapServiceProxy, PaapStateListDto } from '../../../shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";

@Component({
    
    templateUrl: './paapHistoryList.component.html',
    animations: [appModuleAnimation()],
   
})
/** paapHistoryList component*/
export class PaapHistoryListComponent extends AppComponentBase implements OnInit {
    /** paapHistoryList ctor */
    paapStateList: PaapStateListDto[] = [];
    paapId: number;

    constructor(inject: Injector,
        private _paapService: PaapServiceProxy,
        private route: ActivatedRoute,    ) {
        super(inject);
    }

    ngOnInit(): void {
        this.paapId =+ this.route.snapshot.queryParamMap.get('paapId');

        this.getPaapStateList(this.paapId);
    }

    getPaapStateList(paapId){
        this._paapService.getPaapStateListByPaapId(paapId).subscribe(result => {
            this.paapStateList = result;
        });
    }
}