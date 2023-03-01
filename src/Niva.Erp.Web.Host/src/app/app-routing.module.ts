import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { DocTypeComponent } from './conta/nomenclatoare/document/docType.component';
import { DocTypeListEditComponent } from './conta/nomenclatoare/document/docTypeListEdit.component';

import { IssuerComponent } from './issuer/issuer.component';
import { IssuerAddComponent } from './issuer/issuerAdd.component';
import { IssuerEditComponent } from './issuer/issuerEdit.component';
import { AccountListComponent } from './conta/nomenclatoare/accounts/accountList.component';
import { AccountEditComponent } from './conta/nomenclatoare/accounts/accountEdit.component';
import { AccountRelationListComponent } from './conta/nomenclatoare/accounts/accountRelationList.component';
import { AccountRelationEditComponent } from './conta/nomenclatoare/accounts/accountRelationEdit.component';
import { AccountDeductibilityComponent } from './conta/nomenclatoare/accounts/accountDeductibility.component';
import { AccountConfigComponent } from './conta/nomenclatoare/accounts/accountConfig.component';
import { AccountConfigEditComponent } from './conta/nomenclatoare/accounts/accountConfigEdit.component';
import { AutoOperationConfigComponent } from './conta/autoOperation/autoOperationConfig.component';
import { AutoInvoicesComponent } from './conta/autoOperation/autoInvoices.component';
import { AutoOperationsComponent } from './conta/autoOperation/autoOperations.component';

import { PersonComponent } from './setup/person/person.component';
import { ThirdPartyAccComponent } from './setup/banks/thirdPartyAcc.component';
import { ThirdPartyAccEditComponent } from './setup/banks/thirdPartyAccEdit.component';
import { PersonEditComponent } from './setup/person/personEdit.component';

// operations
import { OperationsListComponent } from './conta/operations/operationsList.component';
import { OperationEditComponent } from './conta/operations/operationEdit.component';
import { OperationDefinitionComponent } from './conta/operations/operationsDefinition/operationDefinition.component';
import { ForeignOperationComponent } from './conta/operations/foreignOperation.component';
import { DictionaryComponent } from './conta/operations/dictionary.component';
import { DictionaryEditComponent } from './conta/operations/dictionaryEdit.component';
import { UploadComponent } from './conta/operations/upload.component';
import { TrazactiiListComponent } from './conta/operations/TranzactiiFonduri/tranzactiiList.component'

//Balance
import { BalanceListComponent } from './conta/balance/balanceList.component';
import { SavedBalanceListComponent } from './conta/balance/savedBalanceList.component';
import { InventoryBalanceListComponent } from './conta/balance/inventory/inventoryBalanceList.component';

import { ContractsListComponent } from './economic/contracts/contractsList.component';
import { ContractNewComponent } from './economic/contracts/contractNew.component';
import { ContractCategoryListComponent } from './economic/contractCategory/contractCategoryList.component';
import { ContractCategoryEditComponent } from './economic/contractCategory/contractCategoryEdit.component';
import { DetailElementsListComponent } from './conta/nomenclatoare/categoryElements/detailElementsList.component';
import { DetailElementsNewComponent } from './conta/nomenclatoare/categoryElements/detailElementsNew.component';
import { CategoryDetailElementsListComponent } from './conta/nomenclatoare/categoryElements/categoryDetailElementsList.component';
import { CategoryDetailElementsNewComponent } from './conta/nomenclatoare/categoryElements/categoryDetailElementsNew.component';
import { InvoicesListComponent } from './economic/invoices/invoicesList.component';
import { InvoiceNewComponent } from './economic/invoices/invoiceNew.component';
import { SearchInvoiceComponent } from './economic/invoices/searchInvoice.component';
import { PersonListComponent } from './nomenclatoare/person/personList.component';
import { PersonNewComponent } from './nomenclatoare/person/personNew.component';
import { ThirdPartyAccountComponent } from './nomenclatoare/thirdParty/thirdPartyAcc.component';
import { ThirdPartyAccountEditComponent } from './nomenclatoare/thirdParty/thirdPartyAccEdit.component';
import { PaymentOrdersComponent } from './economic/paymentOrders/paymentOrders.component';
import { ExportCSVComponent } from './economic/paymentOrders/exportCSV.component';
import { ImoSetupComponent } from './conta/imoAsset/imoSetup.component';
import { ImoNomenclatoareComponent } from './conta/imoAsset/imoNomenclatoare.component';
import { ImoAssetOperDocTypeComponent } from './conta/imoAsset/imoAssetOperDocType.component';
import { ImoAssetOperDocTypeEditComponent } from './conta/imoAsset/imoAssetOperDocTypeEdit.component';
import { ImoNomStorageComponent } from './conta/imoAsset/imoNomStorage.component';
import { ImoNomStorageEditComponent } from './conta/imoAsset/imoNomStorageEdit.component';
import { ImoNomClassCodeComponent } from './conta/imoAsset/imoNomClassCode.component';
import { ImoNomClassCodeEditComponent } from './conta/imoAsset/imoNomClassCodeEdit.component';
import { ImoAssetPVComponent } from './conta/imoAsset/imoAssetPV.component';
import { ImoAssetPVAddComponent } from './conta/imoAsset/imoAssetPVAdd.component';
import { ImoAssetPVAddInvoiceComponent } from './conta/imoAsset/imoAssetPVAddInvoice.component';
import { ImoAssetPVAddDirectComponent } from './conta/imoAsset/imoAssetPVAddDirect.component';
import { ImoAssetOperationComponent } from './conta/imoAsset/imoAssetOperation.component';
import { ImoAssetOperationEditComponent } from './conta/imoAsset/imoAssetOperationEdit.component';
import { ImoAssetOperationViewComponent } from './conta/imoAsset/imoAssetOperationView.component';
import { ImoGestListComponent } from './conta/imoAsset/imoGestList.component';
import { ImoGestComputeComponent } from './conta/imoAsset/imoGestCompute.component';
import { ImoGestDeleteComponent } from './conta/imoAsset/imoGestDelete.component';
import { ImoAssetAddInUseComponent } from './conta/imoAsset/imoAssetAddInUse.component';
import { ReportingImoAssetComponent } from './conta/imoAsset/reporting.component';
import { ImoInventariereComponent } from './conta/imoAsset/imoInventariere.component';
import { ImoInventariereEditComponent } from './conta/imoAsset/imoInventariereEdit.component';
import { ImoAssetReportingComponent } from './conta/imoAsset/imoAssetReporting.component';

