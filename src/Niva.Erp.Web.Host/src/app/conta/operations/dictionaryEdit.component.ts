import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { AccountListDDDto, AccountServiceProxy, EnumServiceProxy, EnumTypeDto, FODictionaryEditDto, ForeignOperationDictionaryServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './dictionaryEdit.component.html',
    animations: [appModuleAnimation()]
})
export class DictionaryEditComponent extends AppComponentBase implements OnInit {

    dictionary: FODictionaryEditDto = new FODictionaryEditDto();
    accounts: AccountListDDDto[] = [];
    foDictionaryId: any;
    foDictionaryTypes: EnumTypeDto[] = [];

    constructor(inject: Injector,
        private _foDictionaryService: ForeignOperationDictionaryServiceProxy,
        private _accountService: AccountServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        this.foDictionaryId = +this.route.snapshot.queryParamMap.get('foDictionaryId');

        this._foDictionaryService.fODictionaryEdit(this.foDictionaryId).subscribe(result => {
            this.dictionary = result;
            this.getFODictionaryTypeList();
        });
    }

    getAccountsList() {
        this._accountService.accountListComputingAll().subscribe(result => {
            this.accounts = result;
        });
    }

    getAccountListComputing(event) {
        this._accountService.accountListAll(event.target.value).subscribe(result => {
            this.accounts = result;
        });
    }

    getAccountSymbol(accountId: number) {
        if (accountId === 0) {
            return '';
        }

        return this.dictionary.accountName;
    }

    getFODictionaryTypeList() {
        this._enumService.dictionaryTypeList().subscribe(result => {
            this.foDictionaryTypes = result;
        });
    }

    selectedInput(name: any, itemId: number) {
        this.dictionary.accountId = itemId;
        this.dictionary.accountName = name;
    }

    backToList() {
        this.router.navigate(['/app/conta/operations/dictionary']);
    }

    save() {
        this._foDictionaryService.saveFODictionary(this.dictionary).subscribe(() => {
            abp.notify.success("OKMessage");
            this.router.navigate(['/app/conta/operations/dictionary']);
        });
    }

}