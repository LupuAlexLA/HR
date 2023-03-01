using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.SituatiiFinanciare
{
    public interface ISitFinanRapAppService : IApplicationService
    {
        List<SitFinanCalcBalanceListDto> CalcBalanceList(bool isDailyBalance);
        List<SitFinanCalcRapListDto> CalcRaportList(int? saveBalanceId);
        int RaportColNumber(int raportId);
        List<SitFinanCalcRapListDto> GetCalcRaportListByDateRange(DateTime startDate, DateTime endDate, bool isDailyBalance);
        List<SitFinanReportColumn> GetReportColumnList(int raportId);
    }

    public class SitFinanRapAppService : ErpAppServiceBase, ISitFinanRapAppService
    {
        ISitFinanCalcRepository _sitFinanCalcRepository;
        IPersonRepository _personRepository;
        IRepository<LegalPerson> _legalPersonRepository { get; set; }
        ISavedBalanceRepository _savedBalanceRepository;
        IRepository<SitFinanRapConfigCol> _sitFinanRapConfigColRepository;

        public SitFinanRapAppService(ISitFinanCalcRepository sitFinanCalcRepository, IPersonRepository personRepository, IRepository<LegalPerson> legalPersonRepository, IRepository<SitFinanRapConfigCol> sitFinanRapConfigColRepository,
            IRepository<SitFinanCalcNote> sitFinanCalcNoteRepository, ISavedBalanceRepository savedBalanceRepository)
        {
            _sitFinanCalcRepository = sitFinanCalcRepository;
            _personRepository = personRepository;
            _legalPersonRepository = legalPersonRepository;
            _savedBalanceRepository = savedBalanceRepository;
            _sitFinanRapConfigColRepository = sitFinanRapConfigColRepository;
        }

        //[AbpAuthorize("Conta.SitFinan.Rapoarte.Acces")]
        public List<SitFinanCalcBalanceListDto> CalcBalanceList(bool isDailyBalance)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var list = _sitFinanCalcRepository.GetAllIncluding(f => f.SavedBalance, f => f.SitFinanRap).Where(f => f.SitFinanRap.TenantId == appClient.Id && f.SavedBalance.IsDaily == isDailyBalance)
                                                  .Select(f => new
                                                  {
                                                      f.SavedBalanceId,
                                                      CalcDate = f.SavedBalance.SaveDate,
                                                      BalanceName = f.SavedBalance.BalanceName
                                                  }).Distinct().ToList().Select(f => new SitFinanCalcBalanceListDto
                                                  {
                                                      SavedBalanceId = f.SavedBalanceId,
                                                      CalcDate = f.CalcDate,
                                                      CalcDateStr = LazyMethods.DateToString(f.CalcDate) + " - " + f.BalanceName
                                                  }).OrderByDescending(f => f.CalcDate).ToList();

                return list;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<SitFinanCalcRapListDto> CalcRaportList(int? saveBalanceId)
        {
            if (saveBalanceId != null)
            {
                var list = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRap)
                                                  .Where(f => f.SavedBalanceId == saveBalanceId)
                                                  .Select(f => new SitFinanCalcRapListDto
                                                  {
                                                      RaportId = f.SitFinanRapId,
                                                      RaportName = f.SitFinanRap.ReportName,
                                                      OrderView = f.SitFinanRap.OrderView
                                                  })
                                                  .Distinct()
                                                  .OrderBy(f => f.OrderView)
                                                  .ToList();
                return list;
            }
            else
            {
                return new List<SitFinanCalcRapListDto>();
            }
        }

        public int RaportColNumber(int raportId)
        {
            try
            {
                var raport = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRap).FirstOrDefault(f => f.SitFinanRapId == raportId);
                return raport.SitFinanRap.NrCol;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<SitFinanCalcRapListDto> GetCalcRaportListByDateRange(DateTime startDate, DateTime endDate, bool isDailyBalance)
        {

            var savedBalanceIdsList = _savedBalanceRepository.GetAll().Where(f => f.SaveDate >= startDate && f.SaveDate <= endDate && f.IsDaily == isDailyBalance).GroupBy(f => f.SaveDate).Select(f => f.Max(x => x.Id)).ToList();

            var list = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRap)
                                                          .Where(f => savedBalanceIdsList.Contains(f.SavedBalanceId))
                                                          .Select(f => new SitFinanCalcRapListDto
                                                          {
                                                              RaportId = f.SitFinanRapId,
                                                              RaportName = f.SitFinanRap.ReportName,
                                                              OrderView = f.SitFinanRap.OrderView
                                                          })
                                                          .Distinct()
                                                          .OrderBy(f => f.OrderView)
                                                          .ToList();
            return list;
        }

        public List<SitFinanReportColumn> GetReportColumnList(int raportId)
        {
            var columnList = _sitFinanRapConfigColRepository.GetAllIncluding(f => f.SitFinanRap)
                                                            .Where(f => f.SitFinanRapId == raportId)
                                                            .Select(f => new SitFinanReportColumn
                                                            {
                                                                Id = f.Id,
                                                                ColumnName = f.ColumnName,
                                                                ColumnNr = f.ColumnNr
                                                            })
                                                            .OrderBy(f => f.ColumnNr)
                                                            .ToList();
            return columnList;
        }
    }
}
