import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { PersonListDto, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './person.component.html',
    animations: [appModuleAnimation()],
  //  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PersonComponent extends AppComponentBase implements OnInit {

    personList: PersonListDto;
    personId: number;

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private route: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted("General.DateSocietate.Acces")) {
            this._personService.personSetupList().subscribe(result => {
                this.personList = result;
                this.personId = result.id;
            });
        } else {
            this.route.navigate(['/app/home']);
        }


    }
}