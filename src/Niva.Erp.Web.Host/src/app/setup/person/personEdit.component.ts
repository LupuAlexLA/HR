import { Component, ChangeDetectionStrategy, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { CountryListDto, EnumServiceProxy, EnumTypeDto, IssuerDto, IssuerServiceProxy, PersonEditDto, PersonServiceProxy, RegionListDto } from '../../../shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    templateUrl: './personEdit.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.Default
})
export class PersonEditComponent extends AppComponentBase implements OnInit {

    personId: any;
    countryId: any;
    isDisabled = false;
    person: PersonEditDto;
    countries: CountryListDto[] = [];
    regions: RegionListDto[] = [];
    issuerTypes: EnumTypeDto[];
    isIssuerTypeSelected: boolean;

    issuer: IssuerDto = new IssuerDto();

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private _issuerService: IssuerServiceProxy) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.isGranted('General.DateSocietate.Acces')) {


            this.personId = this.route.snapshot.queryParamMap.get('personId');

            if (this.personId == undefined) {
                this.isDisabled = false;
            } else {
                this.isDisabled = true;
            }


            this._personService.getPersonById(this.personId)
                .subscribe(result => {
                    this.person = result;
                    this.getCountryList();
                });

            this._enumService.issuerTypeList().subscribe(result => {
                this.issuerTypes = result;
            });

            this._issuerService.getIssuerByPersonId(this.personId).subscribe(result => {
                this.issuer = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    getCountryList() {
        this._personService.countryList().subscribe(result => {
            this.countries = result.getCountry;
            this.searchRegion();
        });
    }

    onSelectCountry(event: any) {
        this.countryId = event.target.value;
        this.searchRegion();
    }

    onSelectIssuerType(event: any) {
        if (event.target.value) {
            this.isIssuerTypeSelected = true;
        } else {
            this.isIssuerTypeSelected = false;
        }
    }

    searchRegion() {
        this._personService.regionList(this.person.addressCountryId || this.countryId).subscribe(result => {
            this.regions = result.getRegion;
        });
    }

    savePersonFnc() {
        this._issuerService.saveIssuer(this.issuer).subscribe(() => { });
        this._personService.savePerson(this.person).subscribe(() => {

            abp.notify.info(this.l('PersonAddMessage'));
            this.router.navigate(['/app/setup/person/person']);
        });

    }
}