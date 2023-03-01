import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import {
    BugetPrevDobandaReferintaEditDto, BugetPrevDobandaRefServiceProxy, BugetConfigServiceProxy, EnumServiceProxy, EnumTypeDto,
    BugetConfigDto, GetCurrencyOutput, PersonServiceProxy, ActivityTypeServiceProxy, ActivityTypeDto
} from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './dobandaReferintaAdd.component.html',
    animations: [appModuleAnimation()]
})
export class DobandaReferintaAddComponent extends AppComponentBase implements OnInit {
    anBugetList: BugetConfigDto[] = [];
    dobandaRef: BugetPrevDobandaReferintaEditDto = new BugetPrevDobandaReferintaEditDto();
    dobandaRefId: number;
    plasamentTypeList: EnumTypeDto[] = [];
    currenciesList: GetCurrencyOutput = new GetCurrencyOutput();
    activityTypeList: ActivityTypeDto[] = [];

    constructor(inject: Injector,
        private _dobandaRefService: BugetPrevDobandaRefServiceProxy,
        private _enumTypeService: EnumServiceProxy,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _personService: PersonServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private route: ActivatedRoute,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Prevazut.DobanziReferinta')) {
            this.getAnBugetList();
            this.dobandaRefId = +this.route.snapshot.queryParamMap.get("dobandaRefId");
            this._dobandaRefService.getDobandaRefById(this.dobandaRefId).subscribe(result => {
                this.dobandaRef = result;
                this.getPlasamentTypeList();
                this.getCurrenciesList();
                this.getActivityTypesList();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getAnBugetList() {
        this._bugetConfigService.bugetConfigList().subscribe(result => {
            this.anBugetList = result;
        });
    }

    getPlasamentTypeList() {
        this._enumTypeService.bVC_PlasamentTypeList().subscribe(result => {
            this.plasamentTypeList = result;
        });
    }

    getCurrenciesList() {
        this._personService.currencyList().subscribe(result => {
            this.currenciesList = result;
        })
    }

    getActivityTypesList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypeList = result;
        });
    }

    save() {
        this._dobandaRefService.saveDobandaRef(this.dobandaRef).subscribe(result => {
            this.router.navigate(['/app/buget/prevazut/dobandaReferinta/dobandaReferintaList']);
            abp.notify.success("Inregistrarea a fost salvata");
        });
    }

}