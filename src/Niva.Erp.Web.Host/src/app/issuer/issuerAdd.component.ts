import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { BNR_SectorListDto, BNR_SectorServiceProxy, CountryListDto, EnumServiceProxy, EnumTypeDto, IssuerServiceProxy, PersonEditDto, PersonIssuerEditDto, PersonListDto, PersonServiceProxy, RegionListDto } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './issuerAdd.component.html',
    animations: [appModuleAnimation()],
    //changeDetection: ChangeDetectionStrategy.Default
})
export class IssuerAddComponent extends AppComponentBase implements OnInit {

    personIssuerEdit: PersonIssuerEditDto = new PersonIssuerEditDto();
    persons: PersonListDto[] = [];
    search: string;
    showAddPerson: boolean = false;
    countries: CountryListDto[] = [];
    regions: RegionListDto[] = [];
    issuerTypeList: EnumTypeDto[];
    countryId: number;
    issuerId: number;
    sectorBnrList: BNR_SectorListDto[] = [];

    constructor(injector: Injector,
        private _issuerService: IssuerServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private _bnrSectorService: BNR_SectorServiceProxy,
        private router: Router,
        private route: ActivatedRoute) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('General.Emitenti.Modificare')) {
            this.issuerId = + this.route.snapshot.queryParamMap.get('issuerId');

            this._issuerService.personIssuerInit(this.issuerId || 0).subscribe(result => {
                this.personIssuerEdit = result;
            });

            this._personService.countryList().subscribe(result => {
                this.countries = result.getCountry;
                this.searchRegion();
            });

            this._enumService.issuerTypeList().subscribe(result => {
                this.issuerTypeList = result;
            });
            this.getBnrSectorList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    onSelectCountry(event: any) {
        this.countryId = event.target.value;
        this.searchRegion();
    }

    searchRegion() {
        this._personService.regionList(this.personIssuerEdit.addressCountryId || this.countryId).subscribe(result => {
            this.regions = result.getRegion;
        });
    }

    searchPerson(search: string) {
        this.search = search;
        this._personService.searchPerson(this.search).subscribe(result => {
            this.persons = result;
            this.checkAddPerson();
        });
    }

    checkAddPerson() {
        if (this.persons.length > 0) {
            this.showAddPerson = false;
        } else {
            this.showAddPerson = true;
        }
    }

    selectPerson(id: number) {
        this._issuerService.getPersonIssuerByPersonId(id).subscribe(result => {
            this.nextForm(result, 3);
            abp.notify.success('SelectedPersonSuccessfully');
        });
    }

    nextForm(personIssuerForm, formNr: number) {
        this._issuerService.showForm(formNr, personIssuerForm).subscribe(result => {
            this.personIssuerEdit = result;
        });
    }

    getBnrSectorList() {
        this._bnrSectorService.getBNRSectorList().subscribe(result => {
            this.sectorBnrList = result;
        });
    }

    savePersonIssuer() {
        this._issuerService.save(this.personIssuerEdit).subscribe(() => {
            this.router.navigate(['/app/emitenti']);
            abp.notify.success('SuccessfullySavedMessage');
        });
    }
}