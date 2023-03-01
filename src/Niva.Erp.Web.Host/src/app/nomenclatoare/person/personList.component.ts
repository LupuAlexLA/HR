import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GetPersonOutput, PersonInitForm, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './personList.component.html',
    animations: [appModuleAnimation()]
})
export class PersonListComponent extends AppComponentBase implements OnInit {

    personList: GetPersonOutput;
    personInit: PersonInitForm = new PersonInitForm();
    searchId2: string;
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private router: Router) {
        super(injector);
        this.editPerson = this.editPerson.bind(this);
        this.showThirdPartyAcc = this.showThirdPartyAcc.bind(this);
        this.deletePerson = this.deletePerson.bind(this);
    }

    ngOnInit() {
        if (this.isGranted("General.Persoane.Acces")) {
            this.getPersonList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getPersonList() {
        this._personService.initForm().subscribe(result => {
            this.personInit = result; 
        });
    }

    searchPerson() {
        this._personService.searchPersonList(this.personInit).subscribe(result => {
            this.personInit.personList = result;
        });
    }

    getPersonListCount() {
        if (this.personInit.personList == null) {
            return 0;
        } else {
            return this.personInit.personList?.length;
        }
    }

    deletePerson(e) {
        abp.message.confirm('Persoana va fi stearsa. Sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    this._personService.deletePerson(e.row.key.id)
                        .subscribe(() => {
                            this.getPersonList();
                            abp.notify.info(this.l('OKDelete'));
                        });
                }
            }
        );
    }

    editPerson(e) {
        this.router.navigate(['/app/nomenclatoare/person/personNew'], { queryParams: { personId: e.row.key.id }});
    }

    showThirdPartyAcc(e) {
        this.router.navigate(['/app/nomenclatoare/thirdParty/thirdPartyAcc'], { queryParams: { thirdPartyId: e.row.key.id }});
    }

    actualizarePersonal() {
        this.isLoading = true;
        this._personService.actualizarePerson().pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(() => {
            this.getPersonList();
            abp.notify.info("Personalul a fost actualizat");
        });
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }
}