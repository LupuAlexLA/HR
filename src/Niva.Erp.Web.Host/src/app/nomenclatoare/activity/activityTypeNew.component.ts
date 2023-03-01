import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ActivityTypeDto, ActivityTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './activityTypeNew.component.html',
    animations: [appModuleAnimation()]
})
export class ActivityTypeNewComponent extends AppComponentBase implements OnInit {

    activityTypeId: any;
    activityType: ActivityTypeDto = new ActivityTypeDto();

    constructor(injector: Injector,
        private _activityTypeService: ActivityTypeServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        this.activityTypeId = this.route.snapshot.queryParamMap.get('activityTypeId');

        this._activityTypeService.activityTypeEditInit(this.activityTypeId || 0).subscribe(result => {
            this.activityType = result;
        });
    }

    savaActivityType() {
        this._activityTypeService.saveActivityType(this.activityType).subscribe(result => {
            abp.notify.info(this.l('OKMessage'));
            this.router.navigate(['/app/nomenclatoare/activity/activityType']);
        });
    }
}