import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxPaginationModule } from 'ngx-pagination';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';
import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { AccordionModule } from 'ngx-bootstrap/accordion';

// tenants
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
// roles
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
// users
import { UsersComponent } from '@app/users/users.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
import { FiledocSearchComponent } from './shared/modal/filedoc-search/filedoc-search.component';
// layout
import { HeaderComponent } from './layout/header.component';
import { HeaderLeftNavbarComponent } from './layout/header-left-navbar.component';
import { HeaderLanguageMenuComponent } from './layout/header-language-menu.component';
import { HeaderUserMenuComponent } from './layout/header-user-menu.component';
import { FooterComponent } from './layout/footer.component';
import { SidebarComponent } from './layout/sidebar.component';
import { SidebarLogoComponent } from './layout/sidebar-logo.component';
import { SidebarUserPanelComponent } from './layout/sidebar-user-panel.component';
import { SidebarMenuComponent } from './layout/sidebar-menu.component';

//Conta -> Nomenclatoare
import { OperationsListComponent } from './conta/operations/operationsList.component';
import { OperationEditComponent } from './conta/operations/operationEdit.component';
import { OperationDefinitionComponent } from './conta/operations/operationsDefinition/operationDefinition.component';
import { ForeignOperationComponent } from './conta/operations/foreignOperation.component';
import { DictionaryComponent } from './conta/operations/dictionary.component';
import { DictionaryEditComponent } from './conta/operations/dictionaryEdit.component';
import { SaveAsDialogComponent } from './conta/operations/operationsDefinition/saveAs-dialog/saveAs-dialog.component';
import { OperationDefinitionListDialogComponent } from './conta/operations/operationsDefinition/saveAs-dialog/operationDefinitionList-dialog.component';

//Balance
import { BalanceListComponent } from './conta/balance/balanceList.component';
import { InventoryBalanceListComponent } from './conta/balance/inventory/inventoryBalanceList.component';
import { SavedBalanceListComponent } from './conta/balance/savedBalanceList.component';

import { UploadComponent } from './conta/operations/upload.component';
import { DocTypeComponent } from './conta/nomenclatoare/document/docType.component';
import { DocTypeListEditComponent } from './conta/nomenclatoare/document/docTypeListEdit.component';
import { AccountListComponent } from './conta/nomenclatoare/accounts/accountList.component';
import { AccountEditComponent } from './conta/nomenclatoare/accounts/accountEdit.component';
import { AccountRelationListComponent } from './conta/nomenclatoare/accounts/accountRelationList.component';
import { AccountRelationEditComponent } from './conta/nomenclatoare/accounts/accountRelationEdit.component';
import { AccountDeductibilityComponent } from './conta/nomenclatoare/accounts/accountDeductibility.component';
import { AccountHistoryDialogComponent } from './conta/nomenclatoare/accounts/accountHistory-dialog/accountHistory-dialog.component';
import { PersonComponent } from './setup/person/person.component';
import { PersonEditComponent } from './setup/person/personEdit.component';
import { ThirdPartyAccComponent } from './setup/banks/thirdPartyAcc.component';
import { ThirdPartyAccEditComponent } from './setup/banks/thirdPartyAccEdit.component';
import { IssuerComponent } from './issuer/issuer.component';
import { IssuerEditComponent } from './issuer/issuerEdit.component';
import { IssuerAddComponent } from './issuer/issuerAdd.component';

// Economic
//Contracte
import { ContractsListComponent } from './economic/contracts/contractsList.component';
import { ContractNewComponent } from './economic/contracts/contractNew.component';
import { ContractStateDialogComponent } from './economic/contract-dialog/contract-state-dialog.component';
import { ContractStateListDialogComponent } from './economic/contract-dialog/contract-state-list-dialog.component';

