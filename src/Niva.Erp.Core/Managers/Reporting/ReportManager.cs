using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Managers.Reporting
{
    public class ReportManager : DomainService
    {
        IPersonRepository _personRepository;
        IRepository<LegalPerson> _legalPersonRepository;
        ISavedBalanceRepository _savedBalanceRepository;
        IRepository<SavedBalanceViewDet> _savedBalanceViewDetRepository;
        ICurrencyRepository _currencyRepository;
        ISitFinanCalcRepository _sitFinanCalcRepository;
        IRepository<SitFinanRapConfigCol> _sitFinanRapConfigColRepository;
        IRepository<SitFinanCalcNote> _sitFinanCalcNoteRepository;

        public ReportManager(IPersonRepository personRepository, IRepository<LegalPerson> legalPersonRepository, ISavedBalanceRepository savedBalanceRepository, IRepository<SavedBalanceViewDet> savedBalanceViewDetRepository,
                    ICurrencyRepository currencyRepository, ISitFinanCalcRepository sitFinanCalcRepository, IRepository<SitFinanRapConfigCol> sitFinanRapConfigColRepository, IRepository<SitFinanCalcNote> sitFinanCalcNoteRepository)
        {
            _personRepository = personRepository;
            _legalPersonRepository = legalPersonRepository;
            _savedBalanceRepository = savedBalanceRepository;
            _savedBalanceViewDetRepository = savedBalanceViewDetRepository;
            _currencyRepository = currencyRepository;
            _sitFinanCalcRepository = sitFinanCalcRepository;
            _sitFinanRapConfigColRepository = sitFinanRapConfigColRepository;
            _sitFinanCalcNoteRepository = sitFinanCalcNoteRepository;
        }

        // Balante salvate
        public SavedBalanceViewDto GetSavedBalanceViewList(int savedBalanceId, string searchAccount, int? nivelRand, int currencyId)
        {
            var appClientId = 1; // GetCurrentTenant();

            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            var balView = new SavedBalanceViewDto();
            var savedBalList = new List<SavedBalanceViewDet>();
            balView.AppClientId1 = person.Id1;
            balView.AppClientId2 = person.Id2;
            balView.AppClientName = person.FullName;

            var savedBalance = _savedBalanceRepository.FirstOrDefault(f => f.Id == savedBalanceId);
            balView.BalanceDate = savedBalance.SaveDate;
            balView.BalanceName = savedBalance.BalanceName;

            if (currencyId == 0) // ROn si echivalent RON
            {
                balView.CurrencyName = "Moneda RON si Echivalent RON";

                savedBalList = _savedBalanceRepository.GetSavedBalanceViewDetByNivelRand(savedBalanceId, nivelRand);

                // Pentru totaluri
                balView = CalcSaveBalanceTotal(savedBalanceId, searchAccount, currencyId, balView);
            }

            if (currencyId != 0)
            {
                var currencyName = _currencyRepository.FirstOrDefault(f => f.Id == currencyId).CurrencyName;
                balView.CurrencyName = "Moneda " + currencyName;

                savedBalList = _savedBalanceRepository.GetSavedBalanceViewDetByNivelRand(savedBalanceId, nivelRand);

                //Pentru totaluri
                balView = CalcSaveBalanceTotal(savedBalanceId, searchAccount, currencyId, balView);
            }
            if (searchAccount != null && searchAccount != "")
            {
                savedBalList = savedBalList.Where(f => f.Cont.IndexOf(searchAccount) == 0).ToList();
            }
            var balanceDetList = ObjectMapper.Map<List<BalanceDetailsView>>(savedBalList);
            balView.Details = balanceDetList;

            return balView;
        }

        // Returneaza lista indicatorilor - Rapoarte Situatii financiare
        public SitFinanReportModel SitFinanRapIndicatori(int balanceId, int raportId) {
            try
            {
                var appClientId = 1/*GetCurrentTenant()*/;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

                var ret = new SitFinanReportModel();

                ret.Details = new List<SitFinanReportDetList>();
                ret.AppClientName = person.Name;
                ret.AppClientId1 = person.Id1;
                ret.AppClientId2 = person.Id2;

                var balance = _sitFinanCalcRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.SavedBalanceId == balanceId);
                var raport = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRap).FirstOrDefault(f => f.SitFinanRapId == raportId);

                ret.BalanceDate = balance.SavedBalance.SaveDate;
                ret.ReportName = raport.SitFinanRap.ReportName;
                ret.NrCol = raport.SitFinanRap.NrCol;

                var reportDetail = new List<SitFinanReportDetList>();

                reportDetail = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                                      .Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId)
                                                      .Select(f => new SitFinanReportDetList
                                                      {
                                                          CalcRowId = f.Id,
                                                          RowId = f.SitFinanRapRowId,
                                                          RowName = f.SitFinanRapRow.RowName,
                                                          RowNr = f.SitFinanRapRow.RowNr,
                                                          RowNota = f.SitFinanRapRow.RowNota,
                                                          OrderView = f.SitFinanRapRow.OrderView,
                                                          TotalRow = f.SitFinanRapRow.TotalRow,
                                                          Bold = f.SitFinanRapRow.Bold,
                                                          NegativeValue = f.SitFinanRapRow.NegativeValue,
                                                          DecimalNr = f.SitFinanRapRow.DecimalNr,
                                                          Val1 = f.Val1,
                                                          Val2 = f.Val2,
                                                          Val3 = f.Val3,
                                                          Val4 = f.Val4,
                                                          Val5 = f.Val5,
                                                          Val6 = f.Val6
                                                      })
                                                      .ToList()
                                                      .OrderBy(f => f.OrderView)
                                                      .ToList();

                ret.Details = reportDetail;

                // coloane
                var columnList = _sitFinanRapConfigColRepository.GetAll().Where(f => f.SitFinanRapId == raportId).ToList();
                var rapColumnList = new SitFinanReportColList();

                foreach (var item in columnList)
                {
                    var columnValue = _sitFinanCalcRepository.GetCalcColumnText(item.ColumnName, balanceId, raportId);
                    switch (item.ColumnNr)
                    {
                        case 1:
                            rapColumnList.Col1 = columnValue;
                            break;
                        case 2:
                            rapColumnList.Col2 = columnValue;
                            break;
                        case 3:
                            rapColumnList.Col3 = columnValue;
                            break;
                        case 4:
                            rapColumnList.Col4 = columnValue;
                            break;
                        case 5:
                            rapColumnList.Col5 = columnValue;
                            break;
                        case 6:
                            rapColumnList.Col6 = columnValue;
                            break;
                    }
                }
                ret.ColumnReport = rapColumnList;

                // note
                var nota = _sitFinanCalcNoteRepository.FirstOrDefault(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId);
                if (nota != null)
                {
                    ret.NotaBefore = nota.BeforeNote;
                    ret.NotaAfter = nota.AfterNote;
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;//new UserFriendlyException("Eroare", ex.ToString());
            }
        }
            
        public SitFinanRaport GetSitFinanRaport(int raportId, int? balanceId, bool isDailyBalance, bool isDateRange, DateTime startDate, DateTime endDate, int colNumber)
        {
            try
            {
                var appClientId = 1/*GetCurrentTenant()*/;
                var personId = _personRepository.GetPersonTenantId(appClientId);
                var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

                var ret = new SitFinanRaport();

                ret.Details = new List<SitFinanReportDetailList>();

                var raport = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRap).FirstOrDefault(f => f.SitFinanRapId == raportId);
                var column = _sitFinanRapConfigColRepository.GetAll().Where(f => f.SitFinanRapId == raportId && f.Id == colNumber).FirstOrDefault();

                if (isDailyBalance)
                {
                    if (isDateRange)
                    {
                        _sitFinanCalcRepository.ComputeReportDetails(startDate, endDate, isDailyBalance, raportId, column, out ret);
                    }
                    else
                    {
                        var balance = _sitFinanCalcRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.SavedBalanceId == balanceId);

                        var reportDetail = new List<SitFinanReportDetailList>();
                        reportDetail = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                      .Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId)
                                      .Select(f => new SitFinanReportDetailList
                                      {
                                          CalcRowId = f.Id,
                                          RowId = f.SitFinanRapRowId,
                                          RowName = f.SitFinanRapRow.RowName,
                                          RowNr = f.SitFinanRapRow.RowNr,
                                          RowNota = f.SitFinanRapRow.RowNota,
                                          OrderView = f.SitFinanRapRow.OrderView,
                                          TotalRow = f.SitFinanRapRow.TotalRow,
                                          Bold = f.SitFinanRapRow.Bold,
                                          NegativeValue = f.SitFinanRapRow.NegativeValue,
                                          DecimalNr = f.SitFinanRapRow.DecimalNr,
                                          Val = (column.ColumnNr == 1 ? f.Val1 : (column.ColumnNr == 2 ? f.Val2 : (column.ColumnNr == 3 ? f.Val3 : (column.ColumnNr == 4 ? f.Val4 : (column.ColumnNr == 5 ? f.Val5 : f.Val6))))),
                                          BalanceDate = balance.SavedBalance.SaveDate
                                      })
                                      .ToList()
                                      .OrderBy(f => f.OrderView)
                                      .ToList();

                        ret.Details = reportDetail;
                    }
                }
                else
                {
                    if (isDateRange)
                    {
                        _sitFinanCalcRepository.ComputeReportDetails(startDate, endDate, isDailyBalance, raportId, column, out ret);
                        ret.DateRange = startDate.ToShortDateString() + " - " + endDate.ToShortDateString();
                    }
                }

                ret.AppClientName = person.Name;
                ret.AppClientId1 = person.Id1;
                ret.AppClientId2 = person.Id2;

                ret.BalanceType = isDailyBalance == true ? "Balanta zilnica" : (isDateRange == true ? "Balanta in perioada " : "");
                ret.ReportName = raport.SitFinanRap.ReportName;
                ret.ColumnName = column.ColumnName;

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SavedBalanceViewDto CalcSaveBalanceTotal(int savedBalanceId, string searchAccount, int currencyId, SavedBalanceViewDto balView)
        {
            var analyticsAccountsFromSavedBalance = new List<SavedBalanceViewDet>();

            if (currencyId == 0) // RON si echivalent RON
            {
                analyticsAccountsFromSavedBalance = _savedBalanceViewDetRepository.GetAllIncluding(f => f.SavedBalance, f => f.Currency, f => f.Account)
                                          .Where(f => f.SavedBalanceId == savedBalanceId && /*f.Account.NivelRand <= nivelRand &&*/ f.IsSynthetic == false && f.IsConverted == true)
                                          .OrderBy(f => f.Cont).ToList();
            }
            else
            {
                analyticsAccountsFromSavedBalance = _savedBalanceViewDetRepository.GetAllIncluding(f => f.SavedBalance, f => f.Currency, f => f.Account)
                                         .Where(f => f.SavedBalanceId == savedBalanceId && /*f.Account.NivelRand <= nivelRand &&*/ f.IsSynthetic == false && f.IsConverted == false && f.CurrencyId == currencyId)
                                         .OrderBy(f => f.Cont).ToList();
            }

            if (searchAccount != null && searchAccount != "")
            {
                analyticsAccountsFromSavedBalance = analyticsAccountsFromSavedBalance.Where(f => f.Cont.IndexOf(searchAccount) == 0).ToList();
            }

            var analithicsAccounts = ObjectMapper.Map<List<BalanceDetailsView>>(analyticsAccountsFromSavedBalance);
            balView.TotalDbI = analithicsAccounts.Sum(f => f.DbValueI);
            balView.TotalCrI = analithicsAccounts.Sum(f => f.CrValueI);
            balView.TotalDbP = analithicsAccounts.Sum(f => f.DbValueP);
            balView.TotalCrP = analithicsAccounts.Sum(f => f.CrValueP);
            balView.TotalDbM = analithicsAccounts.Sum(f => f.DbValueM);
            balView.TotalCrM = analithicsAccounts.Sum(f => f.CrValueM);
            balView.TotalDbSum = analithicsAccounts.Sum(f => f.DbValueSum);
            balView.TotalCrSum = analithicsAccounts.Sum(f => f.CrValueSum);
            balView.TotalDbY = analithicsAccounts.Sum(f => f.DbValueY);
            balView.TotalCrY = analithicsAccounts.Sum(f => f.CrValueY);
            balView.TotalDbF = analithicsAccounts.Sum(f => f.DbValueF);
            balView.TotalCrF = analithicsAccounts.Sum(f => f.CrValueF);

            return balView;
        }
    }

    public class SitFinanRaport
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public string ReportName { get; set; }

        public string ColumnName { get; set; }

        public string DateRange { get; set; }

        public string BalanceType { get; set; }

        public string NotaBefore { get; set; }

        public string NotaAfter { get; set; }

        public List<SitFinanReportDetailList> Details { get; set; }
    }

    public class SitFinanReportDetailList
    {
        public int CalcRowId { get; set; }

        public int RowId { get; set; }

        public string RowName { get; set; }

        public string RowNr { get; set; }
        public string RowNota { get; set; }

        public int OrderView { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }

        public decimal? Val { get; set; }

        public decimal? Val6 { get; set; }
        public DateTime? BalanceDate { get; set; }
    }

    public class SitFinanReportModel
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime BalanceDate { get; set; }

        public string ReportName { get; set; }

        public int NrCol { get; set; }

        public string NotaBefore { get; set; }

        public string NotaAfter { get; set; }

        public SitFinanReportColList ColumnReport { get; set; }

        public IList<SitFinanReportDetList> Details { get; set; }
    }

    public class SitFinanReportDetList
    {
        public int CalcRowId { get; set; }

        public int RowId { get; set; }

        public string RowName { get; set; }

        public string RowNr { get; set; }
        public string RowNota { get; set; }

        public int OrderView { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }

        public decimal? Val1 { get; set; }

        public decimal? Val2 { get; set; }

        public decimal? Val3 { get; set; }

        public decimal? Val4 { get; set; }

        public decimal? Val5 { get; set; }

        public decimal? Val6 { get; set; }
        public DateTime? BalanceDate { get; set; }
    }

    public class SitFinanReportColList
    {
        public string Col1 { get; set; }

        public string Col2 { get; set; }

        public string Col3 { get; set; }

        public string Col4 { get; set; }

        public string Col5 { get; set; }

        public string Col6 { get; set; }
    }
}
