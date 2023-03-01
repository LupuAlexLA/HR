using DevExpress.XtraReports.UI;
using Niva.Erp.Web.Host.Reports.Conta;
using Niva.Erp.Web.Host.Reports.Conta.BNR;
using Niva.Erp.Web.Host.Reports.Conta.BugetPreliminat;
using Niva.Erp.Web.Host.Reports.Conta.BugetPrevazut;
using Niva.Erp.Web.Host.Reports.Conta.BVC;
using Niva.Erp.Web.Host.Reports.Conta.Lichiditate;
using Niva.Erp.Web.Host.Reports.Conta.SitFinan;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Web.Host.Reports.Common
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["BalantaRon5Egalitati"] = () => new BalantaRon5Egalitati(),
            ["BalantaRon6Egalitati"] = () => new BalantaRon6Egalitati(),
            ["BalantaValuta6Egalitati"] = () => new BalantaValuta6Egalitati(),
            ["BalantaValuta5Egalitati"] = () => new BalantaValuta5Egalitati(),
            ["RegistruJurnalReport"] = () => new RegistruJurnalReport(),
            ["FisaCont"] = () => new FisaCont(),
            ["RegistruCasaReport"] = () => new RegistruCasaReport(),
            ["RegistruCasaRangeReport"] = () => new RegistruCasaRangeReport(),
            ["PrepaymentsRegReport"] = () => new PrepaymentsRegReport(),
            ["AssetRegReport"] = () => new AssetRegReport(),
            ["AssetFisaReport"] = () => new AssetFisaReport(),
            ["InvObjectAssetReport"] = () => new InvObjectAssetReport(),
            ["BonTransferReport"] = () => new BonTransferReport(),
            ["AssetRegReportV2"] = () => new AssetRegReportV2(),
            ["DispositionReport"] = () => new DispositionReport(),
            ["DeclaratieCasier"] = () => new DeclaratieCasier(),
            ["ConfigReport"] = () => new ConfigReport(),
            ["RegistruInventar"] = () => new RegistruInventar(),
            ["InvoiceReport"] = () => new InvoiceReport(),
            ["SitFinanRapC2"] = () => new SitFinanRapC2(),
            ["SitFinanRapC3"] = () => new SitFinanRapC3(),
            ["SitFinanRapC4"] = () => new SitFinanRapC4(),
            ["SitFinanRapC5"] = () => new SitFinanRapC5(),
            ["SitFinanRapC6"] = () => new SitFinanRapC6(),
            ["InvObjectReport"] = () => new InvObjectReport(),
            ["BonConsumReport"] = () => new BonConsumReport(),
            ["SoldContCurentReport"] = () => new SoldContCurentReport(),
            ["SoldFurnizoriDebitoriReport"] = () => new SoldFurnizoriDebitoriReport(),
            ["BugetPrevAllDepartments"] = () => new BugetPrevAllDepartments(),
            ["BugetPrevDepartment"] = () => new BugetPrevDepartment(),
            ["AnexeReport"] = () => new AnexeReport(),
            ["PrevazutBVC"] = () => new PrevazutBVC(),
            ["BVC_Realizat"] = () => new BVC_Realizat(),
            ["SavedBalanceReport"] = () => new SavedBalanceReport(),
            ["SavedBalanceReportV2"] = () => new SavedBalanceReportV2(),
            ["SitFinanRap"] = () => new SitFinanRap(),
            ["CursValutarBNR"] = () => new CursValutarBNR(),
            ["LichidCalcReport"] = () => new LichidCalcReport(),
            ["LichidCalcCurrReport"] = () => new LichidCalcCurrReport(),
            ["DepozitBancarReport"] = () => new DepozitBancarReport(),
            ["BVC_BalRealizat"] = () => new BVC_BalRealizat(),
            ["BVC_Report"] = () => new BVC_Report(),
            ["BVC_PrevResurse"] = () => new BVC_PrevResurse(),
            ["BVC_BalRealizatIncludPrevAnual"] = () => new BVC_BalRealizatIncludPrevAnual(),
            ["RegistruJurnalExtinsReport"] = () => new RegistruJurnalExtinsReport(),
            ["DetaliuSoldReport"] = ()=> new DetaliuSoldReport(),
            ["BugetPreliminatDetaliiReport"] = ()=> new BugetPreliminatDetaliiReport(),
            ["BugetPreliminatReport"] = ()=> new BugetPreliminatReport()

        };
    }
}