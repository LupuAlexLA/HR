using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories
{
    public class CurrencyRepository : ErpRepositoryBase<Currency, int>, ICurrencyRepository
    {
        public CurrencyRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public Currency GetByCode(string currencyCode)
        {
            var currency = Context.Currency.FirstOrDefault(p => p.CurrencyCode == currencyCode);
            return currency;
        }

        public Currency GetCurrencyById(int id)
        {
            var currency = Context.Currency.FirstOrDefault(p => p.Id == id);
            return currency;
        }
    }
}
