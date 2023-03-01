using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.RegistruInventar;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Conta.RegistruInventar
{
    public interface IRegInventarRepository: IRepository<RegInventar, int>
    {
        List<RegInventar> RegInventarCalc(List<Account> accounts, List<RegInventarExceptii> regInvExceptList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId);
        List<RegInventar> RegInventarCalcFormulaExceptii(RegInventar regInventar, List<RegInventarExceptii> regInvExceptList, List<RegInventar> tempRegInventarList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId);
        void DeleteAllRegInv(DateTime reportDate);
        void RecalculRegInvExcept(DateTime reportDate, int appClientId, int localCurrencyId);
        void RecalculRegInvExceptBal(Balance balance, DateTime reportDate, int appClientId, int localCurrencyId);
        List<RegInventar> RegInventarCalcBal(Balance balance, List<Account> accounts, List<RegInventarExceptii> regInvExceptList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId);
    }
}