import { PrepaymentsInitComponent } from './conta/prepayments/prepaymentsInit.component';
import { PrepaymentsListComponent } from './conta/prepayments/prepaymentsList.component';
import { PrepaymentsAddComponent } from './conta/prepayments/prepaymentsAdd.component';
import { PrepaymentsAddInvoiceComponent } from './conta/prepayments/prepaymentsAddInvoice.component';
import { PrepaymentsAddDirectComponent } from './conta/prepayments/prepaymentsAddDirect.component';
import { PrepaymentsOperDocTypeComponent } from './conta/prepayments/prepaymentsOperDocType.component';
import { PrepaymentsOperDocTypeEditComponent } from './conta/prepayments/prepaymentsOperDocTypeEdit.component';
import { PrepaymentsExitComponent } from './conta/prepayments/prepaymentsExit.component';
import { PrepaymentsGestInitComponent } from './conta/prepayments/prepaymentsGestInit.component';
import { PrepaymentsGestListComponent } from './conta/prepayments/prepaymentsGestList.component';
import { PrepaymentsGestComputeComponent } from './conta/prepayments/prepaymentsGestCompute.component';
import { PrepaymentsGestDeleteComponent } from './conta/prepayments/prepaymentsGestDelete.component';
import { PrepaymentsSetupInitComponent } from './conta/prepayments/prepaymentsSetupInit.component';
import { PrepaymentsSetupComponent } from './conta/prepayments/prepaymentsSetup.component';
import { PrepaymentsReportingComponent } from './conta/prepayments/prepaymentsReporting.component';
import { ReportingComponent } from './conta/prepayments/reporting.component';
import { PreincomesInitComponent } from './conta/prepayments/preincomesInit.component';
import { PreincomesGestInitComponent } from './conta/prepayments/preincomesGestInit.component';
import { PreincomesSetupInitComponent } from './conta/prepayments/preincomesSetupInit.component';
import { PreincomesReportingComponent } from './conta/prepayments/preincomesReporting.component';
import { SitFinanConfigComponent } from './conta/sitFinan/sitFinanConfig.component';
import { SitFinanConfigReportComponent } from './conta/sitFinan/sitFinanConfigReport.component';
import { SitFinanConfigFormulaComponent } from './conta/sitFinan/sitFinanConfigFormula.component';
import { SitFinanConfigNoteComponent } from './conta/sitFinan/sitFinanConfigNote.component';
import { SitFinanCalcComponent } from './conta/sitFinan/sitFinanCalc.component';
import { SitFinanRapComponent } from './conta/sitFinan/sitFinanRap.component';
import { SitFinanConfigFluxComponent } from './conta/sitFinan/SitFinanConfigFlux.component';
import { SitFinanCalcReportComponent } from './conta/sitFinan/sitFinanCalcReport.component';
import { ActivityTypeComponent } from './nomenclatoare/activity/activityType.component';
import { ActivityTypeNewComponent } from './nomenclatoare/activity/activityTypeNew.component';
 
import { appDxWebWiewComponent } from './reporting/appDxWebWiew.component';
import { ReportBalanceComponent } from './reporting/reportBalance.component';
import { ConfigReportingComponent } from './reporting/configReporting.component';
import { InvoiceDetailsReportComponent } from './reporting/invoiceDetailsReport.component';

import { OperationTypeComponent } from './conta/nomenclatoare/operationsType/operationType.component';
import { OperationTypeEditComponent } from './conta/nomenclatoare/operationsType/operationTypeEdit.component';
import { RepAccountSheetComponent } from './reporting/repAccountSheet.component';
import { DispositionListComponent } from './economic/dispositions/dispositionList.component';
import { DispositionNewComponent } from './economic/dispositions/dispositionNew.component';
import { DepositListComponent } from './economic/dispositions/depositList.component';
import { DepositNewComponent } from './economic/dispositions/depositNew.component';
import { SoldListComponent } from './economic/dispositions/soldList.component';
import { SoldNewComponent } from './economic/dispositions/soldNew.component';
import { RegistruCasaComponent } from './reporting/registruCasa.component';
import { OperGenerateListComponent } from './conta/balance/operGenerate/operGenerateList.component';
import { OperGenerateAddComponent } from './conta/balance/operGenerate/operGenerateAdd.component';

