using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.ConfigurareRapoarte;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class ConfigReportRepository : ErpRepositoryBase<ReportConfig, int>, IConfigReportRepository
    {
        string[] dateFormula = { "$PCI$", "$PCF$", "$PCA$", "$PPI$", "$PPF$", "$PPA$" };
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';
        string[] coloaneBalanta = { "D", "C", "RD", "RC" };

        BalanceRepository _balanceRepository;
        AccountRepository _accountRepository;
        AutoOperationRepository _autoOperRepository;

        public ConfigReportRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _balanceRepository = new BalanceRepository(dbContextProvider);
            _accountRepository = new AccountRepository(dbContextProvider);
            _autoOperRepository = new AutoOperationRepository(dbContextProvider);
        }

        public List<ReportCalcItem> CalcReportValue(ReportConfig item, int coloana, int reportId, List<ReportCalcItem> calcRap, DateTime reportDate, int tenantId, int currencyId,
                                                int localCurencyId, List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta)
        {
            decimal rez = 0;

            string formula = item.FormulaRulaj;

            bool calculat = false;
            rez = CalcValCol(formula, coloana, reportId, item.Id, calcRap, reportDate, tenantId, currencyId, localCurencyId, rapoarteList, rulaj, contaOperList, balanta, out calculat);

            if (calculat)
            {
                var calcItem = calcRap.FirstOrDefault(f => f.ReportConfigRowId == item.Id && f.ReportDate == reportDate);
                calcItem.RowValue = rez;
                calcItem.Calculat = true;
            }
            return calcRap;
        }

        public decimal CalcValCol(string formula, int coloana, int reportId, int id, List<ReportCalcItem> calcRap, DateTime reportDate, int tenantId, 
                                  int currencyId, int localCurencyId, List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta, out bool calculat)
        {
            calculat = true;
            decimal rez = 0;
            rez = CalcFormula(formula, reportId, reportDate, tenantId, currencyId, localCurencyId, calcRap, rapoarteList, rulaj, contaOperList, balanta, out calculat);
            return rez;
        }

        public decimal CalcFormula(string formula, int reportId, DateTime reportDate, int tenantId, int currencyId, int localCurencyId, List<ReportCalcItem> calcRap, 
                                   List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta, out bool calculat)
        {
            decimal rez = 0;
            calculat = true;

            if (formula == "" || formula == null)
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

                // e din contabilitate
                if (coloaneBalanta.Contains(tipItem))
                {
                    try
                    {
                        var startDate = LazyMethods.FirstDayNextMonth(reportDate.AddMonths(-1));
                        //var accountId = _autoOperRepository.GetAccount(contItem, tenantId, reportDate, localCurencyId);
                        //var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);
                        //var accountList = _accountRepository.GetAllAnalythics(accountId).Select(f => f.Id).ToList();
                        //accountList.Add(accountId);

                        if (rulaj)
                        {
                            var rulajDb = contaOperList.Where(f => f.OperationDate >= startDate && f.OperationDate <= reportDate && f.DebitSymbol.IndexOf(contItem) == 0 /*&& f.CurrencyId == localCurencyId*/)
                                                       .Sum(f => f.Valoare);

                            var rulajCr = contaOperList.Where(f => f.OperationDate >= startDate && f.OperationDate <= reportDate && f.CreditSymbol.IndexOf(contItem) == 0 /*&& f.CurrencyId == localCurencyId*/)
                                                       .Sum(f => f.Valoare);

                            //var rulajDb = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit).Include(f => f.Credit).Include(f => f.Operation.DocumentType).Include(f => f.Operation.Currency)
                            //                           .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == tenantId &&
                            //                                  f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= reportDate && f.Operation.CurrencyId == localCurencyId && accountList.Contains(f.DebitId)).Sum(f => f.Value);

                            //var rulajCr = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit).Include(f => f.Credit).Include(f => f.Operation.DocumentType).Include(f => f.Operation.Currency)
                            //                           .Where(f => f.Operation.State == State.Active && f.Operation.TenantId == tenantId &&
                            //                                  f.Operation.OperationDate >= startDate && f.Operation.OperationDate <= reportDate && f.Operation.CurrencyId == localCurencyId && accountList.Contains(f.CreditId)).Sum(f => f.Value);

                            if (tipItem == "D" || tipItem == "RD")
                                value = rulajDb;
                            else
                                value = rulajCr;
                        }
                        else
                        {
                            startDate = new DateTime(reportDate.Year, 1, 1);
                            //sold = _balanceRepository.GetSoldTypeAccount(reportDate, _accountRepository.GetAccountBySymbol(account.Symbol).Id, tenantId, currencyId, localCurencyId, false, tipItem);
                            if (tipItem == "D")
                            {
                                value = balanta.Where(f => f.Account.IndexOf(contItem) == 0).Sum(f => f.DbValueF);
                            }
                            else if (tipItem == "C")
                            {
                                value = balanta.Where(f => f.Account.IndexOf(contItem) == 0).Sum(f => f.CrValueF);
                            }
                            else if (tipItem == "RD")
                            {
                                value = contaOperList.Where(f => f.OperationDate >= startDate && f.OperationDate <= reportDate && f.DebitSymbol.IndexOf(contItem) == 0 /*&& f.CurrencyId == localCurencyId*/)
                                                       .Sum(f => f.Valoare);
                            }
                            else if (tipItem == "RC")
                            {
                                value = contaOperList.Where(f => f.OperationDate >= startDate && f.OperationDate <= reportDate && f.CreditSymbol.IndexOf(contItem) == 0 /*&& f.CurrencyId == localCurencyId*/)
                                                       .Sum(f => f.Valoare);
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        throw new Exception("Eroare", ex);
                    }

                }
                else // e din rapoarte
                {
                    var codRaport = tipItem.Substring(0, 1);
                    var coloana = tipItem.Substring(1);
                    decimal codRand = decimal.Parse(contItem);

                    var raport = rapoarteList.FirstOrDefault(f => f.ReportSymbol == codRaport);

                    if (raport == null)
                    {
                        throw new Exception("Raportul cu codul " + codRaport + " nu exista. Revizuiti formula " + formula);
                    }

                    var rand = Context.ReportConfig.FirstOrDefault(f => f.RowCode == codRand && f.ReportId == raport.Id);
                    if (rand == null)
                    {
                        throw new Exception("Randul cu codul " + codRand + " nu exista. Revizuiti formula " + formula);
                    }

                    var calcRow = calcRap.FirstOrDefault(f => f.ReportConfigRowId == rand.Id && f.ReportDate == reportDate);
                    value = calcRow.RowValue;
                    if (!calcRow.Calculat) // gasesc un element necalculat pozitia ramane necalculata
                    {
                        calculat = false;
                        return 0;
                    }
                }

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

    }
}

