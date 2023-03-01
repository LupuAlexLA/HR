using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Logging;
using Abp.UI;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface ICurrencyAppService : IApplicationService
    {
        List<CurrencyDto> CurrencyDDList();

        decimal GetLocalExchangeRate(DateTime exchangeDate, int fromCurrencyId);

        decimal GetLocalExchangeRateForOper(DateTime exchangeDate, int fromCurrencyId);

        decimal GetExchangeRate(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId);

        int GetLocalCurrencyId();
    }

    public class CurrencyAppService : ErpAppServiceBase, ICurrencyAppService
    {
        IRepository<Currency> _currencyRepository;
        IExchangeRatesRepository _exchangeRatesRepository;

        public CurrencyAppService(IRepository<Currency> currencyRepository, IExchangeRatesRepository exchangeRatesRepository)
        {
            _currencyRepository = currencyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        public List<CurrencyDto> CurrencyDDList()
        {
            var currencyList = _currencyRepository.GetAll();
            var ret = ObjectMapper.Map<List<CurrencyDto>>(currencyList);
            return ret;
        }

        public int GetLocalCurrencyId()
        {
            //throw new NotImplementedException("Uncomment");
            var appClient = GetCurrentTenant();
            var localCurrencyId = appClient.LocalCurrencyId.Value;
            return localCurrencyId;
        }

        public decimal GetLocalExchangeRate(DateTime exchangeDate, int fromCurrencyId)
        {
            //throw new NotImplementedException("Uncomment");
            var localCurrencyId = GetLocalCurrencyId();
            var exchangeDateNoHour = new DateTime(exchangeDate.Year, exchangeDate.Month, exchangeDate.Day);
            return _exchangeRatesRepository.GetLocalExchangeRate(exchangeDateNoHour, fromCurrencyId, localCurrencyId);
        }

        public decimal GetLocalExchangeRateForOper(DateTime exchangeDate, int fromCurrencyId)
        {
            //throw new NotImplementedException("Uncomment");
            var localCurrencyId = GetLocalCurrencyId();
            var exchangeDateNoHour = new DateTime(exchangeDate.Year, exchangeDate.Month, exchangeDate.Day).AddDays(-1);
            return _exchangeRatesRepository.GetLocalExchangeRate(exchangeDateNoHour, fromCurrencyId, localCurrencyId);
        }

        public decimal GetExchangeRate(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId)
        {
            try
            {
                decimal ret = _exchangeRatesRepository.GetExchangeRate(exchangeDate, fromCurrencyId, toCurrencyId);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message, LogSeverity.Fatal);
            }

        }
    }
}
