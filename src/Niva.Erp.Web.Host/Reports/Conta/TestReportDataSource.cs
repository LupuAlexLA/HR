using Abp;
using DevExpress.DataAccess.ObjectBinding;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Conta.Balance;
using Niva.Erp.Conta.Prepayments;
using Niva.Erp.Conta.Reports;
using Niva.Erp.Conta.Reports.Dto;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Web.Host.Reports.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Niva.Erp.Web.Host.Reports.Conta
{
    [DisplayName("Employees")]
    [HighlightedClass]
    public class TestReportDataSource
    {
        private IScopedTestReportDataSourceProvider<ReportsAppService> reportManagerProvider { get; set; }

        public TestReportDataSource()
        {
            throw new NotSupportedException();
        }

        public TestReportDataSource(IScopedTestReportDataSourceProvider<ReportsAppService> reportManagerProvider)
        {
            this.reportManagerProvider = reportManagerProvider ?? throw new ArgumentNullException(nameof(reportManagerProvider));

            // NOTE: the repository ctor is invoked in the context of http request. At this point of execution we have access to context-dependent data, like currentUserId.
            // The repository MUST read and store all the required context-dependent values for later use. E.g. notice that we do not store the IUserService (which is context/scope dependent), but read the value of current user and store it.
            //studentId = userService.GetCurrentUserId();
        }

        public BalanceView GetReportData(int balanceId, int currencyType, string startAccount, string endAccount, int nivelRand)
        {
            //var ret = new List<DataItem>() {
            //    new DataItem(1, 101, "Andrew Fuller", "Dr.", "Vice President, Sales"),
            //    new DataItem(1, 102, "Anne Dodsworth", "Ms.", "Sales Representative"),
            //    new DataItem(1, 103, "Michael Suyama", "Mr.", "Sales Representative"),
            //    new DataItem(1, 104, "Janet Leverling", "Ms.", "Sales Representative"),
            //    new DataItem(1, 105, "Elliot Komaroff", "Dr.", "Sales Coordinator"),
            //    new DataItem(2, 201, "Nancy Davolio", "Ms.", "Sales Representative"),
            //    new DataItem(2, 202, "Steven Buchanan", "Mr.", "Sales Manager"),
            //    new DataItem(2, 203, "Laura Callahan", "Ms.", "Sales Coordinator"),
            //    new DataItem(3, 301, "Frédérique Citeaux", "Mr.", "Sales Coordinator"),
            //    new DataItem(3, 302, "Laurence Lebihan", "Mr.", "Sales Representative"),
            //    new DataItem(3, 303, "Elizabeth Lincoln", "Ms.", "Sales Manager"),
            //    new DataItem(3, 304, "Yang Wang", "Mr.", "Sales Representative"),
            //    new DataItem(4, 401, "Antonio Moreno", "Mr.", "Sales Representative"),
            //    new DataItem(4, 402, "Thomas Hardy", "Mr.", "Sales Representative"),
            //    new DataItem(4, 403, "Christina Berglund", "Ms.", "Sales Manager"),
            //    new DataItem(5, 501, "Alejandra Camino", "Ms.", "Sales Representative"),
            //    new DataItem(5, 502, "Matti Karttunen", "Mr.", "Sales Representative"),
            //    new DataItem(5, 503, "Rita Müller", "Mrs.", "Sales Representative"),
            //};
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.ReportBalance(balanceId, currencyType, startAccount, endAccount, nivelRand);
                //var ret = reportManager.GetLegalPersons(count).Select(s => new DataItem { PersonName = s.Nume, Title = s.Prenume }).ToList();
                return ret;
            }
        }

        public List<NameValue<int>> GetParamerData()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("Unu", 1),
                new NameValue<int>("Doi", 2),
                new NameValue<int>("Trei", 3),
                new NameValue<int>("Patru", 4),
                new NameValue<int>("Cinci", 5)};
        }

        public List<BalanceDDDto> GetBalanceData()
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BalanceDDList();
                return ret;
            }
        }

        public List<NameValue<int>> GetBalanceTypeRON()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("RON si Echivalent RON", 0),
                new NameValue<int>("RON", 1)
            };
        }

        public List<NameValue<int>> GetBalanceTypeValuta()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("Valuta echivalent RON", 0),
                new NameValue<int>("Valuta", 2)
            };
        }

        public List<BalanceView> GetBalanceCurrency(int balanceId, int currencyType, string startAccount, string endAccount, int nivelRand)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.ReportBalanceCurrency(balanceId, currencyType, startAccount, endAccount, nivelRand);
                return ret;
            }
        }

        public SavedBalanceViewDto GetSavedBalanceReport(int savedBalanceId, string searchAccount, int nivelRand, int currencyId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.SavedBalanceReport(savedBalanceId, searchAccount, nivelRand, currencyId);
                //var ret = reportManager.GetLegalPersons(count).Select(s => new DataItem { PersonName = s.Nume, Title = s.Prenume }).ToList();
                return ret;
            }
        }

        public RegistruJurnal GetRegistruJurnalData(DateTime startDate, DateTime endDate, int currencyId, int docTypeId, int opStatus, bool raportExtins, DateTime dataCursValutar, string searchAccount1, string searchAccount2)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.RegistruJurnalReport(startDate, endDate, currencyId, docTypeId, opStatus, raportExtins, dataCursValutar, searchAccount1, searchAccount2);
                return ret;
            }
        }

        public List<CurrencyDto> GetCurrencies()
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.CurrencyDDList();
                return ret;
            }
        }

        public List<DocumentTypeListDDDto> GetDocumentType()
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.DocumentTypeList();
                return ret;
            }
        }

        public List<NameValue<int>> GetOpStatus()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("Toate", -1),
                new NameValue<int>("Nevalidat", 0),
                new NameValue<int>("Validat", 1)
            };
        }

        public FisaContModel GetFisaContData(int accountId, DateTime startDate, DateTime endDate, int currencyId, int corespAccountId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.FisaContReport(accountId, startDate, endDate, currencyId, corespAccountId);
                return ret;
            }
        }

        public List<AccountListDDDto> GetAccountList()
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.AccountList();
                return ret;
            }
        }

        public List<RegistruCasaModel> RegistruData(DateTime dataStart, DateTime dataEnd, int currencyId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.RegistruCasaDateRange(dataStart, dataEnd, currencyId);
                return ret;
            }
        }

        public RegistruCasaModel RegistruReportData(DateTime endDate, int currencyId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.RegistruCasa(endDate, currencyId);
                return ret;
            }
        }

        public PrepaymentsRegistruReport PrepaymentsData(int prepaymentType, DateTime repDate)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.PrepaymentsRegReport(repDate, prepaymentType);
                return ret;
            }
        }

        public ImoAssetRegistruReport ImoAssetReport(DateTime repDate, int storage)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.AssetRegistruReport(repDate, storage);
                return ret;
            }
        }

        public ImoAssetRegV2Report ImoAssetReportV2(DateTime repDate, int? storage)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.AssetRegistruV2Report(repDate, storage);
                return ret;
            }
        }

        public ImoAssetFisaReport ImoAssetFisaData(DateTime dataFisa, int inventoryNr)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.AssetFisa(dataFisa, inventoryNr);
                return ret;
            }
        }

        public InvObjectImoAssetReport InvObjectAssetReport(int invOperId, /*int storage, */int inventoryType)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.InventariereReport(invOperId, /*storage,*/ inventoryType);
                return ret;
            }
        }

        public BonTransferModel BonTransfer(int operationId, int inventoryType)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BonTransferReport(operationId, inventoryType);
                return ret;
            }
        }

        // Generare raport plata/incasare dispozitie
        public DispositionModel GetDispositionReport(int dispositionId, int operationTypeId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.DispositionReport(dispositionId, operationTypeId);
                return ret;
            }
        }

        // Generare declaratie casier
        public List<DeclaratieCasierModel> GetDeclaratieCasierModel(DateTime date, string numeCasier, DateTime dataDecizie)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.DeclaratieCasier(date, numeCasier, dataDecizie);
                return ret;
            }
        }

        //generare raport configurabil
        public ReportCalc GetConfigReport(int reportId, DateTime dataStart, DateTime dataEnd, bool isDateRange, bool rulaj, bool convertToLocalCurrency)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.CalculRaport(reportId, dataStart, dataEnd, isDateRange, rulaj, convertToLocalCurrency);
                return ret;
            }
        }

        //Generare raport Registru Inventar
        public RegistruInventarReport GetRegistruInventar(DateTime startDate)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.RegistruInventarReport(startDate);
                return ret;
            }
        }

        //Generare factura client
        public InvoiceModel GetInvoiceReport(int invoiceId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.InvoiceReport(invoiceId);
                return ret;
            }
        }

        //  Raport Situatii financiare
        public SitFinanReportModel GetSitFinanRap(int balanceId, int raportId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.SitFinanReport(balanceId, raportId);
                return ret;
            }
        }

        public SitFinanRaport GetSitFinanRaport(int raportId, int balanceId, bool isDailyBalance, bool isDateRange, DateTime startDate, DateTime endDate, int colNumber)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.GetSitFinanRaport(raportId, balanceId, isDailyBalance, isDateRange, startDate, endDate, colNumber);
                return ret;
            }
        }

        // Raport Obiecte de inventar
        public InvObjectReportModel GetInvObjectReport(DateTime repDate, int storageId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.InvObjectReport(repDate, storageId);
                return ret;
            }
        }

        //Raport Bon consum obiecte de inventar
        public BonConsumModel GetBonConsumReport(int operationId, int inventoryTypeId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BonConsumReport(operationId, inventoryTypeId);
                return ret;
            }
        }

        // Raport Sold Curent
        public List<SoldContCurentModel> GetSoldCurentReport(int currencyId, int accountId, DateTime dataStart, DateTime dataEnd, int periodTypeId, bool isDateRange)
        {
            try
            {
                using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
                {
                    var reportManager = managerScope.TestReportDataSource;
                    var ret = reportManager.SoldCurentReport(currencyId, accountId, dataStart, dataEnd, periodTypeId, isDateRange);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SoldFurnizoriDebitoriModel GetSoldFurnizoriReport(int thirdPartyId, DateTime startDate)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.SoldFurnizoriReport(thirdPartyId, startDate);
                return ret;
            }
        }

        public BugetPrevReportDto GetBugetPrevReport(int departmentId, int bugetPrevId, bool activityType)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BugetPrevReport(departmentId, bugetPrevId, activityType);
                return ret;
            }
        }

        // raportare anexa
        public AnexaReportModel GetAnexaReport(int savedBalanceId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.AnexaReport(savedBalanceId);
                return ret;
            }
        }

        // raportare BVC
        public BVC_Report GetBVC_Report(int variantaBugetId, string tipRand, int nivelRand, int anBuget, int tip, string frecventa, string tipActivitate, int tipRaport)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.RaportBVC(variantaBugetId, tipRand, nivelRand, anBuget, tip, frecventa, tipActivitate, tipRaport);
                return ret;
            }
        }

        // raportare BVC realizat
        public BVC_Realizat_Report GetBVC_RealizatReport(string tipRand, int bugetRealizatId, bool includReferat, int tipRealizat, int tipRaport, int anBuget, int tip, int variantaBugetId, int nivelRand)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BVC_RealizatReport(tipRand, bugetRealizatId, includReferat, tipRealizat, tipRaport, anBuget, tip, variantaBugetId, nivelRand);
                return ret;
            }
        }

        public CursValutarBNRModel GetCursValutarBNR(int balanceId, DateTime startDate, DateTime endDate)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.CursValutarBNR(balanceId);
                return ret;
            }
        }

        // raport lichiditate
        public LichidCalcModel GetLichidCalcReport(int savedBalanceId, int lichidType)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.LichidCalcReport(savedBalanceId, lichidType);
                return ret;
            }
        }

        // raport depozit bancare
        public DepozitBancarDto GetDepozitBancarReport(int balanceId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.DepozitBancarReport(balanceId);
                return ret;
            }
        }

        // raport BVC_BalRealizat
        public BVC_BalRealizat_Report GetBVC_BalRealizatReport(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, string tipActivitate, int tipRaport, bool includPrevazutAnual)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BVC_BalRealizatReport(tipRand, bugetBalRealizatId, tipRealizat, anBuget, tip, variantaBugetId, nivelRand, tipActivitate, tipRaport, includPrevazutAnual);
                return ret;
            }
        }

        public BugetRaportare BugetReport(int AnBVC)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BugetReport(AnBVC);
                return ret;
            }
        }

        public BVC_PrevResurseModel BVC_PrevResurseReport(int anBVC, int variantaBugetId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BVC_PrevResurseReport(anBVC, variantaBugetId);
                return ret;
            }
        }

        public DetaliuSoldReportDto GetDetaliuSoldReport(DateTime startDate, string account, int currencyId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.DetaliuSoldReport(startDate, account, currencyId);
                return ret;
            }
        }
        public BugetPreliminatDto GetBugetPreliminatDetalii(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, int tipRaport, int bugetPreliminatId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BugetPreliminatDetaliiReport(tipRand, bugetBalRealizatId,  tipRealizat, anBuget, tip, variantaBugetId, nivelRand, tipRaport, bugetPreliminatId);
                return ret;
            }
        }
        public BugetPreliminatDto GetBugetPreliminat(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, int tipRaport, int bugetPreliminatId)
        {
            using (var managerScope = reportManagerProvider.GetTestReportDataSourceScope())
            {
                var reportManager = managerScope.TestReportDataSource;
                var ret = reportManager.BugetPreliminatDetaliiReport(tipRand, bugetBalRealizatId, tipRealizat, anBuget, tip, variantaBugetId, nivelRand, tipRaport, bugetPreliminatId);
                return ret;
            }
        }
    }

    public class DataItem
    {
        //public DataItem(int floor, int office, string personName, string titleOfCourtesy, string title)
        //{
        //    Floor = floor;
        //    Office = office;
        //    PersonName = personName;
        //    TitleOfCourtesy = titleOfCourtesy;
        //    Title = title;
        //}
        public int Floor { get; set; }

        public int Office { get; set; }
        public string PersonName { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string Title { get; set; }
    }
}