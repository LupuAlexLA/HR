import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetCheltuieliDto, BugetCheltuieliServiceProxy, BugetPrevContribListDto, BugetPrevContribServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetCheltuieli.component.html',
    animations: [appModuleAnimation()]
})
export class BugetCheltuieliComponent extends AppComponentBase implements OnInit {

    cheltlist: BugetCheltuieliDto[] = [];
    contribId: any;

    constructor(inject: Injector,
        private _bugetPrevContribService: BugetCheltuieliServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
       
        this.getContribList();
            
       
            
        
    }

  
    getContribList() {
        this._bugetPrevContribService.getCheltuieliList().subscribe(result => {
            this.cheltlist = result;
        });

    }

    getCountBugetChelt() {
        if (this.cheltlist.length == 0) {
            return 0;
        } else {
            return this.cheltlist.length;
        }

    }

    search() {

    }

    delete(cheltId: number) {
        abp.message.confirm('Cheltuiala va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevContribService.deleteCheltuiala(cheltId).subscribe(() => {
                        abp.notify.info('Cheltuiala a fost stearsa');
                        this.getContribList();
                    });
                }
            });
    }
}