// Obiecte de inventar
import { InvObjectComponent } from './conta/invObjects/invObject.component';
import { InvObjectAddComponent } from './conta/invObjects/invObjectAdd.component';
import { InvObjectAddDirectComponent } from './conta/invObjects/invObjectAddDirect.component';
import { InvObjectAddInvoiceComponent } from './conta/invObjects/invObjectAddInvoice.component';
import { InvObjectOperationComponent } from './conta/invObjects/invObjectOperation.component';
import { InvObjectOperationEditComponent } from './conta/invObjects/invObjectOperationEdit.component';
import { InvObjectNomenclatoareComponent } from './conta/invObjects/invObjectNomenclatoare.component';
import { InvObjectGestListComponent } from './conta/invObjects/invObjectGestList.component';
import { InvObjectNomStorageComponent } from './conta/invObjects/invObjectNomStorage.component';
import { InvObjectNomStorageEditComponent } from './conta/invObjects/invObjectNomStorageEdit.component';
import { InvObjectNomCategoryComponent } from './conta/invObjects/invObjectNomCategory.component';
import { InvObjectNomCategoryEditComponent } from './conta/invObjects/invObjectNomCategoryEdit.component';
import { InvObjectNomOperDocTypeComponent } from './conta/invObjects/invObjectNomOperDocType.component';
import { InvObjectNomOperDocTypeEditComponent } from './conta/invObjects/invObjectNomOperDocTypeEdit.component';
import { InvObjectGestComputeComponent } from './conta/invObjects/invObjectGestCompute.component';
import { InvObjectsGestDeleteComponent } from './conta/invObjects/invObjectsGestDelete.component';
import { InvObjectInventariereComponent } from './conta/invObjects/invObjectInventariere.component';
import { InvObjectInventariereEditComponent } from './conta/invObjects/invObjectInventariereEdit.component';
import { InvObjectReportComponent } from './conta/invObjects/invObjectReport.component';

// Decont
import { DecontListComponent } from './decont/decontList.component';
import { DecontAddComponent } from './decont/decontAdd.component';
import { DiurnaListComponent } from './conta/nomenclatoare/diurna/diurnaList.component';
import { DiurnaEditComponent } from './conta/nomenclatoare/diurna/diurnaEdit.component';
import { DiurnaPerZiListComponent } from './conta/nomenclatoare/diurna/diurnaPerZiList.component';
import { DiurnaPerZiEditComponent } from './conta/nomenclatoare/diurna/diurnaPerZiEdit.component';
import { DeclaratieCasierComponent } from './reporting/declaratieCasier.component';
import { ReportingInvObjectComponent } from './conta/invObjects/reporting.component';

