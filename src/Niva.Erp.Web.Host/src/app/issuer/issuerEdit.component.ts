import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { BNR_SectorListDto, BNR_SectorServiceProxy, CountryListDto, EnumServiceProxy, EnumTypeDto, IssuerServiceProxy, PersonEditDto, PersonIssuerEditDto, PersonListDto, PersonServiceProxy, RegionListDto } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './issuerEdit.component.html',
    animations: [appModuleAnimation()]
})
export class IssuerEditComponent extends AppComponentBase implements OnInit {

    personIssuerEdit: PersonIssuerEditDto = new PersonIssuerEditDto();
    persons: PersonListDto[] = [];
    selectedPerson: PersonEditDto = new PersonEditDto();
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
        if (this.isGranted('General.Emitenti.Acces')) {
            this.issuerId = + this.route.snapshot.queryParamMap.get('issuerId');

            this._issuerService.personIssuerInit(this.issuerId || 0).subscribe(result => {
                this.personIssuerEdit = result;
                this.getBnrSectorList();
                this.getCountryList();
            });

            this._enumService.issuerTypeList().subscribe(result => {
                this.issuerTypeList = result;
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

    searchRegion() {
        this._personService.regionList(this.personIssuerEdit.addressCountryId || this.countryId).subscribe(result => {
            this.regions = result.getRegion;
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
