import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { GetThirdPartyAccOutput, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './thirdPartyAcc.component.html',
    animations: [appModuleAnimation()]
})
export class ThirdPartyAccComponent extends AppComponentBase implements OnInit {

    thirdPartyAccList: GetThirdPartyAccOutput = new GetThirdPartyAccOutput();

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private router: Router    ) {
        super(injector);
        this.editThirdPartyAcc = this.editThirdPartyAcc.bind(this);
        this.deleteThirdPartySetupAcc = this.deleteThirdPartySetupAcc.bind(this);
    }

    ngOnInit(): void {
        if (this.isGranted('General.ListaConturi.Acces')) {
            this.getThirdPartyAccSetupList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getThirdPartyAccSetupList() {

        this._personService.thirdPartyAccSetupList().subscribe(result => {
            this.thirdPartyAccList = result;
        }); 
    }

    editThirdPartyAcc(e) {
        this.router.navigate(['/app/setup/banks/thirdPartyAccEdit'], { queryParams: { thirdPartyId: e.row.key.thirdPartyId, id: e.row.key.id } });
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    deleteThirdPartySetupAcc(e) {
        abp.message.confirm(
            this.l('DeleteMessage', e.row.key.id),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._personService.deleteThirdPartySetupAcc(e.row.key.id).subscribe(() => {
                            this.getThirdPartyAccSetupList();
                            abp.notify.success(this.l('SuccessfullyDeleted'));

                    }, error => abp.notify.error(error.data.message, error.data.details));
                }
            }
        );
    }
}