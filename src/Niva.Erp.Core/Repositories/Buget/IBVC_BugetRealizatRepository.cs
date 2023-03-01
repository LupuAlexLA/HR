using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using System;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_BugetRealizatRepository : IRepository<BVC_Realizat, int>
    {
        decimal FormulaContaCalc(string formula, DateTime calcDate, int appClientId, int localCurrencyId);
    }
}
