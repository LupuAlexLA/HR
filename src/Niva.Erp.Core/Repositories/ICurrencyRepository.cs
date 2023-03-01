using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories
{
    public interface ICurrencyRepository : IRepository<Currency, int>
    {
        Currency GetByCode(string currencyCode);
        Currency GetCurrencyById(int id);
    }
}
