using Abp.Application.Services;
using Abp.Authorization;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Managers;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Balance
{
    public interface IBalanceAppService : IApplicationService
    {
        BalanceInitDto InitBalanceForm();

        BalanceInitDto BalanceList(BalanceInitDto balanceInit);

        BalanceInitDto BalanceDetailsInit(int balanceId, BalanceInitDto balanceInit, string balanceType);

        BalanceInitDto BalanceDetails(BalanceInitDto balanceInit);

        BalanceInitDto ComputeBalance(BalanceInitDto balanceInit);

        void DeleteBalance(int balanceId);

        List<BalanceDDDto> BalanceDDList();

        List<BalanceCompSummaryDto> BalanceSummaryList(DateTime compDate);
        List<BalanceCompValidDto> BalanceValidList(int balanceId);
        DateTime GetBalanceDateNextDay();
    }

    public class BalanceAppService : ErpAppServiceBase, IBalanceAppService
    {
        IBalanceRepository _balanceRepository;
        ContaManager _contaManager;

        public BalanceAppService(IBalanceRepository balanceRepository, ContaManager contaManager)
        {
            _balanceRepository = balanceRepository;
            _contaManager = contaManager;
        }

        public BalanceInitDto InitBalanceForm()
        {
            try
            {
                var _currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var ret = new BalanceInitDto
                {
                    SearchStartDate = _currentDate.AddYears(-1),
                    SearchEndDate = _currentDate,
                    CalcDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddDays(-1),
                    ShowForm = 1
                };
                ret = BalanceList(ret);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        //[AbpAuthorize("Conta.Balanta.Balante.Acces")]
        public BalanceInitDto BalanceList(BalanceInitDto balanceInit)
        {
            try
            {
                int appClientId = GetCurrentTenant().Id;
                var balanceList = _balanceRepository.GetAll()
                                                    .Where(f => f.Status == State.Active && f.TenantId == appClientId && f.BalanceDate >= balanceInit.SearchStartDate && f.BalanceDate <= balanceInit.SearchEndDate)
                                                    .OrderByDescending(f => f.StartDate)
                                                    .ToList()
                                                    .Select(f => new BalanceListDto { Id = f.Id, BalanceDate = f.BalanceDate.ToShortDateString(), OkDelete = false, OkValid = BalanceValidList(f.Id).All(x => x.Ok == f.OkValid) })
                                                    .ToList();
                try
                {
                    balanceList[0].OkDelete = true;
                }
                catch
                {

                }
                balanceInit.BalanceList = balanceList;
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        public BalanceInitDto BalanceDetailsInit(int balanceId, BalanceInitDto balanceInit, string balanceType)
        {
            try
            {
                var _balanceType = (balanceType == "A") ? BalanceType.Analythic : BalanceType.Synthetic;

                var balanceDetailForm = new ViewBalanceDetailDto
                {
                    Id = balanceId,
                    BalanceTypeStr = _balanceType.ToString(),
                    BalanceType = _balanceType,
                    SearchAccount = ""
                };
                balanceInit.ViewBalanceDetail = balanceDetailForm;
                balanceInit = BalanceDetails(balanceInit);
                balanceInit.ShowForm = 2;
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public BalanceInitDto BalanceDetails(BalanceInitDto balanceInit)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var _balanceType = balanceInit.ViewBalanceDetail.BalanceType;
                var localCurrecyId = appClient.LocalCurrencyId.Value;

                bool convertAllCurrencies = (balanceInit.ViewBalanceDetail.CurrencyId == 0); //RON si echivalent RON
                bool convertToLocalCurrency = (balanceInit.ViewBalanceDetail.CurrencyId == 0);

                var list = _balanceRepository.GetBalanceDetails(balanceInit.ViewBalanceDetail.Id, true, balanceInit.ViewBalanceDetail.SearchAccount,
                                                                balanceInit.ViewBalanceDetail.CurrencyId, convertToLocalCurrency, convertAllCurrencies, localCurrecyId, null);
                balanceInit.ViewBalanceDetail.BalanceDate = list.BalanceDate.ToShortDateString();
                var balanceDetail = ObjectMapper.Map<List<BalanceDetailDto>>(list.Details);
                balanceInit.ViewBalanceDetail.BalanceDetail = balanceDetail;
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new Exception(LazyMethods.GetErrMessage(ex));
            }
        }

        [AbpAuthorize("Conta.Balanta.Balante.Modificare")]
        public BalanceInitDto ComputeBalance(BalanceInitDto balanceInit)
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;
                _balanceRepository.Compute(balanceInit.CalcDate, balanceInit.ClosingMonthOper, appClientId);
                balanceInit = BalanceList(balanceInit);
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        [AbpAuthorize("Conta.Balanta.Balante.Modificare")]
        public void DeleteBalance(int balanceId)
        {
            try
            {
                _balanceRepository.DeleteBalance(balanceId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
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
        public DateTime GetBalanceDateNextDay()
        {
            return _balanceRepository.BalanceDateNextDay();
        }
        public List<BalanceCompSummaryDto> BalanceSummaryList(DateTime compDate)
        {
            try
            {
                var ret = new List<BalanceCompSummaryDto>();
                int appClientId = GetCurrentTenant().Id;

                var _list = _contaManager.BalanceSummaryList(compDate, appClientId);
                ret = ObjectMapper.Map<List<BalanceCompSummaryDto>>(_list);

                return ret;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BalanceCompValidDto> BalanceValidList(int balanceId)
        {

            try
            {
                var ret = new List<BalanceCompValidDto>();
                int appClientId = GetCurrentTenant().Id;

                var _list = _contaManager.BalanceCompValidList(balanceId, appClientId);
                var list = ObjectMapper.Map<List<BalanceCompValidDto>>(_list);

                foreach (var item in list)
                {
                    ret.Add(item);
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
