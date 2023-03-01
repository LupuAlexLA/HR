using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Buget;
using Niva.Erp.Repositories.Buget;
using System;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_BugetRealizatRepository : ErpRepositoryBase<BVC_Realizat, int>, IBVC_BugetRealizatRepository
    {
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';

        AccountRepository _accountRepository;
        AutoOperationRepository _autoOperRepository;
        ExchangeRatesRepository _exchangeRatesRepository;

        public BVC_BugetRealizatRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _accountRepository = new AccountRepository(dbContextProvider);
            _autoOperRepository = new AutoOperationRepository(dbContextProvider);
            _exchangeRatesRepository = new ExchangeRatesRepository(dbContextProvider);
        }

        public decimal FormulaContaCalc(string formula, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                decimal rez = 0;

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
                    decimal rulajDb = 0, rulajCr = 0;

                    var accountId = _autoOperRepository.GetAccount(contItem, appClientId, calcDate, localCurrencyId);
                    var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);
                    var accountList = _accountRepository.GetAllAnalythicsSintetic(accountId).Select(f => f.Id).Distinct().ToList();
                    accountList.Add(accountId);
                    decimal exchangeRate = 1;

                    var valoriRulajDB = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit).Where(f => f.Operation.OperationDate == calcDate && accountList.Contains(f.DebitId)).ToList();
                    foreach (var rulaj in valoriRulajDB)
                    {
                        if (rulaj.Operation.CurrencyId != localCurrencyId)
                        {
                            exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(calcDate, rulaj.Operation.CurrencyId, localCurrencyId);
                        }
                        else
                        {
                            exchangeRate = 1;
                        }
                        rulajDb += Math.Round(rulaj.Value * exchangeRate, 2);
                    }

                    var valoriRulajCr = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Credit).Where(f => f.Operation.OperationDate == calcDate && accountList.Contains(f.CreditId)).ToList();
                    foreach (var rulaj in valoriRulajCr)
                    {
                        if (rulaj.Operation.CurrencyId != localCurrencyId)
                        {
                            exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(calcDate, rulaj.Operation.CurrencyId, localCurrencyId);
                        }
                        else
                        {
                            exchangeRate = 1;
                        }
                        rulajCr += Math.Round(rulaj.Value * exchangeRate, 2);
                    }

                    decimal? value = tipItem == "ORLD" ? rulajDb : rulajCr;

                    IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                    string valueF = value.Value.ToString(formatProvider);
                    if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                    expresie = expresie.Replace("$" + item + "$", valueF);

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
