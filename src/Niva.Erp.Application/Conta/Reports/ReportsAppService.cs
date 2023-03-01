using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Conta.Balance;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.Conta.Prepayments;
using Niva.Erp.Conta.Reports.Dto;
using Niva.Erp.Economic.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Models.Conta.RegistruInventar;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Economic;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.Economic.Casierii.Cupiuri;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.HR;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.ImoAssets;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.ConfigurareRapoarte;
using Niva.Erp.Repositories.Conta.Lichiditate;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.RegistruInventar;
using Niva.Erp.Repositories.Economic;
using Niva.Erp.Repositories.ImoAsset;
using Niva.Erp.Repositories.InvObjects;
using Niva.Erp.Repositories.SectoareBNR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.Conta.Reports
{
    public interface IReportsAppService : IApplicationService
    {
        List<BalanceDDDto> BalanceDDList();

        List<CurrencyDto> CurrencyDDList();

        List<DocumentTypeListDDDto> DocumentTypeList();

        BalanceView ReportBalance(int balanceId, int currencyType, string startAccount, string endAccount, int? nivelRand);

        List<BalanceView> ReportBalanceCurrency(int balanceId, int currencyType, string startAccount, string endAccount, int? nivelRand);

        SavedBalanceViewDto SavedBalanceReport(int savedBalanceId, string searchAccount, int? nivelRand, int currencyId);

        RegistruJurnal RegistruJurnalReport(DateTime startDate, DateTime endDate, int currencyId, int docTypeId, int optStatus, bool raportExtins, DateTime? dataCursValutar, string searchAccount1, string searchAccount2);

        FisaContModel FisaContInit();

        FisaContModel FisaContView(FisaContModel model);

        FisaContModel FisaContChangeCoresp(FisaContModel model);

        FisaContModel FisaContReport(int accountId, DateTime startDate, DateTime endDate, int currencyId, int? corespAccountId);

        List<AccountListDDDto> AccountList();

        List<RegistruCasaModel> RegistruCasaDateRange(DateTime dataStart, DateTime dataEnd, int currencyId);

        RegistruCasaModel RegistruCasa(DateTime dataEnd, int currencyId);

        PrepaymentsRegistruReport PrepaymentsRegReport(DateTime repDate, int prepaymentType);

        //Mijloace fixe
        int InventoryNrExists(int inventoryNr);

        ImoAssetRegistruReport AssetRegistruReport(DateTime repDate, int? storage);

        ImoAssetFisaReport AssetFisa(DateTime dateFisa, int inventoryNr);

        ImoAssetRegV2Report AssetRegistruV2Report(DateTime repDate, int? storage);

        InvObjectImoAssetReport InventariereReport(int invOperId, /*int storageId, */int inventoryType);

        BonTransferModel BonTransferReport(int operationId, int inventoryType);

        DispositionModel DispositionReport(int dispositionId, int operationType);

        List<DeclaratieCasierModel> DeclaratieCasier(DateTime date, string numeCasier, DateTime dataDecizie);

        ReportCalc CalculRaport(int reportId, DateTime reportStartDate, DateTime reportEndDate, bool isDateRange, bool rulaj, bool convertToLocalCurrency);

        RegistruInventarReport RegistruInventarReport(DateTime reportDate);

        InvoiceModel InvoiceReport(int invoiceId);

        SitFinanReportModel SitFinanReport(int balanceId, int raportId);

        SitFinanRaport GetSitFinanRaport(int raportId, int? balanceId, bool isDailyBalance, bool isDateRange, DateTime startDate, DateTime endDate, int colNumber);

        InvObjectReportModel InvObjectReport(DateTime repDate, int? storageId);

        BonConsumModel BonConsumReport(int operationId, int inventoryType);

        List<SoldContCurentModel> SoldCurentReport(int? currencyId, int? accountId, DateTime? dataStart, DateTime? dataEnd, int? periodTypeId, bool isDateRange);

        void VerifySoldPeriod(DateTime dataStart, DateTime dataEnd, int periodTypeId);

        SoldFurnizoriDebitoriModel SoldFurnizoriReport(int thirdPartyId, DateTime startDate);

        BugetPrevReportDto BugetPrevReport(int departmentId, int bugetPrevId, bool activityType);

        AnexaReportModel AnexaReport(int savedBalanceId);

        // Raportare BVC Prevazut
        BVC_Report RaportBVC(int variantaBugetId, string tipRand, int nivelRand, int anBuget, int tip, string frecventa, string tipActivitate, int tipRaport);

        // Raportare BVC Realizat
        BVC_Realizat_Report BVC_RealizatReport(string tipRand, int bugetRealizatId, bool includReferat, int tipRealizat, int tipRaport, int anBuget, int tip, int variantaBugetId, int nivelRand);

        CursValutarBNRModel CursValutarBNR(int balanceId);

        // Rapoarte lichiditate
        LichidCalcModel LichidCalcReport(int savedBalanceId, int lichidType);

        // Raport Depozite bancare
        DepozitBancarDto DepozitBancarReport(int balanceId);

        // Raport BVC_BalRealizat

        BVC_BalRealizat_Report BVC_BalRealizatReport(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, string tipActivitate, int tipRaport, bool includPrevazutAnual);

        BugetRaportare BugetReport(int AnBVC);

        // Raport BVC_PrevResurse
        BVC_PrevResurseModel BVC_PrevResurseReport(int anBVC, int bugetPrevId);

        // Invoice Details
        InvoiceDetailsReportDto InvoiceDetailsInit();

        InvoiceDetailsReportDto SearchInvoiceDetails(InvoiceDetailsReportDto invocieDetail);

        DetaliuSoldReportDto DetaliuSoldReport(DateTime startDate, string account, int currencyId);

        // Buget Preliminat Detalii
        BugetPreliminatDto BugetPreliminatDetaliiReport(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, int tipRaport, int bugetPreliminatId);
    }

    public class ReportsAppService : ErpAppServiceBase, IReportsAppService
    {
        private IRepository<LegalPerson> _legalPersonRepository { get; set; }
        private IBalanceRepository _balanceRepository;
        private IRepository<BalanceDetails> _balanceDetailsRepository { get; set; }
        private ICurrencyRepository _currencyRepository;
        private IRepository<DocumentType> _documentTypeRepository;
        private IOperationRepository _operationRepository { get; set; }
        private IAccountRepository _accountRepository;
        private IRepository<OperationDetails> _operationDetailsRepository;
        private IDispositionRepository _dispositionRepository { get; set; }
        private IRepository<PrepaymentBalance> _prepaymentsBalanceRepository;
        private IRepository<PrepaymentsDurationSetup> _prepaymentsDurationSetupRepository;
        private IPersonRepository _personRepository;
        private IRepository<ImoAssetItem> _imoAssetItemRepository;
        private IRepository<ImoAssetStock> _imoAssetStockRepository;
        private IRepository<ImoAssetStorage> _imoAssetStorageRepository;
        private IRepository<ImoAssetOperDetail> _imoAssetOperDetRepository;
        private IRepository<ImoInventariere> _imoInventariereRepository;
        private IRepository<ImoInventariereDet> _imoInventariereDetailsRepository;
        private IRepository<InvStorage> _invStorageRepository;
        private IRepository<InvObjectInventariere> _invObjInventariereRepository;
        private IRepository<InvObjectInventariereDet> _invObjInventariereDetailsRepository;
        private IImoOperationRepository _imoAssetOperRepository;
        private IInvOperationRepository _invOperationRepository;
        private IRepository<InvObjectOperDetail> _invOperDetailRepository;
        private IRepository<Report> _reportRepository;
        private IConfigReportRepository _configReportRepository;
        private IRepository<RegInventarExceptii> _exceptRegInvRepository;
        private IRegInventarRepository _regInventarRepository;
        private IInvoiceRepository _invoiceRepository;
        private IExchangeRatesRepository _exchangeRatesRepository;
        private IRepository<BankAccount> _bankAccountRepository;
        private IRepository<InvObjectStock> _invObjectStockRepository;
        private IRepository<DispositionInvoice> _dispositionInvoiceRepository;
        private IRepository<PaymentOrderInvoice> _paymentOrderInvoiceRepository;
        private IRepository<BVC_FormRand> _bvcFormRandRepository;
        private IRepository<BVC_BugetPrevRandValue> _bugetPrevRandValueRepository;
        private IRepository<SalariatiDepartamente> _salariatiDepartamentRepository;
        private IRepository<BVC_BugetPrevRand> _bugetPrevRandRepository;
        private IBVC_BugetPrevRepository _bugetPrevRepository;
        private ISavedBalanceRepository _savedBalanceRepository;
        private IRepository<BNR_RaportareRand> _bnrRaportareRandRepository;
        private IRepository<BNR_AnexaDetail> _bnrAnexaDetailRepository;
        private IBNR_RaportareRepository _bnrRaportareRepository;
        private IRepository<BNR_Anexa> _bnrAnexaRepository;
        private IRepository<BVC_Formular> _bvcFormularRepository;
        private IRepository<ActivityType> _activityTypeRepository;
        private IBVC_BugetRealizatRepository _bugetRealizatRepository;
        private IRepository<BVC_RealizatRand> _bvcRealizatRandRepository;
        private ILichidCalcRepository _lichidCalcRepository;
        private IRepository<LichidBenzi> _lichidBenziRepository;
        private IRepository<LichidConfig> _lichidConfigRepository;
        private IRepository<LichidCalcCurr> _lichidCalcCurrRepository;
        private IRepository<LichidBenziCurr> _lichidBenziCurrRepository;
        private IPlasamentLichiditateManager _plasamentLichiditateManager;
        private IRepository<Issuer> _issuerRepository { get; set; }
        private IBVC_BugetBalRealizatRepository _bugetBalRealizatRepository;
        private IRepository<BVC_BalRealizatRand> _bugetBalRealizatRandRepository;
        private ReportManager _reportManager;
        private IRepository<BVC_PrevResurse> _bugetPrevResurseRepository;
        private IBVC_ReportRepository _bugetReportRepository;
        private IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IRepository<User, long> repository;
        IRepository<CupiuriItem> _cupiuriItemRepository;


        public ReportsAppService(IBVC_ReportRepository bugetReportRepository, IRepository<LegalPerson> legalPersonRepository, IBalanceRepository balanceRepository, ICurrencyRepository currencyRepository,
                                IRepository<DocumentType> documentTypeRepository, IOperationRepository operationRepository, IAccountRepository accountRepository,
                                IRepository<OperationDetails> operationDetailsRepository, IDispositionRepository dispositionRepository, IRepository<PrepaymentBalance> prepaymentsBalanceRepository,
                                IRepository<PrepaymentsDurationSetup> prepaymentsDurationSetupRepository, IPersonRepository personRepository, IRepository<ImoAssetItem> imoAssetItemRepository,
                                IRepository<ImoAssetStock> imoAssetStockRepository, IRepository<ImoAssetStorage> imoAssetStorageRepository, IRepository<ImoAssetOperDetail> imoAssetOperDetRepository,
                                IRepository<ImoInventariere> imoInventariereRepository, IRepository<ImoInventariereDet> imoInventariereDetailsRepository, IRepository<InvStorage> invStorageRepository,
                                IRepository<InvObjectInventariere> invObjInventariereRepository, IRepository<InvObjectInventariereDet> invObjInventariereDetailsRepository, IImoOperationRepository imoAssetOperRepository,
                                IInvOperationRepository invOperationRepository, IRepository<InvObjectOperDetail> invOperDetailRepository, IRepository<Report> reportRepository,
                                IConfigReportRepository configReportRepository, IRepository<RegInventarExceptii> exceptRegInvRepository,
                                IRegInventarRepository regInventarRepository, IInvoiceRepository invoiceRepository, IExchangeRatesRepository exchangeRatesRepository,
                                IRepository<BankAccount> bankAccountRepository, IRepository<SitFinanCalcNote> sitFinanCalcNoteRepository, IRepository<InvObjectStock> invObjectStockRepository,
                                IRepository<DispositionInvoice> dispositionInvoiceRepository, IRepository<PaymentOrderInvoice> paymentOrderInvoiceRepository,
                                IRepository<BVC_FormRand> bvcFormRandRepository, IRepository<BVC_BugetPrevRandValue> bugetPrevRandValueRepository, IRepository<SalariatiDepartamente> salariatiDepartamentRepository,
                                IRepository<BVC_BugetPrevRand> bugetPrevRandRepository, IBVC_BugetPrevRepository bugetPrevRepository,
                                ISavedBalanceRepository savedBalanceRepository, IRepository<BNR_RaportareRand> bnrRaportareRandRepository,
                                IRepository<BNR_AnexaDetail> bnrAnexaDetailRepository, IBNR_RaportareRepository bnrRaportareRepository, IRepository<BNR_Anexa> bnrAnexaRepository,
                                IRepository<BVC_Formular> bvcFormularRepository, IRepository<ActivityType> activityTypeRepository,
                                IBVC_BugetRealizatRepository bugetRealizatRepository, IRepository<BVC_RealizatRand> bvcRealizatRandRepository,
                                IRepository<BalanceDetails> balanceDetailsRepository, ILichidCalcRepository lichidCalcRepository, IRepository<LichidBenzi> lichidBenziRepository,
                                IRepository<LichidConfig> lichidConfigRepository, IRepository<LichidCalcCurr> lichidCalcCurrRepository,
                                IRepository<LichidBenziCurr> lichidBenziCurrRepository, IPlasamentLichiditateManager plasamentLichiditateManager,
                                IRepository<Issuer> issuerRepository, IBVC_BugetBalRealizatRepository bugetBalRealizatRepository, IRepository<BVC_BalRealizatRand> bugetBalRealizatRandRepository,
                                IRepository<BVC_BalRealizatRandDetails> bugetBalRealizatRandDetailsRepository, ReportManager reportManager, IRepository<BVC_PrevResurse> bugetPrevResurseRepository,
                                IRepository<InvoiceDetails> invoiceDetailsRepository, IRepository<CupiuriItem> cupiuriItemRepository)
        {
            _cupiuriItemRepository = cupiuriItemRepository;
            _legalPersonRepository = legalPersonRepository;
            _balanceDetailsRepository = balanceDetailsRepository;
            _balanceRepository = balanceRepository;
            _currencyRepository = currencyRepository;
            _documentTypeRepository = documentTypeRepository;
            _operationRepository = operationRepository;
            _accountRepository = accountRepository;
            _operationDetailsRepository = operationDetailsRepository;
            _dispositionRepository = dispositionRepository;
            _prepaymentsBalanceRepository = prepaymentsBalanceRepository;
            _prepaymentsDurationSetupRepository = prepaymentsDurationSetupRepository;
            _personRepository = personRepository;
            _imoAssetItemRepository = imoAssetItemRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _imoAssetStorageRepository = imoAssetStorageRepository;
            _imoAssetOperDetRepository = imoAssetOperDetRepository;
            _imoInventariereRepository = imoInventariereRepository;
            _imoInventariereDetailsRepository = imoInventariereDetailsRepository;
            _invStorageRepository = invStorageRepository;
            _invObjInventariereRepository = invObjInventariereRepository;
            _invObjInventariereDetailsRepository = invObjInventariereDetailsRepository;
            _imoAssetOperRepository = imoAssetOperRepository;
            _invOperationRepository = invOperationRepository;
            _invOperDetailRepository = invOperDetailRepository;
            _reportRepository = reportRepository;
            _configReportRepository = configReportRepository;
            _exceptRegInvRepository = exceptRegInvRepository;
            _regInventarRepository = regInventarRepository;
            _invoiceRepository = invoiceRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _bankAccountRepository = bankAccountRepository;
            _invObjectStockRepository = invObjectStockRepository;
            _paymentOrderInvoiceRepository = paymentOrderInvoiceRepository;
            _dispositionInvoiceRepository = dispositionInvoiceRepository;
            _bvcFormRandRepository = bvcFormRandRepository;
            _bugetPrevRandValueRepository = bugetPrevRandValueRepository;
            _salariatiDepartamentRepository = salariatiDepartamentRepository;
            _bugetPrevRandRepository = bugetPrevRandRepository;
            _bugetPrevRepository = bugetPrevRepository;
            _savedBalanceRepository = savedBalanceRepository;
            _bnrRaportareRandRepository = bnrRaportareRandRepository;
            _bnrAnexaRepository = bnrAnexaRepository;
            _bnrAnexaDetailRepository = bnrAnexaDetailRepository;
            _bnrRaportareRepository = bnrRaportareRepository;
            _bvcFormularRepository = bvcFormularRepository;
            _activityTypeRepository = activityTypeRepository;
            _bugetRealizatRepository = bugetRealizatRepository;
            _bvcRealizatRandRepository = bvcRealizatRandRepository;
            _lichidConfigRepository = lichidConfigRepository;
            _lichidCalcRepository = lichidCalcRepository;
            _lichidBenziRepository = lichidBenziRepository;
            _lichidCalcCurrRepository = lichidCalcCurrRepository;
            _lichidBenziCurrRepository = lichidBenziCurrRepository;
            _plasamentLichiditateManager = plasamentLichiditateManager;
            _issuerRepository = issuerRepository;
            _bugetBalRealizatRepository = bugetBalRealizatRepository;
            _bugetBalRealizatRandRepository = bugetBalRealizatRandRepository;
            _reportManager = reportManager;
            _bugetPrevResurseRepository = bugetPrevResurseRepository;
            _bugetReportRepository = bugetReportRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
        }

        public List<BalanceDDDto> BalanceDDList()
        {
            try
            {
                var balanceList = _balanceRepository.GetAll()
                                                    .Where(f => f.Status == State.Active)
                                                    .OrderByDescending(f => f.StartDate)
                                                    .ToList()
                                                    .Select(f => new BalanceDDDto { Id = f.Id, BalanceDate = f.BalanceDate.ToShortDateString() })
                                                    .ToList();
                return balanceList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.Balanta")]
        public BalanceView ReportBalance(int balanceId, int currencyType, string startAccount, string endAccount, int? nivelRand)
        {
            //var appClient = GetCurrentTenant();

            var localCurrecyId = 1; //appClient.LocalCurrencyId.Value;

            var searchData = (startAccount != "" || endAccount != "") ? startAccount + " " + endAccount : "";

            bool convertAllCurrencies = (currencyType == 0);

            var list = _balanceRepository.GetBalanceDetails(balanceId, true, searchData, currencyType, false, convertAllCurrencies, localCurrecyId, nivelRand);

            return list;
        }

        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.BalantaValuta")]
        public List<BalanceView> ReportBalanceCurrency(int balanceId, int currencyType, string startAccount, string endAccount, int? nivelRand)
        {
            //var balanceView = ReportBalance(0, balanceId, balType, startAccount, endAccount);

            //var balanceCurrencyView =  _balanceRepository.GetBalanceCurrency(balanceView);
            //var ret = balanceCurrencyView.OrderBy(f => f.CurrencyName).ToList();
            // var appClient = GetCurrentTenant();
            var localCurrecyId = 1; //appClient.LocalCurrencyId.Value;
            bool convertToLocalCurrency = (currencyType != 0);
            bool convertAllCurrencies = (currencyType == 0);
            var ret = _balanceRepository.GetBalanceCurrency(balanceId, startAccount, endAccount, convertToLocalCurrency, convertAllCurrencies, localCurrecyId, nivelRand);

            return ret;
        }

        //[UnitOfWork]
        public virtual SavedBalanceViewDto SavedBalanceReport(int savedBalanceId, string searchAccount, int? nivelRand, int currencyId)
        {
            var balView = new SavedBalanceViewDto();
            balView = _reportManager.GetSavedBalanceViewList(savedBalanceId, searchAccount, nivelRand, currencyId);
            return balView;
        }

        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.RegistruJurnal")]
        public virtual RegistruJurnal RegistruJurnalReport(DateTime startDate, DateTime endDate, int currencyId, int docTypeId, int opStatus, bool raportExtins, DateTime? dataCursValutar, string searchAccount1, string searchAccount2)
        {
            var appClientId = 1; // GetCurrentTenant();
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var ret = new RegistruJurnal();
            ret.AppClientName = person.FullName;
            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;
            ret.EndDate = endDate;
            ret.StartDate = startDate;
            ret.CurrencyId = currencyId;
            ret.Parameters = "";
            var localCurrencyId = 1;
            decimal exchangeRateDataTranzactie = 0;
            decimal exchangeRateDataReferinta = 0;

            if (currencyId != 0)
                ret.Parameters = "Moneda " + _currencyRepository.FirstOrDefault(f => f.Id == currencyId).CurrencyCode + "; ";
            else
                ret.Currency = "";
            ret.DocumentTypeId = docTypeId;
            if (docTypeId != 0)
            {
                ret.Parameters += "Tip document: " + _documentTypeRepository.FirstOrDefault(f => f.Id == docTypeId).TypeNameShort + "; ";
            }
            else
            {
                ret.Parameters += "Toate documentele; ";
            }

            ret.Parameters += (opStatus == -1) ? "Toate operatiile; " : ((OperationStatus)opStatus == OperationStatus.Checked) ? "Operatii validate; " : "Operatii nevalidate; ";

            // pregatesc detaliile
            var operations = _operationRepository.GetAllIncludingOperationDetails().Include(f => f.DocumentType).Include(f => f.Currency)
                                                .Where(f => f.State == State.Active && f.OperationDate >= startDate && f.OperationDate <= endDate && f.TenantId == appClientId);

            if (currencyId != 0)
                operations = operations.Where(f => f.CurrencyId == currencyId);
            if (docTypeId != 0)
                operations = operations.Where(f => f.DocumentTypeId == docTypeId);
            if (opStatus != -1)
                operations = operations.Where(f => f.OperationStatus == (OperationStatus)opStatus);
            var operList = operations.OrderBy(f => f.OperationDate).ThenBy(f => f.DocumentNumber).ToList();
            var operListTemp = new List<Operation>();
            if (searchAccount1 != "")
            {
                operListTemp.AddRange(operList.Where(f => f.OperationsDetails.Any(g => g.Debit.Symbol.IndexOf(searchAccount1) >= 0 || g.Credit.Symbol.IndexOf(searchAccount1) >= 0)).ToList());
                if (searchAccount1 != null || searchAccount1 != "")
                    if (searchAccount1 == "")
                    {
                    }
                    else
                    {
                        ret.Parameters += "Cont " + searchAccount1 + "; ";
                    }
            }
            if (searchAccount2 != "")
            {
                operListTemp.AddRange(operList.Where(f => f.OperationsDetails.Any(g => g.Debit.Symbol.IndexOf(searchAccount2) >= 0 || g.Credit.Symbol.IndexOf(searchAccount2) >= 0)).ToList());
                if (searchAccount2 != null || searchAccount2 != "")
                    if (searchAccount2 == "")
                    {
                    }
                    else
                    {
                        ret.Parameters += "Cont " + searchAccount2 + "; ";
                    }
            }

            if (operListTemp.Count > 0)
            {
                operList = operListTemp;
            }

            if (raportExtins)
            {

                ret.ExtinsDetails = new List<RegistruJurnalExtinsDetails>();
                // lista user ABpUsers 
                var userList = _operationRepository.GetUsers().ToList().Distinct();
                foreach (var op in operList)
                {
                    foreach (var det in op.OperationsDetails.OrderBy(x => x.Id))
                    {
                        exchangeRateDataReferinta = _exchangeRatesRepository.GetExchangeRateForOper(dataCursValutar.Value, op.CurrencyId, localCurrencyId);
                        exchangeRateDataTranzactie = _exchangeRatesRepository.GetExchangeRateForOper(op.DocumentDate, op.CurrencyId, localCurrencyId);
                        ret.ExtinsDetails.Add(new RegistruJurnalExtinsDetails
                        {
                            ContaOperationId = op.Id,
                            OperationDate = op.OperationDate,
                            CreationDate = op.CreationTime,
                            DocumentTypeShortName = op.DocumentType.TypeNameShort,
                            DocumentNumber = (op.DocumentNumber != null && det.DetailNr != null) ? (op.DocumentNumber + "." + det.DetailNr)
                                             : ((op.DocumentNumber == null) ? det.DetailNr.ToString() : op.DocumentNumber),
                            DocumentDate = op.DocumentDate,
                            DebitAccount = det.Debit.Symbol,
                            CreditAccount = det.Credit.Symbol,
                            Value = det.Value,
                            CurrencyCode = op.Currency.CurrencyCode,
                            OperationDetailsObservations = det.Details,
                            TipTranzactie = op.ExternalOperation ? "Automat" : "Manual",
                            SumaLeiDataReferinta = (currencyId != localCurrencyId) ? det.Value * exchangeRateDataReferinta : det.Value,
                            SumaLeiDataTranzactie = (currencyId != localCurrencyId) ? det.Value * exchangeRateDataTranzactie : det.Value,
                            UserName = op.CreatorUserId != null ? userList.FirstOrDefault(f => f.Id == op.CreatorUserId).UserName : " "
                        });
                    }
                }

                ret.ExtinsDetails = ret.ExtinsDetails.Where(f => (f.DebitAccount.IndexOf(searchAccount1) >= 0 ||
                                                      f.CreditAccount.IndexOf(searchAccount1) >= 0) &&
                                                     (f.DebitAccount.IndexOf(searchAccount2) >= 0 || f.CreditAccount.IndexOf(searchAccount2) >= 0))
                                         .ToList();
            }
            else
            {
                ret.Details = new List<RegistruJurnalDetails>();
                foreach (var op in operList)
                {
                    foreach (var det in op.OperationsDetails.OrderBy(x => x.Id))
                    {
                        ret.Details.Add(new RegistruJurnalDetails
                        {
                            ContaOperationId = op.Id,
                            OperationDate = op.OperationDate,
                            DocumentTypeShortName = op.DocumentType.TypeNameShort,
                            DocumentNumber = (op.DocumentNumber != null && det.DetailNr != null) ? (op.DocumentNumber + "." + det.DetailNr)
                                             : ((op.DocumentNumber == null) ? det.DetailNr.ToString() : op.DocumentNumber),
                            DocumentDate = op.DocumentDate,
                            DebitAccount = det.Debit.Symbol,
                            CreditAccount = det.Credit.Symbol,
                            Value = det.Value,
                            CurrencyCode = op.Currency.CurrencyCode,
                            OperationDetailsObservations = det.Details
                        });
                    }
                }

                ret.Details = ret.Details.Where(f => (f.DebitAccount.IndexOf(searchAccount1) >= 0 ||
                                                      f.CreditAccount.IndexOf(searchAccount1) >= 0) &&
                                                     (f.DebitAccount.IndexOf(searchAccount2) >= 0 || f.CreditAccount.IndexOf(searchAccount2) >= 0))
                                         .ToList();
            }
            return ret;
        }

        public List<CurrencyDto> CurrencyDDList()
        {
            var currencyList = _currencyRepository.GetAll();
            var ret = ObjectMapper.Map<List<CurrencyDto>>(currencyList);
            return ret;
        }

        //[UnitOfWork]
        public virtual List<DocumentTypeListDDDto> DocumentTypeList()
        {
            var _documentTypes = _documentTypeRepository.GetAll()

                        .ToList()
                        .OrderBy(f => f.TypeName);

            var ret = ObjectMapper.Map<List<DocumentTypeListDDDto>>(_documentTypes);
            return ret;
        }

        public FisaContModel FisaContInit()
        {
            var appClient = 1; // GetCurrentTenant();
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
            var currency = _currencyRepository.FirstOrDefault(f => f.Id == 1);

            var startDate = _balanceRepository.BalanceDateNextDay();
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var ret = new FisaContModel
            {
                AppClientName = person.Name,
                AppClientId1 = person.Id1,
                AppClientId2 = person.Id2,
                StartDate = startDate,
                EndDate = endDate,
                LocalCurrencyId = currency.Id,
                LocalCurrency = currency.CurrencyCode,
                CurrencyId = currency.Id,
                Currency = currency.CurrencyCode,
                SoldInitial = 0,
                SoldPrecedent = 0,
                TotalCredit = 0,
                TotalDebit = 0,
                ShowDetails = false,
                TenantId = appClient
            };

            return ret;
        }

        public FisaContModel FisaContView(FisaContModel model)
        {
            var currency = _currencyRepository.FirstOrDefault(f => f.Id == model.CurrencyId);
            model.Currency = currency.CurrencyCode;

            var account = _accountRepository.FirstOrDefault(f => f.Id == model.AccountId);
            model.AccountName = account.AccountName;
            model.AccountSymbol = account.Symbol;
            model.AccountType = account.AccountTypes;
            var startYear = new DateTime(model.StartDate.Year, 1, 1);

            var accountList = _accountRepository.GetAllAnalythicsSintetic(model.AccountId ?? 0).Select(f => f.Id).Distinct().ToList();

            if (account.ComputingAccount)
            {
                if (!accountList.Contains(account.Id))
                {
                    accountList.Add(account.Id);
                }
            }

            var soldInit = _balanceRepository.GetSoldTypeAccount(startYear.AddDays(-1), account.Id, model.TenantId, model.CurrencyId ?? 0, model.LocalCurrencyId, (model.CurrencyId ?? 0) == 0, null);
            model.SoldInitial = Math.Abs(soldInit.Sold);
            model.SoldInitialType = soldInit.TipSold;

            var rulPrecDeb = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.DocumentType, f => f.Operation.Currency)
                                                        .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == model.TenantId &&
                                                               f.Operation.OperationDate >= startYear && f.Operation.OperationDate < model.StartDate &&
                                                               (accountList.Contains(f.DebitId))
                                                               && f.Operation.CurrencyId == model.CurrencyId//((model.CurrencyId == model.LocalCurrencyId) ? f.Operation.CurrencyId : model.CurrencyId)
                                                               /*&& f.Operation.OperationStatus == OperationStatus.Checked*/)
                                                        .GroupBy(x => true)
                                                        .Select(f => new { TotalRulaj = f.Sum(g => g.Value), TotalRulajCurr = f.Sum(g => g.ValueCurr) })
                                                        .FirstOrDefault();

            var rulPrecCr = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.DocumentType, f => f.Operation.Currency)
                                                       .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == model.TenantId &&
                                                              f.Operation.OperationDate >= startYear && f.Operation.OperationDate < model.StartDate &&
                                                              (accountList.Contains(f.CreditId))
                                                             && f.Operation.CurrencyId == model.CurrencyId//((model.CurrencyId == model.LocalCurrencyId) ? f.Operation.CurrencyId : model.CurrencyId)
                                                               /*&& f.Operation.OperationStatus == OperationStatus.Checked*/)
                                                       .GroupBy(x => true)
                                                        .Select(f => new { TotalRulaj = f.Sum(g => g.Value), TotalRulajCurr = f.Sum(g => g.ValueCurr) })
                                                        .FirstOrDefault();

            var soldPrec = (soldInit.TipSold == "D" ? 1 : -1) * soldInit.Sold;
            model.TotalPrecDebit = 0;
            model.TotalPrecDebit = 0;

            if (rulPrecDeb != null)
            {
                model.TotalPrecDebit = rulPrecDeb.TotalRulaj;
                soldPrec += rulPrecDeb.TotalRulaj;
            }

            if (rulPrecCr != null)
            {
                model.TotalPrecCredit = rulPrecCr.TotalRulaj;
                soldPrec -= rulPrecCr.TotalRulaj;
            }

            model.SoldPrecedent = Math.Abs(soldPrec);
            model.SoldPrecedentType = ((soldPrec > 0) ? ValueTypeEnum.D : ValueTypeEnum.C).ToString();

            var operationDetails = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.DocumentType, f => f.Operation.Currency)
                            .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == model.TenantId && f.Operation.OperationDate >= model.StartDate
                                        && f.Operation.OperationDate <= model.EndDate
                                       && (accountList.Contains(f.DebitId) || accountList.Contains(f.CreditId))
                                        && f.Operation.CurrencyId == model.CurrencyId//((model.CurrencyId == model.LocalCurrencyId) ? f.Operation.CurrencyId : model.CurrencyId)
                                        /*&& f.Operation.OperationStatus == OperationStatus.Checked*/)
                            .OrderBy(f => f.Operation.OperationDate).ThenBy(f => f.Operation.DocumentType.TypeName).ThenBy(f => f.Operation.DocumentNumber).ThenBy(f => f.DetailNr)
                            .ToList();

            //soldPrec = Math.Abs(soldPrec);
            var accountSheetDetails = new List<FisaContModelDetails>();
            var soldCurrent = soldPrec;
            string valueType = "";
            decimal totalDebit = 0, totalCredit = 0;
            decimal soldFinal = 0;
            foreach (var item in operationDetails)
            {
                if (accountList.Contains(item.DebitId))
                {
                    var detail = new FisaContModelDetails();
                    // daca debitul se afla in lista conturilor selectate atunci creditul este cont corespondent

                    detail.CorespAccountId = item.CreditId;
                    detail.CorespAccountSymbol = item.Credit.Symbol;
                    detail.CorespAccountName = item.Credit.AccountName;
                    detail.DebitValue = item.Value;
                    detail.CreditValue = 0;
                    totalDebit += item.Value;

                    detail.OperationDate = item.Operation.OperationDate;
                    detail.DocumentTypeShortName = item.Operation.DocumentType.TypeNameShort;
                    detail.DocumentNumber = (item.Operation.DocumentNumber != null && item.DetailNr != null) ? (item.Operation.DocumentNumber /*+ "." + item.DetailNr*/)
                                             : ((item.Operation.DocumentNumber == null) ? item.DetailNr.ToString() : item.Operation.DocumentNumber);
                    detail.DocumentDate = item.Operation.DocumentDate;
                    detail.CurrencyId = item.Operation.CurrencyId;
                    detail.CurrencyCode = item.Operation.Currency.CurrencyCode;
                    detail.OperationDetailsObservations = item.Details;

                    soldCurrent += (detail.DebitValue - detail.CreditValue);

                    detail.Sold = Math.Abs(soldCurrent);
                    valueType = ((soldCurrent > 0) ? ValueTypeEnum.D : ValueTypeEnum.C).ToString();
                    detail.ValueType = valueType;
                    soldFinal = Math.Abs(soldCurrent);

                    accountSheetDetails.Add(detail);
                }

                if (accountList.Contains(item.CreditId))
                {
                    var detail = new FisaContModelDetails();
                    // daca debitul se afla in lista conturilor selectate atunci creditul este cont corespondent

                    detail.CorespAccountId = item.DebitId;
                    detail.CorespAccountSymbol = item.Debit.Symbol;
                    detail.CorespAccountName = item.Debit.AccountName;
                    detail.DebitValue = 0;
                    detail.CreditValue = item.Value;
                    totalCredit += item.Value;

                    detail.OperationDate = item.Operation.OperationDate;
                    detail.DocumentTypeShortName = item.Operation.DocumentType.TypeNameShort;
                    detail.DocumentNumber = (item.Operation.DocumentNumber != null && item.DetailNr != null) ? (item.Operation.DocumentNumber /*+ "." + item.DetailNr*/)
                                             : ((item.Operation.DocumentNumber == null) ? item.DetailNr.ToString() : item.Operation.DocumentNumber);
                    detail.DocumentDate = item.Operation.DocumentDate;
                    detail.CurrencyId = item.Operation.CurrencyId;
                    detail.CurrencyCode = item.Operation.Currency.CurrencyCode;
                    detail.OperationDetailsObservations = item.Details;

                    soldCurrent += (detail.DebitValue - detail.CreditValue);

                    detail.Sold = Math.Abs(soldCurrent);
                    valueType = ((soldCurrent > 0) ? ValueTypeEnum.D : ValueTypeEnum.C).ToString();
                    detail.ValueType = valueType;
                    soldFinal = Math.Abs(soldCurrent);

                    accountSheetDetails.Add(detail);
                }
            }

            if (operationDetails.Count == 0)
            {
                soldFinal = Math.Abs(soldPrec);
                valueType = model.SoldInitialType;
            }

            model.OperationsDetail = accountSheetDetails;
            model.OperationsDetailSelection = accountSheetDetails;
            model.ShowDetails = true;
            model.TotalDebit = totalDebit;
            model.TotalCredit = totalCredit;
            model.SoldFinal = soldFinal;
            model.SoldFinalType = valueType;

            var corespAccountList = accountSheetDetails.Select(f => new CorespAccount { Id = f.CorespAccountId, Name = f.CorespAccountSymbol + " - " + f.CorespAccountName })
                                                       .OrderBy(f => f.Name)
                                                       .ToList();
            corespAccountList = corespAccountList.GroupBy(f => f.Id).Select(g => g.First()).OrderBy(f => f.Name).ToList();
            var list = CompleteSynthetics(corespAccountList, model.TenantId);
            model.CorespAccountList = list;

            return model;
        }

        private List<CorespAccount> CompleteSynthetics(List<CorespAccount> list, int selectedAppClient)
        {
            var x = new List<CorespAccount>();

            foreach (var item in list)
            {
                x.Add(item);
                var account = _accountRepository.FirstOrDefault(f => f.Id == item.Id);
                if (account.SyntheticAccountId != null)
                {
                    var syntheticAccount = _accountRepository.GetAll()
                                                             .FirstOrDefault(f => f.TenantId == selectedAppClient && f.Status == State.Active
                                                             && f.SyntheticAccount == account.SyntheticAccount && f.SyntheticAccountId != null);
                    if (syntheticAccount != null && !x.Select(f => f.Id).Contains(syntheticAccount.Id))
                    {
                        x.Add(new CorespAccount { Id = syntheticAccount.Id, Name = syntheticAccount.Symbol + " - " + syntheticAccount.AccountName });
                    }
                }
            }

            x = x.OrderBy(f => f.Name).ToList();

            return x;
        }

        public FisaContModel FisaContChangeCoresp(FisaContModel model)
        {
            if (model.CorespAccountId != null)
            {
                var accountList = _accountRepository.GetAllAnalythicsSintetic(model.CorespAccountId ?? 0).Select(f => f.Id).Distinct();
                model.OperationsDetailSelection = model.OperationsDetail.Where(f => accountList.Contains(f.CorespAccountId)).ToList();
            }
            else
            {
                model.OperationsDetailSelection = model.OperationsDetail;
            }
            model = FisaContCalcSoldAfterChange(model);
            return model;
        }

        private FisaContModel FisaContCalcSoldAfterChange(FisaContModel model)
        {
            var soldPrec = ((model.SoldPrecedentType == "C") ? -1 : 1) * model.SoldPrecedent;
            decimal totalDebit = 0, totalCredit = 0, soldCurr = soldPrec, soldFinal = 0;
            string valueType = "";
            foreach (var detail in model.OperationsDetailSelection)
            {
                totalDebit += detail.DebitValue;
                totalCredit += detail.CreditValue;
                soldCurr += (detail.DebitValue - detail.CreditValue);

                detail.Sold = Math.Abs(soldCurr);
                valueType = ((soldCurr > 0) ? ValueTypeEnum.D : ValueTypeEnum.C).ToString();
                detail.ValueType = valueType;
                soldFinal = Math.Abs(soldCurr);
                //}

                detail.Sold = (model.CorespAccountId != null) ? (decimal?)null : detail.Sold;
                detail.ValueType = (model.CorespAccountId != null) ? null : detail.ValueType;
            }

            model.TotalDebit = totalDebit;
            model.TotalCredit = totalCredit;
            model.SoldFinal = soldFinal;
            model.SoldFinalType = valueType;

            model.SoldFinal = (model.CorespAccountId != null) ? (decimal?)null : model.SoldFinal;
            model.SoldFinalType = (model.CorespAccountId != null) ? null : model.SoldFinalType;

            return model;
        }

        //[UnitOfWork]
        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.FisaCont")]
        public virtual FisaContModel FisaContReport(int accountId, DateTime startDate, DateTime endDate, int currencyId, int? corespAccountId)
        {
            var appClient = 1; // GetCurrentTenant();
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var currency = _currencyRepository.FirstOrDefault(f => f.Id == currencyId);
            var localCurrencyId = 1;
            var localCurrency = _currencyRepository.FirstOrDefault(f => f.Id == localCurrencyId);

            corespAccountId = corespAccountId == 0 ? null : corespAccountId;

            var ret = new FisaContModel
            {
                AppClientName = person.Name,
                AppClientId1 = person.Id1,
                AppClientId2 = person.Id2,
                StartDate = startDate,
                EndDate = endDate,
                LocalCurrencyId = localCurrency.Id,
                LocalCurrency = localCurrency.CurrencyCode,
                CurrencyId = currency.Id,
                Currency = currency.CurrencyCode,
                SoldInitial = 0,
                SoldPrecedent = 0,
                TotalCredit = 0,
                TotalDebit = 0,
                ShowDetails = false,
                TenantId = appClient,
                AccountId = accountId,
                CorespAccountId = corespAccountId
            };

            if (corespAccountId != null)
            {
                var corespAccount = _accountRepository.GetAll().FirstOrDefault(f => f.Id == corespAccountId);
                ret.CorespAccountName = corespAccount.Symbol + " - " + corespAccount.AccountName;
            }

            ret = FisaContView(ret);
            ret = FisaContChangeCoresp(ret);

            return ret;
        }

        public List<AccountListDDDto> AccountList()
        {
            var appClient = 1;// GetCurrentTenant();

            var _accounts = _accountRepository.GetAll()
                                                 .Where(f => f.TenantId == appClient && f.Status == State.Active)
                                                 .ToList()
                                                 .OrderBy(f => f.Symbol).ToList();
            var ret = ObjectMapper.Map<List<AccountListDDDto>>(_accounts);
            return ret;
        }

        //[UnitOfWork]
        public virtual List<RegistruCasaModel> RegistruCasaDateRange(DateTime dataStart, DateTime dataEnd, int currencyId)
        {
            var previousDate = dataStart.AddDays(-1);
            var soldPrec = _dispositionRepository.SoldPrec(previousDate, currencyId);

            var appClient = 1; // GetCurrentTenant();
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var currencyName = _currencyRepository.Get(currencyId).CurrencyName;

            var registruList = new List<RegistruCasaModel>();

            var operationsDate = _dispositionRepository.GetAllIncluding(f => f.DocumentType).Where(f => f.State == State.Active &&
                                                                     f.DispositionDate >= dataStart && f.DispositionDate <= dataEnd && f.CurrencyId == currencyId
                                                                     && f.OperationType != OperationType.SoldInitial)
                                                                    .OrderBy(f => f.DispositionDate).Select(f => f.DispositionDate).ToList();
            foreach (var date in operationsDate.Distinct())
            {
                var list = _dispositionRepository.GetAllIncluding(f => f.DocumentType).Where(f => f.State == State.Active &&
                                                                     f.DispositionDate == date && f.CurrencyId == currencyId && f.OperationType != OperationType.SoldInitial).ToList();

                var oper = ObjectMapper.Map<List<DispositionListDto>>(list);
                var soldCurr = soldPrec + oper.Sum(f => f.SumOper);
                var registru = new RegistruCasaModel
                {
                    Dispositions = oper,
                    StartDate = date,
                    SoldPrec = soldPrec,
                    SoldCurr = soldCurr,
                    CurrencyName = currencyName,
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    AppClientName = person.Name,
                    TenantId = 1//appClient.Id
                };
                registruList.Add(registru);

                soldPrec = soldCurr;
            }

            return registruList;
        }

        [UnitOfWork]
        public virtual RegistruCasaModel RegistruCasa(DateTime dataEnd, int currencyId)
        {
            var previousDate = dataEnd.AddDays(-1);
            var soldPrec = _dispositionRepository.SoldPrec(previousDate, currencyId);

            var appClient = 1;//GetCurrentTenant();
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
            var currencyName = _currencyRepository.Get(currencyId).CurrencyName;

            var operations = _dispositionRepository.GetAllIncluding(f => f.DocumentType).Where(f => f.State == State.Active && f.DispositionDate == dataEnd &&
                                                                                               f.CurrencyId == currencyId && f.OperationType != OperationType.SoldInitial)
                                                   .ToList();
            decimal soldCurr = 0;
            soldCurr = soldPrec + operations.Sum(f => f.SumOper);

            var oper = ObjectMapper.Map<List<DispositionListDto>>(operations);
            var registru = new RegistruCasaModel()
            {
                AppClientId1 = person.Id1,
                AppClientId2 = person.Id2,
                AppClientName = person.Name,
                TenantId = appClient,
                CurrencyName = currencyName,
                SoldCurr = soldCurr,
                SoldPrec = soldPrec,
                StartDate = dataEnd,
                Dispositions = oper
            };

            return registru;
        }

        //[UnitOfWork]
        //public virtual decimal SoldPrec(DateTime previousDate, int currencyId)
        //{
        //    decimal value = 0;
        //    DateTime startDate = new DateTime();
        //    var soldInit = _dispositionRepository.GetAll().Where(f => f.State == State.Active && f.OperationType == OperationType.SoldInitial
        //                                                         && f.DispositionDate < previousDate && f.CurrencyId == currencyId)
        //                                                    .OrderByDescending(f => f.DispositionDate)
        //                                                  .FirstOrDefault();
        //    if (soldInit == null)
        //    {
        //        startDate = new DateTime(2000, 1, 1);
        //    }
        //    else
        //    {
        //        value = soldInit.SumOper;
        //        startDate = soldInit.DispositionDate;
        //    }
        //    var operValue = _dispositionRepository.GetAll().Where(f => f.State == State.Active && f.OperationType != OperationType.SoldInitial
        //                                                               && f.DispositionDate >= startDate && f.DispositionDate <= previousDate && f.CurrencyId == currencyId)
        //                                                   .Sum(f => f.SumOper);
        //    value += operValue;

        //    return value;
        //}

        //[UnitOfWork]
        public virtual PrepaymentsRegistruReport PrepaymentsRegReport(DateTime repDate, int prepaymentType)
        {
            var appClient = 1;
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
            var _prepaymentType = (PrepaymentType)prepaymentType;

            var ret = new PrepaymentsRegistruReport();
            ret.AppClientName = person.Name;
            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;
            ret.RegDate = repDate;
            ret.PrepaymentType = _prepaymentType;
            ret.ReportName = ((_prepaymentType == PrepaymentType.CheltuieliInAvans) ? " Cheltuieli " : " Venituri ") + " inregistrate in avans la data " + repDate.ToString("dd/MM/yyyy");

            var setup = _prepaymentsDurationSetupRepository.GetAll().FirstOrDefault();
            PrepaymentDurationCalc modCalcul = (setup == null) ? PrepaymentDurationCalc.Lunar : setup.PrepaymentDurationCalc;
            ret.ModCalc = (int)modCalcul;

            ret.RegDetails = new List<PrepaymentsRegistruDetails>();

            var prepaymentsList = _prepaymentsBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                    .Where(f => f.ComputeDate <= repDate && f.Prepayment.PrepaymentType == _prepaymentType)
                                                    .GroupBy(f => f.PrepaymentId)
                                                    .Select(f => f.Max(x => x.Id))
                                                    .ToList();

            var stockList = _prepaymentsBalanceRepository.GetAllIncluding(f => f.Prepayment, f => f.Prepayment.PrepaymentAccount, f => f.Prepayment.DepreciationAccount, f => f.Prepayment.PrepaymentAccountVAT,
                                                                          f => f.Prepayment.PrimDocumentType, f => f.Prepayment.ThirdParty, f => f.Prepayment.ThirdParty, f => f.Prepayment.InvoiceDetails,
                                                                          f => f.Prepayment.InvoiceDetails.Invoices, f => f.Prepayment.InvoiceDetails.InvoiceElementsDetails)
                                                    .Where(f => prepaymentsList.Contains(f.Id) && f.Quantity != 0)
                                                    .ToList();
            var repList = new List<PrepaymentsRegistruDetails>();
            foreach (var item in stockList)
            {
                var listItem = new PrepaymentsRegistruDetails
                {
                    PrepaymentName = item.Prepayment.Description + (item.Prepayment.InvoiceDetails != null ? ", " + item.Prepayment.InvoiceDetails.InvoiceElementsDetails.Description : ""),
                    SyntheticAccount = item.Prepayment.DepreciationAccount.Symbol,
                    AccountForGroup = item.Prepayment.PrepaymentAccount.Symbol + " - " + item.Prepayment.PrepaymentAccount.AccountName,
                    AccountName = item.Prepayment.DepreciationAccount.AccountName,
                    PrepaymentValue = item.PrepaymentValue + item.Deprec,
                    RemainingPrepaymentValue = item.PrepaymentValue,
                    MonthlyDepreciation = item.MontlyCharge,
                    Depreciation = item.Deprec,
                    Duration = item.Prepayment.DurationInMonths,
                    RemainingDuration = item.Duration,
                    DocumentType = (item.Prepayment.PrimDocumentType != null) ? item.Prepayment.PrimDocumentType.TypeNameShort : "",
                    DocumentNr = item.Prepayment.PrimDocumentNr,
                    DocumentDate = item.Prepayment.PrimDocumentDate,
                    DepreciationStartDate = item.Prepayment.DepreciationStartDate,
                    ThirdParty = (item.Prepayment.ThirdParty != null) ? item.Prepayment.ThirdParty.FullName : ""
                };
                repList.Add(listItem);
                if (item.PrepaymentVAT != 0 || item.Prepayment.PrepaymentVAT != 0)
                {
                    var listItemVAT = new PrepaymentsRegistruDetails
                    {
                        PrepaymentName = item.Prepayment.Description + (item.Prepayment.InvoiceDetails != null ? ", " + item.Prepayment.InvoiceDetails.InvoiceElementsDetails.Description : ""),
                        SyntheticAccount = item.Prepayment.DepreciationAccount.Symbol,
                        AccountForGroup = item.Prepayment.PrepaymentAccountVAT.Symbol + " - " + item.Prepayment.PrepaymentAccountVAT.AccountName,
                        AccountName = item.Prepayment.DepreciationAccount.AccountName,
                        PrepaymentValue = item.PrepaymentVAT + item.DeprecVAT,
                        RemainingPrepaymentValue = item.PrepaymentVAT,
                        MonthlyDepreciation = item.MontlyChargeVAT,
                        Depreciation = item.DeprecVAT,
                        Duration = item.Prepayment.DurationInMonths,
                        RemainingDuration = item.Duration,
                        DocumentType = (item.Prepayment.PrimDocumentType != null) ? item.Prepayment.PrimDocumentType.TypeNameShort : "",
                        DocumentNr = item.Prepayment.PrimDocumentNr,
                        DocumentDate = item.Prepayment.PrimDocumentDate,
                        DepreciationStartDate = item.Prepayment.DepreciationStartDate,
                        ThirdParty = (item.Prepayment.ThirdParty != null) ? item.Prepayment.ThirdParty.FullName : ""
                    };
                    repList.Add(listItemVAT);
                }
            }

            ret.RegDetails = repList;

            return ret;
        }

        //[AbpAuthorize("Conta.MF.Rapoarte.Acces")]
        public int InventoryNrExists(int inventoryNr)
        {
            int rez = 0; // exista
            int appClientId = 1;
            var asset = _imoAssetItemRepository.FirstOrDefault(f => f.TenantId == appClientId && f.InventoryNr == inventoryNr && f.State == State.Active);
            if (asset == null)
            {
                rez = 1;
            }

            return rez;
        }

        //[UnitOfWork]
        public virtual ImoAssetRegistruReport AssetRegistruReport(DateTime repDate, int? storage)
        {
            var appClient = 1;
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var ret = new ImoAssetRegistruReport();
            ret.AppClientName = person.Name;
            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;
            ret.RegDate = repDate;
            ret.RegDetails = new List<ImoAssetRegistruDetails>();

            var assetList = _imoAssetStockRepository.GetAll()
                                                    .Where(f => f.StockDate <= repDate && f.TenantId == appClient)
                                                    .GroupBy(f => f.ImoAssetItemId)
                                                    .Select(f => f.Max(x => x.Id))
                                                    .ToList();

            var stockList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetItem.AssetAccount, f => f.ImoAssetItem.PrimDocumentType,
                                                                          f => f.ImoAssetItem.ThirdParty, f => f.Storage)
                                                   .Where(f => assetList.Contains(f.Id) && f.Quantity != 0)
                                                   .ToList();

            var repList = new List<ImoAssetRegistruDetails>();
            foreach (var item in stockList.OrderBy(f => f.ImoAssetItem.InventoryNr))
            {
                var listItem = new ImoAssetRegistruDetails
                {
                    ImoAssetName = item.ImoAssetItem.Name,
                    InventoryNr = item.ImoAssetItem.InventoryNr,
                    SyntheticAccount = _accountRepository.GetAccountById(item.AssetAccountId).Symbol,
                    AccountForGroup = item.AssetAccount.Symbol + " - " + item.AssetAccount.AccountName,
                    AccountName = item.ImoAssetItem.AssetAccount.AccountName,
                    InventoryValue = item.InventoryValue + item.Deprec,
                    RemainingInventoryValue = item.InventoryValue,
                    MonthlyDepreciation = item.MonthlyDepreciation,
                    Depreciation = item.Deprec,
                    Duration = item.ImoAssetItem.DurationInMonths,
                    RemainingDuration = item.Duration,
                    DocumentType = (item.ImoAssetItem.PrimDocumentType != null) ? item.ImoAssetItem.PrimDocumentType.TypeNameShort : "",
                    DocumentNr = item.ImoAssetItem.PrimDocumentNr,
                    DocumentDate = item.ImoAssetItem.PrimDocumentDate,
                    DepreciationStartDate = item.ImoAssetItem.DepreciationStartDate ?? null,
                    ThirdParty = (item.ImoAssetItem.ThirdParty != null) ? item.ImoAssetItem.ThirdParty.FullName : "",
                    StorageId = item.StorageId,
                    Storage = item.Storage.StorageName
                };
                repList.Add(listItem);
            }

            if (storage != 0)
            {
                repList = repList.Where(f => f.StorageId == storage).ToList();
                var storageName = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == storage);
                //ret.Parameters = "Gestiune: " + storageName.StorageName;
            }

            ret.RegDetails = repList;
            return ret;
        }

        //[UnitOfWork]
        public virtual ImoAssetFisaReport AssetFisa(DateTime dataFisa, int inventoryNr)
        {
            var appClient = 1;
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var ret = new ImoAssetFisaReport();
            ret.AppClientName = person.Name;
            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;

            var asset = _imoAssetItemRepository.GetAllIncluding(f => f.AssetClassCodes, f => f.ThirdParty, f => f.ImoAssetStorage, f => f.PrimDocumentType)
                                               .FirstOrDefault(f => f.InventoryNr == inventoryNr && f.State == State.Active);
            if (asset == null)
            {
                throw new Exception("Nu am identificat mijlocul fix cu numarul de inventar " + inventoryNr.ToString());
            }

            ret.InventoryNr = asset.InventoryNr;
            ret.ImoAssetName = asset.Name;
            ret.DocumentType = (asset.PrimDocumentType != null) ? asset.PrimDocumentType.TypeNameShort : "";
            ret.DocumentNr = asset.PrimDocumentNr;
            ret.DocumentDate = asset.PrimDocumentDate;
            ret.ClassCode = (asset.AssetClassCodes == null) ? null : asset.AssetClassCodes.Code;
            ret.ClassCodeNormDuration = null;
            if (asset.AssetClassCodes != null)
            {
                ret.ClassCodeNormDuration = asset.AssetClassCodes.DurationMax / 12;
            }

            if (asset.UseStartDate != null)
            {
                ret.UseStartDate = asset.UseStartDate.Value;
            }

            var assetStockId = _imoAssetStockRepository.GetAll()
                                                       .Where(f => f.StockDate <= dataFisa && f.TenantId == appClient && f.ImoAssetItemId == asset.Id)
                                                       .GroupBy(f => f.ImoAssetItemId)
                                                       .Select(f => f.Max(x => x.Id))
                                                       .FirstOrDefault();
            var assetStock = _imoAssetStockRepository.GetAll()
                                                     .FirstOrDefault(f => f.Id == assetStockId);
            ret.InventoryValue = assetStock.InventoryValue + assetStock.Deprec;
            if (assetStock.MonthlyDepreciation == 0)
            {
                if (asset.MonthlyDepreciation != null)
                {
                    ret.MonthlyDepreciation = asset.MonthlyDepreciation.Value;
                }
                else
                {
                    ret.MonthlyDepreciation = 0;
                }
            }
            else
            {
                ret.MonthlyDepreciation = assetStock.MonthlyDepreciation;
            }

            ret.ProcDeprec = (ret.InventoryValue == 0) ? 100 : (assetStock.Deprec / ret.InventoryValue * 100);
            ret.ProcDeprec = Math.Round(ret.ProcDeprec, 2);

            if (assetStock.Duration != 0) // daca la data de calcul mai am durata de amortizare o adun la data
            {
                ret.DepreciationEnd = assetStock.StockDate.AddMonths(assetStock.Duration);
            }
            else
            {
                try
                {
                    var durAssetStockId = _imoAssetStockRepository.GetAll()
                                                                  .Where(f => f.StockDate <= dataFisa && f.TenantId == appClient && f.ImoAssetItemId == asset.Id && f.Duration != 0)
                                                                  .GroupBy(f => f.ImoAssetItemId)
                                                                  .Select(f => f.Max(x => x.Id))
                                                                  .FirstOrDefault();
                    var durAssetStock = _imoAssetStockRepository.GetAll()
                                                                .FirstOrDefault(f => f.Id == durAssetStockId);
                    ret.DepreciationEnd = durAssetStock.StockDate.AddMonths(durAssetStock.Duration);
                }
                catch
                {
                    ret.DepreciationEnd = asset.DepreciationStartDate.Value.AddMonths(asset.DurationInMonths);
                    var nrDays = DateTime.DaysInMonth(ret.DepreciationEnd.Year, ret.DepreciationEnd.Month);
                    ret.DepreciationEnd = new DateTime(ret.DepreciationEnd.Year, ret.DepreciationEnd.Month, nrDays);
                }
            }

            var fisaOperList = new List<ImoAssetFisaDetail>();

            var stockList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetOperDet, f => f.ImoAssetOperDet.ImoAssetOper)
                                                     .Where(f => f.ImoAssetItemId == asset.Id && f.StockDate <= dataFisa/* && f.OperType != ImoAssetOperType.AmortizareLunara*/)
                                                     .OrderBy(f => f.StockDate).ThenBy(f => f.ImoAssetItemId)
                                                     .ToList();

            foreach (var item in stockList)
            {
                if (item.OperType == ImoAssetOperType.Intrare)
                {
                    var constituire = new ImoAssetFisaDetail
                    {
                        OperationDate = item.StockDate,
                        DocumentType = (asset.PrimDocumentType != null) ? asset.PrimDocumentType.TypeNameShort : "",
                        DocumentNr = asset.PrimDocumentNr,
                        DocumentDate = asset.PrimDocumentDate,
                        ThirdParty = (asset.ThirdParty == null) ? null : asset.ThirdParty.FullName,
                        OperType = item.OperType.ToString(),
                        Storage = (asset.ImoAssetStorage == null) ? null : asset.ImoAssetStorage.StorageName,
                        Debit = item.TranzInventoryValue > 0 ? item.TranzInventoryValue : 0,
                        Credit = item.TranzInventoryValue < 0 ? -1 * item.TranzInventoryValue : 0,
                        Sold = item.InventoryValue /*+ item.Deprec*/
                    };
                    fisaOperList.Add(constituire);
                }
                else if (item.OperType == ImoAssetOperType.PunereInFunctiune)
                {
                    var punereInFunctiune = new ImoAssetFisaDetail
                    {
                        OperationDate = item.StockDate,
                        DocumentType = (asset.PrimDocumentType != null) ? asset.PrimDocumentType.TypeNameShort : "",
                        DocumentNr = asset.PrimDocumentNr,
                        DocumentDate = asset.PrimDocumentDate,
                        ThirdParty = null,
                        OperType = item.OperType.ToString(),
                        Storage = null,
                        Debit = item.TranzInventoryValue > 0 ? item.TranzInventoryValue : 0,
                        Credit = item.TranzInventoryValue < 0 ? -1 * item.TranzInventoryValue : 0,
                        Sold = item.InventoryValue/* + item.Deprec*/
                    };
                    fisaOperList.Add(punereInFunctiune);
                }
                else if (item.OperType == ImoAssetOperType.AmortizareLunara)
                {
                    var amortizare = new ImoAssetFisaDetail
                    {
                        OperationDate = item.StockDate,
                        DocumentType = (asset.PrimDocumentType != null) ? asset.PrimDocumentType.TypeNameShort : "",
                        DocumentNr = asset.PrimDocumentNr,
                        DocumentDate = asset.PrimDocumentDate,
                        ThirdParty = null,
                        OperType = item.OperType.ToString(),
                        Storage = null,
                        Debit = item.TranzInventoryValue > 0 ? item.TranzInventoryValue : 0,
                        Credit = item.TranzInventoryValue < 0 ? -1 * item.TranzInventoryValue : 0,
                        Sold = item.InventoryValue /*+ item.Deprec*/
                    };
                    fisaOperList.Add(amortizare);
                }
                else
                {
                    var oper = new ImoAssetFisaDetail
                    {
                        OperationDate = item.StockDate,
                        DocumentType = (item.ImoAssetOperDet.ImoAssetOper.DocumentType != null) ? item.ImoAssetOperDet.ImoAssetOper.DocumentType.TypeNameShort : "",
                        DocumentNr = item.ImoAssetOperDet.ImoAssetOper.DocumentNr.ToString(),
                        DocumentDate = item.ImoAssetOperDet.ImoAssetOper.DocumentDate,
                        ThirdParty = null,
                        OperType = item.OperType.ToString(),
                        Storage = null,
                        Debit = item.TranzInventoryValue > 0 ? item.TranzInventoryValue : 0,
                        Credit = item.TranzInventoryValue < 0 ? -1 * item.TranzInventoryValue : 0,
                        Sold = item.InventoryValue /*+ item.Deprec*/
                    };
                    fisaOperList.Add(oper);
                }
            }

            ret.FisaDetail = fisaOperList;

            return ret;
        }

        //[UnitOfWork]
        public virtual ImoAssetRegV2Report AssetRegistruV2Report(DateTime repDate, int? storage)
        {
            var appClient = 1;
            var ret = new ImoAssetRegV2Report();
            var startDate = new DateTime(repDate.Year, 1, 1);

            ret.OperationDate = repDate;
            ret.ImoAssetDetails = new List<ImoAssetV2Details>();

            var assetList = _imoAssetStockRepository.GetAll()
                                        .Where(f => f.StockDate <= repDate && f.TenantId == appClient)
                                        .GroupBy(f => f.ImoAssetItemId)
                                        .Select(f => f.Max(x => x.Id))
                                        .ToList();

            var stockList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetItem.AssetAccount, f => f.ImoAssetItem.DepreciationAccount, f => f.ImoAssetItem.ExpenseAccount,
                                                                     f => f.ImoAssetOperDet, f => f.ImoAssetOperDet.ImoAssetOper, f => f.ImoAssetItem.AssetClassCodes,
                                                                     f => f.ImoAssetItem.PrimDocumentType, f => f.ImoAssetItem.ThirdParty, f => f.Storage, f => f.ImoAssetStockModerniz,
                                                                     f => f.ImoAssetItem.AssetClassCodes.ClassCodeParrent)
                                                   .Where(f => assetList.Contains(f.Id) && f.Quantity != 0)
                                                   .ToList();

            var outList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetItem.AssetAccount, f => f.ImoAssetItem.DepreciationAccount, f => f.ImoAssetItem.ExpenseAccount,
                                                                     f => f.ImoAssetOperDet, f => f.ImoAssetOperDet.ImoAssetOper, f => f.ImoAssetItem.AssetClassCodes,
                                                                     f => f.ImoAssetItem.PrimDocumentType, f => f.ImoAssetItem.ThirdParty, f => f.Storage, f => f.ImoAssetStockModerniz,
                                                                     f => f.ImoAssetItem.AssetClassCodes.ClassCodeParrent)
                                                   .Where(f => assetList.Contains(f.Id) && f.Quantity == 0 && startDate <= f.StockDate && f.StockDate <= repDate)
                                                   .ToList();
            stockList.AddRange(outList);

            if (storage != 0)
            {
                var storageName = _imoAssetStorageRepository.GetAll().FirstOrDefault(f => f.Id == storage && f.State == State.Active && f.TenantId == appClient).StorageName;
                ret.StorageName = storageName;
                stockList = stockList.Where(f => f.StorageId == storage).ToList();
            }

            var repList = new List<ImoAssetV2Details>();

            foreach (var item in stockList.OrderBy(f => f.ImoAssetItem.InventoryNr))
            {
                var listItem = new ImoAssetV2Details
                {
                    ImoAssetName = item.ImoAssetItem.Name,
                    InventoryNumber = item.ImoAssetItem.InventoryNr,
                    CategoryName = item.ImoAssetItem.AssetClassCodes != null ? (item.ImoAssetItem.AssetClassCodes.ClassCodeParrent == null ? item.ImoAssetItem.AssetClassCodes.Name : item.ImoAssetItem.AssetClassCodes.ClassCodeParrent.Name) : null,
                    AssetClassCodes = item.ImoAssetItem.AssetClassCodes != null ? item.ImoAssetItem.AssetClassCodes.Code : null,
                    DatePIF = item.ImoAssetItem.UseStartDate ?? null,
                    DateStartDeprec = item.ImoAssetItem.DepreciationStartDate ?? null,
                    DurataScursaUtila = item.Duration,
                    CurrentValue = item.InventoryValue,
                    InitialValue = item.ImoAssetItem.InventoryValue,
                    MonthlyDeprec = item.MonthlyDepreciation,
                    CumulativDeprec = item.Deprec,
                    StorageName = item.Storage.StorageName,
                    OutOfUse = item.OperType == ImoAssetOperType.Casare ? "Da" : "Nu",
                    IsSold = item.OperType == ImoAssetOperType.Vanzare ? "Da" : "Nu",
                    IsInUse = item.Quantity > 0 ? "Da" : "Nu",
                    ImoAssetAccount = item.ImoAssetItem.AssetAccountInUseId != null ? item.ImoAssetItem.AssetAccountInUse.Symbol : item.ImoAssetItem.AssetAccount.Symbol,
                    DeprecAccount = item.ImoAssetItem.DepreciationAccount != null ? item.ImoAssetItem.DepreciationAccount.Symbol : null,
                    ExpenseAccount = item.ImoAssetItem.ExpenseAccount != null ? item.ImoAssetItem.ExpenseAccount.Symbol : null,
                    HasModerniz = item.ImoAssetStockModerniz.Count > 0 ? "Da" : "Nu",
                    ShowModerniz = item.ImoAssetStockModerniz.Count > 0 ? true : false
                };

                var deprecYear = _imoAssetStockRepository.GetAll().Where(f => f.StockDate <= repDate && f.StockDate >= startDate && f.ImoAssetItemId == item.ImoAssetItemId && f.OperType == ImoAssetOperType.AmortizareLunara).Sum(f => f.TranzDeprec);
                listItem.YearDeprec = deprecYear;

                var duration = _imoAssetStockRepository.GetAll().Where(f => f.StockDate <= repDate && f.StockDate >= startDate && f.ImoAssetItemId == item.ImoAssetItemId && f.TranzDuration > 0 && f.OperType != ImoAssetOperType.Intrare).Sum(f => f.TranzDuration);
                listItem.DurataUtila = item.ImoAssetItem.DurationInMonths + duration;

                var pusInFunctiune = _imoAssetStockRepository.GetAll().Count(f => f.StockDate <= repDate && f.ImoAssetItemId == item.ImoAssetItemId
                                                                             && f.OperType == ImoAssetOperType.PunereInFunctiune);
                if ((pusInFunctiune != 0 || listItem.CumulativDeprec != 0) && listItem.IsSold == "Nu" && listItem.OutOfUse == "Nu")
                {
                    listItem.IsInUse = "Da";
                }
                else
                {
                    listItem.IsInUse = "Nu";
                }

                //var imoAssetOperValue = _imoAssetOperDetRepository.GetAll().Where(f => f.TenantId == appClient && f.ImoAssetItemId == item.ImoAssetItemId && f.ImoAssetOper.State == State.Active).Sum(f => f.InvValueModif);
                //listItem.InitialValue = item.ImoAssetItem.InventoryValue + imoAssetOperValue;

                if (listItem.ShowModerniz == true)
                {
                    var modernizDetails = new List<ImoAssetModerniz>();
                    foreach (var itemModerniz in item.ImoAssetStockModerniz)
                    {
                        var imoAssetOperDetail = _imoAssetOperDetRepository.GetAllIncluding(f => f.ImoAssetOper).FirstOrDefault(f => f.Id == itemModerniz.ImoAssetOperDetailId && f.TenantId == appClient && f.ImoAssetOper.OperationDate <= repDate && f.ImoAssetOper.State == State.Active);
                        var detail = new ImoAssetModerniz
                        {
                            DatePIF = imoAssetOperDetail.ImoAssetOper.OperationDate,
                            DateStartDeprec = LazyMethods.LastDayOfMonth(imoAssetOperDetail.ImoAssetOper.OperationDate),
                            DurataUtila = listItem.DurataUtila,
                            DurataScursaUtila = item.Duration,
                            InitialValue = imoAssetOperDetail.InvValueModif,
                            CurrentValue = itemModerniz.Moderniz,
                            MonthlyDeprec = (item.Duration == 0) ? 0 : itemModerniz.Moderniz / item.Duration,
                            CumulativDeprec = itemModerniz.DeprecModerniz,
                            StorageName = imoAssetOperDetail.ImoAssetItem.ImoAssetStorage.StorageName
                            //  YearDeprec = itemModerniz.DeprecModerniz
                        };
                        var deprecYearModerniz = item.ImoAssetStockModerniz.Where(f => f.ImoAssetOperDetailId == imoAssetOperDetail.Id && f.ImoAssetStock.StockDate <= repDate && f.ImoAssetStock.StockDate >= startDate && f.ImoAssetStock.OperType == ImoAssetOperType.AmortizareLunara).Sum(f => f.DeprecModerniz);// (f => f.DeprecModerniz);
                        detail.YearDeprec = deprecYearModerniz;

                        modernizDetails.Add(detail);
                    }
                    listItem.ImoAssetModernizDetails = modernizDetails.OrderBy(f => f.DatePIF).ToList();
                }
                repList.Add(listItem);
            }

            ret.ImoAssetDetails = repList;

            return ret;
        }

        //[UnitOfWork]

        public virtual InvObjectImoAssetReport InventariereReport(int invOperId, int inventoryType)
        {
            var appClient = 1;
            var ret = new InvObjectImoAssetReport();
            ret.TenantId = appClient;

            if (inventoryType == (int)InventoryTypes.MijloaceFixe)
            {
                var imoOper = _imoInventariereRepository.GetAll().FirstOrDefault(f => f.Id == invOperId && f.State == State.Active && f.TenantId == appClient);

                ret.OperationDate = imoOper.DataInventariere;

                ret.Parameters = "Lista de inventariere mijloace fixe";
                ret.InventoryDetails = new List<InventoryDetails>();

                var assetInvObjectStorageIdList = _imoInventariereDetailsRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetStock, f => f.ImoInventariere)
                                                                                   .Where(f => f.ImoInventariere.DataInventariere == imoOper.DataInventariere && f.TenantId == appClient &&
                                                                                               f.ImoInventariere.State == State.Active && f.ImoInventariereId == invOperId)
                                                                                   .Select(f => f.ImoAssetStock.StorageId)
                                                                                   .Distinct()
                                                                                   .ToList();
                var repList = new List<InventoryDetails>();
                foreach (var item in assetInvObjectStorageIdList)
                {
                    var storage = _imoAssetStorageRepository.GetAll().FirstOrDefault(f => f.Id == item && f.State == State.Active && f.TenantId == appClient);
                    ret.Storage = storage.StorageName;

                    var assetInvObjectList = _imoInventariereDetailsRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetStock, f => f.ImoInventariere)
                                                                              .Where(f => f.ImoInventariere.DataInventariere == imoOper.DataInventariere && f.TenantId == appClient &&
                                                                                          f.ImoInventariere.State == State.Active && f.ImoAssetStock.StorageId == item &&
                                                                                          f.ImoInventariereId == invOperId)
                                                                              .ToList();

                    foreach (var assetInv in assetInvObjectList)
                    {
                        var detail = new InventoryDetails
                        {
                            ImoAssetName = assetInv.ImoAssetItem.Name,
                            InventoryNr = assetInv.ImoAssetItem.InventoryNr,
                            OperationDate = assetInv.ImoAssetItem.UseStartDate,
                            StockScriptic = assetInv.ImoAssetStock.Quantity,
                            StockFaptic = (int)assetInv.StockFaptic,
                            UnitPrice = (assetInv.ImoAssetStock.InventoryValue + assetInv.ImoAssetStock.Deprec) / assetInv.ImoAssetStock.Quantity,
                            UM = "bucata",
                            InventoryValue = (assetInv.ImoAssetStock.InventoryValue + assetInv.ImoAssetStock.Deprec) / assetInv.ImoAssetStock.Quantity,
                            Storage = assetInv.ImoAssetStock.Storage.StorageName,
                            AccountingValue = assetInv.ImoAssetStock.InventoryValue / assetInv.ImoAssetStock.Quantity
                        };
                        repList.Add(detail);
                    }
                }
                ret.InventoryDetails = repList.OrderBy(f => f.InventoryNr).ToList();
            }
            else if (inventoryType == (int)InventoryTypes.ObiecteDeInventar)
            {
                var invOper = _invObjInventariereRepository.GetAll().FirstOrDefault(f => f.Id == invOperId && f.State == State.Active && f.TenantId == appClient);
                ret.OperationDate = invOper.DataInventariere;

                ret.Parameters = "Lista de inventariere obiecte de inventar";

                ret.InventoryDetails = new List<InventoryDetails>();

                var invObjectStorageIdList = _invObjInventariereDetailsRepository.GetAllIncluding(f => f.InvObjectInventariere, f => f.InvObjectStock, f => f.InvObjectStock.Storage)
                                                                                 .Where(f => f.InvObjectInventariere.DataInventariere == invOper.DataInventariere && f.TenantId == appClient &&
                                                                                             f.InvObjectInventariere.State == State.Active && f.InvObjectInventariereId == invOperId)
                                                                                 .Select(f => f.InvObjectStock.StorageId)
                                                                                 .Distinct()
                                                                                 .ToList();

                var repList = new List<InventoryDetails>();
                foreach (var item in invObjectStorageIdList)
                {
                    var storage = _invStorageRepository.GetAll().FirstOrDefault(f => f.Id == item && f.State == State.Active && f.TenantId == appClient);
                    ret.Storage = storage.StorageName;
                    var invObjectList = _invObjInventariereDetailsRepository.GetAllIncluding(f => f.InvObjectInventariere, f => f.InvObjectStock, f => f.InvObjectStock.Storage, f => f.InvObjectItem)
                                                                            .Where(f => f.InvObjectInventariere.DataInventariere == invOper.DataInventariere &&
                                                                                        f.TenantId == appClient && f.InvObjectInventariere.State == State.Active && f.InvObjectStock.StorageId == item &&
                                                                                        f.InvObjectInventariereId == invOperId)
                                                                            .ToList();

                    foreach (var inv in invObjectList)
                    {
                        var detail = new InventoryDetails
                        {
                            ImoAssetName = inv.InvObjectItem.Name,
                            InventoryNr = inv.InvObjectItem.InventoryNr,
                            OperationDate = inv.InvObjectItem.OperationDate,
                            StockScriptic = inv.InvObjectStock.Quantity,
                            StockFaptic = (int)inv.StockFaptic,
                            UnitPrice = inv.InvObjectStock.InventoryValue / inv.InvObjectStock.Quantity,
                            UM = "bucata",
                            InventoryValue = inv.InvObjectStock.InventoryValue / inv.InvObjectStock.Quantity,
                            Storage = inv.InvObjectStock.Storage.StorageName,
                            AccountingValue = inv.InvObjectStock.InventoryValue / inv.InvObjectStock.Quantity
                        };
                        repList.Add(detail);
                    }
                }
                ret.InventoryDetails = repList.OrderBy(f => f.InventoryNr).ToList();
            }
            return ret;
        }

        //[UnitOfWork]
        public virtual BonTransferModel BonTransferReport(int operationId, int inventoryType)
        {
            var appClient = 1;
            var ret = new BonTransferModel();

            ret.TenantId = appClient;

            if (inventoryType == (int)InventoryTypes.MijloaceFixe)
            {
                var imoOper = _imoAssetOperRepository.GetAllIncluding(f => f.AssetsStoreIn, f => f.AssetsStoreOut, f => f.PersonStoreIn, f => f.PersonStoreOut).FirstOrDefault(f => f.Id == operationId && f.State == State.Active && f.TenantId == appClient && (f.AssetsOperType == ImoAssetOperType.Transfer || f.AssetsOperType == ImoAssetOperType.BonMiscare));

                ret.StorageInName = imoOper.AssetsStoreIn.StorageName;
                ret.StorageOutName = imoOper.AssetsStoreOut.StorageName;
                ret.PersonStoreInName = (imoOper.PersonStoreIn != null ? imoOper.PersonStoreIn.FullName : "");
                ret.PersonStoreOutName = (imoOper.PersonStoreOut != null ? imoOper.PersonStoreOut.FullName : "");
                ret.DocumentNumber = imoOper.DocumentNr;
                ret.OperationDate = imoOper.OperationDate;
                ret.Parameters = "Bon de" + (imoOper.AssetsOperType == ImoAssetOperType.Transfer ? " transfer" : " miscare") + " mijloace fixe";

                ret.BonTransferDetails = new List<BonTransferDetail>();

                var imoOperList = _imoAssetOperDetRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetOper).Where(f => f.ImoAssetOper.OperationDate == imoOper.OperationDate &&
                                                                                                                            f.TenantId == appClient &&
                                                                                                                            f.State == State.Active &&
                                                                                                                            f.ImoAssetOper.Processed == true &&
                                                                                                                            f.ImoAssetOper.AssetsStoreInId == imoOper.AssetsStoreInId &&
                                                                                                                            f.ImoAssetOper.AssetsStoreOutId == imoOper.AssetsStoreOutId &&
                                                                                                                            f.ImoAssetOperId == operationId &&
                                                                                                                            (f.ImoAssetOper.AssetsOperType == ImoAssetOperType.Transfer || f.ImoAssetOper.AssetsOperType == ImoAssetOperType.BonMiscare)).ToList();

                var repList = new List<BonTransferDetail>();
                foreach (var item in imoOperList)
                {
                    var detail = new BonTransferDetail
                    {
                        Name = item.ImoAssetItem.Name,
                        InventoryNumber = item.ImoAssetItem.InventoryNr,
                        Quantity = item.ImoAssetItem.Quantity,
                        InventoryValue = item.ImoAssetItem.InventoryValue / item.ImoAssetItem.Quantity
                    };
                    repList.Add(detail);
                }

                ret.BonTransferDetails = repList;
            }
            else if (inventoryType == (int)InventoryTypes.ObiecteDeInventar)
            {
                var invObjectOper = _invOperationRepository.GetAllIncluding(f => f.InvObjectsStoreIn, f => f.InvObjectsStoreOut, f => f.PersonStoreIn, f => f.PersonStoreOut).FirstOrDefault(f => f.Id == operationId && f.State == State.Active && f.TenantId == appClient);

                ret.StorageInName = invObjectOper.InvObjectsStoreIn.StorageName;
                ret.StorageOutName = invObjectOper.InvObjectsStoreOut.StorageName;
                ret.PersonStoreInName = (invObjectOper.PersonStoreIn != null ? invObjectOper.PersonStoreIn.FullName : "");
                ret.PersonStoreOutName = (invObjectOper.PersonStoreOut != null ? invObjectOper.PersonStoreOut.FullName : "");
                ret.DocumentNumber = invObjectOper.DocumentNr;
                ret.OperationDate = invObjectOper.OperationDate;
                string tipBon = "";
                switch (invObjectOper.InvObjectsOperType)
                {
                    case InvObjectOperType.Transfer:
                        tipBon = "transfer";
                        break;

                    case InvObjectOperType.BonMiscare:
                        tipBon = "miscare";
                        break;

                    case InvObjectOperType.DareInConsum:
                        tipBon = "consum";
                        break;
                }
                ret.Parameters = "Bon de  " + tipBon + " obiecte de inventar";

                ret.BonTransferDetails = new List<BonTransferDetail>();

                var imoOperList = _invOperDetailRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectOper).Where(f => f.InvObjectOper.OperationDate == invObjectOper.OperationDate &&
                                                                                                                            f.TenantId == appClient &&
                                                                                                                            f.State == State.Active &&
                                                                                                                            f.InvObjectOper.Processed == true &&
                                                                                                                            f.InvObjectOper.InvObjectsStoreInId == invObjectOper.InvObjectsStoreInId &&
                                                                                                                            f.InvObjectOper.InvObjectsStoreOutId == invObjectOper.InvObjectsStoreOutId &&
                                                                                                                            f.InvObjectOperId == operationId &&
                                                                                                                            (f.InvObjectOper.InvObjectsOperType == InvObjectOperType.Transfer || f.InvObjectOper.InvObjectsOperType == InvObjectOperType.BonMiscare || f.InvObjectOper.InvObjectsOperType == InvObjectOperType.DareInConsum)).ToList();

                var repList = new List<BonTransferDetail>();
                foreach (var item in imoOperList)
                {
                    var detail = new BonTransferDetail
                    {
                        Name = item.InvObjectItem.Name,
                        InventoryNumber = item.InvObjectItem.InventoryNr,
                        Quantity = item.InvObjectItem.Quantity,
                        InventoryValue = item.InvObjectItem.InventoryValue / item.InvObjectItem.Quantity
                    };
                    repList.Add(detail);
                }

                ret.BonTransferDetails = repList;
            }

            return ret;
        }

        //[UnitOfWork]
        public virtual DispositionModel DispositionReport(int dispositionId, int operationType)
        {
            var appClient = 1;
            var personId = _personRepository.GetPersonTenantId(appClient);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
            var ret = new DispositionModel();

            var disposition = _dispositionRepository.GetAllIncluding(f => f.BankAccount, f => f.Currency, f => f.ThirdParty).FirstOrDefault(f => f.Id == dispositionId && f.State == State.Active && f.TenantId == appClient);

            // Plata
            if (operationType == (int)OperationType.Plata)
            {
                ret.HeaderParams = "DISPOZITIE DE PLATA CATRE CASIERIE";
                ret.FooterParams = "Platit suma de";
                ret.Serie_CI = disposition.ThirdParty.Id2;
                ret.PlataSum = disposition.Value;
                ret.PlataCurrencyCode = disposition.Currency.CurrencyCode;
            }
            else if (operationType == (int)OperationType.Incasare)
            {
                ret.HeaderParams = "DISPOZITIE DE INCASARE CATRE CASIERIE";
                ret.FooterParams = "Incasat suma de";
                ret.PlataSum = null;
                ret.PlataCurrencyCode = "";
                ret.NrChitanta = disposition.NrChitanta;
            }

            ret.AppClientName = person.FullName;
            ret.DocumentDate = disposition.DispositionDate;
            ret.DocumentNumber = disposition.DispositionNumber;
            ret.Sum = disposition.Value;
            ret.CurrencyCode = disposition.Currency.CurrencyCode;
            ret.Description = disposition.Description;
            ret.SumInWords = LazyMethods.CifreToLitere(Math.Truncate(disposition.Value));
            ret.Name = disposition.ThirdParty.FullName;
            ret.NumePrenume = disposition.NumePrenume;
            ret.TipDoc = disposition.TipDoc;
            ret.ActIdentitate = disposition.ActIdentitate;

            return ret;
        }

        //[UnitOfWork]
        public virtual List<DeclaratieCasierModel> DeclaratieCasier(DateTime date, string numeCasier, DateTime dataDecizie)
        {
            var appClient = 1;
            var ret = new List<DeclaratieCasierModel>();
            var intrariList = new List<DeclaratieCasierDetails>();

            var monedeList = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                   .Where(f => f.TenantId == appClient && f.DispositionDate <= date && f.State == State.Active)
                                                   .Select(f => f.CurrencyId)
                                                   .Distinct()
                                                   .ToList();

            foreach (var moneda in monedeList) // intrari
            {
                var model = new DeclaratieCasierModel();
                model.CurrencyId = moneda;
                model.CurrencyName = _currencyRepository.FirstOrDefault(f => f.Id == moneda).CurrencyName;
                // model.OperationType = dispo.OperationType.ToString();
                model.DataStart = date;
                model.DataDecizie = dataDecizie;
                model.NumeCasier = numeCasier.Replace('-', ' ');

                model.DetailsIn = new List<DeclaratieCasierDetails>();

                var maxData = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                    .Where(f => f.TenantId == appClient && f.DispositionDate <= date && f.State == State.Active && f.CurrencyId == moneda && f.OperationType == OperationType.Incasare)
                                                    .Max(f => f.DispositionDate);
                var disposition = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                        .Where(f => f.TenantId == appClient && f.DispositionDate == maxData && f.OperationType == OperationType.Incasare && f.State == State.Active && f.CurrencyId == moneda)
                                                        .OrderByDescending(f => f.Id)
                                                        .FirstOrDefault();
                if (disposition != null) // am gasit dispozitie => o preiau si am terminat
                {
                    model.DetailsIn.Add(new DeclaratieCasierDetails
                    {
                        Description = disposition.Description,
                        DispositionDate = disposition.DispositionDate,
                        DispositionNumber = disposition.DispositionNumber,
                        DocumentTypeName = disposition.DocumentType != null ? disposition.DocumentType.TypeName : " ",
                        Price = disposition.Value,
                        Quantity = 1,//invoiceDetail.Quantity,
                        Value = disposition.Value
                    });
                }
                else // caut depuneri/retrageri
                {
                    var retragere = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                          .Where(f => f.TenantId == appClient && f.DispositionDate == maxData && f.OperationType == OperationType.Retragere
                                                                 && f.State == State.Active && f.CurrencyId == moneda)
                                                          .OrderByDescending(f => f.Id)
                                                          .FirstOrDefault();
                    if (retragere != null) // am gasit retragere/depunere => o preiau si am terminat
                    {
                        model.DetailsIn.Add(new DeclaratieCasierDetails
                        {
                            Description = retragere.Description,
                            DispositionDate = retragere.DispositionDate,
                            DispositionNumber = retragere.DispositionNumber,
                            DocumentTypeName = retragere.DocumentType != null ? retragere.DocumentType.TypeName : " ",
                            Price = retragere.Value,
                            Quantity = 1,//invoiceDetail.Quantity,
                            Value = retragere.Value
                        });
                    }
                    else // caut sold initial
                    {
                        var soldInitial = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                          .Where(f => f.TenantId == appClient && f.DispositionDate == maxData && f.OperationType == OperationType.SoldInitial
                                                                 && f.State == State.Active && f.CurrencyId == moneda)
                                                          .OrderByDescending(f => f.Id)
                                                          .FirstOrDefault();

                        if (soldInitial != null)
                        {
                            model.DetailsIn.Add(new DeclaratieCasierDetails
                            {
                                Description = soldInitial.Description,
                                DispositionDate = soldInitial.DispositionDate,
                                DispositionNumber = soldInitial.DispositionNumber,
                                DocumentTypeName = soldInitial.DocumentType != null ? soldInitial.DocumentType.TypeName : "Sold initial",
                                Price = soldInitial.Value,
                                Quantity = 1,//invoiceDetail.Quantity,
                                Value = soldInitial.Value
                            });
                        }
                    }
                }
                if (model.DetailsIn.Count > 0)
                {
                    ret.Add(model);
                }
            }

            foreach (var moneda in monedeList) // iesiri
            {
                var model = new DeclaratieCasierModel();
                model.CurrencyId = moneda;
                model.CurrencyName = _currencyRepository.FirstOrDefault(f => f.Id == moneda).CurrencyName;
                // model.OperationType = dispo.OperationType.ToString();
                model.DataStart = date;
                model.DataDecizie = dataDecizie;
                model.NumeCasier = numeCasier.Replace('-', ' ');

                model.DetailsOut = new List<DeclaratieCasierDetails>();

                var maxData = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                    .Where(f => f.TenantId == appClient && f.DispositionDate <= date && f.State == State.Active && f.CurrencyId == moneda && f.OperationType == OperationType.Plata)
                                                    .Max(f => f.DispositionDate);

                var disposition = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                        .Where(f => f.TenantId == appClient && f.DispositionDate == maxData && f.OperationType == OperationType.Plata && f.State == State.Active && f.CurrencyId == moneda)
                                                        .OrderByDescending(f => f.Id)
                                                        .FirstOrDefault();
                if (disposition != null) // am gasit dispozitie => o preiau si am terminat
                {
                    model.DetailsOut.Add(new DeclaratieCasierDetails
                    {
                        Description = disposition.Description,
                        DispositionDate = disposition.DispositionDate,
                        DispositionNumber = disposition.DispositionNumber,
                        DocumentTypeName = disposition.DocumentType != null ? disposition.DocumentType.TypeName : " ",
                        Price = disposition.Value,
                        Quantity = 1,//invoiceDetail.Quantity,
                        Value = disposition.Value
                    });
                }
                else // caut depuneri/retrageri
                {
                    var retragere = _dispositionRepository.GetAllIncluding(f => f.DispositionInvoices, f => f.Currency, f => f.DocumentType)
                                                          .Where(f => f.TenantId == appClient && f.DispositionDate == maxData && f.OperationType == OperationType.Depunere
                                                                 && f.State == State.Active && f.CurrencyId == moneda)
                                                          .OrderByDescending(f => f.Id)
                                                          .FirstOrDefault();
                    if (retragere != null) // am gasit retragere/depunere => o preiau si am terminat
                    {
                        model.DetailsOut.Add(new DeclaratieCasierDetails
                        {
                            Description = retragere.Description,
                            DispositionDate = retragere.DispositionDate,
                            DispositionNumber = retragere.DispositionNumber,
                            DocumentTypeName = retragere.DocumentType != null ? retragere.DocumentType.TypeName : " ",
                            Price = retragere.Value,
                            Quantity = 1,//invoiceDetail.Quantity,
                            Value = retragere.Value
                        });
                    }
                }
                if (model.DetailsOut.Count > 0)
                {
                    ret.Add(model);
                }
            }

          

            var list = _cupiuriItemRepository.GetAllIncluding(f => f.CupiuriInit,f => f.CupiuriDetails, f => f.Currency).Where(f => f.CupiuriInit.OperationDate >= date && f.State == State.Active).ToList();
            
            foreach (var item in list)
            {

                var model = new DeclaratieCasierModel();
                model.CurrencyId = (int)item.CurrencyId;
                model.CurrencyName = _currencyRepository.FirstOrDefault(f => f.Id == item.CurrencyId).CurrencyName;
                // model.OperationType = dispo.OperationType.ToString();
                model.DataStart = date;
                model.DataDecizie = dataDecizie;
                model.NumeCasier = numeCasier.Replace('-', ' ');
                model.CupiuriDetails = ObjectMapper.Map<List<CupiuriDeclaratieCaserieDetails>>(item.CupiuriDetails);

                var sold = _dispositionRepository.GetAllIncluding(f => f.Currency).Where(f => f.State == State.Active &&
                                                                                         f.OperationType == OperationType.SoldInitial &&
                                                                                         f.DispositionDate >= date && f.CurrencyId == item.CurrencyId).FirstOrDefault();
                model.CupiuriSold = sold?.Value ?? 0;
                model.CupiuriTotal = (decimal)item.CupiuriDetails.Where(f => f.State == State.Active).Sum(f => f.Value * f.Quantity);


                if (model.CupiuriDetails.Count > 0)
                {
                    ret.Add(model);
                }

            }

          
            return ret;
        }

       

        //private List<DeclaratieCasierModel> DispositionsByCurrency(List<Disposition> operLsit, int item, DateTime date, string numeCasier, DateTime dataDecizie)
        //{
        //    var declaratieModel = new List<DeclaratieCasierModel>();
        //    var detIn = new List<DeclaratieCasierDetails>();
        //    var detOut = new List<DeclaratieCasierDetails>();
        //    var model = new DeclaratieCasierModel();
        //    foreach (var dispo in operLsit.Where(f => f.CurrencyId == item))
        //    {
        //        model.CurrencyId = item;
        //        model.CurrencyName = dispo.Currency.CurrencyName;
        //        model.OperationType = dispo.OperationType.ToString();
        //        model.DataStart = date;
        //        model.DataDecizie = dataDecizie;
        //        model.NumeCasier = numeCasier.Replace('-', ' ');

        //        DeclaratieCasierDetails detailIn;
        //        DeclaratieCasierDetails detailOut;

        //        if (dispo.OperationType == OperationType.Plata)
        //        {
        //            detailOut = new DeclaratieCasierDetails
        //            {
        //                Description = dispo.Description,
        //                DispositionDate = dispo.DispositionDate,
        //                DispositionNumber = dispo.DispositionNumber,
        //                DocumentTypeName = dispo.DocumentType.TypeName,
        //                Price = dispo.Value,
        //                Quantity = 1,//invoiceDetail.Quantity,
        //                Value = dispo.Value
        //            };
        //            detOut.Add(detailOut);
        //            model.DetailsOut = detOut;
        //        }
        //        else if (dispo.OperationType == OperationType.Incasare)
        //        {
        //            detailIn = new DeclaratieCasierDetails
        //            {
        //                Description = dispo.Description,
        //                DispositionDate = dispo.DispositionDate,
        //                DispositionNumber = dispo.DispositionNumber,
        //                DocumentTypeName = dispo.DocumentType.TypeName,
        //                Price = dispo.Value,
        //                Quantity = 1,//invoiceDetail.Quantity,
        //                Value = dispo.Value
        //            };
        //            detIn.Add(detailIn);
        //            model.DetailsIn = detIn;
        //        }
        //        else if (dispo.OperationType == OperationType.Retragere)
        //        {
        //            detailOut = new DeclaratieCasierDetails
        //            {
        //                Description = dispo.Description,
        //                DispositionDate = dispo.DispositionDate,
        //                DispositionNumber = dispo.DispositionNumber,
        //                DocumentTypeName = dispo.DocumentType != null ? dispo.DocumentType.TypeName : " ",
        //                Price = dispo.Value,
        //                Quantity = 1,//invoiceDetail.Quantity,
        //                Value = dispo.Value
        //            };
        //            detOut.Add(detailOut);
        //            model.DetailsOut = detOut;
        //        }
        //        else if (dispo.OperationType == OperationType.Depunere)
        //        {
        //            detailIn = new DeclaratieCasierDetails
        //            {
        //                Description = dispo.Description,
        //                DispositionDate = dispo.DispositionDate,
        //                DispositionNumber = dispo.DispositionNumber,
        //                DocumentTypeName = dispo.DocumentType != null ? dispo.DocumentType.TypeName : " ",
        //                Price = dispo.Value,
        //                Quantity = 1,//invoiceDetail.Quantity,
        //                Value = dispo.Value
        //            };
        //            detIn.Add(detailIn);
        //            model.DetailsIn = detIn;
        //        }
        //    }

        //    declaratieModel.Add(model);

        //    //var lastDisposition = dispositions.LastOrDefault(f => f.CurrencyId == item);

        //    ////var invoiceDetail = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails).FirstOrDefault(f => f.InvoicesId == lastDisposition.InvoiceId && f.State == State.Active);
        //    //declaratieModel.CurrencyId = item;
        //    //declaratieModel.CurrencyName = lastDisposition.Currency.CurrencyName;
        //    //declaratieModel.OperationType = lastDisposition.OperationType.ToString();

        //    //DeclaratieCasierDetails detailIn;
        //    //DeclaratieCasierDetails detailOut;

        //    //if (lastDisposition.OperationType == OperationType.Plata)
        //    //{
        //    //    detailOut = new DeclaratieCasierDetails
        //    //    {
        //    //        Description = lastDisposition.Description,
        //    //        DispositionDate = lastDisposition.DispositionDate,
        //    //        DispositionNumber = lastDisposition.DispositionNumber,
        //    //        DocumentTypeName = lastDisposition.DocumentType.TypeName,
        //    //        Price = lastDisposition.Value,
        //    //        Quantity = 1,//invoiceDetail.Quantity,
        //    //        Value = lastDisposition.Value
        //    //    };
        //    //    detOut.Add(detailOut);
        //    //    declaratieModel.DetailsOut = detOut;
        //    //}
        //    //else if (lastDisposition.OperationType == OperationType.Incasare)
        //    //{
        //    //    detailIn = new DeclaratieCasierDetails
        //    //    {
        //    //        Description = lastDisposition.Description,
        //    //        DispositionDate = lastDisposition.DispositionDate,
        //    //        DispositionNumber = lastDisposition.DispositionNumber,
        //    //        DocumentTypeName = lastDisposition.DocumentType.TypeName,
        //    //        Price = lastDisposition.Value,
        //    //        Quantity = 1,//invoiceDetail.Quantity,
        //    //        Value = lastDisposition.Value
        //    //    };
        //    //    detIn.Add(detailIn);
        //    //    declaratieModel.DetailsIn = detIn;
        //    //}

        //    return declaratieModel;
        //}

        //[UnitOfWork]
        public virtual ReportCalc CalculRaport(int reportId, DateTime reportStartDate, DateTime reportEndDate, bool isDateRange, bool rulaj, bool convertToLocalCurrency)
        {
            try
            {
                var appClient = 1;
                var personId = _personRepository.GetPersonTenantId(appClient);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

                var report = new ReportCalc();
                report.ReportCalcItems = new List<ReportCalcItem>();
                report.AppClientName = person.FullName;

                var reportName = _reportRepository.GetAll().FirstOrDefault(f => f.Id == reportId).ReportName;

                if (isDateRange)
                {
                    report.ReportTitle = reportName + " in perioada " + LazyMethods.DateToString(reportStartDate) + " - " + LazyMethods.DateToString(reportEndDate);
                    report.SubTitle = rulaj ? "Rulaje in perioada selectata" : "Solduri in perioada selectata";
                }
                else
                {
                    report.ReportTitle = reportName + " la data " + LazyMethods.DateToString(reportEndDate);
                }

                var calcRap = new List<ReportCalcItem>();

                calcRap = CalcRaportNoTotal(reportId, reportStartDate, reportEndDate, isDateRange, rulaj, convertToLocalCurrency); // calcul raport fara randul 'Total'
                //calcRap = CalcRaportTotal(reportId, reportStartDate, reportEndDate, isDateRange, rulaj); // calcul raport cu randul 'Total'

                var ret = calcRap.OrderBy(f => f.OrderView).ToList();
                report.ReportCalcItems.AddRange(ret);
                return report;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<ReportCalcItem> CalcRaportNoTotal(int reportId, DateTime reportStartDate, DateTime reportEndDate, bool isDateRange, bool rulaj, bool convertToLocalCurrency)
        {
            try
            {
                var report = _reportRepository.FirstOrDefault(f => f.Id == reportId);
                var rowList = _configReportRepository.GetAll().Where(f => f.ReportId == reportId).OrderBy(f => f.OrderView).ToList();
                var rapoarteList = _reportRepository.GetAll().Where(f => f.State == State.Active && f.ReportInitId == report.ReportInitId).ToList();
                var calcRap = new List<ReportCalcItem>();
                var appClientId = 1;
                var localCurrencyId = 1;
                DateTime operListStartDate = new DateTime();

                if (!isDateRange)
                {
                    reportStartDate = new DateTime(reportEndDate.Year, 1, 1);
                }
                if (reportStartDate.Year == 2020)
                {
                    operListStartDate = new DateTime(2020, 1, 1);
                }
                else
                {
                    operListStartDate = _balanceRepository.LastBalanceDay(reportStartDate);
                }

                var contaOperList = _balanceRepository.ContaOperationList(operListStartDate, reportEndDate, appClientId, 0, localCurrencyId, convertToLocalCurrency);

                // initializez clasa de calcul
                try
                {
                    if (isDateRange)
                    {
                        for (DateTime i = reportStartDate; i <= reportEndDate; i = i.AddDays(1))
                        {
                            if (i == LazyMethods.LastDayOfMonth(i) || /*i == reportStartDate ||*/ i == reportEndDate)
                            {
                                var list = rowList.Select(f => new ReportCalcItem
                                {
                                    ReportDate = i,
                                    RowName = f.RowName,
                                    RowValue = 0,
                                    OrderView = f.OrderView,
                                    ReportConfigRowId = f.Id,
                                    Bold = f.Bold,
                                    Calculat = false
                                }).ToList();
                                calcRap.AddRange(list);
                            }
                        }
                    }
                    else
                    {
                        calcRap = rowList.Select(f => new ReportCalcItem
                        {
                            ReportDate = reportEndDate,
                            RowName = f.RowName,
                            RowValue = 0,
                            OrderView = f.OrderView,
                            ReportConfigRowId = f.Id,
                            Bold = f.Bold,
                            Calculat = false
                        }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // calculez raportul pentru fiecare data
                foreach (var data in calcRap.OrderBy(f => f.ReportDate).Select(f => f.ReportDate).Distinct())
                {
                    var balance = _balanceRepository.GetBalanceAnyDate(data, false, appClientId, localCurrencyId, convertToLocalCurrency);

                    //calculez valorile
                    foreach (var item in rowList.Where(f => f.TotalRow == false).OrderBy(f => f.OrderView))
                    {
                        int modCalc = 2;

                        if (isDateRange)
                        {
                            if (data == reportStartDate || data == reportEndDate)
                            {
                                modCalc = 1;
                            }
                        }
                        else
                        {
                            modCalc = 1;
                        }
                        calcRap = _configReportRepository.CalcReportValue(item, modCalc, reportId, calcRap, data, appClientId, 0, localCurrencyId, rapoarteList, rulaj, contaOperList, balance);
                    }

                    // calculez totaluri
                    var calculatOK = false;
                    int contor = 1;

                    while (!calculatOK && contor < 50)
                    {
                        foreach (var item in rowList.Where(f => f.TotalRow == true).OrderBy(f => f.OrderView))
                        {
                            int modCalc = 2;
                            if (isDateRange)
                            {
                                if (data == reportStartDate || data == reportEndDate)
                                {
                                    modCalc = 1;
                                }
                            }
                            else
                            {
                                modCalc = 1;
                            }
                            calcRap = _configReportRepository.CalcReportValue(item, modCalc, reportId, calcRap, data, appClientId, 0, localCurrencyId, rapoarteList, rulaj, contaOperList, balance);
                        }

                        var count = calcRap.Count(f => f.ReportDate == data && !f.Calculat);
                        if (count == 0)
                            calculatOK = true;
                        contor++;
                    }
                    if (!calculatOK)
                    {
                        throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                    }
                }
                var ret = calcRap.OrderBy(f => f.OrderView).ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[UnitOfWork]
        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.RegistruInventar")]
        public virtual RegistruInventarReport RegistruInventarReport(DateTime reportDate)
        {
            try
            {
                var accountExceptList = _exceptRegInvRepository.GetAll().Where(f => f.State == State.Active).ToList();

                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var localCurrencyId = 1;

                var report = new RegistruInventarReport();
                report.AppClientName = person.FullName;
                report.ReportDate = reportDate;

                var reportList = new List<RegistruInventarItem>();
                var list = new List<RegInventar>();

                var accountsDb = _accountRepository.GetAnalythicsWithoutActivityType().ToList();

                try
                {
                    var balance = _balanceRepository.CreateTempBalance(reportDate, false, appClientId);
                    //list = _regInventarRepository.RegInventarCalc(accountsDb, accountExceptList, reportDate, appClientId, 0, localCurrencyId);
                    list = _regInventarRepository.RegInventarCalcBal(balance, accountsDb, accountExceptList, reportDate, appClientId, 0, localCurrencyId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var _regList = ObjectMapper.Map<List<RegistruInventarItem>>(list);
                report.RegistruInventarList = _regList.OrderBy(f => f.AccountName).ToList();
                return report;
            }
            catch (Exception ex)
            {
                throw ex;// new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[UnitOfWork]
        public virtual InvoiceModel InvoiceReport(int invoiceId)
        {
            try
            {
                var appClientId = 1;
                var localCurrencyId = 1;
                decimal exchangeRate = 0;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.GetAllIncluding(f => f.Bank, f => f.BankAccount).FirstOrDefault(f => f.Id == personId);

                var invoice = _invoiceRepository.GetAllIncluding(f => f.Currency, f => f.ThirdParty, f => f.DocumentType, f => f.InvoiceDetails, f => f.ThirdParty.AddressRegion, f => f.ThirdParty.AddressCountry)
                                                .FirstOrDefault(f => f.Id == invoiceId && f.State == State.Active && f.TenantId == appClientId);

                var invoiceModel = new InvoiceModel
                {
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    AppClientName = person.FullName,
                    AppClientAddress = person.AddressStreet ?? "" + ", " + person.AddressNo ?? "" + ", " + person.AddressBlock ?? "" + ", " + person.AddressApartment ?? "" + ", " + person.AddressFloor ?? "" + ", " +
                                        person.AddressLocality ?? "" + " , " + person.AddressRegion.RegionName,
                    AppClientBankAccount = _bankAccountRepository.FirstOrDefault(f => f.PersonId == personId && f.CurrencyId == localCurrencyId && f.TenantId == appClientId).IBAN,
                    AppClientBank = _personRepository.BanksForThirdParty(personId).FirstOrDefault(f => f.TenantId == appClientId).LegalPerson.Name,
                    ClientId1 = invoice.ThirdParty.Id1,
                    ClientId2 = invoice.ThirdParty.Id2,
                    ClientName = invoice.ThirdParty.FullName,
                    ClientAddress = (invoice.ThirdParty.AddressStreet != null ? invoice.ThirdParty.AddressStreet + ',' : null +
                                    invoice.ThirdParty.AddressNo != null ? invoice.ThirdParty.AddressNo + ',' : null +
                                    invoice.ThirdParty.AddressBlock != null ? invoice.ThirdParty.AddressBlock + ',' : null +
                                    invoice.ThirdParty.AddressApartment != null ? invoice.ThirdParty.AddressApartment + ',' : null +
                                    invoice.ThirdParty.AddressFloor != null ? invoice.ThirdParty.AddressFloor + ',' : null +
                                    invoice.ThirdParty.AddressLocality != null ? invoice.ThirdParty.AddressLocality.ToString() + ',' : null +
                                    invoice.ThirdParty.AddressCountry != null ? invoice.ThirdParty.AddressCountry.CountryName + ',' : null +
                                    invoice.ThirdParty.AddressRegion != null ? invoice.ThirdParty.AddressRegion.RegionName : null),
                    ClientBankAccount = (_bankAccountRepository.FirstOrDefault(f => f.PersonId == invoice.ThirdPartyId && f.CurrencyId == localCurrencyId && f.TenantId == appClientId) != null) ?
                                         _bankAccountRepository.FirstOrDefault(f => f.PersonId == invoice.ThirdPartyId && f.CurrencyId == localCurrencyId && f.TenantId == appClientId).IBAN : "",
                    ClientBank = (_personRepository.BanksForThirdParty(invoice.ThirdPartyId.Value).FirstOrDefault(f => f.TenantId == appClientId) != null) ?
                                  _personRepository.BanksForThirdParty(invoice.ThirdPartyId.Value).FirstOrDefault(f => f.TenantId == appClientId).LegalPerson.Name :
                                  "",
                    CurrencyName = _currencyRepository.FirstOrDefault(f => f.Id == localCurrencyId).CurrencyName,
                    InvoiceDate = invoice.InvoiceDate,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceSeries = invoice.InvoiceSeries,
                    Total = invoice.ValueLocalCurr,
                };

                var invoiceDetailsModel = new List<InvoiceDetailsReport>();

                if (invoice.CurrencyId != localCurrencyId)
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoice.InvoiceDate, invoice.CurrencyId, localCurrencyId);
                }

                foreach (var item in invoice.InvoiceDetails)
                {
                    var detail = new InvoiceDetailsReport
                    {
                        Description = item.Element,
                        Quantity = item.Quantity,
                        Value = (invoice.CurrencyId != localCurrencyId) ? item.Value * exchangeRate : item.Value
                    };
                    invoiceDetailsModel.Add(detail);
                }

                invoiceModel.InvoiceDetails = new List<InvoiceDetailsReport>();
                invoiceModel.InvoiceDetails = invoiceDetailsModel;

                return invoiceModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[UnitOfWork]
        public virtual SitFinanReportModel SitFinanReport(int balanceId, int raportId)
        {
            try
            {
                var ret = new SitFinanReportModel();
                ret = _reportManager.SitFinanRapIndicatori(balanceId, raportId);

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;//new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[UnitOfWork]
        public virtual InvObjectReportModel InvObjectReport(DateTime repDate, int? storageId)
        {
            try
            {
                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

                var ret = new InvObjectReportModel();
                ret.AppClientId1 = person.Id1;
                ret.AppClientId2 = person.Id2;
                ret.AppClientName = person.Name;
                ret.InvObjectDate = repDate;

                var invObjectList = _invObjectStockRepository.GetAll().Where(f => f.StockDate <= repDate && f.TenantId == appClientId).GroupBy(f => f.InvObjectItemId).Select(f => f.Max(x => x.Id)).ToList();
                var stockList = _invObjectStockRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectItem.InvObjectAccount, f => f.InvObjectItem.ExpenseAccount, f => f.InvObjectItem.ThirdParty,
                                                                          f => f.InvObjectItem.PrimDocumentType, f => f.Storage).Where(f => invObjectList.Contains(f.Id) && f.Quantity != 0).ToList();

                var rapList = new List<InvObjectReportDetail>();
                foreach (var item in stockList.OrderBy(f => f.InvObjectItem.InventoryNr))
                {
                    var rapItem = new InvObjectReportDetail
                    {
                        AccountForGroup = item.InvObjectItem.InvObjectAccount.Symbol + " - " + item.InvObjectItem.InvObjectAccount.AccountName,
                        DocumentDate = item.InvObjectItem.DocumentDate,
                        ExpenseAccount = item.InvObjectItem.ExpenseAccount.Symbol,
                        InUse = (item.InvObjectItem.Id == item.InvObjectItemId) ? "Da" : "Nu",
                        InUseDate = item.StockDate,
                        InventoryNr = item.InvObjectItem.InventoryNr,
                        InvObjectAccount = item.InvObjectItem.InvObjectAccount.Symbol,
                        InvObjectName = item.InvObjectItem.Name,
                        Storage = item.Storage.StorageName,
                        StorageId = item.StorageId,
                        ThirdParty = (item.InvObjectItem.ThirdParty != null) ? item.InvObjectItem.ThirdParty.FullName : "",
                        Value = item.InvObjectItem.InventoryValue
                    };
                    rapList.Add(rapItem);
                }

                if (storageId != 0)
                {
                    rapList = rapList.Where(f => f.StorageId == storageId).ToList();
                }

                ret.InvObjectDetails = rapList;
                return ret;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[UnitOfWork]
        public virtual BonConsumModel BonConsumReport(int operationId, int inventoryType)
        {
            var appClientId = 1;
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var ret = new BonConsumModel();

            ret.TenantId = appClientId;
            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;
            ret.AppClientName = person.Name;

            if (inventoryType == (int)InventoryTypes.ObiecteDeInventar)
            {
                var invObjectOper = _invOperationRepository.GetAllIncluding(f => f.InvObjectsStoreIn, f => f.InvObjectsStoreOut, f => f.PersonStoreIn, f => f.PersonStoreOut).FirstOrDefault(f => f.Id == operationId && f.State == State.Active && f.TenantId == appClientId);

                ret.PersonStoreInName = (invObjectOper.PersonStoreIn != null ? invObjectOper.PersonStoreIn.FullName : "");
                ret.PersonStoreOutName = (invObjectOper.PersonStoreOut != null ? invObjectOper.PersonStoreOut.FullName : "");
                ret.DocumentNumber = invObjectOper.DocumentNr;
                ret.OperationDate = invObjectOper.OperationDate;

                ret.BonConsumDetails = new List<BonConsumDetail>();

                var invOperList = _invOperDetailRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectOper).Where(f => f.InvObjectOper.OperationDate == invObjectOper.OperationDate &&
                                                                                                                            f.TenantId == appClientId &&
                                                                                                                            f.State == State.Active &&
                                                                                                                            f.InvObjectOper.Processed == true &&
                                                                                                                            f.InvObjectOper.InvObjectsStoreInId == invObjectOper.InvObjectsStoreInId &&
                                                                                                                            f.InvObjectOper.InvObjectsStoreOutId == invObjectOper.InvObjectsStoreOutId &&
                                                                                                                            f.InvObjectOperId == operationId &&
                                                                                                                            f.InvObjectOper.InvObjectsOperType == InvObjectOperType.DareInConsum).ToList();
                var repList = new List<BonConsumDetail>();
                foreach (var item in invOperList)
                {
                    var detail = new BonConsumDetail
                    {
                        Name = item.InvObjectItem.Name,
                        Quantity = item.InvObjectItem.Quantity,
                        InventoryValue = item.InvObjectItem.InventoryValue / item.InvObjectItem.Quantity
                    };
                    repList.Add(detail);
                }

                ret.BonConsumDetails = repList;
            }

            return ret;
        }

        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.SolduriConturiCurente")]
        public virtual List<SoldContCurentModel> SoldCurentReport(int? currencyId, int? accountId, DateTime? dataStart, DateTime? dataEnd, int? periodTypeId, bool isDateRange)
        {
            try
            {
                var list = new List<SoldContCurentModel>();
                var appClientId = 1;

                var accountList = _accountRepository.GetAllIncluding(f => f.BankAccount, f => f.Currency, f => f.BankAccount.Bank.LegalPerson, f => f.AnalyticAccounts)
                                                    .Where(f => f.TenantId == appClientId && f.Status == State.Active && f.AccountFuncType == AccountFuncType.ContBancar && f.BankAccount != null
                                                           && f.AnalyticAccounts.Count == 0)
                                                    .ToList();
                if (accountId != 0)
                {
                    accountList = accountList.Where(f => f.Id == accountId).ToList();
                }

                if (currencyId != 0)
                {
                    accountList = accountList.Where(f => f.CurrencyId == currencyId).ToList();
                }

                if (isDateRange)
                {
                    if (currencyId == 0)
                    {
                        switch (periodTypeId)
                        {
                            case (int)TipPerioadaSold.Lunar:
                                list = ComputeSoldMonthlyForAllCurrencies(dataStart, dataEnd, list, appClientId, accountList);
                                break;

                            case (int)TipPerioadaSold.Zilnic:
                                list = ComputeSoldDailyForAllCurrencies(dataStart, dataEnd, list, appClientId, accountList);
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (periodTypeId)
                        {
                            case (int)TipPerioadaSold.Lunar:
                                list = ComputeSoldMonthlyByCurrencyId(currencyId, dataStart, dataEnd, list, appClientId, accountList);
                                break;

                            case (int)TipPerioadaSold.Zilnic:
                                list = ComputeSoldDailyByCurrencyId(currencyId, dataStart, dataEnd, list, appClientId, accountList);
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if (currencyId == 0)
                    {
                        foreach (var currency in accountList.OrderBy(f => f.CurrencyId).Select(f => f.CurrencyId).Distinct())
                        {
                            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(dataEnd.Value, currency, 1);
                            var accountCurrList = accountList.Where(f => f.CurrencyId == currency).ToList();

                            var ret = ComputeSoldDetail(dataEnd, appClientId, accountCurrList, currency, exchangeRate);
                            ret.SoldDetails = ret.SoldDetails.OrderBy(f => f.IC).ToList();
                            list.Add(ret);
                        }
                    }
                    else
                    {
                        var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(dataEnd.Value, currencyId.Value, 1);
                        var ret = ComputeSoldDetail(dataEnd, appClientId, accountList, currencyId.Value, exchangeRate);
                        ret.SoldDetails = ret.SoldDetails.OrderBy(f => f.IC).ToList();
                        list.Add(ret);
                    }
                }
                foreach (var item in list)
                {
                    item.SoldDetails = item.SoldDetails.OrderBy(f => f.CurrentDate).ThenBy(f => f.CurrencyName).ThenBy(f => f.IC).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<SoldContCurentModel> ComputeSoldDailyByCurrencyId(int? currencyId, DateTime? dataStart, DateTime? dataEnd, List<SoldContCurentModel> list, int appClientId, List<Account> accountList)
        {
            var startPeriod = dataStart.Value; // LazyMethods.FirstDayNextMonth(dataStart.Value);
            var endPeriod = dataEnd.Value; // LazyMethods.LastDayOfMonth(startPeriod);
            while (endPeriod <= dataEnd.Value)
            {
                for (DateTime date = startPeriod; date <= endPeriod; date = date.AddDays(1))
                {
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(date, currencyId.Value, 1);

                    var ret = ComputeSoldDetail(date, appClientId, accountList, currencyId.Value, exchangeRate);
                    list.Add(ret);
                }
                startPeriod = LazyMethods.FirstDayNextMonth(endPeriod);
                endPeriod = startPeriod.Month == endPeriod.Month ? dataEnd.Value : LazyMethods.LastDayOfMonth(startPeriod);
            }
            return list;
        }

        private List<SoldContCurentModel> ComputeSoldMonthlyByCurrencyId(int? currencyId, DateTime? dataStart, DateTime? dataEnd, List<SoldContCurentModel> list, int appClientId, List<Account> accountList)
        {
            for (DateTime date = dataStart.Value; date < dataEnd.Value; date = date.AddMonths(1))
            {
                var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(date, currencyId.Value, 1);

                var ret = ComputeSoldDetail(LazyMethods.LastDayOfMonth(date), appClientId, accountList, currencyId.Value, exchangeRate);
                list.Add(ret);
            }
            return list;
        }

        private List<SoldContCurentModel> ComputeSoldDailyForAllCurrencies(DateTime? dataStart, DateTime? dataEnd, List<SoldContCurentModel> list, int appClientId, List<Account> accountList)
        {
            foreach (var currency in accountList.OrderBy(f => f.CurrencyId).Select(f => f.CurrencyId).Distinct())
            {
                var startPeriod = dataStart.Value; // (dataStart.Value.Day == 1) ? dataStart.Value : LazyMethods.FirstDayNextMonth(dataStart.Value);
                var endPeriod = dataEnd.Value; // startPeriod.Month == dataEnd.Value.Month ? dataEnd.Value : LazyMethods.LastDayOfMonth(startPeriod);

                while (endPeriod <= dataEnd.Value)
                {
                    for (DateTime date = startPeriod; date <= endPeriod; date = date.AddDays(1))
                    {
                        var exchangeRate = _exchangeRatesRepository.GetExchangeRate(date, currency, 1);

                        var ret = ComputeSoldDetail(date, appClientId, accountList, currency, exchangeRate);
                        list.Add(ret);
                    }
                    startPeriod = LazyMethods.FirstDayNextMonth(endPeriod);
                    endPeriod = startPeriod.Month == endPeriod.Month ? dataEnd.Value : LazyMethods.LastDayOfMonth(startPeriod);
                }
            }

            return list;
        }

        private List<SoldContCurentModel> ComputeSoldMonthlyForAllCurrencies(DateTime? dataStart, DateTime? dataEnd, List<SoldContCurentModel> list, int appClientId, List<Account> accountList)
        {
            foreach (var currency in accountList.OrderBy(f => f.CurrencyId).Select(f => f.CurrencyId).Distinct())
            {
                for (DateTime i = dataStart.Value; i < dataEnd.Value; i = i.AddMonths(1))
                {
                    var lastDayOfMonth = LazyMethods.LastDayOfMonth(i);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(lastDayOfMonth, currency, 1);

                    var ret = ComputeSoldDetail(lastDayOfMonth, appClientId, accountList, currency, exchangeRate);
                    list.Add(ret);
                }
            }
            return list;
        }

        private SoldContCurentModel ComputeSoldDetail(DateTime? dataEnd, int appClientId, List<Account> accountList, int currency, decimal exchangeRate)
        {
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var ret = new SoldContCurentModel();

            ret.AppClientId1 = person.Id1;
            ret.AppClientId2 = person.Id2;
            ret.AppClientName = person.FullName;
            ret.TipPerioada = TipPerioadaSold.Zilnic.ToString();
            var startDate = new DateTime(dataEnd.Value.Year, 1, 1);

            var soldDetails = new List<SoldDetail>();
            foreach (var item in accountList.Where(f => f.CurrencyId == currency))
            {
                var accList = _accountRepository.GetAllAnalythics(item.Id).Where(f => f.CurrencyId == currency).Select(f => f.Id).Distinct();
                foreach (var account in accList)
                {
                    var acc = accountList.FirstOrDefault(f => f.Id == account);
                    var soldValutaObj = _balanceRepository.GetSoldTypeAccount(dataEnd.Value, acc.Id, appClientId, acc.CurrencyId, 1, false, null);
                    var soldValuta = (soldValutaObj.TipSold == "D" ? 1 : -1) * soldValutaObj.Sold;

                    var rulPrecDeb = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.DocumentType, f => f.Operation.Currency)
                                            .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == appClientId
                                                        && f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= dataEnd &&
                                                        (accList.Contains(f.DebitId))
                                                        && f.Operation.CurrencyId == acc.CurrencyId)
                                            .GroupBy(x => true)
                                            .Select(f => new { TotalRulaj = f.Sum(g => g.Value), TotalRulajCurr = f.Sum(g => g.ValueCurr) })
                                            .FirstOrDefault();

                    var rulPrecCr = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.DocumentType, f => f.Operation.Currency)
                                                               .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == appClientId
                                                                           && f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= dataEnd &&
                                                                           (accList.Contains(f.CreditId))
                                                                           && f.Operation.CurrencyId == acc.CurrencyId)
                                                               .GroupBy(x => true)
                                                                .Select(f => new { TotalRulaj = f.Sum(g => g.Value), TotalRulajCurr = f.Sum(g => g.ValueCurr) })
                                                                .FirstOrDefault();

                    var soldPrec = soldValuta;
                    var totalPrecDebit = 0;
                    var totalPrecCredit = 0;

                    if (rulPrecDeb != null)
                    {
                        totalPrecDebit = (int)rulPrecDeb.TotalRulaj;
                        soldPrec += rulPrecDeb.TotalRulaj;
                    }

                    if (rulPrecCr != null)
                    {
                        totalPrecCredit = (int)rulPrecCr.TotalRulaj;
                        soldPrec -= rulPrecCr.TotalRulaj;
                    }

                    var dobanda = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.Currency)
                                                             .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == appClientId && f.Operation.CurrencyId == acc.CurrencyId &&
                                                                    f.DebitId == account && f.Credit.Symbol.Contains("70951")
                                                                    && f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= dataEnd)
                                                             .GroupBy(x => true).Select(f => new { DobandaIncasata = f.Sum(g => g.Value) }).FirstOrDefault();
                    var comision = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit, f => f.Operation.Currency)
                                                           .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == appClientId && f.Operation.CurrencyId == acc.CurrencyId &&
                                                                  f.CreditId == account && f.Debit.Symbol.Contains("60959")
                                                                  && f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= dataEnd)
                                                           .GroupBy(x => true).Select(f => new { Comision = f.Sum(g => g.Value) }).FirstOrDefault();
                    var soldDetail = new SoldDetail
                    {
                        AccountId = acc.Id,
                        AccountName = acc.Symbol,
                        CurrencyName = acc.Currency.CurrencyName,
                        IC = acc.BankAccount.Bank.LegalPerson.FullName,
                        SoldValuta = soldValuta,
                        SoldEchivalent = Math.Round(soldValuta * exchangeRate, 2),
                        RulajDb = totalPrecDebit,
                        RulajCr = totalPrecCredit,
                        CurrentDate = dataEnd.Value,
                        ComisionPerceput = comision.Comision,
                        DobandaIncasata = dobanda.DobandaIncasata
                    };
                    soldDetails.Add(soldDetail);
                }
            }
            ret.SoldDetails = soldDetails;
            return ret;
        }

        public void VerifySoldPeriod(DateTime dataStart, DateTime dataEnd, int periodTypeId)
        {
            try
            {
                if (periodTypeId == (int)TipPerioadaSold.Lunar)
                {
                    if (dataStart.Day != 1 || dataEnd.Day != LazyMethods.LastDayOfMonth(dataEnd).Day)
                    {
                        throw new Exception("Data de inceput trebuie sa fie inceputul lunii, iar cea de sfarsit trebuie sa fie sfarsitul lunii");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[UnitOfWork]
        //[AbpAuthorize("Conta.Rapoarte.Rapoarte.SoldFurnizoriDebitori")]
        public virtual SoldFurnizoriDebitoriModel SoldFurnizoriReport(int thirdPartyId, DateTime startDate)
        {
            try
            {
                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.GetAllIncluding(f => f.AddressCountry, f => f.AddressRegion).FirstOrDefault(f => f.Id == personId);

                var ret = new SoldFurnizoriDebitoriModel();

                ret.AppClientId1 = person.Id1;
                ret.AppClientId2 = person.Id2;
                ret.AppClientName = person.FullName;
                ret.AppClientAddress = person.AddressCountry.CountryName + ", " + person.AddressRegion.RegionName + ", " + person.AddressLocality + ", " + person.AddressStreet;

                var thirdParty = _personRepository.GetAllIncluding(f => f.AddressCountry, f => f.AddressRegion).FirstOrDefault(f => f.Id == thirdPartyId);
                if (thirdParty == null)
                {
                    throw new Exception("Nu am identificat furnizorul");
                }

                ret.ThirdPartyId1 = thirdParty.Id1;
                ret.ThirdPartyName = thirdParty.FullName;
                ret.ThirdPartyAddress = thirdParty.AddressCountry.CountryName + ", " + thirdParty.AddressRegion.RegionName + ", " + thirdParty.AddressLocality + ", " + thirdParty.AddressStreet;

                ret.OperationDate = startDate;

                var invoices = _invoiceRepository.GetAllIncluding(f => f.PaymentOrderInvoices, f => f.DispositionInvoices, f => f.ThirdParty, f => f.DocumentType).ToList()
                                                        .Where(f => f.ThirdPartyId == thirdPartyId && f.ThirdPartyQuality == ThirdPartyQuality.Furnizor && f.State == State.Active &&
                                                                    f.OperationDate <= startDate/* && f.RestPlata != 0*/)/*.ToList()*/;

                var details = new List<SoldFurnizoriDebitoriDetails>();

                foreach (var item in invoices)
                {
                    var totalPayedPaymentOrder = _paymentOrderInvoiceRepository.GetAllIncluding(f => f.Invoice, f => f.PaymentOrder).ToList()
                                                  .Where(f => f.PaymentOrder.OrderDate <= startDate && f.InvoiceId == item.Id)
                                                  .Sum(f => f.PayedValue);

                    var totalPayedDispositions = _dispositionInvoiceRepository.GetAllIncluding(f => f.Invoice, f => f.Disposition).ToList()
                                                              .Where(f => f.Disposition.DispositionDate <= startDate && f.InvoiceId == item.Id)
                                                              .Sum(f => f.PayedValue);
                    if (item.Value - totalPayedPaymentOrder - totalPayedDispositions != 0)
                    {
                        var detail = new SoldFurnizoriDebitoriDetails()
                        {
                            DocumentDate = item.OperationDate,
                            DocumentNumber = item.InvoiceNumber,
                            DocumentType = item.DocumentType.TypeNameShort,
                            Value = item.Value,
                            RestPlata = item.Value - totalPayedPaymentOrder - totalPayedDispositions
                        };
                        details.Add(detail);
                    }
                }

                ret.Details = details;

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[UnitOfWork]
        public virtual BugetPrevReportDto BugetPrevReport(int departmentId, int bugetPrevId, bool activityType)
        {
            try
            {
                var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
                var appClient = 1;
                var personId = _personRepository.GetPersonTenantId(appClient);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var bugetPrev = new BugetPrevReportDto();
                bugetPrev.AppClientName = person.FullName;

                var bugetListAllDepByMonth = new List<BugePrevAllDepartmentByMonth>();
                var bugetListDepByMonth = new List<BugePrevDepartmentByMonth>();

                if (departmentId == 0)
                {
                    bugetPrev.DepartmentName = "Toate departamentele";
                    var bvcFormRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).OrderBy(f => f.OrderView).Select(f => f.Id);
                    var bugetPrevDetailList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand.Departament, f => f.BugetPrevRand.FormRand, f => f.BugetPrevRand,
                                                    f => f.BugetPrevRand.BugetPrev.Formular, f => f.BugetPrevRand.ValueList,
                                                    f => f.ActivityType)
                                   .Where(f => f.BugetPrevRand.BugetPrev.FormularId == formularId && bvcFormRandList.Contains(f.BugetPrevRand.FormRandId) && f.BugetPrevRand.BugetPrevId == bugetPrevId)
                                   //.Where(f=>f.BugetPrevRand.FormRandId == 2289)
                                   .OrderBy(f => f.BugetPrevRand.FormRand.OrderView).ToList();

                    for (int i = 1; i <= 12; i++)
                    {
                        foreach (var item in bugetPrevDetailList.Where(f => f.DataLuna.Month == i).GroupBy(f => f.DataLuna))
                        {
                            if (item.Key.Month == i)
                            {
                                var detailList = bugetPrevDetailList.Where(f => f.DataLuna.Month == i).ToList();
                                var detail = bugetPrevDetailList.Where(f => f.DataLuna.Month == i).GroupBy(f => new
                                {
                                    Id = f.BugetPrevRand.FormRandId,
                                    Descriere = f.BugetPrevRand.FormRand.Descriere,
                                    Validat = bugetPrevDetailList.All(f => f.BugetPrevRand.Validat == true),
                                    ActivityTypeName = f.ActivityType.ActivityName,
                                    OrderView = f.BugetPrevRand.FormRand.OrderView,
                                }).Select(f => new BugePrevAllDepartmentByMonth
                                {
                                    Id = f.Key.Id,
                                    Description = f.Key.Descriere,
                                    Value = f.Sum(g => g.Value),
                                    ValueActivity = f.Sum(g => g.Value),
                                    MonthName = item.Key,
                                    ActivityType = f.Key.ActivityTypeName,
                                    OrderView = f.Key.OrderView,
                                }).ToList();

                                ;
                                bugetListAllDepByMonth.AddRange(detail);
                            }
                        }
                        bugetPrev.BugetPrevAllDepMonths = bugetListAllDepByMonth.OrderBy(f => f.OrderView).ToList();
                    }
                }
                else
                {
                    var departament = _salariatiDepartamentRepository.GetAllIncluding(f => f.Departament).FirstOrDefault(f => f.DepartamentId == departmentId);

                    bugetPrev.DepartmentName = departament.Departament.Name;

                    var bugetPrevDetailList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand.Departament, f => f.BugetPrevRand.FormRand, f => f.BugetPrevRand,
                                                                        f => f.BugetPrevRand.BugetPrev.Formular, f => f.BugetPrevRand.ValueList,
                                                                        f => f.ActivityType)
                                                       .Where(f => f.BugetPrevRand.BugetPrev.FormularId == formularId && f.BugetPrevRand.DepartamentId == departament.DepartamentId && f.BugetPrevRand.BugetPrevId == bugetPrevId)
                                                       .OrderBy(f => f.BugetPrevRand.FormRand.OrderView)
                                                       .ToList();
                    for (int i = 1; i <= 12; i++)
                    {
                        foreach (var det in bugetPrevDetailList.GroupBy(f => f.BugetPrevRandId).Distinct())
                        {
                            var bugetRandValue = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand).Where(f => f.BugetPrevRandId == det.Key && f.DataLuna.Month == i).OrderBy(f => f.BugetPrevRand.FormRand.OrderView);
                            var bugetRand = _bugetPrevRandRepository.GetAllIncluding(f => f.FormRand).FirstOrDefault(f => f.Id == det.Key);

                            var detail = bugetRandValue.OrderBy(f => f.BugetPrevRand.FormRand.OrderView).Select(f => new BugePrevDepartmentByMonth
                            {
                                Description = bugetRand.FormRand.Descriere,
                                Value = bugetRandValue.Where(f => f.BugetPrevRandId == bugetRand.Id).Sum(f => f.Value),
                                MonthName = bugetRandValue.FirstOrDefault(f => f.DataLuna.Month == i).DataLuna,
                                ActivityType = f.ActivityType.ActivityName,
                                ValueActivity = f.Value,
                                OrderView = f.BugetPrevRand.FormRand.OrderView,
                            }).Distinct();
                            bugetListDepByMonth.AddRange(detail);
                        }
                    }
                    bugetPrev.BugetPrevDepMonths = bugetListDepByMonth.OrderBy(f => f.OrderView).ToList();
                }
                return bugetPrev;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[UnitOfWork]
        public virtual AnexaReportModel AnexaReport(int savedBalanceId)
        {
            try
            {
                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var savedBalance = _savedBalanceRepository.FirstOrDefault(f => f.Id == savedBalanceId);
                var savedBalanceDate = savedBalance.SaveDate;

                var report = new AnexaReportModel();
                report.AppClientName = person.FullName;
                report.SavedBalanceDate = savedBalanceDate;
                report.SavedBalanceId = savedBalanceId;

                var anexaIdsList = _bnrAnexaRepository.GetAll().Select(f => f.Id).ToList();

                foreach (var anexaId in anexaIdsList)
                {
                    var anexa = _bnrAnexaRepository.FirstOrDefault(f => f.Id == anexaId);
                    switch (anexa.Denumire)
                    {
                        case "Anexa A1":
                            report.AnexaA1Details = GenerareRaportAnexa1(anexaId, savedBalanceId);
                            break;

                        case "Anexa B1":
                            report.AnexaB1Details = GenerareRaportAnexa1(anexaId, savedBalanceId);
                            break;

                        case "Anexa 2":
                            report.Anexa2Details = GenerareRaportAnexa2(anexaId, savedBalanceId);
                            break;

                        case "Anexa 3":
                            report.Anexa3Details = GenerareRaportAnexa3(anexaId, savedBalanceId);
                            break;

                        case "Anexa 4":
                            report.Anexa4Details = GenerareRaportAnexa4(anexaId, savedBalanceId);
                            break;
                    }
                }
                return report;
            }
            catch (Exception ex)
            {
                throw ex;//new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<Anexa1Model> GenerareRaportAnexa1(int anexaId, int savedBalanceId)
        {
            try
            {
                var anexaRaport = new List<Anexa1Model>();
                var anexaDetailList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.AnexaId == anexaId).OrderBy(f => f.NrCrt).ToList();
                foreach (var detail in anexaDetailList)
                {
                    var raportareRandIdList = _bnrRaportareRandRepository.GetAllIncluding(f => f.BNR_AnexaDetail, f => f.BNR_Raportare, f => f.BNR_Sector)
                                                                    .Where(f => f.BNR_AnexaDetail.AnexaId == anexaId && f.AnexaDetailId == detail.Id && f.BNR_Raportare.SavedBalanceId == savedBalanceId)
                                                                    .GroupBy(f => new { f.AnexaDetailId, f.SectorId })
                                                                    //.GroupBy(f=>f.AnexaDetailId)
                                                                    .Select(f => f.Max(g => g.BNR_RaportareId))
                                                                    .Distinct()
                                                                    .ToList();
                    if (raportareRandIdList.Count > 0)
                    {
                        var raportareRandList = _bnrRaportareRandRepository.GetAllIncluding(f => f.BNR_AnexaDetail, f => f.BNR_Raportare, f => f.BNR_Sector)
                                                                           .Where(f => raportareRandIdList.Contains(f.BNR_RaportareId) && f.AnexaDetailId == detail.Id && f.BNR_Raportare.SavedBalanceId == savedBalanceId /*&& sectorIdsList.Contains(f.SectorId)*/ /* f.SectorId == sector.Id*/)
                                                                           .ToList()
                                                                           .GroupBy(f => new { AnexaDetailId = f.AnexaDetailId, DenumireRand = f.BNR_AnexaDetail.DenumireRand, TipInstrument = f.BNR_AnexaDetail.TipInstrument, NrCrt = f.BNR_AnexaDetail.NrCrt, CodRand = f.BNR_AnexaDetail.CodRand, OrderView = f.BNR_AnexaDetail.OrderView/*, SectorId = f.SectorId*/ })
                                                                           .Select(g => new Anexa1Model
                                                                           {
                                                                               AnexaDetailId = g.Key.AnexaDetailId,
                                                                               AnexaDetailName = g.Key.DenumireRand,
                                                                               InstrumentFinanciar = g.Key.TipInstrument,
                                                                               NrCrt = g.Key.NrCrt,
                                                                               CodRand = g.Key.CodRand,
                                                                               OrderView = g.Key.OrderView,
                                                                               Sector_S121 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S121")).Sum(f => f.Valoare),
                                                                               Sector_S122 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S122")).Sum(f => f.Valoare),
                                                                               Sector_S1311 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S1311")).Sum(f => f.Valoare),
                                                                               Sector_S1313 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S1313")).Sum(f => f.Valoare),
                                                                               Sector_S1314 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S1314")).Sum(f => f.Valoare),
                                                                               Sector_S123 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S123")).Sum(f => f.Valoare),
                                                                               Sector_S124 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S124")).Sum(f => f.Valoare),
                                                                               Sector_S125 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S125")).Sum(f => f.Valoare),
                                                                               Sector_S126 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S126")).Sum(f => f.Valoare),
                                                                               Sector_S128 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S128")).Sum(f => f.Valoare),
                                                                               Sector_S129 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S129")).Sum(f => f.Valoare),
                                                                               Sector_S11 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S11")).Sum(f => f.Valoare),
                                                                               Sector_S14 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S14")).Sum(f => f.Valoare),
                                                                               Sector_S15 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S15")).Sum(f => f.Valoare),
                                                                               Sector_S211 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S211")).Sum(f => f.Valoare),
                                                                               Sector_S212 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S212")).Sum(f => f.Valoare),
                                                                               Sector_S22 = g.Where(f => f.SectorId != null && f.BNR_Sector.Sector.Contains("S22")).Sum(f => f.Valoare),
                                                                               Total = g.Sum(f => f.Valoare)
                                                                           }).ToList();

                        anexaRaport.AddRange(raportareRandList);
                    }
                    else
                    {
                        var rand = new Anexa1Model
                        {
                            AnexaDetailId = detail.Id,
                            AnexaDetailName = detail.DenumireRand,
                            InstrumentFinanciar = detail.TipInstrument,
                            NrCrt = detail.NrCrt,
                            CodRand = detail.CodRand,
                            OrderView = detail.OrderView,
                            Sector_S121 = 0,
                            Sector_S122 = 0,
                            Sector_S1311 = 0,
                            Sector_S1314 = 0,
                            Sector_S123 = 0,
                            Sector_S125 = 0,
                            Sector_S126 = 0,
                            Sector_S128 = 0,
                            Sector_S129 = 0,
                            Sector_S11 = 0,
                            Sector_S14 = 0,
                            Sector_S15 = 0,
                            Sector_S211 = 0,
                            Sector_S212 = 0,
                            Sector_S22 = 0,
                            Total = 0
                        };
                        anexaRaport.Add(rand);
                    }
                }

                return anexaRaport.OrderBy(f => f.OrderView).ToList();
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<Anexa2Model> GenerareRaportAnexa2(int anexaId, int savedBalanceId)
        {
            var savedBalance = _savedBalanceRepository.GetAll().FirstOrDefault(f => f.Id == savedBalanceId);
            var anexaDetails = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.AnexaId == anexaId).ToList();
            var raportList = new List<Anexa2Model>();

            foreach (var detail in anexaDetails)
            {
                for (int i = 1; i <= 10; i += 3)
                {
                    var dataStart = new DateTime(savedBalance.SaveDate.Year, i, 1);
                    var dataEnd = LazyMethods.LastDayOfMonth(new DateTime(savedBalance.SaveDate.Year, i + 2, 1));

                    var list = _operationDetailsRepository.GetAllIncluding(f => f.Operation, f => f.Debit, f => f.Credit)
                                                           .Where(f => f.Operation.OperationDate >= dataStart && f.Operation.OperationDate <= dataEnd
                                                                  && f.Operation.OperationDate <= savedBalance.SaveDate
                                                                  && f.Operation.State == State.Active);

                    int indexStart = detail.FormulaConta.IndexOf("$");
                    int indexEnd = detail.FormulaConta.IndexOf("$", indexStart + 1);
                    string item = detail.FormulaConta.Substring(indexStart + 1, indexEnd - indexStart - 1);
                    string[] splitItem = item.Split("#");
                    string tipItem = splitItem[0];
                    string contItem = splitItem[1];

                    if (tipItem == "ORD")
                    {
                        list = list = list.Where(f => f.Debit.Symbol.IndexOf(contItem) == 0);
                    }

                    if (tipItem == "ORC")
                    {
                        list = list.Where(f => f.Credit.Symbol.IndexOf(contItem) == 0);
                    }

                    decimal value = list.Sum(f => f.Value);

                    var rand = new Anexa2Model
                    {
                        AnexaDetailName = detail.DenumireRand,
                        NrCrt = detail.NrCrt,
                        Valoare = value,
                        OperationDate = dataEnd,
                        OrderView = detail.OrderView
                    };
                    raportList.Add(rand);
                }
            }

            return raportList;
        }

        private List<Anexa3Model> GenerareRaportAnexa3(int anexaId, int savedBalanceId)
        {
            try
            {
                var anexa3ModelList = new List<Anexa3Model>();
                var anexaDetailsList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.AnexaId == anexaId).ToList();
                foreach (var row in anexaDetailsList)
                {
                    var value = _bnrRaportareRepository.CalculFormulaAnexa3(row.FormulaConta, savedBalanceId);
                    var rowAnexa = new Anexa3Model
                    {
                        Indicator = row.DenumireRand,
                        NrCrt = row.NrCrt,
                        OrderView = row.OrderView,
                        Valoare = value
                    };
                    anexa3ModelList.Add(rowAnexa);
                }

                return anexa3ModelList;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<Anexa4Model> GenerareRaportAnexa4(int anexaId, int savedBalanceId)
        {
            var anexa4ModelList = new List<Anexa4Model>();

            var anexaDetailList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.AnexaId == anexaId).ToList();

            foreach (var row in anexaDetailList)
            {
                var formulaCrestere = row.FormulaCresteri;
                var formulaReducere = row.FormulaReduceri;
                decimal valueCrestere = 0, valueReducere = 0;

                if (formulaCrestere != null && formulaCrestere != "")
                {
                    valueCrestere = _bnrRaportareRepository.CalculFormulaAnexa4(formulaCrestere, savedBalanceId);
                }

                if (formulaReducere != null && formulaReducere != "")
                {
                    valueReducere = _bnrRaportareRepository.CalculFormulaAnexa4(formulaReducere, savedBalanceId);
                }

                var raportRow = new Anexa4Model
                {
                    DenumireRand = row.DenumireRand,
                    NrCrt = row.NrCrt,
                    OrderView = row.OrderView,
                    ValoareCresteri = valueCrestere,
                    ValoareReduceri = valueReducere
                };
                anexa4ModelList.Add(raportRow);
            }
            return anexa4ModelList;
        }

        //[UnitOfWork]
        public virtual BVC_Report RaportBVC(int variantaBugetId, string tipRand, int nivelRand, int anBuget, int tip, string frecventa, string tipActivitate, int tipRaport)
        {
            try
            {
                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                string activityTypeName = tipActivitate != "0" ? _activityTypeRepository.FirstOrDefault(f => f.Id == int.Parse(tipActivitate)).ActivityName : "Toate";
                var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == anBuget);
                int anBVC = bugetFormular.AnBVC;

                var bugetPrev = _bugetPrevRepository.FirstOrDefault(f => f.Id == variantaBugetId);

                var raport = new BVC_Report();
                raport.AppClientName = person.FullName;
                raport.Frecventa = frecventa;
                int countSelectedTipRand = 0;
                var tipRandList = new List<string>();
                //BVC Prevazut
                if (tipRaport == 0)
                {
                    raport.Titlu = "Buget prevazut " + bugetPrev.BVC_Tip + " (" + bugetPrev.Descriere + ")";
                    raport.AnBuget = anBVC;
                    raport.BugetDetails = GenerareRaportPrevazutBVC(anBuget, tip, tipRand, nivelRand, frecventa, tipActivitate, bugetPrev.Id, out countSelectedTipRand, out tipRandList);
                    raport.Parameters = (Enum.GetNames(typeof(BVC_RowType)).Length == countSelectedTipRand) ? "" : LazyMethods.ReturnStringFromEnum(typeof(BVC_RowType), tipRandList);
                    raport.Parameters += "\n" + "Frecventa: " + (frecventa == "0" ? "Total" : frecventa == "1" ? "Trimestrial" : "Lunar" + ";Tip activitate: ") + activityTypeName;
                }
                return raport;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<BugetPrevazutModel> GenerareRaportPrevazutBVC(int formularId, int tip, string tipRand, int nivelRand, string frecventa, string tipActivitate, int bugetPrevId, out int countSelectedTipRand, out List<string> tipRandList)
        {
            try
            {
                var prevazutModel = new List<BugetPrevazutModel>();
                var formRanduriList = new List<BVC_FormRand>();
                tipRandList = new List<string>();

                var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == formularId);
                int anBVC = bugetFormular.AnBVC;

                string[] tipRanduriList = tipRand.Split("-");
                tipRandList.AddRange(tipRanduriList);
                countSelectedTipRand = tipRanduriList.Length;

                // lista cu randurile din configurare
                foreach (var rand in tipRanduriList)
                {
                    var list = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId && f.TipRand == (BVC_RowType)int.Parse(rand) && f.NivelRand <= nivelRand).ToList();
                    formRanduriList.AddRange(list);
                }
                //var formRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId && f.TipRand ==0 /*(BVC_RowType)tipRand*/ && f.NivelRand <= nivelRand).ToList();
                if (tip == (int)BVC_Tip.BVC)
                {
                    formRanduriList = formRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formRanduriList = formRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formRanduriList)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        var dataStart = new DateTime(anBVC, i, 1);
                        var dataEnd = LazyMethods.LastDayOfMonth(new DateTime(anBVC, i, 1));
                        var randValuesList = _bugetPrevRepository.GetBugetPrevValues(rand, dataStart, dataEnd, bugetPrevId);
                        prevazutModel.AddRange(randValuesList);
                    }
                }

                var randuriList = new List<BugetPrevazutModel>();

                // populez lista in functie de tipul de activitate selectat
                randuriList = tipActivitate == "0" ? prevazutModel : prevazutModel.Where(f => f.ActivityTypeId == Int32.Parse(tipActivitate)).ToList();

                switch (frecventa)
                {
                    case "0": // total
                        randuriList = _bugetPrevRepository.ComputePrevazutTotal(randuriList);
                        break;

                    case "1": // trimestrial
                        randuriList = _bugetPrevRepository.ComputePrevazutTrimestrial(randuriList);
                        break;

                    case "2": //lunar
                        randuriList = _bugetPrevRepository.ComputePrevazutLunar(randuriList);
                        break;
                }
                return randuriList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[UnitOfWork]
        public virtual BVC_Realizat_Report BVC_RealizatReport(string tipRand, int bugetRealizatId, bool includReferat, int tipRealizat, int tipRaport, int anBuget, int tip, int variantaBugetId, int nivelRand)
        {
            try
            {
                var raport = new BVC_Realizat_Report();

                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                raport.AppClientName = person.FullName;

                var tipRandList = new List<string>();
                string[] tipRanduriList = tipRand.Split("-");
                tipRandList.AddRange(tipRanduriList);

                var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == anBuget);
                int anBVC = bugetFormular.AnBVC;
                raport.An = anBVC;

                var bugetRealizat = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId && f.BVC_Tip == (BVC_Tip)tip);

                if (bugetRealizat is { })
                {
                    switch (tipRealizat)
                    {
                        case 1:
                            raport.Titlu = "Realizat in perioada / total prevazut la data " + LazyMethods.DateToString(bugetRealizat.SavedBalance.SaveDate) + (includReferat ? " (cu referate)" : " (fara referate)");
                            raport.Details = ComputeRealizatPeriodPrevTotal(bugetRealizatId, includReferat, tipRandList, anBuget, tip, variantaBugetId, nivelRand);
                            break;

                        case 2:
                            raport.Titlu = "Realizat in perioada / prevazut in perioada la data " + LazyMethods.DateToString(bugetRealizat.SavedBalance.SaveDate) + (includReferat ? " (cu referate)" : " (fara referate)");
                            raport.Details = ComputeRealizatPrevPeriod(bugetRealizatId, includReferat, tipRandList, anBuget, tip, variantaBugetId, nivelRand);
                            break;

                        case 3:
                            raport.Titlu = "Realizat in luna / prevazut in luna la data " + LazyMethods.DateToString(bugetRealizat.SavedBalance.SaveDate) + (includReferat ? " (cu referate)" : " (fara referate)");
                            raport.Details = ComputeRealizatPrevMonth(bugetRealizatId, includReferat, tipRandList, anBuget, tip, variantaBugetId, nivelRand);
                            break;
                    }
                }

                return raport;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private List<BVC_RealizatDetail> ComputeRealizatPeriodPrevTotal(int bugetRealizatId, bool includReferat, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand)
        {
            try
            {
                var reportDetails = new List<BVC_RealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active &&
                                                                       f.TipRand == (BVC_RowType)int.Parse(tipRand) && f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();

                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var realizatDate = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId).SavedBalance.SaveDate;
                    var realizatStartDate = new DateTime(realizatDate.Year, 1, 1);

                    var prevStartDate = new DateTime(realizatDate.Year, 1, 1);
                    var prevEndDate = LazyMethods.LastDayOfMonth(new DateTime(realizatDate.Year, 12, 1));

                    var valoareRealizat = _bvcRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_Realizat, f => f.BVC_Realizat.SavedBalance)
                                                                    .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_Realizat.BVC_Tip == (BVC_Tip)tip &&
                                                                           f.BVC_FormRandId == rand.Id && realizatStartDate <= f.BVC_Realizat.SavedBalance.SaveDate &&
                                                                           f.BVC_Realizat.SavedBalance.SaveDate <= realizatDate && f.BVC_RealizatId == bugetRealizatId)
                                                                    .Sum(f => (includReferat) ? f.ValoareCuReferat : f.ValoareFaraReferat);

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_RealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        ValoareRealizat = valoareRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareRealizat != 0) ? Math.Round(valoareRealizat / valoarePrevazut * 100, 2) : 0
                    };
                    reportDetails.Add(detail);
                }
                return reportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BVC_RealizatDetail> ComputeRealizatPrevPeriod(int bugetRealizatId, bool includReferat, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand)
        {
            try
            {
                var reportDetails = new List<BVC_RealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active && f.TipRand == (BVC_RowType)int.Parse(tipRand) &&
                                                                       f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();
                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var realizatDate = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId).SavedBalance.SaveDate;

                    var startDate = new DateTime(realizatDate.Year, 1, 1);

                    var valoareRealizat = _bvcRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_Realizat, f => f.BVC_Realizat.SavedBalance)
                                                                    .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_Realizat.BVC_Tip == (BVC_Tip)tip &&
                                                                           f.BVC_FormRandId == rand.Id && startDate <= f.BVC_Realizat.SavedBalance.SaveDate &&
                                                                           f.BVC_Realizat.SavedBalance.SaveDate <= realizatDate)
                                                                    .Sum(f => (includReferat) ? f.ValoareCuReferat : f.ValoareFaraReferat);

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              startDate <= f.DataOper && f.DataOper <= realizatDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_RealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        ValoareRealizat = valoareRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareRealizat != 0) ? Math.Round(valoareRealizat / valoarePrevazut * 100, 2) : 0
                    };
                    reportDetails.Add(detail);
                }
                return reportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BVC_RealizatDetail> ComputeRealizatPrevMonth(int bugetRealizatId, bool includReferat, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand)
        {
            try
            {
                var reportDetails = new List<BVC_RealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active &&
                                                                       f.TipRand == (BVC_RowType)int.Parse(tipRand) && f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();

                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var realizatDate = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId).SavedBalance.SaveDate;

                    var valoareRealizat = _bvcRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_Realizat, f => f.BVC_Realizat.SavedBalance)
                                                                    .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_Realizat.BVC_Tip == (BVC_Tip)tip &&
                                                                           f.BVC_FormRandId == rand.Id && f.BVC_Realizat.SavedBalance.SaveDate == realizatDate)
                                                                    .Sum(f => (includReferat) ? f.ValoareCuReferat : f.ValoareFaraReferat);

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              f.DataOper == realizatDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_RealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        ValoareRealizat = valoareRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareRealizat != 0) ? Math.Round(valoareRealizat / valoarePrevazut * 100, 2) : 0
                    };
                    reportDetails.Add(detail);
                }
                return reportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[UnitOfWork]
        public virtual SitFinanRaport GetSitFinanRaport(int raportId, int? balanceId, bool isDailyBalance, bool isDateRange, DateTime startDate, DateTime endDate, int colNumber)
        {
            try
            {
                var ret = new SitFinanRaport();
                ret = _reportManager.GetSitFinanRaport(raportId, balanceId, isDailyBalance, isDateRange, startDate, endDate, colNumber);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private void ComputeReportDetails(DateTime startDate, DateTime endDate, bool isDailyBalance, int raportId, SitFinanRapConfigCol column, out SitFinanRaport ret)
        //{
        //    try
        //    {
        //        ret = new SitFinanRaport();
        //        ret.Details = new List<SitFinanReportDetailList>();
        //        var savedBalanceIdsList = _savedBalanceRepository.GetAll().Where(f => f.SaveDate >= startDate && f.SaveDate <= endDate && f.IsDaily == isDailyBalance).GroupBy(f => f.SaveDate).Select(f => f.Max(x => x.Id)).ToList();
        //        var sitFinanCalcBalanceIdsList = _sitFinanCalcRepository.GetAllIncluding(f => f.SavedBalance).Where(f => savedBalanceIdsList.Contains(f.SavedBalanceId)).OrderBy(f => f.SavedBalanceId).Select(f => f.SavedBalanceId).Distinct().ToList();
        //        foreach (var balanceId in sitFinanCalcBalanceIdsList)
        //        {
        //            var balance = _sitFinanCalcRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.SavedBalanceId == balanceId);

        //            var reportDetail = new List<SitFinanReportDetailList>();

        //            reportDetail = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
        //                                                  .Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId)
        //                                                  .Select(f => new SitFinanReportDetailList
        //                                                  {
        //                                                      CalcRowId = f.Id,
        //                                                      RowId = f.SitFinanRapRowId,
        //                                                      RowName = f.SitFinanRapRow.RowName,
        //                                                      RowNr = f.SitFinanRapRow.RowNr,
        //                                                      RowNota = f.SitFinanRapRow.RowNota,
        //                                                      OrderView = f.SitFinanRapRow.OrderView,
        //                                                      TotalRow = f.SitFinanRapRow.TotalRow,
        //                                                      Bold = f.SitFinanRapRow.Bold,
        //                                                      NegativeValue = f.SitFinanRapRow.NegativeValue,
        //                                                      DecimalNr = f.SitFinanRapRow.DecimalNr,
        //                                                      Val = (column.ColumnNr == 1 ? f.Val1 : (column.ColumnNr == 2 ? f.Val2 : (column.ColumnNr == 3 ? f.Val3 : (column.ColumnNr == 4 ? f.Val4 : (column.ColumnNr == 5 ? f.Val5 : f.Val6))))),
        //                                                      BalanceDate = balance.SavedBalance.SaveDate
        //                                                  })
        //                                                  .ToList()
        //                                                  .OrderBy(f => f.OrderView)
        //                                                  .ToList();

        //            ret.Details.AddRange(reportDetail);

        //            // note
        //            var nota = _sitFinanCalcNoteRepository.FirstOrDefault(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId);
        //            if (nota != null)
        //            {
        //                ret.NotaBefore = nota.BeforeNote;
        //                ret.NotaAfter = nota.AfterNote;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public virtual CursValutarBNRModel CursValutarBNR(int balanceId)
        {
            try
            {
                var appClientId = 1/*GetCurrentTenant()*/;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

                var balance = _balanceRepository.GetAll().Where(e => e.Id == balanceId).FirstOrDefault();
                var startDate = new DateTime(balance.BalanceDate.Year - 1, 12, 31);
                var endDate = balance.BalanceDate;

                var ret = new CursValutarBNRModel
                {
                    Details = new List<CursValutarBNRDetails>(),
                    AppClientName = person.Name,
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    DataStart = startDate,
                    DataEnd = endDate,
                };

                var item = _balanceDetailsRepository.GetAll().Where(e => e.BalanceId == balanceId && e.CurrencyId != 1).Select(e => new { e.CurrencyId, e.Currency.CurrencyName }).Distinct().ToList();
                // _exchangeRatesRepository.GetExchangeRate();
                foreach (var value in item)
                {
                    var vp = _exchangeRatesRepository.GetExchangeRate(startDate, value.CurrencyId, 1);
                    var vc = _exchangeRatesRepository.GetExchangeRate(endDate, value.CurrencyId, 1);

                    var obj = new CursValutarBNRDetails()
                    {
                        CurrencyId = value.CurrencyId,
                        CurrencyName = value.CurrencyName,
                        ValoarePrecedenta = vp,
                        ValoareCurenta = vc,
                        Variatie = (vc - vp) / vp
                    };

                    ret.Details.Add(obj);
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;//new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public virtual LichidCalcModel LichidCalcReport(int savedBalanceId, int lichidType)
        {
            try
            {
                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var savedBalance = _savedBalanceRepository.FirstOrDefault(f => f.Id == savedBalanceId);

                var report = new LichidCalcModel
                {
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    AppClientName = person.FullName,
                    Description = savedBalance.BalanceName + " - " + savedBalance.SaveDate.ToShortDateString(),
                    CalcDate = savedBalance.SaveDate
                };

                switch (lichidType)
                {
                    case (int)LichidCalcType.LichidBenzi:
                        report.LichidCalcList = new List<LichidCalcListDetDto>();

                        var lichidCalcIdsList = _lichidCalcRepository.GetAllIncluding(f => f.LichidConfig).Where(f => f.SavedBalanceId == savedBalanceId).Select(f => f.LichidConfigId).Distinct().ToList();
                        var lichidConfigList = _lichidConfigRepository.GetAllIncluding(f => f.LichidBenzi).Where(f => lichidCalcIdsList.Contains(f.Id)).ToList();
                        var lichidBenziRows = _lichidBenziRepository.GetAll().ToList();

                        foreach (var conf in lichidConfigList)
                        {
                            var lichidCalcDet = new LichidCalcListDetDto
                            {
                                Descriere = conf.DenumireRand,
                                LichidConfigId = conf.Id,
                                RandTotal = (conf.FormulaTotal != null && conf.FormulaTotal != "") ? true : false,
                                TenantId = conf.TenantId
                            };
                            foreach (var banda in lichidBenziRows)
                            {
                                var lichidCalcRow = _lichidCalcRepository.GetAllIncluding(f => f.LichidConfig, f => f.LichidBenzi, f => f.SavedBalance)
                                                   .FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == conf.Id && f.LichidBenziId == banda.Id);

                                switch (banda.DurataInLuniMaxima)
                                {
                                    case 3:
                                        lichidCalcDet.ValoareBanda1 = lichidCalcRow.Valoare;
                                        break;

                                    case 12:
                                        lichidCalcDet.ValoareBanda2 = lichidCalcRow.Valoare;
                                        break;

                                    case 60:
                                        lichidCalcDet.ValoareBanda3 = lichidCalcRow.Valoare;
                                        break;

                                    case 9999:
                                        lichidCalcDet.ValoareBanda4 = lichidCalcRow.Valoare;
                                        break;

                                    case 0:
                                        lichidCalcDet.ValoareBanda5 = lichidCalcRow.Valoare;
                                        break;
                                }
                                lichidCalcDet.TotalInit += lichidCalcRow.Valoare;
                                lichidCalcDet.TotalActualiz += lichidCalcRow.Valoare;
                            }
                            report.LichidCalcList.Add(lichidCalcDet);
                        }

                        break;

                    case (int)LichidCalcType.LichidValuta:
                        report.LichidCalcCurrList = new List<LichidCalcCurrListDto>();
                        var lichidCalcCurrIdsList = _lichidCalcCurrRepository.GetAllIncluding(f => f.LichidConfig)
                                                                .Where(f => f.SavedBalanceId == savedBalanceId)
                                                                .Select(f => f.LichidConfigId)
                                                                .Distinct()
                                                                .ToList();

                        var lichidConfList = _lichidConfigRepository.GetAll().Where(f => lichidCalcCurrIdsList.Contains(f.Id)).ToList();
                        var lichidBenziCurrRows = _lichidBenziCurrRepository.GetAll().ToList();

                        foreach (var conf in lichidConfList)
                        {
                            var lichidCalcDet = new LichidCalcCurrListDto
                            {
                                Descriere = conf.DenumireRand,
                                LichidConfigId = conf.Id,
                                RandTotal = (conf.FormulaTotal != null && conf.FormulaTotal != "") ? true : false,
                                TenantId = conf.TenantId
                            };
                            foreach (var banda in lichidBenziCurrRows)
                            {
                                var lichidCalcRow = _lichidCalcCurrRepository.GetAllIncluding(f => f.LichidConfig, f => f.LichidBenziCurr, f => f.SavedBalance)
                                                   .FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == conf.Id && f.LichidBenziCurrId == banda.Id);

                                switch (banda.Descriere)
                                {
                                    case "RON":
                                        lichidCalcDet.ValoareRON = lichidCalcRow.Valoare;
                                        break;

                                    case "EUR":
                                        lichidCalcDet.ValoareEUR = lichidCalcRow.Valoare;
                                        break;

                                    case "USD":
                                        lichidCalcDet.ValoareUSD = lichidCalcRow.Valoare;
                                        break;

                                    case "GBP":
                                        lichidCalcDet.ValoareGBP = lichidCalcRow.Valoare;
                                        break;

                                    default:
                                        lichidCalcDet.ValoareAlteMonede = lichidCalcRow.Valoare;
                                        break;
                                }
                                lichidCalcDet.TotalInit += lichidCalcRow.Valoare;
                                lichidCalcDet.TotalActualiz += lichidCalcRow.Valoare;
                            }
                            report.LichidCalcCurrList.Add(lichidCalcDet);
                        }
                        break;
                }

                return report;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual DepozitBancarDto DepozitBancarReport(int balanceId)
        {
            try
            {
                var appClientId = 1;
                var localCurrencyId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var balance = _balanceRepository.GetAll().FirstOrDefault(f => f.Id == balanceId);

                var depozitBancar = new DepozitBancarDto
                {
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    AppClientName = person.FullName,
                    BalanceDate = balance.BalanceDate,
                    ReportName = "Riscul legat de rata dobanzii"
                };

                depozitBancar.Details = new List<DepozitBancarDetailDto>();
                var plasamenteList = _plasamentLichiditateManager.PlasamenteLichiditateList(balance.BalanceDate);
                plasamenteList = plasamenteList.Where(f => f.tipPlasament == "D").ToList();
                var currency = _currencyRepository.GetCurrencyById(localCurrencyId);
                var plasamentCurrencyList = plasamenteList.Where(f => f.moneda != currency.CurrencyCode).Select(f => f.moneda).Distinct().ToList();

                foreach (var item in plasamentCurrencyList)
                {
                    var fromCurrency = _currencyRepository.GetByCode(item);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(balance.BalanceDate, fromCurrency.Id, localCurrencyId);
                    foreach (var plasament in plasamenteList.Where(f => f.moneda == item))
                    {
                        plasament.valoareContabila = Math.Round(plasament.valoareContabila * exchangeRate, 2);
                        plasament.valoareCreanta = Math.Round(plasament.valoareCreanta * exchangeRate, 2);
                        plasament.valoareInvestita = Math.Round(plasament.valoareInvestita * exchangeRate, 2);
                        plasament.valoareDepreciere = Math.Round(plasament.valoareDepreciere * exchangeRate, 2);
                    }
                }
                decimal totalContCurent = 0;
                decimal totalDepozit = 0;
                decimal totalDepozitCont = 0;

                var plasamentEmitentiList = plasamenteList.GroupBy(f => f.emitent).Select(f => new
                {
                    emitent = f.Key,
                    maturityDate = f.Max(x => x.maturityDate),
                    totalDepozite = f.Sum(x => x.valoareInvestita),
                    totalDepoziteCuDobanda = f.Sum(x => x.valoareInvestita * x.procentDobanda)
                }).ToList();
                foreach (var emitent in plasamentEmitentiList)
                {
                    var issuer = _issuerRepository.GetAllIncluding(f => f.LegalPerson).FirstOrDefault(f => f.IbanAbrv == emitent.emitent && f.TenantId == appClientId && f.IssuerType == IssuerType.Banca);
                    if (issuer == null)
                    {
                        throw new Exception("Nu am identificat banca cu codul " + emitent.emitent);
                    }

                    var detail = new DepozitBancarDetailDto
                    {
                        DenumireBanca = issuer.LegalPerson.FullName,
                        Dobanda = Math.Round(emitent.totalDepoziteCuDobanda / emitent.totalDepozite, 4),
                        SumaInvestita = emitent.totalDepozite,
                        IdPlasament = "",
                        MaximRezidual = LazyMethods.MonthsBetween(balance.BalanceDate, emitent.maturityDate)
                    };
                    depozitBancar.Details.Add(detail);
                }
                totalDepozit = depozitBancar.Details.Sum(f => f.SumaInvestita);
                var contCurentSoldList = _balanceDetailsRepository.GetAllIncluding(f => f.Account, f => f.Currency)
                                                                  .Where(f => f.BalanceId == balanceId && f.Account.AccountFuncType == AccountFuncType.ContBancar)
                                                                  .Select(f => new BalanceDetailsDto
                                                                  {
                                                                      AccountId = f.AccountId,
                                                                      CrValueI = f.CrValueI,
                                                                      CrValueF = f.CrValueF,
                                                                      CrValueM = f.CrValueM,
                                                                      CrValueY = f.CrValueY,
                                                                      DbValueI = f.DbValueI,
                                                                      DbValueF = f.DbValueF,
                                                                      DbValueM = f.DbValueM,
                                                                      DbValueY = f.DbValueY,
                                                                      CurrencyId = f.CurrencyId
                                                                  })
                                                                  .ToList();

                var contCurentSoldCurrencyIdsList = contCurentSoldList.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var currencyId in contCurentSoldCurrencyIdsList)
                {
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(balance.BalanceDate, currencyId, localCurrencyId);
                    foreach (var contCurent in contCurentSoldList.Where(f => f.CurrencyId == currencyId))
                    {
                        contCurent.DbValueF = Math.Round(contCurent.DbValueF * exchangeRate, 2);
                    }
                }

                totalContCurent = contCurentSoldList.Sum(f => f.DbValueF);
                totalDepozitCont = totalContCurent + totalDepozit;

                depozitBancar.TotalDepozit = totalDepozit;
                depozitBancar.ConturiCurente = totalContCurent;
                depozitBancar.TotalDepozitConturi = totalContCurent + totalDepozit;

                return depozitBancar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual BVC_BalRealizat_Report BVC_BalRealizatReport(string tipRand, int bugetBalRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, string tipActivitate, int tipRaport, bool includPrevazutAnual)
        {
            try
            {
                var raport = new BVC_BalRealizat_Report();

                var appClientId = 1;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                raport.AppClientName = person.FullName;
                raport.TipBuget = ((BVC_Tip)tip).ToString();

                string activityTypeName = tipActivitate != "0" ? _activityTypeRepository.FirstOrDefault(f => f.Id == int.Parse(tipActivitate)).ActivityName : "Toate";

                var tipRandList = new List<string>();
                string[] tipRanduriList = tipRand.Split("-");
                tipRandList.AddRange(tipRanduriList);

                var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == anBuget);
                int anBVC = bugetFormular.AnBVC;
                raport.An = anBVC;
                var bugetBalRealizat = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetBalRealizatId && f.BVC_Tip == (BVC_Tip)tip);

                switch (tipRealizat)
                {
                    case 1:
                        raport.Titlu = "Realizat in perioada / total prevazut la data " + LazyMethods.DateToString(bugetBalRealizat.SavedBalance.SaveDate);
                        raport.Details = ComputeBalRealizatPeriodPrevTotal(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        if (tipRandList.Select(s => int.Parse(s)).ToList().Contains((int)BVC_RowType.Venituri))
                        {
                            raport.HideTotalVenituri = true;
                            raport.VenitDinFonduri = ComputeBalRealizatPeriodPrevTotalByFond(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        }
                        break;

                    case 2:
                        raport.Titlu = "Realizat in perioada / prevazut in perioada la data " + LazyMethods.DateToString(bugetBalRealizat.SavedBalance.SaveDate);
                        raport.Details = ComputeBalRealizatPrevPeriod(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        if (tipRandList.Select(s => int.Parse(s)).ToList().Contains((int)BVC_RowType.Venituri))
                        {
                            raport.HideTotalVenituri = true;
                            raport.VenitDinFonduri = ComputeBalRealizatPeriodPrevTotalByFond(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        }
                        break;

                    case 3:
                        raport.Titlu = "Realizat in luna / prevazut in luna la data " + LazyMethods.DateToString(bugetBalRealizat.SavedBalance.SaveDate);
                        raport.Details = ComputeBalRealizatPrevMonth(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        if (tipRandList.Select(s => int.Parse(s)).ToList().Contains((int)BVC_RowType.Venituri))
                        {
                            raport.HideTotalVenituri = true;
                            raport.VenitDinFonduri = ComputeBalRealizatPeriodPrevTotalByFond(bugetBalRealizatId, tipRandList, anBuget, tip, variantaBugetId, nivelRand, activityTypeName, includPrevazutAnual);
                        }
                        break;
                }

                return raport;
            }
            catch (Exception ex)
            {
                throw ex; //new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private List<VenitDinFond> ComputeBalRealizatPeriodPrevTotalByFond(int bugetBalRealizatId, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand, string activityType, bool includPrevazutAnual)
        {
            try
            {
                var balRealizatReportFondDetails = new List<VenitDinFond>();
                var balRealizatDate = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetBalRealizatId).SavedBalance.SaveDate;
                var balRealizatStartDate = new DateTime(balRealizatDate.Year, 1, 1);

                var prevStartDate = new DateTime(balRealizatDate.Year, 1, 1);
                var prevEndDate = LazyMethods.LastDayOfMonth(new DateTime(balRealizatDate.Year, 12, 1));

                decimal valoareBalRealizat = 0;
                decimal valoareAprobat = 0;
                decimal valoarePrevazut = 0;
                var bugetBalRealizatList = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                       .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                               balRealizatStartDate <= f.BVC_BalRealizat.SavedBalance.SaveDate && !f.BVC_FormRand.BalIsTotal &&
                              f.BVC_BalRealizat.SavedBalance.SaveDate <= balRealizatDate &&
                              f.BVC_FormRand.TipRand == BVC_RowType.Venituri).ToList();
                var bugetPrevRanvValueList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= balRealizatDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip &&
                                                                              f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Venituri && !f.BugetPrevRand.FormRand.BalIsTotal)
                                                                       .ToList();

                if (activityType == "Toate")
                {
                    var activityTypeList = _activityTypeRepository.GetAll().ToList();
                    foreach (var item in activityTypeList)
                    {
                        valoareBalRealizat = bugetBalRealizatList.Where(f => f.ActivityTypeId == item.Id).Sum(f => f.Valoare);
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                              .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip && f.ActivityTypeId == item.Id &&
                                                                              f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Venituri && !f.BugetPrevRand.FormRand.IsTotal)
                                                                       .Sum(f => f.Value);
                        }
                        valoarePrevazut = bugetPrevRanvValueList.Where(f => f.ActivityTypeId == item.Id).Sum(f => f.Value);
                        var detailFond = new VenitDinFond
                        {
                            DenumireRand = item.ActivityName == "FGDB" ? "Venituri cuvenite din investirea resuselor financiare disponibile ale fondului de garantare și alte venituri" :
                                            "Venituri cuvenite din investirea resurselor financiare disponibile ale fondului de rezolutie și alte venituri",
                            ValoareFondBalRealizat = valoareBalRealizat,
                            ValoareFondPrevazut = valoarePrevazut,
                            ValoareFondDiferenta = valoareBalRealizat - valoarePrevazut,
                            Procent = (valoarePrevazut != 0 && valoareBalRealizat != 0) ? Math.Round(valoareBalRealizat / valoarePrevazut * 100, 2) : 0,
                            AprobatFond = includPrevazutAnual ? valoareAprobat : 0,
                            FondRamasDeRealizat = includPrevazutAnual ? valoareAprobat - valoareBalRealizat : 0
                        };
                        balRealizatReportFondDetails.Add(detailFond);
                    }
                }
                else
                {
                    var activityTypeId = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == activityType).Id;
                    valoareBalRealizat = bugetBalRealizatList.Where(f => f.ActivityTypeId == activityTypeId).Sum(f => f.Valoare);
                    if (includPrevazutAnual)
                    {
                        valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                              .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip && f.ActivityTypeId == activityTypeId &&
                                                                              f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Venituri && !f.BugetPrevRand.FormRand.IsTotal)
                                                                       .Sum(f => f.Value);
                    }
                    valoarePrevazut = bugetPrevRanvValueList.Where(f => f.ActivityTypeId == activityTypeId).Sum(f => f.Value);
                    var detailFond = new VenitDinFond
                    {
                        DenumireRand = activityType == "FGDB" ? "Venituri cuvenite din investirea resuselor financiare disponibile ale fondului de garantare și alte venituri" :
                        "Venituri cuvenite din investirea resurselor financiare disponibile ale fondului de rezolutie și alte venituri",
                        ValoareFondBalRealizat = valoareBalRealizat,
                        ValoareFondPrevazut = valoarePrevazut,
                        ValoareFondDiferenta = valoareBalRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareBalRealizat != 0) ? Math.Round(valoareBalRealizat / valoarePrevazut * 100, 2) : 0,
                        AprobatFond = includPrevazutAnual ? valoareAprobat : 0,
                        FondRamasDeRealizat = includPrevazutAnual ? valoareAprobat - valoareBalRealizat : 0
                    };
                    balRealizatReportFondDetails.Add(detailFond);
                }

                return balRealizatReportFondDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<BVC_BalRealizatDetail> ComputeBalRealizatPeriodPrevTotal(int bugetBalRealizatId, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand, string activityType, bool includPrevazutAnual)
        {
            try
            {
                var balRealizatReportDetails = new List<BVC_BalRealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active &&
                                                                       f.TipRand == (BVC_RowType)int.Parse(tipRand) && f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();

                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var balRealizatDate = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetBalRealizatId).SavedBalance.SaveDate;
                    var balRealizatStartDate = new DateTime(balRealizatDate.Year, 1, 1);

                    var prevStartDate = new DateTime(balRealizatDate.Year, 1, 1);
                    var prevEndDate = LazyMethods.LastDayOfMonth(new DateTime(balRealizatDate.Year, 12, 1));

                    decimal valoareBalRealizat = 0;
                    decimal valoareAprobat = 0;
                    if (activityType == "Toate")
                    {
                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                       f.BVC_FormRandId == rand.Id && balRealizatStartDate <= f.BVC_BalRealizat.SavedBalance.SaveDate &&
                                                       f.BVC_BalRealizat.SavedBalance.SaveDate <= balRealizatDate)
                                                .Sum(f => f.Valoare);
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);
                        }
                    }
                    else
                    {
                        var activityTypeId = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == activityType).Id;
                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                .FirstOrDefault(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                       f.BVC_FormRandId == rand.Id && balRealizatStartDate <= f.BVC_BalRealizat.SavedBalance.SaveDate &&
                                                       f.BVC_BalRealizat.SavedBalance.SaveDate <= balRealizatDate && f.ActivityTypeId == activityTypeId)
                                                .Valoare;
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip && f.ActivityTypeId == activityTypeId &&
                                                                              f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Venituri && !f.BugetPrevRand.FormRand.IsTotal)
                                                                       .Sum(f => f.Value);
                        }
                    }

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              prevStartDate <= f.DataOper && f.DataOper <= prevEndDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_BalRealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        ValoareBalRealizat = valoareBalRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareBalRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareBalRealizat != 0) ? Math.Round(valoareBalRealizat / valoarePrevazut * 100, 2) : 0,
                        Aprobat = includPrevazutAnual ? valoareAprobat : 0,
                        RamasDeRealizat = includPrevazutAnual ? valoareAprobat - valoareBalRealizat : 0
                    };
                    balRealizatReportDetails.Add(detail);
                }
                return balRealizatReportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BVC_BalRealizatDetail> ComputeBalRealizatPrevPeriod(int bugetBalRealizatId, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand, string activityType, bool includPrevazutAnual)
        {
            try
            {
                var balReportDetails = new List<BVC_BalRealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active && f.TipRand == (BVC_RowType)int.Parse(tipRand) &&
                                                                       f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();
                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var balRealizatDate = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetBalRealizatId).SavedBalance.SaveDate;

                    var startDate = new DateTime(balRealizatDate.Year, 1, 1);
                    var endYearDate = new DateTime(balRealizatDate.Year, 12, 31);

                    decimal valoareBalRealizat = 0;
                    decimal valoareAprobat = 0;
                    if (activityType == "Toate")
                    {
                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                                        .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                                               f.BVC_FormRandId == rand.Id && startDate <= f.BVC_BalRealizat.SavedBalance.SaveDate &&
                                                                               f.BVC_BalRealizat.SavedBalance.SaveDate <= balRealizatDate)
                                                                        .Sum(f => f.Valoare);
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              startDate <= f.DataOper && f.DataOper <= endYearDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);
                        }
                    }
                    else
                    {
                        var activityTypeId = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == activityType).Id;

                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance, f => f.ActivityType)
                                                                        .FirstOrDefault(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                                               f.BVC_FormRandId == rand.Id && startDate <= f.BVC_BalRealizat.SavedBalance.SaveDate &&
                                                                               f.BVC_BalRealizat.SavedBalance.SaveDate <= balRealizatDate && f.ActivityTypeId == activityTypeId)
                                                                        .Valoare;
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              startDate <= f.DataOper && f.DataOper <= endYearDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip && f.ActivityTypeId == activityTypeId)
                                                                       .Sum(f => f.Value);
                        }
                    }

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              startDate <= f.DataOper && f.DataOper <= balRealizatDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_BalRealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        ValoareBalRealizat = valoareBalRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareBalRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareBalRealizat != 0) ? Math.Round(valoareBalRealizat / valoarePrevazut * 100, 2) : 0,
                        Aprobat = includPrevazutAnual ? valoareAprobat : 0,
                        RamasDeRealizat = includPrevazutAnual ? valoareAprobat - valoareBalRealizat : 0
                    };
                    balReportDetails.Add(detail);
                }
                return balReportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BVC_BalRealizatDetail> ComputeBalRealizatPrevMonth(int bugetBalRealizatId, List<string> tipRandList, int anBuget, int tip, int variantaBugetId, int nivelRand, string activityType, bool includPrevazutAnual)
        {
            try
            {
                var balReportDetails = new List<BVC_BalRealizatDetail>();
                var formularRanduriList = new List<BVC_FormRand>();

                foreach (var tipRand in tipRandList)
                {
                    var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                                .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active &&
                                                                       f.TipRand == (BVC_RowType)int.Parse(tipRand) && f.NivelRand <= nivelRand)
                                                                .OrderBy(f => f.OrderView)
                                                                .ToList();

                    formularRanduriList.AddRange(formularRanduri);
                }

                if (tip == (int)BVC_Tip.BVC)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
                }

                if (tip == (int)BVC_Tip.CashFlow)
                {
                    formularRanduriList = formularRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
                {
                    var balRealizatDate = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetBalRealizatId).SavedBalance.SaveDate;
                    var startDate = new DateTime(balRealizatDate.Year, 1, 1);
                    var endYearDate = new DateTime(balRealizatDate.Year, 12, 31);

                    decimal valoareBalRealizat = 0;
                    decimal valoareAprobat = 0;
                    if (activityType == "Toate")
                    {
                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                      .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                             f.BVC_FormRandId == rand.Id && f.BVC_BalRealizat.SavedBalance.SaveDate == balRealizatDate)
                                                      .Sum(f => f.Valoare);
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                                startDate <= f.DataOper && f.DataOper <= endYearDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);
                        }
                    }
                    else
                    {
                        var activityTypeId = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == activityType).Id;

                        valoareBalRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                  .FirstOrDefault(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                         f.BVC_FormRandId == rand.Id && f.BVC_BalRealizat.SavedBalance.SaveDate == balRealizatDate && f.ActivityTypeId == activityTypeId)
                                                 .Valoare;
                        if (includPrevazutAnual)
                        {
                            valoareAprobat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              startDate <= f.DataOper && f.DataOper <= endYearDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip && f.ActivityTypeId == activityTypeId)
                                                                       .Sum(f => f.Value);
                        }
                    }

                    var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                                       .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                              f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                              f.DataOper == balRealizatDate && f.BugetPrevRand.State == State.Active &&
                                                                              f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                                       .Sum(f => f.Value);

                    var detail = new BVC_BalRealizatDetail
                    {
                        DenumireRand = rand.Descriere,
                        Aprobat = valoareAprobat,
                        RamasDeRealizat = includPrevazutAnual ? valoareAprobat - valoareBalRealizat : 0,
                        ValoareBalRealizat = valoareBalRealizat,
                        ValoarePrevazut = valoarePrevazut,
                        ValoareDiferenta = valoareBalRealizat - valoarePrevazut,
                        Procent = (valoarePrevazut != 0 && valoareBalRealizat != 0) ? Math.Round(valoareBalRealizat / valoarePrevazut * 100, 2) : 0
                    };
                    balReportDetails.Add(detail);
                }
                return balReportDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Metoda pentru raportul Calcul resurse
        public virtual BVC_PrevResurseModel BVC_PrevResurseReport(int anBVC, int bugetPrevId) // bugetPrevId formularId
        {
            try
            {
                var appClientId = 1/*GetCurrentTenant()*/;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == anBVC);
                int anFormular = bugetFormular.AnBVC;

                var ret = new BVC_PrevResurseModel
                {
                    PrevResurse = new List<BVC_PrevResurseDto>(),
                    AppClientName = person.Name,
                    AppClientId1 = person.Id1,
                    AppClientId2 = person.Id2,
                    An = anFormular
                };

                ret.PrevResurse = _bugetPrevResurseRepository.GetAllIncluding(f => f.ActivityType).Where(f => f.BugetPrevId == bugetPrevId && f.TenantId == appClientId).Select(f => new BVC_PrevResurseDto
                {
                    ActivityTypeId = f.ActivityTypeId,
                    OrderView = f.OrderView,
                    Suma = f.Suma,
                    ActivityTypeName = f.ActivityType.ActivityName,
                    Descriere = f.Descriere,
                    TenantId = f.TenantId
                })
.OrderBy(f => f.OrderView)
.ToList();

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BugetRaportare BugetReport(int AnBVC)
        {
            var appClientId = 1;
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == AnBVC);
            int anBVC = bugetFormular.AnBVC;
            var ret = _bugetReportRepository.GetBugetRaportareDetails(anBVC);

            return new BugetRaportare { AnBVC = anBVC, AppClientName = person.FullName, Titlu = "Cashflow achizitii PAAP", RaportareDetails = ret };
        }

        public InvoiceDetailsReportDto InvoiceDetailsInit()
        {
            var ret = new InvoiceDetailsReportDto();
            var currentDate = LazyMethods.Now();

            ret = new InvoiceDetailsReportDto
            {
                EndDate = currentDate,
                StartDate = new DateTime(currentDate.Year, currentDate.Month - 1, 1),
                InvoiceElementsDetailsCategoryId = null,
                InvoiceElementsDetailsId = null,
            };
            return ret;
        }

        public InvoiceDetailsReportDto SearchInvoiceDetails(InvoiceDetailsReportDto invoiceDetail)
        {
            try
            {
                var appClientId = 1;
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId;
                invoiceDetail.Details = new List<InvoiceDetailsList>();
                var details = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.ThirdParty,
                                                                       f => f.ContaOperationDetail, f => f.ContaOperationDetail.Debit, f => f.Invoices.Currency, f => f.Invoices.DocumentType)
                                                       .Where(f => f.Invoices.OperationDate >= invoiceDetail.StartDate && f.Invoices.OperationDate <= invoiceDetail.EndDate && f.Invoices.State == State.Active &&
                                                                   f.Invoices.TenantId == appClientId && f.ContaOperationDetail.Debit != null);
                var operationDetailsList = _operationDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetailsCategory, f => f.Operation, f => f.Debit, f => f.Credit,
                                                                                       f => f.Operation.DocumentType)
                                                                      .Where(f => f.Operation.OperationDate >= invoiceDetail.StartDate && f.Operation.OperationDate <= invoiceDetail.EndDate &&
                                                                                  f.Operation.State == State.Active && f.Operation.TenantId == appClientId &&
                                                                                  f.InvoiceElementsDetailsCategoryId != null && f.InvoiceElementsDetailsId != null);

                if (invoiceDetail.InvoiceElementsDetailsCategoryId != null)
                {
                    details = details.Where(f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory.Id == invoiceDetail.InvoiceElementsDetailsCategoryId);
                    operationDetailsList = operationDetailsList.Where(f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory.Id == invoiceDetail.InvoiceElementsDetailsCategoryId);

                    if (invoiceDetail.InvoiceElementsDetailsId != null)
                    {
                        details = details.Where(f => f.InvoiceElementsDetails.Id == invoiceDetail.InvoiceElementsDetailsId);
                        operationDetailsList = operationDetailsList.Where(f => f.InvoiceElementsDetailsId == invoiceDetail.InvoiceElementsDetailsId);
                    }
                }

                if (invoiceDetail.ContCheltuialaId != null)
                {
                    var accountList = _accountRepository.GetAllAnalythicsSintetic(invoiceDetail.ContCheltuialaId ?? 0).Select(f => f.Id).Distinct().ToList();
                    details = details.Where(f => accountList.Contains(f.ContaOperationDetail.DebitId));
                    operationDetailsList = operationDetailsList.Where(f => accountList.Contains(f.DebitId));
                }

                invoiceDetail.Details.AddRange(details.ToList().Select(f => new InvoiceDetailsList
                {
                    InvoiceDate = f.Invoices.OperationDate,
                    ContCheltuiala = f.ContaOperationDetail?.Debit.Symbol + " - " + f.ContaOperationDetail?.Debit.AccountName,
                    DocumentNumber = f.Invoices.InvoiceNumber,
                    DocumentTypeName = f.Invoices.DocumentType.TypeNameShort,
                    Explicatii = f.Element,
                    InvoiceElementsDetails = f.InvoiceElementsDetails?.Description,
                    InvoiceElementsDetailsCategory = f.InvoiceElementsDetails?.InvoiceElementsDetailsCategory.CategoryElementDetName,
                    TenantId = appClientId,
                    ThirdPartyName = f.Invoices.ThirdParty.FullName,
                    Value = f.Value,
                    ValoareValuta = f.Invoices.CurrencyId != localCurrencyId ?
                                    f.Value * _exchangeRatesRepository.GetExchangeRate(f.Invoices.OperationDate, f.Invoices.CurrencyId, localCurrencyId.Value) : f.Value,
                    CurrencyName = f.Invoices.Currency.CurrencyName
                })
                .OrderBy(f => f.InvoiceDate).ToList());

                invoiceDetail.Details.AddRange(operationDetailsList.ToList().Select(f => new InvoiceDetailsList
                {
                    InvoiceDate = f.Operation.OperationDate,
                    ContCheltuiala = f.Debit.Symbol + " - " + f.Debit.AccountName,
                    DocumentNumber = f.Operation.DocumentNumber,
                    DocumentTypeName = f.Operation.DocumentType.TypeNameShort,
                    Explicatii = f.Details,
                    InvoiceElementsDetails = f.InvoiceElementsDetails?.Description,
                    InvoiceElementsDetailsCategory = f.InvoiceElementsDetails?.InvoiceElementsDetailsCategory.CategoryElementDetName,
                    TenantId = appClientId,
                    ThirdPartyName = "",
                    Value = f.Value,
                    ValoareValuta = f.Operation.CurrencyId != localCurrencyId ?
                                    f.Value * _exchangeRatesRepository.GetExchangeRate(f.Operation.OperationDate, f.Operation.CurrencyId, localCurrencyId.Value) : f.Value,
                    CurrencyName = f.Operation.Currency.CurrencyName
                })
                .OrderBy(f => f.InvoiceDate).ToList());

                return invoiceDetail;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public virtual DetaliuSoldReportDto DetaliuSoldReport(DateTime startDate, string account, int currencyId)
        {
            try
            {
                var appClientId = 1; // GetCurrentTenant();
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);
                var localCurrencyId = 1;

                var ret = new DetaliuSoldReportDto();
                ret.AppClientName = person.FullName;
                ret.AppClientId1 = person.Id1;
                ret.AppClientId2 = person.Id2;
                ret.StartDate = startDate;
                ret.CurrencyId = currencyId;
                ret.AccountName = account;

                var balance = _balanceRepository.GetAllIncludingBalanceDetails().FirstOrDefault(f => f.BalanceDate == startDate && f.Status == State.Active);
                if (balance == null)
                {
                    balance = _balanceRepository.CreateTempBalance(startDate, false, appClientId);
                }

                var accountList = _accountRepository.GetAllIncluding(f => f.AnalyticAccounts, f => f.Currency).Where(f => f.Symbol.IndexOf(account) == 0 && f.Status == State.Active);
                var accountIdList = accountList.Select(f => f.Id).ToList();

                var balanceAccountList = balance.BalanceDetails.Where(f => accountIdList.Contains(f.AccountId));

                if (currencyId != 0)
                {
                    balanceAccountList = balanceAccountList.Where(f => f.CurrencyId == currencyId).ToList();
                }

                var details = new List<SoldDetailBase>();
                foreach (var currency in balanceAccountList.Select(f => f.CurrencyId).Distinct().ToList())
                {
                    decimal exchangeRate = 1;
                    foreach (var balanceItem in balanceAccountList.Where(f => f.CurrencyId == currency))
                    {

                        if (currency != localCurrencyId)
                        {
                            exchangeRate = _exchangeRatesRepository.GetExchangeRate(startDate, currency, localCurrencyId);
                        }
                        if (balanceItem.CrValueF != 0 || balanceItem.DbValueF != 0)
                        {
                            details.Add(new SoldDetailBase
                            {
                                AccountId = balanceItem.AccountId,
                                AccountName = balanceItem.Account.AccountName,
                                Symbol = balanceItem.Account.Symbol,
                                CurrencyId = balanceItem.CurrencyId,
                                CurrencyName = balanceItem.Currency.CurrencyName,
                                CreditValue = balanceItem.CrValueF,
                                DebitValue = balanceItem.DbValueF,
                                SoldValuta = balanceItem.DbValueF - balanceItem.CrValueF,
                                SoldEchivalent = Math.Round(balanceItem.DbValueF * exchangeRate, 2) - Math.Round(balanceItem.CrValueF * exchangeRate, 2)
                            });
                        }
                    }

                }
                ret.SoldDetails = details.OrderBy(f => f.Symbol).ToList();
                return ret;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public virtual BugetPreliminatDto BugetPreliminatDetaliiReport(string tipRand, int bugetRealizatId, int tipRealizat, int anBuget, int tip, int variantaBugetId, int nivelRand, int tipRaport, int bugetPreliminatId)
        {
            var appClientId = 1;
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var bugetFormular = _bvcFormularRepository.FirstOrDefault(f => f.Id == anBuget);
            int anBVC = bugetFormular.AnBVC;


            var ret = new BugetPreliminatDto
            {
                AnBVC = anBVC,
                AppClientName = person.FullName,
                BugetPreliminatDetails = new List<BugetPreliminatDetalii>(),
                Titlu = "Buget preliminat detalii"
            };

            var tipRandList = new List<string>();
            string[] tipRanduriList = tipRand.Split("-");
            tipRandList.AddRange(tipRanduriList);

            var formularRanduriList = new List<BVC_FormRand>();

            foreach (var tipR in tipRandList)
            {
                var formularRanduri = _bvcFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular)
                                                            .Where(f => f.FormularId == anBuget && f.Formular.State == State.Active &&
                                                                   f.TipRand == (BVC_RowType)int.Parse(tipR) && f.NivelRand <= nivelRand)
                                                            .OrderBy(f => f.OrderView)
                                                            .ToList();

                formularRanduriList.AddRange(formularRanduri);
            }

            if (tip == (int)BVC_Tip.BVC)
            {
                formularRanduriList = formularRanduriList.Where(f => f.AvailableBVC).ToList();
            }


            var bugetRealizat = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId && f.BVC_Tip == (BVC_Tip)tip);
            var bugetPreliminatDb = _bugetPrevRepository.GetAllIncluding(f => f.Formular).FirstOrDefault(f => f.Id == bugetPreliminatId);

            var realizatDate = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.Id == bugetRealizatId).SavedBalance.SaveDate;
            DateTime startDateRealizat = new DateTime(anBVC, 1, 1);
            DateTime endDateRealizat = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 12, 1));

            DateTime startDatePreliminat = new DateTime(anBVC, bugetPreliminatDb.MonthStart, 1);
            DateTime endDatePreliminat = LazyMethods.LastDayOfMonth(new DateTime(bugetPreliminatDb.Formular.AnBVC, bugetPreliminatDb.MonthEnd, 1));

            DateTime startDatePrev = new DateTime(anBVC, 1, 1);
            DateTime endDatePrev = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 12, 1));

            foreach (var rand in formularRanduriList.OrderBy(f => f.OrderView))
            {
                var valoarePrevazut = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                           .Where(f => f.BugetPrevRand.BugetPrevId == variantaBugetId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                                  f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                                  startDatePrev <= f.DataOper && f.DataOper <= endDatePrev && f.BugetPrevRand.State == State.Active &&
                                                                  f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                           .Sum(f => f.Value);
                decimal valoareRealizat = 0, valoarePreliminat = 0;
                var detail = new BugetPreliminatDetalii
                {
                    DenumireRand = rand.Descriere
                };

                if (realizatDate.Month < startDatePreliminat.Month)
                {


                    for (DateTime i = startDateRealizat; i <= endDateRealizat; i = LazyMethods.LastDayOfMonth(i.AddMonths(1)))
                    {
                        valoareRealizat = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.BVC_BalRealizat, f => f.BVC_BalRealizat.SavedBalance)
                                                .Where(f => f.BVC_FormRand.FormularId == anBuget && f.BVC_BalRealizat.BVC_Tip == (BVC_Tip)tip &&
                                                       f.BVC_FormRandId == rand.Id && i == f.BVC_BalRealizat.SavedBalance.SaveDate)
                                                .Sum(f => f.Valoare);

                        detail.Date = i;
                        switch (i.Month)
                        {
                            case 1:
                                detail.ValoareIanuarie += valoareRealizat;
                                break;
                            case 2:
                                detail.ValoareFebruarie += valoareRealizat;
                                break;
                            case 3:
                                detail.ValoareMartie += valoareRealizat;
                                break;
                            case 4:
                                detail.ValoareAprilie += valoareRealizat;
                                break;
                            case 5:
                                detail.ValoareMai += valoareRealizat;
                                break;
                            case 6:
                                detail.ValoareIunie += valoareRealizat;
                                break;
                            case 7:
                                detail.ValoareIulie += valoareRealizat;
                                break;
                            case 8:
                                detail.ValoareAugust += valoareRealizat;
                                break;
                            case 9:
                                detail.ValoareSeptembrie += valoareRealizat;
                                break;
                            case 10:
                                detail.ValoareOctombrie += valoareRealizat;
                                break;
                            default:
                                break;
                        }
                        detail.TotalRealizat = detail.ValoareIanuarie + detail.ValoareFebruarie + detail.ValoareMartie + detail.ValoareAprilie + detail.ValoareMai + detail.ValoareIunie + detail.ValoareIulie +
                                               detail.ValoareAugust + detail.ValoareSeptembrie + detail.ValoareOctombrie;
                    }
                }
                else
                {
                    for (DateTime i = startDatePreliminat; i <= endDatePreliminat; i = LazyMethods.LastDayOfMonth(i.AddMonths(1)))
                    {
                        valoarePreliminat = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand)
                                                 .Where(f => f.BugetPrevRand.BugetPrevId == bugetPreliminatId && f.BugetPrevRand.FormRandId == rand.Id &&
                                                        f.BugetPrevRand.FormRand.FormularId == anBuget &&
                                                        i == f.DataOper && f.BugetPrevRand.State == State.Active &&
                                                        f.BugetPrevRand.BugetPrev.BVC_Tip == (BVC_Tip)tip)
                                                 .Sum(f => f.Value);
                        detail.Date = i;
                        switch (i.Month)
                        {
                            case 11:
                                detail.ValoareNoiembrie += valoarePreliminat;
                                break;
                            case 12:
                                detail.ValoareDecembrie += valoarePreliminat;
                                break;
                            default:
                                break;
                        }
                        detail.TotalPreliminat = detail.ValoareNoiembrie + detail.ValoareDecembrie;
                    }
                }
                detail.Prevazut = valoarePrevazut;
                detail.TotalRealizPrelim = detail.TotalRealizat + detail.TotalPreliminat;
                detail.GradRealizare = detail.Prevazut == 0 ? 0 : (detail.TotalRealizPrelim / detail.Prevazut) * 100;
                ret.BugetPreliminatDetails.Add(detail);
            }
            return ret;
        }
    }
}