import { ContractCategoryListComponent } from './economic/contractCategory/contractCategoryList.component';
import { ContractCategoryEditComponent } from './economic/contractCategory/contractCategoryEdit.component';
// Elemente
import { DetailElementsListComponent } from './conta/nomenclatoare/categoryElements/detailElementsList.component';
import { DetailElementsNewComponent } from './conta/nomenclatoare/categoryElements/detailElementsNew.component';
// Facturi
import { InvoicesListComponent } from './economic/invoices/invoicesList.component';
import { InvoiceNewComponent } from './economic/invoices/invoiceNew.component';
import { InvoiceImportComponent } from './economic/invoices/invoiceImport.component';
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
import { ImoInventariereComponent } from './conta/imoAsset/imoInventariere.component';
import { ImoInventariereEditComponent } from './conta/imoAsset/imoInventariereEdit.component';
import { ReportingImoAssetComponent } from './conta/imoAsset/reporting.component';
import { PrepaymentsListComponent } from './conta/prepayments/prepaymentsList.component';
import { PrepaymentsInitComponent } from './conta/prepayments/prepaymentsInit.component';
import { PrepaymentsAddComponent } from './conta/prepayments/prepaymentsAdd.component';
import { PrepaymentsAddInvoiceComponent } from './conta/prepayments/prepaymentsAddInvoice.component';
import { PrepaymentsOperDocTypeComponent } from './conta/prepayments/prepaymentsOperDocType.component';
import { PrepaymentsOperDocTypeEditComponent } from './conta/prepayments/prepaymentsOperDocTypeEdit.component';
import { PrepaymentsAddDirectComponent } from './conta/prepayments/prepaymentsAddDirect.component';
import { PrepaymentsExitComponent } from './conta/prepayments/prepaymentsExit.component';
import { PrepaymentsGestListComponent } from './conta/prepayments/prepaymentsGestList.component';
import { PrepaymentsGestInitComponent } from './conta/prepayments/prepaymentsGestInit.component';
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
import { AccountConfigComponent } from './conta/nomenclatoare/accounts/accountConfig.component';
import { AccountConfigEditComponent } from './conta/nomenclatoare/accounts/accountConfigEdit.component';
import { AutoOperationConfigComponent } from './conta/autoOperation/autoOperationConfig.component';
import { AutoInvoicesComponent } from './conta/autoOperation/autoInvoices.component';
import { AutoOperationsComponent } from './conta/autoOperation/autoOperations.component';
import { SitFinanConfigComponent } from './conta/sitFinan/sitFinanConfig.component';
import { SitFinanConfigReportComponent } from './conta/sitFinan/sitFinanConfigReport.component';
import { SitFinanConfigFormulaComponent } from './conta/sitFinan/sitFinanConfigFormula.component';
import { SitFinanConfigNoteComponent } from './conta/sitFinan/sitFinanConfigNote.component';
import { SitFinanCalcComponent } from './conta/sitFinan/sitFinanCalc.component';
import { SitFinanCalcReportComponent } from './conta/sitFinan/sitFinanCalcReport.component';
import { SitFinanRapComponent } from './conta/sitFinan/sitFinanRap.component';
import { SitFinanConfigFluxComponent } from './conta/sitFinan/SitFinanConfigFlux.component';
import { SitFinanCalcDetailsDialogComponent } from './conta/sitFinan/sitFinanCalc-dialog/sitFinanCalcDetailsDialog.component';

import { ActivityTypeComponent } from './nomenclatoare/activity/activityType.component';
import { ActivityTypeNewComponent } from './nomenclatoare/activity/activityTypeNew.component';

//TranzactiiFonduri

import { TrazactiiListComponent } from './conta/operations/TranzactiiFonduri/tranzactiiList.component'

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
import { InvObjectNomCategoryComponent } from './conta/invObjects/invObjectNomCategory.component';
import { InvObjectNomCategoryEditComponent } from './conta/invObjects/invObjectNomCategoryEdit.component';
import { InvObjectNomOperDocTypeComponent } from './conta/invObjects/invObjectNomOperDocType.component';
import { InvObjectNomOperDocTypeEditComponent } from './conta/invObjects/invObjectNomOperDocTypeEdit.component';
import { InvObjectNomStorageEditComponent } from './conta/invObjects/invObjectNomStorageEdit.component';
import { InvObjectsGestDeleteComponent } from './conta/invObjects/invObjectsGestDelete.component';
import { InvObjectGestComputeComponent } from './conta/invObjects/invObjectGestCompute.component';
import { InvObjectInventariereComponent } from './conta/invObjects/invObjectInventariere.component';
import { InvObjectInventariereEditComponent } from './conta/invObjects/invObjectInventariereEdit.component';
import { InvObjectReportComponent } from './conta/invObjects/invObjectReport.component';
import { ReportingInvObjectComponent } from './conta/invObjects/reporting.component';

import { appDxWebWiewComponent } from './reporting/appDxWebWiew.component';
//reporting

import { DxReportViewerModule, DxReportDesignerModule } from 'devexpress-reporting-angular';

import { ConfigReportingComponent } from './reporting/configReporting.component';
import {
    DxDataGridModule,
    DxBulletModule,
    DxTemplateModule,
} from 'devextreme-angular';
//import { ReportViewerComponent } from './reportviewer/report-viewer';
//import { ReportDesignerComponent } from './reportdesigner/report-designer';

