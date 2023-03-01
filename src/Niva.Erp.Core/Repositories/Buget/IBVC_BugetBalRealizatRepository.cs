using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_BugetBalRealizatRepository : IRepository<BVC_BalRealizat, int>
    {
        decimal FormulaBalantaCalc(string formula, DateTime calcDate, int appClientId, int localCurrencyId, int activityTypeId, List<SavedBalanceDetails> savedBalanceDetails, 
                                    out List<BVC_BalRealizatRandDetails> balRealizatDetails);
    }
}
