using Abp.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.RegistruInventar;
using Niva.Erp.Repositories.Conta.RegistruInventar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.RegistruInventar
{
    public class RegInventarRepository : ErpRepositoryBase<RegInventar, int>, IRegInventarRepository
    {
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';
        string[] coloaneBalanta = { "D", "C" };

        AccountRepository _accountRepository;
        AutoOperationRepository _autoOperRepository;
        BalanceRepository _balanceRepository;

        public RegInventarRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _accountRepository = new AccountRepository(dbContextProvider);
            _autoOperRepository = new AutoOperationRepository(dbContextProvider);
            _balanceRepository = new BalanceRepository(dbContextProvider);
        }

        public void DeleteAllRegInv(DateTime reportDate)
        {
            try
            {
                var regInvList = Context.RegInventar.Where(f => f.State == State.Active && f.ReportDate == reportDate).ToList();

                if (regInvList.Count > 0)
                {
                    Context.RegInventar.RemoveRange(regInvList);
                    Context.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw new Exception("Operatiunea nu poate fi inregistrata.");
            }

        }

        public void RecalculRegInvExcept(DateTime reportDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DeleteAllRegInv(reportDate);
                var accountList = _accountRepository.GetAnalythicsWithoutActivityType().ToList();
                var regInvExceptAccountList = Context.RegInventarExceptii.Where(f => f.State == State.Active).ToList();

                var reportList = accountList.Select(f => new RegInventar
                {
                    AccountId = f.Id,
                    Value = 0,
                    ReportDate = reportDate,
                    TenantId = appClientId
                }).ToList();

                var tempReportList = reportList;

                foreach (var item in reportList.ToList())
                {
                    tempReportList = RegInventarCalcFormulaExceptii(item, regInvExceptAccountList, tempReportList, reportDate, appClientId, 0, localCurrencyId);
                }

                Context.RegInventar.AddRange(tempReportList.Where(f => f.Value != 0));
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Operatiunea nu poate fi inregistrata");
            }
        }

        public List<RegInventar> RegInventarCalc(List<Account> accounts, List<RegInventarExceptii> regInvExceptList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId)
        {
            var regInventarList = Context.RegInventar.Where(f => f.ReportDate == reportDate && f.State == State.Active).ToList();

            if (regInventarList.Count == 0)
            {
                var reportList = accounts.Select(f => new RegInventar
                {
                    AccountId = f.Id,
                    Value = 0,
                    ReportDate = reportDate,
                    TenantId = appClientId
                }).ToList();

                var tempReportList = reportList;

                foreach (var item in reportList.ToList())
                {
                    tempReportList = RegInventarCalcFormulaExceptii(item, regInvExceptList, tempReportList, reportDate, appClientId, currencyId, localCurrencyId);
                }
                Context.RegInventar.AddRange(tempReportList);
                Context.SaveChanges();

                regInventarList = tempReportList;
            }

            return regInventarList;
        }

        public List<RegInventar> RegInventarCalcFormulaExceptii(RegInventar regInventar, List<RegInventarExceptii> regInvExceptList, List<RegInventar> tempReportList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId)
        {
            decimal rez = 0;
            decimal value = 0;

            // parcurg lista de exceptii
            foreach (var exceptAccount in regInvExceptList)
            {
                if ((exceptAccount.Formula != null || exceptAccount.Formula != "") && exceptAccount.AccountId == regInventar.AccountId)
                {
                    string expresie = exceptAccount.Formula;
                    while (expresie.IndexOf(separatorExpresie) >= 0)
                    {
                        int indexStart = expresie.IndexOf(separatorExpresie);
                        int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                        string item = expresie.Substring(indexStart + 1, indexEnd - indexStart - 1);
                        string[] splitItem = item.Split(separator);

                        string tipItem = splitItem[0];
                        string contItem = splitItem[1];

                        try
                        {
                            var accountId = _autoOperRepository.GetAccount(contItem, appClientId, reportDate, localCurrencyId);
                            var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);

                            //value = Math.Abs(_balanceRepository.GetSoldAccountAll(reportDate, _accountRepository.GetAccountBySymbol(account.Symbol), currencyId, localCurrencyId, tipItem));
                            value = Math.Abs(_balanceRepository.GetSoldTypeAccount(reportDate, _accountRepository.GetAccountBySymbol(account.Symbol).Id, appClientId, currencyId, localCurrencyId, true, tipItem).Sold);

                            if (exceptAccount.AccountId != account.Id)
                            {
                                tempReportList.RemoveAll(f => f.AccountId == account.Id);
                            }

                        }
                        catch (Exception ex)
                        {

                            throw new Exception("Eroare", ex);
                        }
                        IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                        string valueF = value.ToString(formatProvider);
                        if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                        expresie = expresie.Replace("$" + item + "$", valueF);

                    }

                    try
                    {
                        rez = Convert.ToDecimal(new DataTable().Compute(expresie, null));
                        regInventar.Value = rez;
                    }
                    catch (Exception ex)
                    {
                        rez = 0;
                    }
                }
                else
                {

                    if (regInventar.Value == 0)
                    {
                        var account = _accountRepository.FirstOrDefault(f => f.Id == regInventar.AccountId);
                        //value = Math.Abs(_balanceRepository.GetSoldAccountAll(reportDate, _accountRepository.GetAccountBySymbol(account.Symbol), currencyId, localCurrencyId, null));
                        value = Math.Abs(_balanceRepository.GetSoldTypeAccount(reportDate, _accountRepository.GetAccountBySymbol(account.Symbol).Id, appClientId, currencyId, localCurrencyId, true, "").Sold);

                        regInventar.Value = value;
                    }
                }

            }

            return tempReportList;
        }

        public void RecalculRegInvExceptBal(Balance balance, DateTime reportDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DeleteAllRegInv(reportDate);
                var accountList = _accountRepository.GetAnalythicsWithoutActivityType().ToList();
                var regInvExceptAccountList = Context.RegInventarExceptii.Where(f => f.State == State.Active).ToList();

                var reportList = accountList.Select(f => new RegInventar
                {
                    AccountId = f.Id,
                    Value = 0,
                    ReportDate = reportDate,
                    TenantId = appClientId
                }).ToList();

                var tempReportList = reportList;

                foreach (var item in reportList.OrderBy(f => f.AccountId).ToList())
                {
                    tempReportList = RegInventarCalcFormulaExceptiiBal(balance, item, regInvExceptAccountList, tempReportList, reportDate, appClientId, localCurrencyId, localCurrencyId);
                }

                Context.RegInventar.AddRange(tempReportList.Where(f => f.Value != 0));
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RegInventar> RegInventarCalcBal(Balance balance, List<Account> accounts, List<RegInventarExceptii> regInvExceptList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId)
        {
            var regInventarList = Context.RegInventar.Where(f => f.ReportDate == reportDate && f.State == State.Active).ToList();

            if (regInventarList.Count == 0)
            {
                var reportList = accounts.Select(f => new RegInventar
                {
                    AccountId = f.Id,
                    Value = 0,
                    ReportDate = reportDate,
                    TenantId = appClientId
                }).ToList();

                var tempReportList = reportList;

                foreach (var item in reportList.ToList())
                {
                    tempReportList = RegInventarCalcFormulaExceptiiBal(balance, item, regInvExceptList, tempReportList, reportDate, appClientId, currencyId, localCurrencyId);
                }
                tempReportList = tempReportList.Where(f => f.Value != 0).ToList();
                Context.RegInventar.AddRange(tempReportList);
                Context.SaveChanges();

                regInventarList = tempReportList;
            }

            return regInventarList;
        }

        private List<RegInventar> RegInventarCalcFormulaExceptiiBal(Balance balance, RegInventar regInventar, List<RegInventarExceptii> regInvExceptList, List<RegInventar> tempReportList, DateTime reportDate, int appClientId, int currencyId, int localCurrencyId)
        {
            try
            {


                decimal rez = 0;
                decimal value = 0;

                // parcurg lista de exceptii
                foreach (var exceptAccount in regInvExceptList)
                {
                    if ((exceptAccount.Formula != null || exceptAccount.Formula != "") && exceptAccount.AccountId == regInventar.AccountId)
                    {
                        string expresie = exceptAccount.Formula;
                        while (expresie.IndexOf(separatorExpresie) >= 0)
                        {
                            int indexStart = expresie.IndexOf(separatorExpresie);
                            int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                            string item = expresie.Substring(indexStart + 1, indexEnd - indexStart - 1);
                            string[] splitItem = item.Split(separator);

                            string tipItem = splitItem[0];
                            string contItem = splitItem[1];

                            try
                            {
                                var accountId = _autoOperRepository.GetAccount(contItem, appClientId, reportDate, localCurrencyId);
                                var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);


                                value = GetSoldFromBal(balance, account.Symbol, localCurrencyId);
                                if (exceptAccount.AccountId != account.Id)
                                {
                                    tempReportList.RemoveAll(f => f.AccountId == account.Id);
                                }

                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                            IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                            string valueF = value.ToString(formatProvider);
                            if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                            expresie = expresie.Replace("$" + item + "$", valueF);

                        }

                        try
                        {
                            rez = Convert.ToDecimal(new DataTable().Compute(expresie, null));
                            regInventar.Value = rez;
                        }
                        catch (Exception ex)
                        {
                            rez = 0;
                        }
                    }
                    else
                    {

                        if (regInventar.Value == 0)
                        {
                            var account = _accountRepository.FirstOrDefault(f => f.Id == regInventar.AccountId);
                            value = GetSoldFromBal(balance, account.Symbol, localCurrencyId);
                            regInventar.Value = value;
                        }
                    }

                }

                return tempReportList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private decimal GetSoldFromBal(Balance balance, string account, int localCurrencyId)
        {
            decimal sold = 0;
            var accountBalList = balance.BalanceDetails.Where(f => f.Account.Symbol.IndexOf(account) == 0).ToList();
            foreach (var item in accountBalList)
            {
                decimal exchangeRate = 1;

                if (localCurrencyId != item.Currency.Id)
                {
                    exchangeRate = Context.ExchangeRates.Where(f => f.CurrencyId == item.Currency.Id && f.ExchangeDate <= balance.BalanceDate).OrderByDescending(f => f.ExchangeDate).FirstOrDefault().Value;
                }
                decimal value = (item.Account.AccountTypes == AccountTypes.Active || item.Account.AccountTypes == AccountTypes.Bifunctional)
                                ? (item.DbValueF - item.CrValueF) : (item.CrValueF - item.DbValueF);
                value = Math.Round(value * exchangeRate, 2);
                sold += value;
            }
            return sold;
        }

    }
}