// Imprumuturi
import { ImprumuturiTermenComponent } from './imprumuturi/imprumuturiTermen/imprumuturiTermen.component';
import { ImprumuturiTermenEditComponent } from './imprumuturi/imprumuturiTermen/imprumuturiTermenEdit.component';
import { ImprumuturiNomenclatoareComponent } from './imprumuturi/imprumuturiNomenclatoare/imprumuturiNomenclatoare.component';
import { ImprumuturiTipuriComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipuri.component';
import { ImprumuturiTipuriEditComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipuriEdit.component';
import { ImprumuturiListComponent } from './imprumuturi/imprumuturi/imprumuturiList.component';
import { ImprumuturiEditComponent } from './imprumuturi/imprumuturi/imprumuturiEdit.component';
import { ImprumuturiTipDetaliuListComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/imprumuturiTipDetaliuList.component';
import { ImprumuturiTipDetaliuEditComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/imprumuturiTipDetaliuEdit.component';

// Imprumuturi State
import { ImprumuturiStateComponent } from './imprumuturi/imprumutState/imprumuturiState.component';

// Calculator Dobanda
import { CalculatorDobandaComponent } from './imprumuturi/calculatorDobanda/calculatorDobanda.component';

//Comisioane
import { ComisioaneListComponent } from './imprumuturi/comisioane/comisioaneList.component';
import { ComisioaneEditComponent } from './imprumuturi/comisioane/comisioaneEdit.component';

//Date Comision
import { DateComisionListComponent } from './imprumuturi/dateComision/dateComisionList.component';

// Garantii
import { GarantiiListComponent } from './imprumuturi/garantie/garantiiList.component';
import { GarantiiEditComponent } from './imprumuturi/garantie/garantiiEdit.component';
import { GarantieNomenclatoareComponent } from './imprumuturi/garantieNomenclatoare/garantieNomenclatoare.component';
import { GarantieTipComponent } from './imprumuturi/garantieNomenclatoare/garantieTip/garantieTip.component';
import { GarantieTipEditComponent } from './imprumuturi/garantieNomenclatoare/garantieTip/garantieTipEdit.component';
import { GarantieCeGaranteazaComponent } from './imprumuturi/garantieNomenclatoare/GarantieCeGaranteaza/GarantieCeGaranteaza.component';
import { GarantieCeGaranteazaEditComponent } from './imprumuturi/garantieNomenclatoare/GarantieCeGaranteaza/GarantieCeGaranteazaEdit.component';
import { OperatieGarantieComponent } from './imprumuturi/OperatieGarantie/OperatieGarantie.component';

//Rate
import { RateListComponent } from './imprumuturi/rate/rateList.component';

// Dobanda de Referinta
import { DobanziReferintaComponent } from './imprumuturi/dobandaDeReferinta/dobanziReferinta.component';
import { DobanziReferintaEditComponent } from './imprumuturi/dobandaDeReferinta/dobanziReferintaEdit.component';
import { DateDobanziReferintaComponent } from './imprumuturi/dateDobandaDeReferinta/dateDobanziReferinta.component';
import { DateDobanziReferintaEditComponent } from './imprumuturi/dateDobandaDeReferinta/dateDobanziReferintaEdit.component';

// Configurare rapoarte
import { ReportConfigListComponent } from './conta/nomenclatoare/configRapoarte/reportConfigList.component';
import { ReportConfigEditComponent } from './conta/nomenclatoare/configRapoarte/reportConfigEdit.component';
import { RapConfigComponent } from './conta/nomenclatoare/configRapoarte/rapConfig.component';
import { RepConfigFormulaComponent } from './conta/nomenclatoare/configRapoarte/repConfigFormula.component';

//Registru inventar
import { ExceptionListComponent } from './conta/registruInventar/exceptii/exceptionList.component';

//Schimb valutar
import { ExchangeListComponent } from './economic/exchange/exchangeList.component';
import { ExchangeEditComponent } from './economic/exchange/exchangeEdit.component';

// Cupiuri 
import { CupiuriListComponent } from './economic/cupiuri/cupiuriList.component';
import { CupiuriAddComponent } from './economic/cupiuri/cupiuriAdd.component';

// Buget
import { PaapListComponent } from './buget/paap/paapList.component';
import { PaapAddComponent } from './buget/paap/paapAdd.component';
import { PaapValidatorComponent } from './buget/paap/paapValidator.component';
import { PaapAlocareCheltuieliComponent } from './buget/paap/paapAlocareCheltuieli.component';
import { PaapHistoryListComponent } from './buget/paap/paapHistoryList.component';
import { ExchangeRateForecastComponent } from './buget/paap/exchangeRateForecast.component';
import { ExchangeRateForecastAddComponent } from './buget/paap/exchangeRateForecastAdd.component';
import { BugetConfigListComponent } from './buget/bugetConfig/BugetConfigList.component';
import { BugetConfigEditComponent } from './buget/bugetConfig/BugetConfigEdit.component';
import { BugetRandListComponent } from './buget/bugetConfig/BugetRandList.component';
import { BugetPrevListComponent } from './buget/prevazut/bugetPrevList.component';
import { BugetPrevAddComponent } from './buget/prevazut/bugetPrevAdd.component';
import { BugetPrevDetailsComponent } from './buget/prevazut/bugetPrevDetails.component';
import { BugetPrevAutoValueListComponent } from './buget/prevazut/bugetPrevAutoValueList.component';
import { BugetPrevAutoValueAddComponent } from './buget/prevazut/bugetPrevAutoValueAdd.component';
import { BugetPrevContribListComponent } from './buget/prevazut/bugetPrevContribList.component';
import { BugetPrevContribAddComponent } from './buget/prevazut/bugetPrevContribAdd.component';
import { DobandaReferintaListComponent } from './buget/prevazut/dobandaReferinta/dobandaReferintaList.component';
import { DobandaReferintaAddComponent } from './buget/prevazut/dobandaReferinta/dobandaReferintaAdd.component';
import { BugetRealizatListComponent } from './buget/realizat/bugetRealizatList.component';
import { BugetRealizatAddComponent } from './buget/realizat/bugetRealizatAdd.component';
import { BugetRealizatRandListComponent } from './buget/realizat/bugetRealizatRandList.component';
import { BugetVenitListComponent } from './buget/venituri/bugetVenitList.component';
import { BugetVenitAddComponent } from './buget/venituri/bugetVenitAdd.component';
import { BugetVenitDetailsComponent } from './buget/venituri/bugetVenitDetails.component';
import { BugetReinvestComponent } from './buget/venituri/bugetReinvest.component';
import { BugetAplicaComponent } from './buget/venituri/bugetAplica.component';
import { BugetRepartizatComponent } from './buget/venituri/bugetRepartizat.component';
import { ReportBVCComponent } from './reporting/reportBVC.component';
import { BugetCheltuieliComponent } from './buget/cheltuieli/bugetCheltuieli.component';
import { RedistribuirePaapListComponent } from './buget/paap/redistribuire/redistribuirePaapList.component';
import { RedistribuirePaapAddComponent } from './buget/paap/redistribuire/redistribuirePaapAdd.component';

// Curs valutar
import { ExchangeRatesListComponent } from './conta/nomenclatoare/exchangeRates/exchangeRatesList.component';
import { ExchangeRatesListAddComponent } from './conta/nomenclatoare/exchangeRates/exchangeRatesListAdd.component';

// Cota TVA
import { CotaTVAListComponent } from './conta/nomenclatoare/cotaTVA/cotaTVAList.component';
import { CotaTVAEditComponent } from './conta/nomenclatoare/cotaTVA/cotaTVAEdit.component';

// Sectoare Bnr
  //Anexa Bnr
import { AnexaBnrComponent } from './sectoareBnr/anexaBnr.component';
import { CalculBnrComponent } from './sectoareBnr/calculBnr.component';
import { RaportareBNRComponent } from './sectoareBnr/raportareBNR.component';
import { DetaliiBNRComponent } from './sectoareBnr/detaliiBNR.component'

// BNR Sector
import { BnrSectorListComponent } from './conta/bnrSector/bnrSectorList.component';
import { BnrSectorAddComponent } from './conta/bnrSector/bnrSectorAdd.component';

//Lichiditate
import { LichidConfigComponent } from './conta/lichiditate/lichidConfig.component';
import { LichidCalculComponent } from './conta/lichiditate/lichidCalcul.component';
import { BugetRealizatBalantaListComponent } from './buget/realizat/BalRealizat/bugetRealizatBalantaList.component';
import { BugetCheltuieliEditComponent } from './buget/cheltuieli/bugetCheltuieliEdit.component';
import { BugetParametriiComponent } from './buget/parametrii/BugetParametrii.component';
import { FileDocErrorsComponent } from './setup/fileDoc/file-doc-errors.component';
import { PaapAlocareFacturiComponent } from './buget/paap/paapAlocareFacturi.component';
import { OperatieComisionDobandaComponent } from './imprumuturi/OperatiiComisionDobanda/operatieComisionDobanda.component';
import { ComisionV2ListComponent } from './imprumuturi/comisionV2/comisionV2List.component';
import { ComisionV2EditComponent } from './imprumuturi/comisionV2/comisionV2Edit.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' }, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent },
                    { path: 'update-password', component: ChangePasswordComponent },

                    //Economic
                    //Contracte
                    { path: 'economic/contracts/contractsList', component: ContractsListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/contracts/contractNew', component: ContractNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/contractCategory/contractCategoryList', component: ContractCategoryListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/contractCategory/contractCategoryEdit', component: ContractCategoryEditComponent, canActivate: [AppRouteGuard] },
                    // Elemente
                    { path: 'conta/nomenclatoare/categoryElements/detailElementsList', component: DetailElementsListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/categoryElements/detailElementsNew', component: DetailElementsNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/categoryElements/categoryDetailElementsList', component: CategoryDetailElementsListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/categoryElements/categoryDetailElementsNew', component: CategoryDetailElementsNewComponent, canActivate: [AppRouteGuard] },

                    // Facturi
                    { path: 'economic/invoices/invoicesList', component: InvoicesListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/invoices/invoiceNew', component: InvoiceNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/invoices/searchInvoice', component: SearchInvoiceComponent, canActivate: [AppRouteGuard] },

                    // Ordine de plata
                    { path: 'economic/paymentOrders/paymentOrders', component: PaymentOrdersComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/paymentOrders/exportCSV', component: ExportCSVComponent, canActivate: [AppRouteGuard] },

                    //Dispozitii
                    { path: 'economic/dispositions/dispositionList', component: DispositionListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/dispositions/dispositionNew', component: DispositionNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/dispositions/depositList', component: DepositListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/dispositions/depositNew', component: DepositNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/dispositions/soldList', component: SoldListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/dispositions/soldNew', component: SoldNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'reporting/registruCasa', component: RegistruCasaComponent, canActivate: [AppRouteGuard] },
                    { path: 'reporting/declaratieCasier', component: DeclaratieCasierComponent, canActivate: [AppRouteGuard] },
                    { path: 'reporting/invoiceDetailsReport', component: InvoiceDetailsReportComponent, canActivate: [AppRouteGuard] },

                    //Emitenti
                    { path: 'emitenti', component: IssuerComponent, canActivate: [AppRouteGuard] },
                    { path: 'emitentAdd', component: IssuerAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'emitentEdit', component: IssuerEditComponent, canActivate: [AppRouteGuard] },

                    //Operations
                    { path: 'conta/operations/operationsList', component: OperationsListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/operationEdit', component: OperationEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/operationDefinition', component: OperationDefinitionComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/upload', component: UploadComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/foreignOperation', component: ForeignOperationComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/dictionary', component: DictionaryComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/dictionaryEdit', component: DictionaryEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/operations/tranzactiiList', component: TrazactiiListComponent, canActivate: [AppRouteGuard] },

                    // Balance
                    { path: 'conta/balance/balanceList', component: BalanceListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/balance/savedBalanceList', component: SavedBalanceListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/document/docType', component: DocTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/balance/operGenerate/operGenerateList', component: OperGenerateListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/balance/operGenerate/operGenerateAdd', component: OperGenerateAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/balance/inventory/inventoryBalanceList', component: InventoryBalanceListComponent, canActivate: [AppRouteGuard] },

                    //Conta -> Nomenclatoare
                    { path: 'conta/nomenclatoare/document/docTypeListEdit', component: DocTypeListEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountList', component: AccountListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountEdit', component: AccountEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountRelationList', component: AccountRelationListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountRelationEdit', component: AccountRelationEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountDeductibility', component: AccountDeductibilityComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountConfig', component: AccountConfigComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/accounts/accountConfigEdit', component: AccountConfigEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/operationsType/operationType', component: OperationTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/operationsType/operationTypeEdit', component: OperationTypeEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/diurna/diurnaList', component: DiurnaListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/diurna/diurnaEdit', component: DiurnaEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/diurna/diurnaPerZiList', component: DiurnaPerZiListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/diurna/diurnaPerZiEdit', component: DiurnaPerZiEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/configRapoarte/rapConfigList', component: ReportConfigListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/configRapoarte/rapConfigEdit', component: ReportConfigEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/configRapoarte/rapConfig', component: RapConfigComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/configRapoarte/repConfigFormula', component: RepConfigFormulaComponent, canActivate: [AppRouteGuard] },

                    //Conta -> Situatii financiare                  
                    { path: 'conta/sitFinan/sitFinanConfig', component: SitFinanConfigComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanConfigReport', component: SitFinanConfigReportComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanConfigFormula', component: SitFinanConfigFormulaComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanCalc', component: SitFinanCalcComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanCalcReport', component: SitFinanCalcReportComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanRap', component: SitFinanRapComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanFlux', component: SitFinanConfigFluxComponent, canActivate: [AppRouteGuard] },

                    //Conta -> AutoOperation
                    { path: 'conta/autoOperation/autoOperationConfig', component: AutoOperationConfigComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/autoOperation/autoInvoices', component: AutoInvoicesComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/autoOperation/autoOperations', component: AutoOperationsComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/sitFinan/sitFinanConfigNote', component: SitFinanConfigNoteComponent, canActivate: [AppRouteGuard] },

                    // Decont
                    { path: 'decont', component: DecontListComponent, canActivate: [AppRouteGuard] },
                    { path: 'decontAdd', component: DecontAddComponent, canActivate: [AppRouteGuard] },

                    //ImoAsset
                    { path: 'conta/imoAsset/imoSetup', component: ImoSetupComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoNomenclatoare', component: ImoNomenclatoareComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetOperDocType', component: ImoAssetOperDocTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetOperDocTypeEdit', component: ImoAssetOperDocTypeEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoNomStorage', component: ImoNomStorageComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoNomStorageEdit', component: ImoNomStorageEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoNomClassCode', component: ImoNomClassCodeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoNomClassCodeEdit', component: ImoNomClassCodeEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetPV', component: ImoAssetPVComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetPVAdd', component: ImoAssetPVAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetPVAddInvoice', component: ImoAssetPVAddInvoiceComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetPVAddDirect', component: ImoAssetPVAddDirectComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetOperation', component: ImoAssetOperationComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetOperationEdit', component: ImoAssetOperationEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetOperationView', component: ImoAssetOperationViewComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoGestList', component: ImoGestListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoGestCompute', component: ImoGestComputeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoGestDelete', component: ImoGestDeleteComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/reporting', component: ReportingImoAssetComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetAddInUse', component: ImoAssetAddInUseComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoInventariere', component: ImoInventariereComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoInventariereEdit', component: ImoInventariereEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/imoAsset/imoAssetReporting', component: ImoAssetReportingComponent, canActivate: [AppRouteGuard] },

                    //InvObjects
                    { path: 'conta/invObjects/invObject', component: InvObjectComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectAdd', component: InvObjectAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectAddDirect', component: InvObjectAddDirectComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectAddInvoice', component: InvObjectAddInvoiceComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectOperation', component: InvObjectOperationComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectOperationEdit', component: InvObjectOperationEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomenclatoare', component: InvObjectNomenclatoareComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectGestList', component: InvObjectGestListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomStorage', component: InvObjectNomStorageComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomStorageEdit', component: InvObjectNomStorageEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomCategory', component: InvObjectNomCategoryComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomCategoryEdit', component: InvObjectNomCategoryEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomOperDocType', component: InvObjectNomOperDocTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectNomOperDocTypeEdit', component: InvObjectNomOperDocTypeEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectGestCompute', component: InvObjectGestComputeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectGestDelete', component: InvObjectsGestDeleteComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectInventariere', component: InvObjectInventariereComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectInventariereEdit', component: InvObjectInventariereEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/invObjectReport', component: InvObjectReportComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/invObjects/reporting', component: ReportingInvObjectComponent, canActivate: [AppRouteGuard] },

                    //Prepayments
                    { path: 'conta/prepayments/prepaymentsInit', component: PrepaymentsInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsList', component: PrepaymentsListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsAdd', component: PrepaymentsAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsAddInvoice', component: PrepaymentsAddInvoiceComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsAddDirect', component: PrepaymentsAddDirectComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsOperDocType', component: PrepaymentsOperDocTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsOperDocTypeEdit', component: PrepaymentsOperDocTypeEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsExit', component: PrepaymentsExitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsGestInit', component: PrepaymentsGestInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsGestList', component: PrepaymentsGestListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsGestCompute', component: PrepaymentsGestComputeComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsGestDelete', component: PrepaymentsGestDeleteComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsSetupInit', component: PrepaymentsSetupInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsSetup', component: PrepaymentsSetupComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/prepaymentsReporting', component: PrepaymentsReportingComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/reporting', component: ReportingComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/preincomesInit', component: PreincomesInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/preincomesGestInit', component: PreincomesGestInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/preincomesSetupInit', component: PreincomesSetupInitComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/prepayments/preincomesReporting', component: PreincomesReportingComponent, canActivate: [AppRouteGuard] },

                    //Reporting
                    { path: 'dXWebView/:reportName', component: appDxWebWiewComponent, canActivate: [AppRouteGuard]},
                    { path: 'reporting', component: ReportBalanceComponent, canActivate: [AppRouteGuard] },
                    { path: 'reporting/repAccountSheet', component: RepAccountSheetComponent, canActivate: [AppRouteGuard] },
                    { path: 'reporting/configReporting', component: ConfigReportingComponent, canActivate: [AppRouteGuard] },

                    // Nomenclatoare
                    { path: 'nomenclatoare/person/personList', component: PersonListComponent, canActivate: [AppRouteGuard] },
                    { path: 'nomenclatoare/person/personNew', component: PersonNewComponent, canActivate: [AppRouteGuard] },
                    { path: 'nomenclatoare/thirdParty/thirdPartyAcc', component: ThirdPartyAccountComponent, canActivate: [AppRouteGuard] },
                    { path: 'nomenclatoare/thirdParty/thirdPartyAccEdit', component: ThirdPartyAccountEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'nomenclatoare/activity/activityType', component: ActivityTypeComponent, canActivate: [AppRouteGuard] },
                    { path: 'nomenclatoare/activity/activityTypeNew', component: ActivityTypeNewComponent, canActivate: [AppRouteGuard] },

                    // Setup
                    { path: 'setup/person/person', component: PersonComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/person/personEdit', component: PersonEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/banks/thirdPartyAcc', component: ThirdPartyAccComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/banks/thirdPartyAccEdit', component: ThirdPartyAccEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/fileDoc/fileDocErrors', component: FileDocErrorsComponent, canActivate: [AppRouteGuard] },

                    //Imprumuturi
                    { path: 'imprumuturi/imprumuturiTermen', component: ImprumuturiTermenComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiTermenEdit', component: ImprumuturiTermenEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiNomenclatoare', component: ImprumuturiNomenclatoareComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiTipuri', component: ImprumuturiTipuriComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiTipuriEdit', component: ImprumuturiTipuriEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiList', component: ImprumuturiListComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiEdit', component: ImprumuturiEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiState', component: ImprumuturiStateComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/list', component: ImprumuturiTipDetaliuListComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/edit', component: ImprumuturiTipDetaliuEditComponent, canActivate: [AppRouteGuard] },

                    //Garantii
                    { path: 'imprumuturi/imprumuturiGarantiiList', component: GarantiiListComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiGarantiiEdit', component: GarantiiEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/garantieNomenclatoare', component: GarantieNomenclatoareComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/garantieTip', component: GarantieTipComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/garantieTipEdit', component: GarantieTipEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/garantieCeGaranteaza', component: GarantieCeGaranteazaComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/garantieCeGaranteazaEdit', component: GarantieCeGaranteazaEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/operatieGarantie', component: OperatieGarantieComponent, canActivate: [AppRouteGuard] },
                    
                    
                    //Rate
                    { path: 'rate/imprumuturiRateList', component: RateListComponent, canActivate: [AppRouteGuard] },

                    //Calculator Dobanda
                    { path: 'imprumuturi/calculatorDobanda', component: CalculatorDobandaComponent, canActivate: [AppRouteGuard] },

                    // Comisioane
                    { path: 'imprumuturi/imprumuturiComisioaneList', component: ComisioaneListComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiComisioaneEdit', component: ComisioaneEditComponent, canActivate: [AppRouteGuard] },

                    // Comisioane
                    { path: 'imprumuturi/imprumuturiComisioaneV2List', component: ComisionV2ListComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/imprumuturiComisioaneV2Edit', component: ComisionV2EditComponent, canActivate: [AppRouteGuard] },

                    // Operatie
                    { path: 'imprumuturi/OperatieComisionDobanda', component: OperatieComisionDobandaComponent, canActivate: [AppRouteGuard] },

                    //Date Comision
                    { path: 'imprumuturi/dateComisionList', component: DateComisionListComponent, canActivate: [AppRouteGuard] },
                    
                    // Dobanda de Referinta
                    { path: 'imprumuturi/dobandaDeReferinta', component: DobanziReferintaComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/dobandaDeReferintaEdit', component: DobanziReferintaEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/dateDobandaDeReferinta', component: DateDobanziReferintaComponent, canActivate: [AppRouteGuard] },
                    { path: 'imprumuturi/dateDobandaDeReferintaEdit', component: DateDobanziReferintaEditComponent, canActivate: [AppRouteGuard] },

                    //Registru inventar
                    { path: 'conta/registruInventar/exceptii/exceptionList', component: ExceptionListComponent, canActivate: [AppRouteGuard] },

                    //Schimb valutar
                    { path: 'economic/exchange/exchangeList', component: ExchangeListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/exchange/exchangeEdit', component: ExchangeEditComponent, canActivate: [AppRouteGuard] },

                    // Cupiuri
                    { path: 'economic/cupiuri/cupiuriList', component: CupiuriListComponent, canActivate: [AppRouteGuard] },
                    { path: 'economic/cupiuri/cupiuriAdd', component: CupiuriAddComponent, canActivate: [AppRouteGuard] },

                    // Buget
                    { path: 'buget/paap/paapList', component: PaapListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/paapAdd', component: PaapAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/paapValidator', component: PaapValidatorComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/paapAlocareCheltuieli', component: PaapAlocareCheltuieliComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/paapHistoryList', component: PaapHistoryListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/exchangeRateForecast', component: ExchangeRateForecastComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/exchangeRateForecastAdd', component: ExchangeRateForecastAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/alocareFacturi', component: PaapAlocareFacturiComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/bugetConfig/bugetConfigList', component: BugetConfigListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/bugetConfig/bugetConfigEdit', component: BugetConfigEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/bugetConfig/bugetRandList', component: BugetRandListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevList', component: BugetPrevListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevAdd', component: BugetPrevAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevDetails', component: BugetPrevDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevAutoValueList', component: BugetPrevAutoValueListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevAutoValueAdd', component: BugetPrevAutoValueAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevContribList', component: BugetPrevContribListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/bugetPrevContribAdd', component: BugetPrevContribAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/dobandaReferinta/dobandaReferintaList', component: DobandaReferintaListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/prevazut/dobandaReferinta/dobandaReferintaAdd', component: DobandaReferintaAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/realizat/bugetRealizatList', component: BugetRealizatListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/realizat/bugetRealizatAdd', component: BugetRealizatAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/realizat/bugetRealizatRandList', component: BugetRealizatRandListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/realizat/bugetBalRealizatRandList', component: BugetRealizatBalantaListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/venituri/bugetVenitList', component: BugetVenitListComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/venituri/bugetVenitAdd', component: BugetVenitAddComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/venituri/bugetVenitDetails', component: BugetVenitDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/venituri/bugetReinvest', component: BugetReinvestComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/venituri/bugetAplica', component: BugetAplicaComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/repartizat/bugetRepartizat', component: BugetRepartizatComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/reportBVC', component: ReportBVCComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/cheltuieli/bugetCheltuieliList', component: BugetCheltuieliComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/cheltuieli/bugetCheltuieliEdit', component: BugetCheltuieliEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/cheltuieli/bugetParametrii', component: BugetParametriiComponent, canActivate: [AppRouteGuard] },
                    { path: 'buget/paap/redistribuire/redistribuirePaapList', component: RedistribuirePaapListComponent, canActivate:[AppRouteGuard] },
                    { path: 'buget/paap/redistribuire/redistribuirePaapAdd', component: RedistribuirePaapAddComponent, canActivate:[AppRouteGuard] },
                    
                    // Curs valutar
                    { path: 'conta/nomenclatoare/exchangeRates/exchangeRatesList', component: ExchangeRatesListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/exchangeRates/exchangeRatesListAdd', component: ExchangeRatesListAddComponent, canActivate: [AppRouteGuard] },

                    // Cota TVA
                    { path: 'conta/nomenclatoare/cotaTVA/cotaTVAList', component: CotaTVAListComponent, canActivate: [AppRouteGuard]},
                    { path: 'conta/nomenclatoare/cotaTVA/cotaTVAEdit', component: CotaTVAEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/nomenclatoare/cotaTVA/cotaTVAEdit', component: CotaTVAEditComponent, canActivate: [AppRouteGuard] },

                    //BNR
                    { path: 'conta/bnrSector/bnrSectorList', component: BnrSectorListComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/bnrSector/bnrSectorAdd', component: BnrSectorAddComponent, canActivate: [AppRouteGuard] },

                    // Sectoare Bnr                     
                    { path: 'sectoareBnr/anexaBnr', component: AnexaBnrComponent, canActivate: [AppRouteGuard] },
                    { path: 'sectoareBnr/calculBnr', component: CalculBnrComponent, canActivate: [AppRouteGuard] },
                    { path: 'sectoareBnr/raportareBNR', component: RaportareBNRComponent, canActivate: [AppRouteGuard] },
                    { path: 'sectoareBnr/detaliiBNR', component: DetaliiBNRComponent, canActivate: [AppRouteGuard] },

                    //Lichiditate
                    { path: 'conta/lichiditate/lichidConfig', component: LichidConfigComponent, canActivate: [AppRouteGuard] },
                    { path: 'conta/lichiditate/lichidCalcul', component: LichidCalculComponent, canActivate: [AppRouteGuard] },
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
