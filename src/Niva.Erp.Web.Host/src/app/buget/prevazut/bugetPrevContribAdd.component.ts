import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ActivityTypeDto, ActivityTypeServiceProxy, BankListDto, BugetPrevContribAddDto, BugetPrevContribServiceProxy, CurrencyListDto, EnumServiceProxy, EnumTypeDto, PersonServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevContribAdd.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevContribAddComponent extends AppComponentBase implements OnInit {

    contribId: number;
    bugetPrevContrib: BugetPrevContribAddDto = new BugetPrevContribAddDto();
    currencyList: CurrencyListDto[] = [];
    bankList: BankListDto[] = [];
    activityTypeList: ActivityTypeDto[] = [];
    tipIncasareList: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _bugetPrevContribService: BugetPrevContribServiceProxy,
        private _personService: PersonServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router   ) {
        super(inject);
    }

    ngOnInit() {
        this.contribId = +this.route.snapshot.queryParamMap.get('contribId');
        this.Enums();
        this._bugetPrevContribService.getContribById(this.contribId || 0).subscribe(result => {
            this.bugetPrevContrib = result;
            this.getActivityType();
            this.getCurrencyList();
            this.getBankList();
        });

    }
    Enums() {
        this._enumService.bVC_BugetPrevContributieTipIncasare().subscribe(result => {
            console.log(result);
            this.tipIncasareList = result;
        });
    }

    onChangeTipIncasare() {
        if (this.bugetPrevContrib.tipIncasare != 0) {
            this.bugetPrevContrib.bankId = null;
        }
    }

    getCurrencyList() {
        this._personService.currencyList().subscribe(result => {
            this.currencyList = result.getCurrency;
        });
    }

    getActivityType() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypeList = result;
        });
    }

    getBankList() {
        this._personService.bankList().subscribe(result => {
            this.bankList = result.getBank;
        });
    }

    save() {
        this._bugetPrevContribService.saveContrib(this.bugetPrevContrib).subscribe(result => {
            this.router.navigate(['/app/buget/prevazut/bugetPrevContribList']);
            abp.notify.success("Operatia a fost inregistrata");
        });
    }
}