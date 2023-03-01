import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AppConsts } from "../../../shared/AppConsts";
import { BugetPrevItemDto, BugetPrevStatCalculDto, BugetPrevMonthsDto, BugetPrevServiceProxy, DepartamentListDto, DepartamentServiceProxy, PaapServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './bugetPrevDetails.component.html',
    animations: [appModuleAnimation()]
})
export class BugetPrevDetailsComponent extends AppComponentBase implements OnInit{

    departamentList: DepartamentListDto[] = [];
    bugetPrevDetailList: BugetPrevItemDto = new BugetPrevItemDto();
    bugetPrevStatCalculList: BugetPrevStatCalculDto[] = [];
    monthList: BugetPrevMonthsDto[] = [];
    departamentId: number = 0;
    departamentName: string = null;
    bugetPrevId: number;
    isLoading: boolean = false;
    month: string = 'all';
    bugetPrevParams: any = {};
    url: string = '';
    values0: boolean = false;

    constructor(inject: Injector,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _departmentService: DepartamentServiceProxy,
        private _paapService: PaapServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Prevazut.Acces')) {
            this.bugetPrevId = +this.route.snapshot.queryParamMap.get('bugetPrevId');
            this.getDepartmentList();
            this.getUserDeptId();
            this.getBugetPrevDataLunaList(this.bugetPrevId);
            this.search();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getDepartmentList() {
        this._departmentService.getSalariatDepartamentList().subscribe(result => {
            this.departamentList = result;
        });
    }

    getBugetPrevDataLunaList(bugetPrevId) {
        this._bugetPrevService.getBugetPrevDataLunaList(bugetPrevId).subscribe(result => {
            this.monthList = result;
        });
    }

    search() {
        this.isLoading = true;
        this._bugetPrevService.getBugetPrevDetailsV2(this.departamentId, this.bugetPrevId, this.month, this.values0).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.bugetPrevDetailList = result;
            if (this.bugetPrevDetailList.bugetPrevByDepartmentList !== null && this.departamentId != 0) {
                this.departamentName = this.bugetPrevDetailList.bugetPrevByDepartmentList.departamentName;
            }
            else {
                this.getBugetPrevStatCalcList();
            }
        });
    }

    save() {
        this._bugetPrevService.save(this.bugetPrevId, this.departamentId, this.month, this.bugetPrevDetailList.bugetPrevByDepartmentList).subscribe(result => {
            abp.notify.success("Modificarile au fost salvate");
            this.search();
        });
    }

    validate() {
        if (this.departamentId == 0) {
            this.validateAll();
        } else {
            this.validateByDepartament();
        }
    }
    /**
     * validare pentru toate departamentele
     * */
    validateAll() {
        this._bugetPrevService.validateAll(this.bugetPrevId).subscribe(result => {
            abp.notify.success("Toate departamentele au fost validate");
            this.search();
        });
    }

    validateByDepartament() {
        this._bugetPrevService.validateByDepartament(this.bugetPrevId, this.departamentId).subscribe(result => {
            abp.notify.success("Departamentul a fost validat");
            this.search();
        });
    }

    cancel() {
        if (this.departamentId == 0) {
            this.cancelAll();
        } else {
            this.cancelByDepartament();
        }
    }

    cancelAll() {
        abp.message.confirm('Validarile vor fi anulate. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevService.cancelAll(this.bugetPrevId).subscribe(() => {
                        abp.notify.info('Validarile au fost sterse');
                        this.search();
                    });
                }
            });
    }

    cancelByDepartament() {
        abp.message.confirm('Validarile vor fi anulate. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._bugetPrevService.cancelByDepartament(this.bugetPrevId, this.departamentId).subscribe(() => {
                        abp.notify.info('Validarile au fost sterse');
                        this.search();
                    });
                }
            });
    }

    showReport(activityType: boolean) {
        this.bugetPrevParams.departmentId = this.departamentId;
        this.bugetPrevParams.bugetPrevId = this.bugetPrevId;
        this, this.bugetPrevParams.activityType = activityType;
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        if (this.departamentId == 0) {
            this.url += "BugetPrevAllDepartments";
            this.url += '?' + this.ConvertToQueryStringParameters(this.bugetPrevParams);
        } else {
            this.url += "BugetPrevDepartment";
            this.url += '?' + this.ConvertToQueryStringParameters(this.bugetPrevParams);
        }
        window.open(this.url);
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }



    getBugetPrevStatCalcList() {
        this._bugetPrevService.bugetPrevStatCalcList(this.bugetPrevId).subscribe(result => {
            this.bugetPrevStatCalculList = result;
        });
    }



    getUserDeptId() {
        if (this.isGranted('Buget.BVC.Prevazut.Modificare')) {
            this.departamentId = 0;
        }
        else {
            this._paapService.getUserDeptId().subscribe(result => {
                this.departamentId = result;
            });
        }
    }
}