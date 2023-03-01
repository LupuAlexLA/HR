import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AccountServiceProxy, DocumentTypeServiceProxy, GetAccountOutput, GetDocumentTypeOutput, GetThirdPartyOutput, PersonServiceProxy, PrepaymentsAddDirectDto, PrepaymentsServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './prepaymentsAddDirect.component.html',
    animations: [appModuleAnimation()]
})
export class PrepaymentsAddDirectComponent extends AppComponentBase implements OnInit {

    prepaymentId: number;
    prepaymentType: number;
    oper: PrepaymentsAddDirectDto = new PrepaymentsAddDirectDto();
    documentType: GetDocumentTypeOutput;
    thirdParties: GetThirdPartyOutput;
    accounts: GetAccountOutput;
    modCalcul: number;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _prepaymentsService: PrepaymentsServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _personService: PersonServiceProxy,
        private _accountService: AccountServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.CheltAvans.Chelt.Acces')) {
            this.prepaymentId = +this.route.snapshot.queryParamMap.get('prepaymentId');
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentsService.addDirectInit(this.prepaymentId || 0, this.prepaymentType).subscribe(result => {
                this.oper = result;
                this.documentTypeList();
                this.searchThirdParty(this.oper.thirdPartyName);
                this.accountList();
                this.durationDetails();
            });
        } else if (this.isGranted("Conta.VenitAvans.Venituri.Acces")) {
            this.prepaymentId = +this.route.snapshot.queryParamMap.get('prepaymentId');
            this.prepaymentType = +this.route.snapshot.queryParamMap.get('prepaymentType');

            this._prepaymentsService.addDirectInit(this.prepaymentId || 0, this.prepaymentType).subscribe(result => {
                this.oper = result;
                this.documentTypeList();
                this.searchThirdParty(this.oper.thirdPartyName);
                this.accountList();
                this.durationDetails();
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

    changeDate() {
        this._prepaymentsService.addDirectChangeDate(this.oper).subscribe(result => {
            this.oper = result;
        });
    }

    searchThirdParty(thirdPartyName: string) {
        this._personService.thirdPartySearch(thirdPartyName).subscribe(result => {
            this.thirdParties = result;
        });
    }

    searchThirdPartyByInput(event: any) {
        this._personService.thirdPartySearch(event.target.value).subscribe(result => {
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

    accountList() {
        this._accountService.prepaymentsAccountList(this.prepaymentType).subscribe(result => {
            this.accounts = result;
        });
    }

    durationDetails() {
        this._prepaymentsService.durationSetupDetails(this.prepaymentType).subscribe(result => {
            this.modCalcul = result.prepaymentDurationCalcId;
        });
    }

    calcDurata() {
        this._prepaymentsService.calcDurata(this.oper.depreciationStartDate, this.oper.endDateChelt, this.modCalcul).subscribe(result => {
            this.oper.durationInMonths = result;
        });
    }

    savePrepaymentsDirect() {
        this.isLoading = true;

        this._prepaymentsService.savePrepaymentDirect(this.oper).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.oper = result;
            abp.notify.info('AddMessage');
        });
    }

    checkAutoNumberForDoc(primDocumentTypeId: number) {
        this._documentTypeService.getDocTypeById(primDocumentTypeId).subscribe(result => {
            if (result.autoNumber === true) {
                this._documentTypeService.nextDocumentNumber(moment(this.oper.paymentDate), result).subscribe(result => {
                    this.oper.primDocumentNr = result;
                });
            } else {
                this.oper.primDocumentNr = "";
            }
        });
    }
}