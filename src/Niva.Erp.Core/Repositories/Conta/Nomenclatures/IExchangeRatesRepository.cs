 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;

namespace Niva.Erp.Repositories.Conta.Nomenclatures
{
    public interface IExchangeRatesRepository : IRepository<ExchangeRates, int>
    {
        decimal GetExchangeRate(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId);

        decimal GetExchangeRateForOper(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId);

        decimal GetLocalExchangeRate(DateTime exchangeDate, int fromCurrencyId, int localCurrecyId);

    }
}
