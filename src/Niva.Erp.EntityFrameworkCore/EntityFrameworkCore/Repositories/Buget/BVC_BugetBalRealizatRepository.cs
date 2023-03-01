using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_BugetBalRealizatRepository : ErpRepositoryBase<BVC_BalRealizat, int>, IBVC_BugetBalRealizatRepository
    {
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';

        public BVC_BugetBalRealizatRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider) {}

        public decimal FormulaBalantaCalc(string formula, DateTime calcDate, int appClientId, int localCurrencyId, int activityTypeId, List<SavedBalanceDetails> savedBalanceDetails, out List<BVC_BalRealizatRandDetails> balRealizatDetails)
        {
            try
            {
                decimal rez = 0;
                balRealizatDetails = new List<BVC_BalRealizatRandDetails>();

                if (formula == null || formula == "")
                {
                    return 0;
                }

                string expresie = formula;
                while (expresie.IndexOf(separatorExpresie) >= 0)
                {
                    int indexStart = expresie.IndexOf(separatorExpresie);
                    int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                    string item = expresie.Substring(indexStart + 1, indexEnd - indexStart - 1);
                    string[] splitItem = item.Split(separator);

                    string tipItem = splitItem[0];
                    string contItem = splitItem[1];
                    decimal? value = 0;

                    value = (decimal)savedBalanceDetails.Where(f => f.Account.ActivityTypeId == activityTypeId && f.Account.Symbol.IndexOf(contItem) == 0)
                                                                     .Sum(f => f.DbValueM);

                    IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                    string valueF = value.Value.ToString(formatProvider);
                    if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                    expresie = expresie.Replace("$" + item + "$", valueF);
                    balRealizatDetails.Add(new BVC_BalRealizatRandDetails
                    {
                        Valoare = value.Value,
                        Descriere = contItem
                    });
                }

                try
                {
                    rez = Convert.ToDecimal(new DataTable().Compute(expresie, null));
                }
                catch (Exception ex)
                {
                    rez = 0;
                }

                return rez;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
