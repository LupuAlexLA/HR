import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetThirdPartyAccOutput, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './thirdPartyAcc.component.html',
    animations: [appModuleAnimation()]
})
export class ThirdPartyAccountComponent extends AppComponentBase implements OnInit {

    thirdPartyAccList: GetThirdPartyAccOutput;
    thirdPartyId: number;

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('General.Persoane.Acces')) {
            this.thirdPartyId = + this.route.snapshot.queryParamMap.get('thirdPartyId');
            this.getThirdPartyAccList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getThirdPartyAccList() {
        this._personService.thirdPartyAccList(this.thirdPartyId).subscribe(result => {
            this.thirdPartyAccList = result;
        });
    }

    getCountThirdPartyAcc() {
        if (this.thirdPartyAccList?.getThirdPartyAcc.length > 0) {
            return this.thirdPartyAccList.getThirdPartyAcc.length;
        } else {
            return 0;
        }
    }

    delete(id: number) {
        abp.message.confirm('Contul bancar va fi sters. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._personService.deleteThirdPartySetupAcc(id)
                        .subscribe(() => {
                            this.getThirdPartyAccList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            });
    }
}