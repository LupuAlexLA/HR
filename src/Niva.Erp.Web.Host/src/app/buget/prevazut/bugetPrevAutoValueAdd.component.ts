import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { BugetPrevAutoServiceProxy, BugetPrevAutoValueAddDto, DepartamentListDto, DepartamentServiceProxy, EnumServiceProxy, EnumTypeDto } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevAutoValueAdd.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevAutoValueAddComponent extends AppComponentBase implements OnInit {

    bugetPrevAutoValue: BugetPrevAutoValueAddDto = new BugetPrevAutoValueAddDto();
    bugetPrevAutoValueId: number;
    departamentList: DepartamentListDto[] = [];
    bvcRowTypeList: EnumTypeDto[] = [];
    bvcRowTypeIncomeList: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _departmentService: DepartamentServiceProxy,
        private _bugetPrevAutoService: BugetPrevAutoServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.bugetPrevAutoValueId = +this.route.snapshot.queryParamMap.get('bugetPrevAutoValueId');

        this._bugetPrevAutoService.editBugetPrevAuto(this.bugetPrevAutoValueId).subscribe(result => {
            this.bugetPrevAutoValue = result;
            this.getRowType();
            this.getRowTypeIncome();
            this.getDepartmentList();
        });
    }

    getRowType() {
        this._enumService.bVC_RowTypeList().subscribe(result => {
            this.bvcRowTypeList = result;
        });
    }

    getRowTypeIncome() {
        this._enumService.bVC_RowTypeIncome().subscribe(result => {
            this.bvcRowTypeIncomeList = result;
        });
    }

    getDepartmentList() {
        this._departmentService.getSalariatDepartamentList().subscribe(result => {
            this.departamentList = result;
        });
    }

    save() {
        this._bugetPrevAutoService.saveBugetPrevAuto(this.bugetPrevAutoValue).subscribe(() => {
            abp.notify.info("Modificarile au fost salvate");
            this.router.navigate(['/app/buget/prevazut/bugetPrevAutoValueList']);
        });
    }

}