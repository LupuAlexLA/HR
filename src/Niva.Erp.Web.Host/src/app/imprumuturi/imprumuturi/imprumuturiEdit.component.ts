import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { DocumentTypeListDDDto, PersonServiceProxy, DocumentTypeListDto, DocumentTypeServiceProxy, GetDocumentTypeOutput, ImprumutEditDto, ImprumutServiceProxy, ImprumuturiTipuriDto, ImprumuturiTipuriServiceProxy, ThirdPartyAccListDto, ImprumuturiTipuriEditDto, DocumentTypeEditDto, ThirdPartyAccEditDto, ImprumuturiTipDurata, ImprumuturiTermenDto, ImprumuturiTermenServiceProxy, ImprumuturiTermenEditDto, DobanziReferintaServiceProxy, DobanziReferintaDto, DateDobanziReferintaDto, ActivityTypeServiceProxy, ActivityTypeDto, CurrencyListDto, CurrencyServiceProxy, GetBankOutput, BankListDto, TipCreditare, GetThirdPartyOutput, ThirdPartyListDto, AccountServiceProxy, AccountListDDDto } from '../../../shared/service-proxies/service-proxies';
import { FormControl } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, startWith, switchMap } from 'rxjs/operators';

import * as moment from 'moment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { SoldDbDialogComponent } from '../../conta/operations/sold-dialog/soldDb-dialog.component';

@Component({

    templateUrl: './imprumuturiEdit.component.html',
    animations: [appModuleAnimation()]
})