import { OperationTypeComponent } from './conta/nomenclatoare/operationsType/operationType.component';
import { OperationTypeEditComponent } from './conta/nomenclatoare/operationsType/operationTypeEdit.component';
import { ReportBalanceComponent } from './reporting/reportBalance.component';
import { RepAccountSheetComponent } from './reporting/repAccountSheet.component';
import { CategoryDetailElementsListComponent } from './conta/nomenclatoare/categoryElements/categoryDetailElementsList.component';
import { CategoryDetailElementsNewComponent } from './conta/nomenclatoare/categoryElements/categoryDetailElementsNew.component';
import { DispositionListComponent } from './economic/dispositions/dispositionList.component';
import { DispositionNewComponent } from './economic/dispositions/dispositionNew.component';
import { DepositListComponent } from './economic/dispositions/depositList.component';
import { DepositNewComponent } from './economic/dispositions/depositNew.component';
import { SoldListComponent } from './economic/dispositions/soldList.component';
import { SoldNewComponent } from './economic/dispositions/soldNew.component';
import { RegistruCasaComponent } from './reporting/registruCasa.component';
import { OperGenerateListComponent } from './conta/balance/operGenerate/operGenerateList.component';
import { OperGenerateAddComponent } from './conta/balance/operGenerate/operGenerateAdd.component';
import { ImoAssetAddInUseComponent } from './conta/imoAsset/imoAssetAddInUse.component';

// Decont
import { DecontListComponent } from './decont/decontList.component';
import { DecontAddComponent } from './decont/decontAdd.component';
import { MatDatepickerModule, MatGridListModule, MatPaginatorModule, MatProgressSpinnerModule } from '@angular/material';
import { ImoAssetReportingComponent } from './conta/imoAsset/imoAssetReporting.component';
import { DiurnaListComponent } from './conta/nomenclatoare/diurna/diurnaList.component';
import { DiurnaEditComponent } from './conta/nomenclatoare/diurna/diurnaEdit.component';
import { DiurnaPerZiListComponent } from './conta/nomenclatoare/diurna/diurnaPerZiList.component';
import { DiurnaPerZiEditComponent } from './conta/nomenclatoare/diurna/diurnaPerZiEdit.component';
import { DeclaratieCasierComponent } from './reporting/declaratieCasier.component';
// Imprumuturi

