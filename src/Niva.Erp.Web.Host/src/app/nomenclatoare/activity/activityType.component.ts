import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { error } from 'console';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ActivityTypeDto, ActivityTypeServiceProxy, EnumTypeDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './activityType.component.html',
    animations: [appModuleAnimation()]
})
export class ActivityTypeComponent extends AppComponentBase implements OnInit {

    activitiesType: ActivityTypeDto[] = [];

    constructor(injector: Injector,
        private _activityTypeService: ActivityTypeServiceProxy,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.TipActivitate.Acces')) {
            this.getActivityTypeList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getActivityTypeList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activitiesType = result;
        });
    }

    activityTypeListCount() {
        if (this.activitiesType != null) {
            return this.activitiesType.length;
        } else {
            return 0;
        }
    }

    deleteActivityType(activityTypeId: number) {
        abp.message.confirm(this.l("DeleteActivityTypeMessage", activityTypeId),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._activityTypeService.deleteActivityType(activityTypeId).subscribe(() => {
                        this.getActivityTypeList();
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }
}