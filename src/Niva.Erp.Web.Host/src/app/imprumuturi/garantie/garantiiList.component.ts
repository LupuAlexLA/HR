import { Component, Injector, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { result } from 'lodash';
import { filter, map } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { GarantieDto, GarantieServiceProxy, GetPersonOutput, ImprumutDto, ImprumutServiceProxy, PersonServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './garantiiList.component.html',
    animations: [appModuleAnimation()]

})
/** GarantiiList component*/
export class GarantiiListComponent extends AppComponentBase implements OnInit {
    /** GarantiiList ctor */
    imprumut: ImprumutDto = new ImprumutDto();
    garantii: GarantieDto[] = [];
    personList: GetPersonOutput;
    imprumutId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumutService: ImprumutServiceProxy,
        private _garantieService: GarantieServiceProxy,
       /* private _personService: PersonServiceProxy,*/
    ) {
        super(injector);

        
    }
    ngOnInit(): void {
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.getImprumutId(this.imprumutId);
        this.getGarantiiListId(this.imprumutId);
    }
    getGarantiiListId(imprumutId: number) {
        this._garantieService.garantieListId(imprumutId).subscribe(result => this.garantii = result);
    }

    //getPersonList() {
    //    this._personService.personList().subscribe(result => {
    //        this.personList = result;
    //    });
    //}

    getImprumutId(imprumutId: number) {

        this._imprumutService.imprumutId(imprumutId).subscribe(result => this.imprumut = result);


        //this._imprumutService.imprumutList().pipe(
        //    filter(data => !!data),
        //    map((data) => {
        //        return data.filter(option => option.id == imprumutId)
        //    })
        //).subscribe(result => this.imprumut = result)
    }
    getGarantiiCount() {
        
        if (this.garantii.length > 0) {
            return this.garantii.length;
        } else {
            return 0;
        }
    }
    delete(garantieId: number) {
        this._garantieService.deleteGarantie(garantieId).subscribe(() => {
            
            this.getGarantiiListId(this.imprumutId);

            abp.notify.success(this.l('DeleteMessage'));
        })
    }
}