//Imprumuturi
import { ImprumuturiTermenComponent } from './imprumuturi/imprumuturiTermen/imprumuturiTermen.component';
import { ImprumuturiTermenEditComponent } from './imprumuturi/imprumuturiTermen/imprumuturiTermenEdit.component';
import { ImprumuturiNomenclatoareComponent } from './imprumuturi/imprumuturiNomenclatoare/imprumuturiNomenclatoare.component';
import { ImprumuturiTipuriComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipuri.component';
import { ImprumuturiTipuriEditComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipuriEdit.component';
import { ImprumuturiListComponent } from './imprumuturi/imprumuturi/imprumuturiList.component';
import { ImprumuturiEditComponent } from './imprumuturi/imprumuturi/imprumuturiEdit.component';
import { ImprumuturiTipDetaliuListComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/imprumuturiTipDetaliuList.component';
import { ImprumuturiTipDetaliuEditComponent } from './imprumuturi/imprumuturiTipuri/imprumuturiTipDetaliu/imprumuturiTipDetaliuEdit.component';

//Garantii
import { GarantiiListComponent } from './imprumuturi/garantie/garantiiList.component';
import { GarantiiEditComponent } from './imprumuturi/garantie/garantiiEdit.component';
import { GarantieNomenclatoareComponent } from './imprumuturi/garantieNomenclatoare/garantieNomenclatoare.component';
import { GarantieTipComponent } from './imprumuturi/garantieNomenclatoare/garantieTip/garantieTip.component';
import { GarantieTipEditComponent } from './imprumuturi/garantieNomenclatoare/garantieTip/garantieTipEdit.component';
import { GarantieCeGaranteazaComponent } from './imprumuturi/garantieNomenclatoare/GarantieCeGaranteaza/GarantieCeGaranteaza.component';
import { GarantieCeGaranteazaEditComponent } from './imprumuturi/garantieNomenclatoare/GarantieCeGaranteaza/GarantieCeGaranteazaEdit.component';
import { OperatieGarantieComponent } from './imprumuturi/OperatieGarantie/OperatieGarantie.component'

//Rate
import { RateListComponent } from './imprumuturi/rate/rateList.component';
import { RateListDialogComponent } from './imprumuturi/rate/rateList-dialog.component';

//Comisioane

import { ComisioaneListComponent } from './imprumuturi/comisioane/comisioaneList.component';
import { ComisionV2ListComponent } from './imprumuturi/comisionV2/comisionV2List.component';
import { ComisionV2EditComponent } from './imprumuturi/comisionV2/comisionV2Edit.component';
import { ComisioaneEditComponent } from './imprumuturi/comisioane/comisioaneEdit.component';

//DateComision
import { DateComisionListComponent } from './imprumuturi/dateComision/dateComisionList.component';

//CalculatorDobanda
import { CalculatorDobandaComponent } from './imprumuturi/calculatorDobanda/calculatorDobanda.component';

//Dobanda de referinta

import { DobanziReferintaComponent } from './imprumuturi/dobandaDeReferinta/dobanziReferinta.component';
import { DobanziReferintaEditComponent } from './imprumuturi/dobandaDeReferinta/dobanziReferintaEdit.component';
import { DateDobanziReferintaComponent } from './imprumuturi/dateDobandaDeReferinta/dateDobanziReferinta.component';
import { DateDobanziReferintaEditComponent } from './imprumuturi/dateDobandaDeReferinta/dateDobanziReferintaEdit.component';

// Configurare rapoarte
import { ReportConfigListComponent } from './conta/nomenclatoare/configRapoarte/reportConfigList.component';
import { ReportConfigEditComponent } from './conta/nomenclatoare/configRapoarte/reportConfigEdit.component';
import { RapConfigComponent } from './conta/nomenclatoare/configRapoarte/rapConfig.component';
import { RepConfigFormulaComponent } from './conta/nomenclatoare/configRapoarte/repConfigFormula.component';

// Istoric Imprumut (state)
import { ImprumuturiStateComponent } from './imprumuturi/imprumutState/imprumuturiState.component';

// Operatie Comision Dobanda
import { OperatieComisionDobandaComponent } from './imprumuturi/OperatiiComisionDobanda/operatieComisionDobanda.component';

// Nomenclatoare
// person

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
import { ShowCancelPAAPDialogComponent } from './buget/paap/cancel-paap/cancel-paap-dialog.component';
import { ShowAmanarePAAPDialogComponent } from './buget/paap/amanare-paap/amanare-paap-dialog.component';
import { PaapAlocareCheltuieliComponent } from './buget/paap/paapAlocareCheltuieli.component';
import { PaapHistoryListComponent } from './buget/paap/paapHistoryList.component';
import { AddReferatDialogComponent } from './buget/referat/add-referat-dialog.component';
import { ExchangeRateForecastComponent } from './buget/paap/exchangeRateForecast.component';
import { ExchangeRateForecastAddComponent } from './buget/paap/exchangeRateForecastAdd.component';
import { BugetConfigListComponent } from './buget/bugetConfig/BugetConfigList.component';
import { BugetConfigEditComponent } from './buget/bugetConfig/BugetConfigEdit.component';
import { BugetConfigDuplicateDialogComponent } from './buget/bugetConfig/bugetConfigDuplicate-dialog/bugetConfig-duplicate-dialog.component';
import { BugetParametriiComponent } from './buget/parametrii/BugetParametrii.component';
import { BugetRandListComponent } from './buget/bugetConfig/BugetRandList.component';
import { BugetPrevListComponent } from './buget/prevazut/bugetPrevList.component';
import { BugetPrevAddComponent } from './buget/prevazut/bugetPrevAdd.component';
import { BugetPrevDetailsComponent } from './buget/prevazut/bugetPrevDetails.component';
import { BugetCheltuieliEditComponent } from './buget/cheltuieli/bugetCheltuieliEdit.component';
import { BugetCheltuieliComponent } from './buget/cheltuieli/bugetCheltuieli.component';
import { BugetPrevAutoValueListComponent } from './buget/prevazut/bugetPrevAutoValueList.component';
import { BugetPrevAutoValueAddComponent } from './buget/prevazut/bugetPrevAutoValueAdd.component';
import { BugetPrevStateDialogComponent } from './buget/prevazut/bugetPrevState-dialog/bugetPrev-state-dialog.component';
import { BugetPrevContribListComponent } from './buget/prevazut/bugetPrevContribList.component';
import { BugetPrevContribAddComponent } from './buget/prevazut/bugetPrevContribAdd.component';
import { DobandaReferintaListComponent } from './buget/prevazut/dobandaReferinta/dobandaReferintaList.component';
import { DobandaReferintaAddComponent } from './buget/prevazut/dobandaReferinta/dobandaReferintaAdd.component';
import { BugetPrevDuplicateDialogComponent } from './buget/prevazut/bugetPrevDuplicate-dialog/bugetPrev-duplicate-dialog.component';
import { BugetRealizatListComponent } from './buget/realizat/bugetRealizatList.component';
import { BugetRealizatAddComponent } from './buget/realizat/bugetRealizatAdd.component';
import { BugetRealizatRandListComponent } from './buget/realizat/bugetRealizatRandList.component';
import { BugetRealizatRowDetailDialogComponent } from './buget/realizat/realizatRowDetail-dialog/bugetRealizatRowDetailDialog.component';
import { BugetRealizatBalRowDetailDialogComponent } from './buget/realizat/BalRealizat/realizatBalRowDetail-dialog/bugetRealizatBalRowDetailDialog.component';
import { BugetVenitListComponent } from './buget/venituri/bugetVenitList.component';
import { BugetVenitAddComponent } from './buget/venituri/bugetVenitAdd.component';
import { BugetVenitDetailsComponent } from './buget/venituri/bugetVenitDetails.component';
import { BugetVenituriBVCDetailsDialogComponent } from './buget/venituri/bugetVenituriBVC-dialog/bugetVenituriBVCDetailsDialog.component';
import { BugetVenituriCFDetailsDialogComponent } from './buget/venituri/bugetVenituriCF-dialog/bugetVenituriCFDetailsDialog.component';
import { BugetReinvDialogComponent } from './buget/venituri/bugetReinvest-dialog/bugetReinvDialog.component';
import { TabelReinvDialogComponent } from './buget/venituri/TabelReinvest-dialog/TabelReinvestDialog.component';
import { BugetReinvestComponent } from './buget/venituri/bugetReinvest.component';
import { BugetAplicaComponent } from './buget/venituri/bugetAplica.component';
import { BugetRepartizatComponent } from './buget/venituri/bugetRepartizat.component';
import { ReportBVCComponent } from './reporting/reportBVC.component';
import { BugetRealizatBalantaListComponent } from './buget/realizat/BalRealizat/bugetRealizatBalantaList.component'
import { BugetPrevResurseDialogComponent } from './buget/prevazut/bugetPrevResurse-dialog/bugetPrevResurseDialog.component';
import { PaapAlocareFacturiComponent } from './buget/paap/paapAlocareFacturi.component';
import { RedistribuirePaapListComponent } from './buget/paap/redistribuire/redistribuirePaapList.component';
import { RedistribuirePaapAddComponent } from './buget/paap/redistribuire/redistribuirePaapAdd.component';
import { RedistribuirePaapDetailsDialogComponent } from './buget/paap/redistribuire/redistribuirePaapDetails-dialog.component';

// Curs valutar
import { ExchangeRatesListComponent } from './conta/nomenclatoare/exchangeRates/exchangeRatesList.component';
import { ExchangeRatesListAddComponent } from './conta/nomenclatoare/exchangeRates/exchangeRatesListAdd.component';

// Cota TVA
import { CotaTVAListComponent } from './conta/nomenclatoare/cotaTVA/cotaTVAList.component';
import { CotaTVAEditComponent } from './conta/nomenclatoare/cotaTVA/cotaTVAEdit.component';

import { SoldDbDialogComponent } from './conta/operations/sold-dialog/soldDb-dialog.component';

// Sectoare Bnr
//Anexa Bnr
import { AnexaBnrComponent } from './sectoareBnr/anexaBnr.component';
import { CalculBnrComponent } from './sectoareBnr/calculBnr.component';
import { CalculBnrDetailDialogComponent } from './sectoareBnr/calculBnrDetail-dialog.component';
import { RaportareBNRComponent } from './sectoareBnr/raportareBNR.component';
import { DetaliiBNRComponent } from './sectoareBnr/detaliiBNR.component';
import { RaportareBNRDetailDialogComponent } from './sectoareBnr/raportareBNR-dialog/raportareBNRDetail-dialog.component';

// Sectorizare BNR
import { BnrSectorListComponent } from './conta/bnrSector/bnrSectorList.component';
import { BnrSectorAddComponent } from './conta/bnrSector/bnrSectorAdd.component';

// Lichiditate
import { LichidConfigComponent } from './conta/lichiditate/lichidConfig.component';
import { LichidCalculComponent } from './conta/lichiditate/lichidCalcul.component';
import { LichidCalcDetDialogComponent } from './conta/lichiditate/lichidCalcDet-dialog/lichidCalcDet-dialog.component';
import { LichidCalcCurrDetDialogComponent } from './conta/lichiditate/lichidCalcCurrDet-dialog/lichidCalcCurrDet-dialog.component';
import { FileDocAttachmentComponent } from './shared/filedoc/file-doc-attachment.component';
import { SearchInvoiceComponent } from './economic/invoices/searchInvoice.component';
import { LichidCalcExportComponent } from './conta/lichiditate/lichidCalcExport-dialog/lichidCalcExport-dialog.component';
import { LichidCalcCurrExportComponent } from './conta/lichiditate/lichidCalcCurrExport-dialog/lichidCalcCurrExport-dialog.component';

import { FileDocErrorsComponent } from './setup/fileDoc/file-doc-errors.component';
import { InvoiceDetailsReportComponent } from './reporting/invoiceDetailsReport.component';
import { ShowRelocarePAAPDialogComponent } from './buget/paap/realocare/realocare-paap-dialog.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AboutComponent,
        // tenants
        TenantsComponent,
        CreateTenantDialogComponent,
        EditTenantDialogComponent,
        // roles
        RolesComponent,
        CreateRoleDialogComponent,
        EditRoleDialogComponent,
        // users
        UsersComponent,
        CreateUserDialogComponent,
        EditUserDialogComponent,
        ChangePasswordComponent,
        ResetPasswordDialogComponent,
        // layout
        HeaderComponent,
        HeaderLeftNavbarComponent,
        HeaderLanguageMenuComponent,
        HeaderUserMenuComponent,
        FooterComponent,
        SidebarComponent,
        SidebarLogoComponent,
        SidebarUserPanelComponent,
        SidebarMenuComponent,
        // Economic-> Contracts
        ContractStateListDialogComponent,
        ContractsListComponent,
        ContractNewComponent,
        ContractCategoryListComponent,
        ContractCategoryEditComponent,
        ContractStateDialogComponent,

        //Elemente
        DetailElementsListComponent,
        DetailElementsNewComponent,
        CategoryDetailElementsListComponent,
        CategoryDetailElementsNewComponent,

        //Operatiuni cu numerar
        DispositionListComponent,
        DispositionNewComponent,
        DepositListComponent,
        DepositNewComponent,
        SoldListComponent,
        SoldNewComponent,
        RegistruCasaComponent,
        DeclaratieCasierComponent,

        //Facturi
        InvoicesListComponent,
        InvoiceNewComponent,
        InvoiceImportComponent,
        // paymentOrders
        PaymentOrdersComponent,
        ExportCSVComponent,
        SearchInvoiceComponent,

        //Issuer
        IssuerComponent,
        IssuerAddComponent,
        IssuerEditComponent,
        //Conta -> Nomenclatoare
        //Operations
        OperationsListComponent,
        OperationEditComponent,
        ForeignOperationComponent,
        DictionaryComponent,
        DictionaryEditComponent,
        OperationDefinitionComponent,
        SaveAsDialogComponent,
        OperationDefinitionListDialogComponent,

        AutoInvoicesComponent,
        AutoOperationsComponent,
        UploadComponent,

        // Balance
        BalanceListComponent,
        SavedBalanceListComponent,
        OperGenerateListComponent,
        OperGenerateAddComponent,
        InventoryBalanceListComponent,

        //DocumentType
        DocTypeComponent,
        DocTypeListEditComponent,

        //Operation Type
        OperationTypeComponent,
        OperationTypeEditComponent,

        // ImoAsset
        ImoNomenclatoareComponent,
        ImoSetupComponent,
        ImoAssetOperDocTypeComponent,
        ImoAssetOperDocTypeEditComponent,
        ImoNomStorageComponent,
        ImoNomStorageEditComponent,
        ImoNomClassCodeComponent,
        ImoNomClassCodeEditComponent,
        ImoAssetPVComponent,
        ImoAssetPVAddComponent,
        ImoAssetPVAddInvoiceComponent,
        ImoAssetPVAddDirectComponent,
        ImoAssetOperationComponent,
        ImoAssetOperationEditComponent,
        ImoAssetOperationViewComponent,
        ImoGestListComponent,
        ImoGestComputeComponent,
        ImoGestDeleteComponent,
        ImoAssetAddInUseComponent,
        ReportingImoAssetComponent,
        ImoInventariereComponent,
        ImoInventariereEditComponent,
        ImoAssetReportingComponent,
        ReportingInvObjectComponent,

        //Obiecte de inventar
        InvObjectComponent,
        InvObjectAddComponent,
        InvObjectAddDirectComponent,
        InvObjectAddInvoiceComponent,
        InvObjectOperationComponent,
        InvObjectOperationEditComponent,
        InvObjectNomenclatoareComponent,
        InvObjectGestListComponent,
        InvObjectNomStorageComponent,
        InvObjectNomStorageEditComponent,
        InvObjectNomCategoryComponent,
        InvObjectNomCategoryEditComponent,
        InvObjectNomOperDocTypeComponent,
        InvObjectNomOperDocTypeEditComponent,
        InvObjectGestComputeComponent,
        InvObjectsGestDeleteComponent,
        InvObjectInventariereComponent,
        InvObjectInventariereEditComponent,
        InvObjectReportComponent,

        DiurnaListComponent,
        DiurnaEditComponent,
        DiurnaPerZiListComponent,
        DiurnaPerZiEditComponent,

        //Conta -> situatii financiare
        SitFinanConfigComponent,
        SitFinanConfigReportComponent,
        SitFinanConfigFormulaComponent,
        SitFinanConfigNoteComponent,
        SitFinanCalcComponent,
        SitFinanCalcReportComponent,
        SitFinanRapComponent,
        SitFinanConfigFluxComponent,
        SitFinanCalcDetailsDialogComponent,

        // Decont
        DecontListComponent,
        DecontAddComponent,

        //Prepayments
        PrepaymentsInitComponent,
        PrepaymentsListComponent,
        PrepaymentsAddComponent,
        PrepaymentsAddInvoiceComponent,
        PrepaymentsAddDirectComponent,
        PrepaymentsOperDocTypeComponent,
        PrepaymentsOperDocTypeEditComponent,
        PrepaymentsExitComponent,
        PrepaymentsGestInitComponent,
        PrepaymentsGestListComponent,
        PrepaymentsGestComputeComponent,
        PrepaymentsGestDeleteComponent,
        PrepaymentsSetupInitComponent,
        PrepaymentsSetupComponent,
        PrepaymentsReportingComponent,
        ReportingComponent,
        PreincomesInitComponent,
        PreincomesGestInitComponent,
        PreincomesSetupInitComponent,
        PreincomesReportingComponent,

        //Reporting
        appDxWebWiewComponent,
        ReportBalanceComponent,
        RepAccountSheetComponent,
        ConfigReportingComponent,
        InvoiceDetailsReportComponent,

        //Accounts
        AccountListComponent,
        AccountEditComponent,
        AccountRelationListComponent,
        AccountRelationEditComponent,
        AccountDeductibilityComponent,
        AccountConfigComponent,
        AccountConfigEditComponent,
        AccountHistoryDialogComponent,

        //AutoOperationConfigComponent
        AutoOperationConfigComponent,

        //nomenclatoare
        PersonListComponent,
        PersonNewComponent,
        ThirdPartyAccountComponent,
        ThirdPartyAccountEditComponent,
        ActivityTypeComponent,
        ActivityTypeNewComponent,

        //Setup
        PersonComponent,
        PersonEditComponent,
        ThirdPartyAccComponent,
        ThirdPartyAccEditComponent,

        // Imprumuturi
        ImprumuturiNomenclatoareComponent,
        ImprumuturiTermenComponent,
        ImprumuturiTermenEditComponent,
        ImprumuturiTipuriComponent,
        ImprumuturiTipuriEditComponent,
        ImprumuturiListComponent,
        ImprumuturiEditComponent,
        ImprumuturiTipDetaliuListComponent,
        ImprumuturiTipDetaliuEditComponent,

        // Istoric Imprumut
        ImprumuturiStateComponent,

        // Calculator Dobanda
        CalculatorDobandaComponent,

        // Garantii
        GarantiiListComponent,
        GarantiiEditComponent,
        GarantieNomenclatoareComponent,
        GarantieTipComponent,
        GarantieTipEditComponent,
        GarantieCeGaranteazaComponent,
        GarantieCeGaranteazaEditComponent,
        OperatieGarantieComponent,

        //Rate
        RateListComponent,
        RateListDialogComponent,

        //Comisioane
        ComisioaneListComponent,
        ComisioaneEditComponent,
        ComisionV2ListComponent,
        ComisionV2EditComponent,

        //Operatie Comision Dobanda
        OperatieComisionDobandaComponent,

        // Date Comision

        DateComisionListComponent,

        //Dobonda de Referinta

        DobanziReferintaComponent,
        DobanziReferintaEditComponent,
        DateDobanziReferintaComponent,
        DateDobanziReferintaEditComponent,

        //Configurare rapoarte
        ReportConfigListComponent,
        ReportConfigEditComponent,
        RapConfigComponent,
        RepConfigFormulaComponent,

        //Registru inventar
        ExceptionListComponent,

        //Schimb valutar
        ExchangeListComponent,
        ExchangeEditComponent,

        //Cupiuri
        CupiuriListComponent,
        CupiuriAddComponent,

        // Buget
        PaapListComponent,
        PaapAddComponent,
        PaapValidatorComponent,
        ShowCancelPAAPDialogComponent,
        PaapAlocareCheltuieliComponent,
        PaapHistoryListComponent,
        ShowAmanarePAAPDialogComponent,
        AddReferatDialogComponent,
        ExchangeRateForecastComponent,
        ExchangeRateForecastAddComponent,
        BugetConfigListComponent,
        BugetConfigEditComponent,
        BugetRandListComponent,
        BugetPrevListComponent,
        BugetPrevAddComponent,
        BugetPrevDetailsComponent,
        BugetPrevAutoValueListComponent,
        BugetPrevAutoValueAddComponent,
        BugetPrevStateDialogComponent,
        BugetPrevContribListComponent,
        BugetPrevContribAddComponent,
        DobandaReferintaListComponent,
        DobandaReferintaAddComponent,
        BugetPrevDuplicateDialogComponent,
        BugetRealizatListComponent,
        BugetRealizatAddComponent,
        BugetRealizatRandListComponent,
        BugetRealizatRowDetailDialogComponent,
        BugetVenitListComponent,
        BugetVenitAddComponent,
        BugetVenitDetailsComponent,
        BugetVenituriBVCDetailsDialogComponent,
        BugetVenituriCFDetailsDialogComponent,
        BugetReinvDialogComponent,
        BugetReinvestComponent,
        BugetAplicaComponent,
        BugetRepartizatComponent,
        BugetConfigDuplicateDialogComponent,
        ReportBVCComponent,
        BugetRealizatBalantaListComponent,
        BugetRealizatBalRowDetailDialogComponent,
        BugetCheltuieliComponent,
        BugetCheltuieliEditComponent,
        TabelReinvDialogComponent,
        BugetParametriiComponent,
        BugetPrevResurseDialogComponent,
        PaapAlocareFacturiComponent,
        RedistribuirePaapListComponent,
        RedistribuirePaapAddComponent,
        RedistribuirePaapDetailsDialogComponent,
        ShowRelocarePAAPDialogComponent,

        // Curs valutar
        ExchangeRatesListComponent,
        ExchangeRatesListAddComponent,

        //Cota TVA
        CotaTVAListComponent,
        CotaTVAEditComponent,

        SoldDbDialogComponent,

        // Sectoare Bnr
        // Anexa Bnr
        AnexaBnrComponent,
        CalculBnrComponent,
        CalculBnrDetailDialogComponent,
        RaportareBNRComponent,
        DetaliiBNRComponent,
        RaportareBNRDetailDialogComponent,

        //BNR
        BnrSectorListComponent,
        BnrSectorAddComponent,

        //Modal dialogs
        FiledocSearchComponent,

        //Lichiditate
        LichidConfigComponent,
        LichidCalculComponent,
        LichidCalcDetDialogComponent,
        LichidCalcCurrDetDialogComponent,
        LichidCalcExportComponent,
        LichidCalcCurrExportComponent,

        FileDocAttachmentComponent,

        // Tranzactii Fonduri

        TrazactiiListComponent,

        FileDocErrorsComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        HttpClientJsonpModule,
        ModalModule.forChild(),
        BsDropdownModule,
        CollapseModule,
        TabsModule,
        AppRoutingModule,
        ServiceProxyModule,
        SharedModule,
        NgxPaginationModule,
        AccordionModule.forRoot(),
        BsDatepickerModule.forRoot(),
        DatepickerModule.forRoot(),
        DxDataGridModule,
        DxBulletModule,
        DxTemplateModule,
        DxReportViewerModule,
        DxReportDesignerModule,
        MatPaginatorModule,
        MatProgressSpinnerModule,
        MatDatepickerModule,
        MatGridListModule,
    ],
    providers: [],
    entryComponents: [
        // tenants
        CreateTenantDialogComponent,
        EditTenantDialogComponent,
        // roles
        CreateRoleDialogComponent,
        EditRoleDialogComponent,
        // users
        CreateUserDialogComponent,
        EditUserDialogComponent,
        ResetPasswordDialogComponent,
        //shared modals
        FiledocSearchComponent
    ]
})
export class AppModule { }