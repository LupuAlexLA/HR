import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from "../../../shared/app-component-base";
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { ContractDto, ContractsServiceProxy, CotaTVAListDto, CotaTVAServiceProxy, CurrencyListDto, DepartamentListDto, DepartamentServiceProxy, EnumServiceProxy, EnumTypeDto, GetCurrencyOutput, InvoiceServiceProxy, PersonServiceProxy, ThirdPartyListDDDto } from '../../../shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';

@Component({
    templateUrl: './contractNew.component.html',
    animations: [appModuleAnimation()]
})
export class ContractNewComponent extends AppComponentBase implements OnInit {

    contract: ContractDto = new ContractDto();
    searchContractsRes: ContractDto[] = [];
    currencies: GetCurrencyOutput;
    contractsCategory = [];
    contractId: any;
    thirdParties: ThirdPartyListDDDto[] = [];
    contractsStatus: EnumTypeDto[] = [];
    cotaTvaList: CotaTVAListDto[] = [];
    departamentList: DepartamentListDto[] = [];
   

    constructor(injector: Injector,
        private _contractService: ContractsServiceProxy,
        private _cotaTvaService: CotaTVAServiceProxy,
        private _personService: PersonServiceProxy,
        private _departamentService: DepartamentServiceProxy,
        private _invoiceService: InvoiceServiceProxy,
        private _enumService: EnumServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {

        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('General.Contracte.Modificare')) {
            this.contractId = +  this.route.snapshot.queryParamMap.get('contractId');

            if (this.contractId) {
                
                this._contractService.getContract(this.contractId).subscribe(result => {
                    this.searchContracts(result.thirdPartyId);
                    this.contract = result;
                    this.getParams();
                });
            }
            else {
                
                this.contract.contractDate = moment();
                this.getParams();
            }

            
        } else {
            this.router.navigate(['app/home']);
        }
    }


    getParams() {
        this.getContractsStatus();
        this.searchCurrency();
        this.getContractsCategory();
        this.getCotaTvaList();
        this.getDepartamentList();
    }

    getCotaTvaList() {
        console.log("called");
        this._cotaTvaService.getTVAListByYear(this.contract.contractDate).subscribe(result => {
            this.cotaTvaList = result;
            console.log(result);
        });
    }

    getDepartamentList() {
        this._departamentService.getDepartamentList().subscribe(result => {
            this.departamentList = result;
        });
    }

    searchCurrency() {
        this._personService.currencyList().subscribe(result => {
            this.currencies = result;
        });
    }

    searchThirdParty(thirdPartyName: any) {
        this._invoiceService.getThirdParty(thirdPartyName.target.value).subscribe(result => {
            this.thirdParties = result;
        });
    }

    searchContracts(thirdPartyId) {
        this._contractService.getContractsForThirdParty(thirdPartyId).subscribe(result => {
            this.searchContractsRes = result;
        });
    }

    getContractsStatus() {
        this._enumService.contractsStatusList().subscribe(result => {
            this.contractsStatus = result;
        });
    }

    getContractsCategory() {
        this._contractService.categoryList().subscribe(result => {
            this.contractsCategory = result;
        });
    }

    selectedThirdParty(thirdPartyId: number, thirdPartyName: string) {
        this.contract.thirdPartyId = thirdPartyId;
        this.contract.thirdPartyName = thirdPartyName;
        this.searchContracts(thirdPartyId);
    }

    selectedContractRes(contractId: number) {
        this.contract.masterContractId = contractId;
    }

    getThirdPartyName(thirdPartyId: number) {
        if (thirdPartyId === null || thirdPartyId === undefined)
            return '';

        return this.contract.thirdPartyName;
    }

    saveContract() {    
        this._contractService.saveContract(this.contract).subscribe(() => {
            abp.notify.info(this.l('ContractAddedMessage'));
            this.router.navigate(['app/economic/contracts/contractsList']);
        });
    }
}