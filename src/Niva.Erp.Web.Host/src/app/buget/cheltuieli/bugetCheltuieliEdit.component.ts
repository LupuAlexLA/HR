import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ActivityTypeDto, ActivityTypeServiceProxy, BankListDto, BugetCheltuieliEditDto, BugetCheltuieliServiceProxy, BugetFormRandDto, BugetPrevContribAddDto, BugetPrevContribServiceProxy, CurrencyListDto, DepartamentListDto, DepartamentServiceProxy, EnumServiceProxy, EnumTypeDto, PersonServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetCheltuieliEdit.component.html',
    animations: [appModuleAnimation()]
})
export class BugetCheltuieliEditComponent extends AppComponentBase implements OnInit {

    contribId: number;
    bugetCheltuieli: BugetCheltuieliEditDto = new BugetCheltuieliEditDto();
    currencyList: CurrencyListDto[] = [];
    departmentList: DepartamentListDto[] = [];
    activityTypeList: ActivityTypeDto[] = [];
    cheltuieliList: BugetFormRandDto[] = [];

    constructor(inject: Injector,
        private _bugetCheltuieliService: BugetCheltuieliServiceProxy,
        private _personService : PersonServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _departmentService: DepartamentServiceProxy,
        private route: ActivatedRoute,
        private router: Router   ) {
        super(inject);
    }

    ngOnInit() {
        this.contribId = +this.route.snapshot.queryParamMap.get('contribId');
        this._bugetCheltuieliService.getCheltuieliById(this.contribId || 0).subscribe(result => {
            this.bugetCheltuieli = result;
            this.getActivityType();
            this.getCurrencyList();
            this.getCheltuieliDisponibile();
            this.getDepartmentList();
        });

    }


    getCheltuieliDisponibile() {
        this._bugetCheltuieliService.cheltuieliDisponobileList(this.bugetCheltuieli).subscribe(result => {
            this.cheltuieliList = result;
        });
    }

    onChangeTipCheltuiala() {
        
        this.bugetCheltuieli.currencyId = null;
        
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

    getDepartmentList() {
        this._departmentService.getDepartamentList().subscribe(result => {
            this.departmentList = result;
        });
    }

    

    save() {
        this._bugetCheltuieliService.saveCheltuieli(this.bugetCheltuieli).subscribe(() => {
            this.router.navigate(['/app/buget/cheltuieli/bugetCheltuieliList']);
            abp.notify.success("Operatia a fost inregistrata");
        });
    }
}