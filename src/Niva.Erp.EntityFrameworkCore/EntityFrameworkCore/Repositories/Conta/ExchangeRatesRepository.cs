using Abp.EntityFramework;
using Abp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.EntityFramework.Repositories.Nomenclatures
{
    public class ExchangeRatesRepository : ErpRepositoryBase<ExchangeRates, int>, IExchangeRatesRepository
    {
        public ExchangeRatesRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public decimal GetExchangeRate(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId)
        {
            decimal ret;
            if (fromCurrencyId == toCurrencyId)
            {
                ret = 1;
            }
            else
            {
                var _exchangeDate = new DateTime(exchangeDate.Year, exchangeDate.Month, exchangeDate.Day);

                if (fromCurrencyId == 0)
                {
                    throw new Exception("Trebuie sa selectati moneda.");
                }

                var exchangeRate = Context.ExchangeRates.Where(f => f.CurrencyId == fromCurrencyId && f.ExchangeDate <= _exchangeDate) // se ia cursul din data
                                                        .OrderByDescending(f => f.ExchangeDate)
                                                        .FirstOrDefault();
                //todo check if not to old
                if (exchangeRate == null)
                {
                    throw new Exception("No exchange rate found");
                }
                else
                {
                    ret = exchangeRate.Value;
                }
            }
            return ret;
        }

        public decimal GetExchangeRateForOper(DateTime exchangeDate, int fromCurrencyId, int toCurrencyId)
        {
            decimal ret;
            if (fromCurrencyId == toCurrencyId)
            {
                ret = 1;
            }
            else
            {
                var _exchangeDate = new DateTime(exchangeDate.Year, exchangeDate.Month, exchangeDate.Day).AddDays(-1);

                if (fromCurrencyId == 0)
                {
                    throw new Exception("Trebuie sa selectati moneda.");
                }

                var exchangeRate = Context.ExchangeRates.Where(f => f.CurrencyId == fromCurrencyId && f.ExchangeDate <= _exchangeDate) // se ia cursul din data - 1
                                                        .OrderByDescending(f => f.ExchangeDate)
                                                        .FirstOrDefault();
                //todo check if not to old
                if (exchangeRate == null)
                {
                    throw new Exception("No exchange rate found");
                }
                else
                {
                    ret = exchangeRate.Value;
                }
            }
            return ret;
        }

        public decimal GetLocalExchangeRate(DateTime exchangeDate, int fromCurrencyId, int localCurrencyId)
        {
            var exchangeDateNoHour = new DateTime(exchangeDate.Year, exchangeDate.Month, exchangeDate.Day);
            return GetExchangeRate(exchangeDateNoHour, fromCurrencyId, localCurrencyId);
        }
    }
}
