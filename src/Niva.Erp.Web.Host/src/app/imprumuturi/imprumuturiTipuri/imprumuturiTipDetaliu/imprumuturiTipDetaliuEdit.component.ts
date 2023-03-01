import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Route, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { ActivityTypeDto, ActivityTypeServiceProxy, EnumImprumutTipDetaliuDescriereDto, EnumServiceProxy, EnumTypeDto, ImprumutTipDetaliuEditDto, ImprumuturiTipuriServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './imprumuturiTipDetaliuEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImprumuturiTipDetaliuEditComponent extends AppComponentBase implements OnInit {

    imprumutTipDetaliuId: number;
    imprumutTipId: number;
    imprumutTipDetaliuEdit: ImprumutTipDetaliuEditDto = new ImprumutTipDetaliuEditDto();
    activityTypeList: ActivityTypeDto[] = [];
    imprumutTipDetaliuDescriereList: EnumImprumutTipDetaliuDescriereDto[] = [];

    constructor(inject: Injector,
        private _imprumutTipService: ImprumuturiTipuriServiceProxy,
        private route: ActivatedRoute,
        private router: Router,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _enumService: EnumServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.imprumutTipDetaliuId = +this.route.snapshot.queryParamMap.get("imprumuturiTipDetaliuId");
        this.imprumutTipId = +this.route.snapshot.queryParamMap.get("imprumuturiTipId");

        this._imprumutTipService.getImprumutTipDetaliuId(this.imprumutTipDetaliuId || 0, this.imprumutTipId).subscribe(result => {
            this.imprumutTipDetaliuEdit = result;
            console.log(this.imprumutTipDetaliuEdit);
            this.getActivityTypeList();
            this.getImprumutTipDescriereList();
        });
    }

    getActivityTypeList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypeList = result;
        });
    };

    getImprumutTipDescriereList() {
        this._enumService.imprumutTipDetaliuDescriere().subscribe(result => {
            this.imprumutTipDetaliuDescriereList = result;
        });
    }

    save() {
        this._imprumutTipService.saveImprumutTipDetaliu(this.imprumutTipDetaliuEdit).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
         /*   this.router.navigate(['/app/imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/list', "?imprumuturiTipId="+this.imprumutTipId]);*/
        });
    }
}