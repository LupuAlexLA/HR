import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import {
    AccountServiceProxy, DocumentTypeServiceProxy, EnumServiceProxy, EnumTypeDto, GetAccountOutput, GetDocumentTypeOutput, GetImoAssetStorageOutput, GetThirdPartyOutput, ImoAssetAddDirectDto,
    ImoAssetClassCodeEditDto, ImoAssetClassCodeListDDDto, ImoAssetClassCodeServiceProxy, ImoAssetServiceProxy, ImoAssetStorageServiceProxy, PersonServiceProxy
} from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './imoAssetPVAddDirect.component.html',
    animations: [appModuleAnimation()]
})
export class ImoAssetPVAddDirectComponent extends AppComponentBase implements OnInit {

    oper: ImoAssetAddDirectDto = new ImoAssetAddDirectDto();
    assetTypeList: EnumTypeDto[] = [];
    documentType: GetDocumentTypeOutput;
    thirdParties: GetThirdPartyOutput;
    storageList: GetImoAssetStorageOutput;
    classCode: ImoAssetClassCodeListDDDto[] = [];
    accounts: GetAccountOutput;
    assetId: any;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _imoAssetService: ImoAssetServiceProxy,
        private _imoAssetStorageService: ImoAssetStorageServiceProxy,
        private _imoAssetClassCodeService: ImoAssetClassCodeServiceProxy,
        private _accountService: AccountServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _personService: PersonServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute) {
        super(inject);
    }

    ngOnInit() {
        this.assetId = this.route.snapshot.queryParamMap.get('assetId');

        this._imoAssetService.addDirectInit(this.assetId || 0).subscribe(result => {
            this.oper = result;
            this.documentTypeList();
            this.searchThirdParty(this.oper.thirdPartyName);
            this.imoAssetTypeList();
            this.searchStorage();
            this.searchClassCode();
            this.searchAccounts();
        });

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

    imoAssetTypeList() {
        this._enumService.imoAssetTypeList().subscribe(result => {
            this.assetTypeList = result;
        });
    }

    addDirectChangeType() {
        this._imoAssetService.addDirectChangeType(this.oper).subscribe(result => {
            this.oper = result;
        });
    }

    addDirectChangeDate() {
        this._imoAssetService.addDirectChangeDate(this.oper).subscribe(result => {
            this.oper = result;
        });
    }

    searchStorage() {
        this._imoAssetStorageService.imoAssetStorageList().subscribe(result => {
            this.storageList = result;
        });
    }

    searchClassCode() {
        this._imoAssetClassCodeService.imoAssetClassCodeListDD().subscribe(result => {
            this.classCode = result;
        });
    }

    getClassCode() {
        var classCode = new ImoAssetClassCodeEditDto();
        var classCodeId = this.oper.assetClassCodesId;
        this._imoAssetClassCodeService.getClassCodeById(classCodeId).subscribe(result => {
            classCode = result;
            this.oper.assetAccountId = classCode.assetAccountId;
            this.oper.depreciationAccountId = classCode.depreciationAccountId;
            this.oper.expenseAccountId = classCode.expenseAccountId;
            this.oper.durationInMonths = classCode.durationMin;
        });
    }

    searchAccounts() {
        this._accountService.imoAssetAccountList().subscribe(result => {
            this.accounts = result;
        });
    }

    saveAssetDirect() {
        this.isLoading = true;
        this._imoAssetService.saveAssetDirect(this.oper).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        )
            .subscribe(result => {
                abp.notify.info('AddMessage');
                this.oper = result;
            });
    }

    setDepreciationDate() {
        var startDate = this.oper.useStartDate;
        const startOfNextMonth = moment(startDate).add(1, 'M').startOf('month').format('YYYY-MM-DD hh:mm:ss');

        this.oper.depreciationStartDate = moment(startOfNextMonth);
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