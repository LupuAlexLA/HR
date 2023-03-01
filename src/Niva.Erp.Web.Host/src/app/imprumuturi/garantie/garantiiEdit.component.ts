import { Component, Injector, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, startWith, switchMap } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CurrencyListDto, EnumServiceProxy, EnumTypeDto, GarantieCeGaranteazaDto, GarantieCeGaranteazaServiceProxy, GarantieEditDto, GarantieServiceProxy,  GarantieTipDto,  GarantieTipServiceProxy,  PersonListDto,  PersonServiceProxy, ThirdPartyAccListDto} from '../../../shared/service-proxies/service-proxies';

@Component({

    animations: [appModuleAnimation()],
    templateUrl: './garantiiEdit.component.html',
    
})
/** garantiiEdit component*/
export class GarantiiEditComponent extends AppComponentBase implements OnInit{
    garantie: GarantieEditDto = new GarantieEditDto();
    currency: CurrencyListDto[] = [];
    garantieTip: GarantieTipDto[] = [];
    garantieCeGaranteaza: GarantieCeGaranteazaDto[] = [];
    garanriePrimitaDataList: EnumTypeDto[] = [];
   // garantieTip: GarantieTipDto[] = [];
    editValoare: boolean = false;
    imprumutId: number;
    garantieId: number;
    myControlIban = new FormControl('');
    filteredOptionsIban: Observable<ThirdPartyAccListDto[]>;
    myControlLegalPerson = new FormControl('');
    filteredOptionsLegalPerson: Observable<PersonListDto[]>;

    /** garantiiEdit ctor */
    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _garantieService: GarantieServiceProxy,
        private _personService: PersonServiceProxy,
        private _garantieTipService: GarantieTipServiceProxy,
        private _garantieCeGaranteazaService: GarantieCeGaranteazaServiceProxy,
        private _enumService: EnumServiceProxy ) {

        super(injector);
        
    }
    ngOnInit(): void {
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.garantieId = + this.route.snapshot.queryParamMap.get('garantieId');
        this.currencyList();
        this.garantieTipList();
        this.garantieCeGaranteazaList();
        this.tipGarantiePrimitaDataList();

        if (this.imprumutId !== 0) {
            this.garantie.imprumutId = this.imprumutId;
            this.garantie.currencyId = 1;
        }
        if (this.garantieId !== 0) {

            this.editValoare = true;
            
            this._garantieService.getGarantieId(this.garantieId).subscribe(result => {
                this.garantie = result;
                this.imprumutId = this.garantie.imprumutId;
                this.BankIbanById(this.garantie.garantieAccountId);
                this.LegalPersonById(this.garantie.legalPersonId);
            });
        }

        this.filteredOptionsIban = this.myControlIban.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterBank(value)),
        );

        this.filteredOptionsLegalPerson = this.myControlLegalPerson.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterLegalPerson(value)),
        );
    }

    BankIbanById(id) {
        this._personService.getBankAccById(id).subscribe(result =>
            this.myControlIban.setValue(result))
    }

    LegalPersonById(id) {
        let ret : PersonListDto = new PersonListDto()
        this._personService.getPersonById(id).subscribe(result => {
            ret.id = result.id;
            ret.fullName = result.name;
            
            this.myControlLegalPerson.setValue(ret)
        } )
    }

    displayFnBankIban(project): string {
        return project ? project.iban : project;
    }

    displayFnLegalPerson(project): string {
        return project ? project.fullName : project;
    }

    private _filterBank(value: string) {

        let filterValue = value.toString().toLowerCase();
        return this._personService.thirdPartyAccSetupList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.getThirdPartyAcc.filter(option => option.iban.toLowerCase().includes(filterValue.toString()))
            })
        )
    }


    private _filterLegalPerson(value: string) {

        let filterValue = value.toString().toLowerCase();
        return this._personService.personList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.getPerson.filter(option => option.fullName.toLowerCase().includes(filterValue.toString()))
            })
        )
    }

    tipGarantiePrimitaDataList() {
        this._enumService.garantieTipPrimitaData().subscribe(result => {
            this.garanriePrimitaDataList = result;
        });
    }

    currencyList() {
        this._personService.currencyList().subscribe(result => {
            this.currency = result.getCurrency;
        })
    }

    garantieTipList() {
        this._garantieTipService.garantieTipList().subscribe(result => {
            this.garantieTip = result;
        })
    }
    garantieCeGaranteazaList() {
        this._garantieCeGaranteazaService.garantieCeGaranteazaList().subscribe(result => {
            this.garantieCeGaranteaza = result;
        })
    }

    saveGarantie() {
        this.garantie.garantieAccountId = this.myControlIban.value.id;
        this.garantie.legalPersonId = this.myControlLegalPerson.value.id;

        this._garantieService.saveGarantie(this.garantie).subscribe(() => {
            abp.notify.info(this.l('UpdateMessage'));
            
            this.router.navigate(['/app/imprumuturi/imprumuturiGarantiiList'], { queryParams: { imprumutId : this.imprumutId } } );
        });
    }
}