/** imprumuturiEdit component*/
export class ImprumuturiEditComponent extends AppComponentBase implements OnInit{
    /** imprumuturiEdit ctor */
    imprumut: ImprumutEditDto = new ImprumutEditDto();
    imprumutId: number;
    bankName: string;
    bankObj: BankListDto = new BankListDto();
    activitatiTip: ActivityTypeDto[] = [];
    currency: CurrencyListDto[] = [];
    myControlTipuri = new FormControl('');
    myControlDocTipuri = new FormControl('');
    myControlIban = new FormControl('');
    myControlIbanPlata = new FormControl('');
    myControlTermene = new FormControl('');
    myControlThirdParty = new FormControl('');
    myControlDobanzi = new FormControl(new DateDobanziReferintaDto());
    filteredOptionsTipuri: Observable<ImprumuturiTipuriDto[]>;
    filteredOptionsDocumentTypes: Observable<DocumentTypeListDDDto[]>;
    filteredOptionsIban: Observable<ThirdPartyAccListDto[]>;
    filteredOptionsIbanPlata: Observable<ThirdPartyAccListDto[]>;
    filteredOptionsTermene: Observable<ImprumuturiTermenDto[]>;
    filteredDobanziReferinta: Observable<DobanziReferintaDto[]>;
    filteredOptionsThirdParty: Observable<ThirdPartyListDto[]>
    accounts: AccountListDDDto[] = [];
    selectedAccountName: string;
    selectedAccountId: number;
    showAccounts: any[] = [];
    banks: GetBankOutput;
    credit: number = 1;
    thirdPartyList: GetThirdPartyOutput;
    
    
    
    

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumutService: ImprumutServiceProxy,
        private _documentService: DocumentTypeServiceProxy,
        private _tipuriService: ImprumuturiTipuriServiceProxy,
        private _personService: PersonServiceProxy,
        private _termeneService: ImprumuturiTermenServiceProxy,
        private _dobanziService: DobanziReferintaServiceProxy,
        private _activitateService: ActivityTypeServiceProxy,
        private _currencyService: PersonServiceProxy,
        private _accountService: AccountServiceProxy,
        private _modalService: BsModalService,
            ) {
        super(injector);
        
    }
    
    ngOnInit() {

        
        
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.activitatiTipList();
        this.currencyList();
        this.banksList();
        this.thirdPartyListFnc();
        
        if (this.imprumutId !== 0) {

            this._imprumutService.getImprumutId(this.imprumutId).subscribe(result => {
                this.imprumut = result;
                this._accountService.getAccountbyId(this.imprumut.contContabilId).subscribe(result => this.accounts.push(result));
                this.bankObj.issuerId = this.imprumut.bankId;
                this.populateAllFields(this.imprumut);
            });
        }

        else {
            
            this.imprumut.documentDate = moment(new Date());
            this.imprumut.perioadaTipDurata = 0;
            this.imprumut.imprumuturiTipDurata = 0;
            this.imprumut.tipDobanda = 0;
            this.imprumut.currencyId = 1;
            this.imprumut.tipCreditare = 0;
        }

        this.filteredOptionsThirdParty = this.myControlThirdParty.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterThirdParty(value)),

        );

        this.filteredOptionsTipuri = this.myControlTipuri.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterTipuri(value)),
                    
        );

        this.filteredOptionsTermene = this.myControlTermene.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterTermene(value)),

        );
        

        this.filteredOptionsDocumentTypes = this.myControlDocTipuri.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterDocumentTypes(value)),
           
        );
        

        this.filteredOptionsIban = this.myControlIban.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterBank(value)),
        );

        this.filteredOptionsIbanPlata = this.myControlIbanPlata.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterBank(value)),
        );

        this.filteredDobanziReferinta = this.myControlDobanzi.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterDobanzi(value)),
        );

       
        
    }

    populateAllFields(imprumut) {
        if (imprumut.dobanziReferintaId !== null) {
            this._dobanziService.getDobandaReferintaId(imprumut.dobanziReferintaId).subscribe(result =>
                this.myControlDobanzi.setValue(result))
        }
        if (imprumut.imprumuturiTipuriId !== null) {
            this._tipuriService.getTipId(imprumut.imprumuturiTipuriId).subscribe(result =>
                this.myControlTipuri.setValue(result))
        }
        if (imprumut.loanAccountId !== null) {
            this._personService.getBankAccById(imprumut.loanAccountId).subscribe(result =>
                this.myControlIban.setValue(result))
        }
        if (imprumut.paymentAccountId !== null) {
            this._personService.getBankAccById(imprumut.paymentAccountId).subscribe(result =>
                this.myControlIbanPlata.setValue(result))
        }
        if (imprumut.imprumuturiTermenId !== null) {
            this._termeneService.getTermenId(imprumut.imprumuturiTermenId).subscribe(result =>
                this.myControlTermene.setValue(result))
        }
        if (imprumut.documentTypeId !== null) {
            this._documentService.getDocTypeById(imprumut.documentTypeId).subscribe(result =>
                this.myControlDocTipuri.setValue(result))
        }
        if (imprumut.thirdPartyId !== null) {
            this._personService.thirdPartyList().subscribe(result => {
                
                this.myControlThirdParty.setValue(result.getThirdParty.find(f => f.personId == imprumut.thirdPartyId))
            });
        }
    }

    getAccountListComputing(search: any) {
        this._accountService.accountListComputing(search.target.value, this.imprumut.currencyId).subscribe(result => {
            this.accounts = result;
            
        });
    }

    setAccountName(itemId, itemName) {
        this.imprumut.accountName.id = itemId;
        this.imprumut.accountName.name = itemName;
        this.imprumut.contContabilId = itemId;
    }

    getName(item) {
        if (!item) {
            return '';
        }
        return item.name;  
    }

    
    showDbSold(accountId: number) {
        let showDbSoldDialog: BsModalRef;

        showDbSoldDialog = this._modalService.show(
            SoldDbDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    accountId: accountId,
                    currencyId: this.imprumut.currencyId
                },
            }
        );

    }

    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdPartyList = result;
        });
    }

    thirdPartyListFnc() {
        this._personService.thirdPartyList().subscribe(result => {
            this.thirdPartyList = result;
            
        });
        
    }


    activitatiTipList() {
        this._activitateService.activityTypeList().subscribe(result => {
            this.activitatiTip = result;
        });
    }
    currencyList() {
        this._currencyService.currencyList().subscribe(result => {
            this.currency = result.getCurrency;
        })
    }
    banksList() {
        this._personService.bankList().subscribe(result => {
            this.banks = result;
        });
    }

    displayFn(project): string {
        
        return project ? project.description : project;
    }
    displayFnDobanzi(project): string {

        return project ? project.dobanda : project;
    }
    displayFnDocTypes(project): string {
        
        return project ? project.typeName : project;
    }
    displayFnBankIban(project): string {
        
        return project ? project.iban : project;
    }
    displayFnBankIbanPlata(project): string {
        
        return project ? project.iban : project;
    }

    displayFnBankImprumuturiTermene(project): string {
        return project ? project.description : project;
    }

    displayFnThirdParty(project): string {
        return project ? project.fullName : project;
    }

    private _filterBank(value: string) {
        
        let filterValue = value.toString().toLowerCase();
        
        
        return this._personService.thirdPartyAccSetupList().pipe(
            filter(data => !!data),
            map((data) => {           
                return data.getThirdPartyAcc.filter(option => option.iban.toLowerCase().includes(filterValue.toString()) && (this.bankObj.bankName == undefined || option.bankName == this.bankObj.bankName) && (option.currency == this.currency.find(e => e.id == this.imprumut.currencyId).currencyName) );
            })
        )

        
    
    }

    private _filterDobanzi(value: string) {

        let filterValue = value.toString().toLowerCase();


        return this._dobanziService.dobanziReferintaList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.filter(option => option.dobanda.toLowerCase().includes(filterValue.toString()))
            })
        )

    }

    private _filterBankPlata(value: string) {

        let filterValue = value.toString().toLowerCase();


        return this._personService.thirdPartyAccSetupList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.getThirdPartyAcc.filter(option => option.iban.toLowerCase().includes(filterValue.toString()))
            })
        )

    }

    private _filterTipuri(value: string) {

        let filterValue = value.toString().toLowerCase();

        
        return this._tipuriService.imprumuturiTipuriList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.filter(option => option.description.toLowerCase().includes(filterValue.toString()))
            })
        )
      //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }

    private _filterThirdParty(value: string) {

        let filterValue = value.toString().toLowerCase();


        return this._personService.thirdPartyList().pipe(
            filter(data => !!data),
            map((data) => {
                
                return data.getThirdParty.filter(option => option.fullName.toLowerCase().includes(filterValue.toString()))
            })
        )
        //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }

    private _filterDocumentTypes(value: string) {

        let filterValue = value.toString().toLowerCase();

            
        return this._documentService.documentTypeList().pipe(
            filter(data => !!data),
            map((data) => {
                
                return data.getDocumentType.filter(option => option.typeName.toLowerCase().includes(filterValue.toString()))
            })
        )
        //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }
    private _filterTermene(value: string) {

        let filterValue = value.toString().toLowerCase();


        return this._termeneService.imprumuturiTermenList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.filter(option => option.description.toLowerCase().includes(filterValue.toString()))
            })
        )
        //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }

    getDuration() {
        
        if (this.imprumut.startDate && this.imprumut.endDate) {
            this.imprumut.durata = Math.floor(this.imprumut.endDate.diff((this.imprumut.startDate), 'months', true));
        }
    }

    calculateEndDateWithDuration(durata) {
        //cand este selectat luni 
        if (this.imprumut.imprumuturiTipDurata == 0) {         
            this.imprumut.endDate = moment(this.imprumut.startDate).add(durata, 'M');
        }
        
    }

    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        if (this.thirdPartyList) {
            
            return this.thirdPartyList.getThirdParty.find(f => f.personId == thirdPartyId).fullName;
        }
        
    }


    
    getBankName() {
        
        this.bankObj.bankName = this.banks.getBank.find(f => f.issuerId == this.imprumut.bankId).bankName;
        this.bankObj.issuerId = this.imprumut.bankId;
        this.filteredOptionsIban = this._filterBank("");
        this.filteredOptionsIbanPlata = this._filterBank("");
    }

    changeIbanWhenCurrencyChange() {
        this.filteredOptionsIban = this._filterBank("");
        this.filteredOptionsIbanPlata = this._filterBank("");
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.imprumut.thirdPartyId = thirdPartyId;
    }

    saveImprumut() {


        this.imprumut.dobanziReferintaId = this.myControlDobanzi.value.id;
        this.imprumut.imprumuturiTipuriId = this.myControlTipuri.value.id;
        this.imprumut.documentTypeId = this.myControlDocTipuri.value.id;
        this.imprumut.loanAccountId = this.myControlIban.value.id;
        this.imprumut.paymentAccountId = this.myControlIbanPlata.value.id;
        this.imprumut.imprumuturiTermenId = this.myControlTermene.value.id;
        this.imprumut.thirdPartyId = this.myControlThirdParty.value.personId;

        if (this.imprumut.tipCreditare == TipCreditare._1) {
            this.imprumut.startDate = this.imprumut.loanDate;
        }
        
        
            

            this._imprumutService.saveImprumut(this.imprumut).subscribe(() => {
                abp.notify.info(this.l('UpdateMessage'));
                this.router.navigate(['/app/imprumuturi/imprumuturiList']);
            });
       
        
    }
}
