using Abp.Domain.Repositories;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Conta
{
    public interface ISavedBalanceRepository : IRepository<SavedBalance, int>
    {
        //SavedBalance AddSavedBalance(int id, string Name, bool externalSave);
        SavedBalance AddSavedBalance(Balance balance, string name, bool balantaZilnica);
        BalanceView GetBalanceDetails(int balanceId, Boolean addTotals, string searchData, int idCurrency, int? nivelRand);

        void DeleteSavedBalance(int id);

        List<SavedBalanceViewDet> GetSavedBalanceViewDetByNivelRand(int savedBalanceId, int? nivelRand);
    }
}
