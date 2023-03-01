using Abp.Application.Services;
using Abp.UI;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.RegistruInventar;
using System;

namespace Niva.Erp.Conta.RegistruInventar
{
    public interface IRegInventarAppService : IApplicationService
    {
        void RecalculRegInventar(DateTime reportDate);
    }

    public class RegInventarAppService: ErpAppServiceBase, IRegInventarAppService
    {

        IRegInventarRepository _regInvRepository;
        IBalanceRepository _balanceRepository;
        public RegInventarAppService(IRegInventarRepository regInvRepository, IBalanceRepository balanceRepository)
        {
            _regInvRepository = regInvRepository;
            _balanceRepository = balanceRepository;
        }

        //public void RecalculRegInventar(DateTime reportDate)
        //{

        //    var appClientId = GetCurrentTenant().Id;
        //    var localCurrencyId = 1;
        //    try
        //    {
        //        _regInvRepository.RecalculRegInvExcept(reportDate, appClientId, localCurrencyId);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new UserFriendlyException("Eroare", ex.Message);
        //    }
        //}

        public void RecalculRegInventar(DateTime reportDate)
        {

            var appClientId = GetCurrentTenant().Id;
            var localCurrencyId = 1;
            try
            {
                var balance = _balanceRepository.CreateTempBalance(reportDate, false, appClientId);
                _regInvRepository.RecalculRegInvExceptBal(balance, reportDate, appClientId, localCurrencyId);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
