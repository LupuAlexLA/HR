import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetCheltuieliDto, BugetCheltuieliServiceProxy, BugetPrevContribListDto, BugetPrevContribServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetParametrii.component.html',
    animations: [appModuleAnimation()]
})
export class BugetParametriiComponent extends AppComponentBase implements OnInit {

    

    constructor(inject: Injector,
        private _bugetPrevContribService: BugetCheltuieliServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
       
       
            
        
    }

  
    
}