import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AbpHttpInterceptor } from 'abp-ng2-module';

import * as ApiServiceProxies from './service-proxies';

@NgModule({
    providers: [
        ApiServiceProxies.RoleServiceProxy,
        ApiServiceProxies.SessionServiceProxy,
        ApiServiceProxies.TenantServiceProxy,
        ApiServiceProxies.UserServiceProxy,
        ApiServiceProxies.TokenAuthServiceProxy,
        ApiServiceProxies.AccountServiceProxy,
        ApiServiceProxies.ConfigurationServiceProxy,
        ApiServiceProxies.DocumentTypeServiceProxy,
        ApiServiceProxies.CurrencyServiceProxy,
        ApiServiceProxies.PersonServiceProxy,
        ApiServiceProxies.EnumServiceProxy,
        ApiServiceProxies.AccountRelationServiceProxy,
        ApiServiceProxies.IssuerServiceProxy,
        ApiServiceProxies.OperationServiceProxy,
        ApiServiceProxies.BalanceServiceProxy,
        ApiServiceProxies.SavedBalanceServiceProxy,
        ApiServiceProxies.ContractsServiceProxy,
        ApiServiceProxies.InvoiceServiceProxy,
        ApiServiceProxies.PaymentOrdersServiceProxy,
        ApiServiceProxies.ImoAssetServiceProxy,
        ApiServiceProxies.ImoAssetSetupServiceProxy,
        ApiServiceProxies.ImoAssetOperDocTypeServiceProxy,
        ApiServiceProxies.ImoAssetStorageServiceProxy,
        ApiServiceProxies.ImoAssetClassCodeServiceProxy,
        ApiServiceProxies.ImoAssetOperServiceProxy,
        ApiServiceProxies.ImoGestServiceProxy,
        ApiServiceProxies.PrepaymentsServiceProxy,
        ApiServiceProxies.PrepaymentsOperDocTypeServiceProxy,
        ApiServiceProxies.PrepaymentBalanceServiceProxy,
        ApiServiceProxies.PrepaymentsReportingServiceProxy,
        ApiServiceProxies.AccountConfigServiceProxy,
        ApiServiceProxies.AutoOperationConfigServiceProxy,
        ApiServiceProxies.AutoOperationServiceProxy,
        ApiServiceProxies.InvoiceAutoOperationServiceProxy,
        ApiServiceProxies.SitFinanConfigServiceProxy,
        ApiServiceProxies.SitFinanCalcServiceProxy,
        ApiServiceProxies.ActivityTypeServiceProxy,
        ApiServiceProxies.ReportsServiceProxy,
        ApiServiceProxies.OperationTypesServiceProxy,
        ApiServiceProxies.ReportsServiceProxy,
        ApiServiceProxies.DispositionServiceProxy,
        ApiServiceProxies.DepositServiceProxy,
        ApiServiceProxies.SoldInitialServiceProxy,
        ApiServiceProxies.OperGenerateServiceServiceProxy,
        ApiServiceProxies.InvObjectServiceProxy,
        ApiServiceProxies.InvObjectStorageServiceProxy,
        ApiServiceProxies.InvObjectCategoryServiceProxy,
        ApiServiceProxies.InvObjectOperDocTypeServiceProxy,
        ApiServiceProxies.InvObjectGestServiceProxy,
        ApiServiceProxies.InvObjectOperServiceProxy,
        ApiServiceProxies.DecontServiceProxy,
        ApiServiceProxies.ImoInventariereServiceProxy,
        ApiServiceProxies.InvObjectInventariereServiceProxy,
        ApiServiceProxies.ForeignOperationServiceProxy,
        ApiServiceProxies.ForeignOperationDictionaryServiceProxy,
        ApiServiceProxies.DiurnaServiceProxy,
        ApiServiceProxies.ImprumuturiTermenServiceProxy,
        ApiServiceProxies.ImprumuturiTipuriServiceProxy,
        ApiServiceProxies.ImprumutServiceProxy,
        ApiServiceProxies.GarantieServiceProxy,
        ApiServiceProxies.RataServiceProxy,
        ApiServiceProxies.ReportConfigServiceProxy,
        ApiServiceProxies.RegInventarExceptiiServiceProxy,
        ApiServiceProxies.RegInventarExceptiiEliminareServiceProxy,
        ApiServiceProxies.RegInventarServiceProxy,
        ApiServiceProxies.ExchangeServiceProxy,
        ApiServiceProxies.DobanziReferintaServiceProxy,
        ApiServiceProxies.DateDobanziReferintaServiceProxy,
        ApiServiceProxies.GarantieTipServiceProxy,
        ApiServiceProxies.GarantieCeGaranteazaServiceProxy,
        ApiServiceProxies.CupiuriServiceProxy,
        ApiServiceProxies.ComisioaneServiceProxy,
        ApiServiceProxies.DataComisionServiceProxy,
        ApiServiceProxies.PaapServiceProxy,
        ApiServiceProxies.DepartamentServiceProxy,
        ApiServiceProxies.PaapReferatServiceProxy,
        ApiServiceProxies.ExchangeRateForecastServiceProxy,
        ApiServiceProxies.CotaTVAServiceProxy,
        ApiServiceProxies.SitFinanRapServiceProxy,
        ApiServiceProxies.BugetConfigServiceProxy,
        ApiServiceProxies.BugetPrevServiceProxy,
        ApiServiceProxies.BugetPrevAutoServiceProxy,
        ApiServiceProxies.BugetPrevContribServiceProxy,
        ApiServiceProxies.BNR_AnexaDetailServiceProxy,
        ApiServiceProxies.BNR_SectorServiceProxy,
        ApiServiceProxies.BNR_AnexaServiceProxy,
        ApiServiceProxies.BNR_SectorCalcServiceProxy,
        ApiServiceProxies.BugetPrevDobandaRefServiceProxy,
        ApiServiceProxies.SavedBalanceServiceProxy,
        ApiServiceProxies.BugetRealizatServiceProxy,
        ApiServiceProxies.BugetVenituriServiceProxy,
        ApiServiceProxies.BNR_RaportareServiceProxy,
        ApiServiceProxies.BugetRepartizatServiceProxy,
        ApiServiceProxies.TragereServiceProxy,
        ApiServiceProxies.FileDocExternServiceProxy,
        ApiServiceProxies.OperationDefinitionServiceProxy,
        ApiServiceProxies.LichidConfigServiceProxy,
        ApiServiceProxies.LichidCalcServiceProxy,
        ApiServiceProxies.LichidCalcCurrServiceProxy,
        ApiServiceProxies.TranzactiiFonduriServiceProxy,
        ApiServiceProxies.BugetCheltuieliServiceProxy,
        ApiServiceProxies.PaapRedistribuireServiceProxy,
        ApiServiceProxies.NotificareServiceProxy,
        ApiServiceProxies.OperatieComisionDobandaServiceProxy,
        { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true }
    ]
})
export class ServiceProxyModule { }
