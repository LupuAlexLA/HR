import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AppConsts } from "../../../shared/AppConsts";
import { GetCountryOutput, GetRegionOutput, PersonEditDto, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './personNew.component.html',
    animations: [appModuleAnimation()]
})
export class PersonNewComponent extends AppComponentBase implements OnInit {

    person: PersonEditDto = new PersonEditDto();
    personId: number;
    countryId: number;
    countries: GetCountryOutput;
    regions: GetRegionOutput;
    isDisabled = false;

    constructor(injector: Injector,
        private _personService: PersonServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted("General.Persoane.Modificare")) {
            this.personId = + this.route.snapshot.queryParamMap.get('personId');

            if (this.personId == 0) {
                this.isDisabled = false;
            } else {
                this.isDisabled = true;
            }

            if (this.personId != 0) {
                this._personService.getPersonById(this.personId).subscribe(result => {
                    this.person = result;
                    this.searchCountry();
                });
            } else {
                this.searchCountry();
                this.person.personType = "LP";
            }
        } else {
            this.router.navigate(['app/home']);
        }
    }

    anafFnc() {      
        if (this.person.id1) {
            this._personService.anafDateFirma(Number(this.person.id1)).subscribe(result => {
                this.person.addressStreet = result.found[0].adresa;
                this.person.id2 = result.found[0].nrRegCom;
                this.person.name = result.found[0].denumire;
                this.person.isVATPayer = result.found[0].scpTVA;
                this.person.isVATCollector = result.found[0].statusTvaIncasare;
                try {
                    this.person.startDateVATPayment = moment(result.found[0].data_inceput_ScpTVA, 'YYYY-MM-DD');
                }
                catch {
                    abp.notify.warn("Eroare preluare data inceput platitor TVA (" + result.found[0].data_inceput_ScpTVA + ")");
                }
            });
        }   
    }

    savePerson() {     
        this._personService.savePerson(this.person).subscribe(result => {
            abp.notify.info(this.l('Actualizare reusita'));

            //window.open('/app/nomenclatoare/thirdParty/thirdPartyAcc?thirdPartyId=' + result.id);
            //this.router.navigate(['/app/nomenclatoare/thirdParty/thirdPartyAcc?thirdPartyId=' + result.id]);
            this.router.navigate([]).then(response => {
                window.open(AppConsts.appBaseUrl + '/app/nomenclatoare/thirdParty/thirdPartyAcc?thirdPartyId=' + result.id, '_blank'); });
        });
    }

    // Populare lista tari
    searchCountry() {
        this._personService.countryList().subscribe(result => {
            this.countries = result;
            this.searchRegion();
        });
    }

    onSelectCountry(event: any) {
        this.countryId = event.target.value;
        this.searchRegion();
    }

    // Populare lista judete
    searchRegion() {
        this._personService.regionList(this.person.addressCountryId || this.countryId).subscribe(result => {
            this.regions = result;
        });
    }
}