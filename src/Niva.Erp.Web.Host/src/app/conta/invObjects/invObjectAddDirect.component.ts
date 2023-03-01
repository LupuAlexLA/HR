import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    AccountServiceProxy, DocumentTypeServiceProxy, GetAccountOutput, GetDocumentTypeOutput, GetInvCategoryOutput, GetInvObjectStorageOutput, GetThirdPartyOutput, InvObjectAddDirectDto, InvObjectCategoryServiceProxy,
    InvObjectServiceProxy, InvObjectStorageServiceProxy, PersonServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invObjectAddDirect.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectAddDirectComponent extends AppComponentBase implements OnInit {

    oper: InvObjectAddDirectDto = new InvObjectAddDirectDto();
    documentType: GetDocumentTypeOutput;
    thirdParties: GetThirdPartyOutput;
    storageList: GetInvObjectStorageOutput;
    accounts: GetAccountOutput;
    categories: GetInvCategoryOutput;
    invObjectId: number;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _accountService: AccountServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _personService: PersonServiceProxy,
        private _invCategService: InvObjectCategoryServiceProxy,
        private _invObjectService: InvObjectServiceProxy,
        private _invObjectStorageService: InvObjectStorageServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);

    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Intrari.Acces')) {
            this.invObjectId = + this.route.snapshot.queryParamMap.get('invObjectId');

            this._invObjectService.addDirectInit(this.invObjectId || 0).subscribe(result => {
                this.oper = result;
                this.documentTypeList();
                this.searchThirdParty(this.oper.thirdPartyName);
                this.searchAccounts();
                this.searchStorage();
                this.searchCategory();
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    documentTypeList() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentType = result;
        });
    }

    searchThirdParty(thirdPartyName: string) {
        this._personService.thirdPartySearch(thirdPartyName).subscribe(result => {
            this.thirdParties = result;
        })
    }

    searchThirdPartyByInput(search: any) {
        this._personService.thirdPartySearch(search.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId == 0) {
            return '';
        }
        return this.oper.thirdPartyName;
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.oper.thirdPartyId = thirdPartyId;
        this.oper.thirdPartyName = thirdPartyName;
    }

    searchStorage() {
        this._invObjectStorageService.invObjectStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    searchAccounts() {
        this._accountService.invObjAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    searchCategory() {
        this._invCategService.categoryList().subscribe(result => {
            this.categories = result;
        });
    }

    saveInvObjectDirect() {
        this.isLoading = true;
        this._invObjectService.saveObjectDirect(this.oper)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                abp.notify.info('Obiectul de inventar a fost salvat');
                this.oper = result;
            });
    }

    checkAutoNumberForDoc(primDocumentTypeId: number) {
        this._documentTypeService.getDocTypeById(primDocumentTypeId).subscribe(result => {
            if (result.autoNumber === true) {
                this._documentTypeService.nextDocumentNumber(moment(this.oper.operationDate), result).subscribe(result => {
                    this.oper.primDocumentNr = result;
                });
            } else {
                this.oper.primDocumentNr = "";
            }
        });
    }
}
