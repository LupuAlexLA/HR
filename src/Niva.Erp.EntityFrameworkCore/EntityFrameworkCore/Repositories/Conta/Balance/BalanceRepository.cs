using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Conta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class BalanceRepository : ErpRepositoryBase<Balance, int>, IBalanceRepository
    {
        AccountRepository _accountRepository;
        OperationRepository _operationRepository;
        ExchangeRatesRepository _exchangeRatesRepository;

        public BalanceRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _accountRepository = new AccountRepository(context);
            _operationRepository = new OperationRepository(context);
            _exchangeRatesRepository = new ExchangeRatesRepository(context);
        }

        public BalanceView GetBalanceDetails(int balanceId, Boolean addTotals, string searchData, int idCurrency, bool convertToLocalCurrency, bool convertToAllCurrencies, int localCurrencyId, int? nivelRand)
        {
            var balance = GetBalanceById(balanceId);

            var ret = ConvertBalanceToType(balance, searchData, idCurrency, convertToLocalCurrency, convertToAllCurrencies, localCurrencyId);

            if (nivelRand != null)
            {
                ret.Details = ret.Details.Where(f => f.NivelRand <= nivelRand).ToList();
            }
            return ret;
        }

        public Balance GetBalanceById(int id)
        {
            var balance = Context.Balance.Include(f => f.BalanceDetails)
                .ThenInclude(f => f.Account).ThenInclude(f => f.SyntheticAccount).ThenInclude(f => f.AnalyticAccounts)
                .ThenInclude(f => f.Currency)
                                             // .Include(f => f.BalanceDetails.Select(g => g.Account))
                                             .FirstOrDefault(f => f.Id == id);

            return balance;
        }

        public BalanceView ConvertBalanceToType(Balance b, string SearchData, int IdCurrency, bool convertToLocalCurrency, bool convertToAllCurrencies, int localCurrencyId)
        {
            try
            {
                var a = new BalanceView();
                var det = new List<BalanceDetailsView>();

                a.BalanceDate = b.BalanceDate;
                a.StartDate = b.StartDate;
                a.Id = b.Id;

                var list = new List<BalanceDetails>();
                var rows = b.BalanceDetails.Count();
                var accountList = _accountRepository.AccountListForReports().ToArray();

                var tenant = Context.Tenants.FirstOrDefault(f => f.Id == b.TenantId);
                var person = Context.Persons.Where(x => x.Id == tenant.LegalPersonId).FirstOrDefault();
                a.AppClientId1 = person.Id1;
                a.AppClientId2 = person.Id2;
                a.AppClientName = person.FullName;

                //Get only computing account
                //for (int i = 0; i < rows; ++i)
                //{
                //    if (nom.ComputingAccount(b.BalanceDetails[i].Account, accountList))
                //        list.Add(b.BalanceDetails[i]);
                //}

                //if (IdCurrency == 0) // RON si echivalent RON / Valuta si echivalent RON
                //{ 
                //    list = b.BalanceDetails.ToList();
                //}
                //else // RON / Valuta
                //{
                //    list = b.BalanceDetails.Where(x => x.Currency.Id == IdCurrency).ToList();
                //}

                switch (IdCurrency)
                {
                    case 0: // RON si valuta echivalent RON / Valuta echivalent RON
                        list = b.BalanceDetails.ToList();
                        break;
                    //case 2: // Valuta
                    //    list = b.BalanceDetails.Where(x => x.Currency.Id != localCurrencyId).ToList();
                    //    break;
                    default:
                        list = b.BalanceDetails.Where(x => x.Currency.Id == IdCurrency).ToList();
                        a.CurrencyId = IdCurrency;
                        a.CurrencyName = Context.Currency.FirstOrDefault(f => f.Id == IdCurrency).CurrencyName;
                        break;
                }

                //if (convertToLocalCurrency) // RON si echivalent RON / Valuta echivalent RON
                //{
                //    list = b.BalanceDetails.ToList();
                //}
                //else  // RON / Valuta
                //{
                //    list = b.BalanceDetails.Where(x => x.Currency.Id == IdCurrency).ToList();
                //    a.CurrencyId = IdCurrency;
                //    a.CurrencyName = Context.Currency.FirstOrDefault(f => f.Id == IdCurrency).CurrencyName;
                //}


                if (SearchData != "")
                {
                    var text = SearchData.Split(" ");
                    if (text.Length > 1)
                    {
                        var balanceDetailsList = new List<BalanceDetails>();
                        var startAccount = text[0];
                        var endAccount = text[1];

                        //if (startAccount != "" && startAccount != null)
                        //    balanceDetailsList.AddRange(list.Where(f => f.Account.Symbol.IndexOf(startAccount) >= 0).ToList());
                        //if (endAccount != "" && endAccount != null)
                        //{
                        //var vSymbolEnd = endAccount.PadRight(10, '9');
                        //list = list.Where(f => String.Compare(f.Account.Symbol, vSymbolEnd) <= 0).ToList();
                        //     balanceDetailsList.AddRange(list.Where(f => f.Account.Symbol.IndexOf(endAccount) >= 0).ToList());

                        //}
                        balanceDetailsList = list;

                        if (startAccount != "" && startAccount != null)
                            balanceDetailsList = balanceDetailsList.Where(f => f.Account.Symbol.CompareTo(startAccount) >= 0).ToList();
                        if (endAccount != "" && endAccount != null)
                        {
                            var vSymbolEnd = endAccount.PadRight(10, '9');
                            balanceDetailsList = balanceDetailsList.Where(f => f.Account.Symbol.CompareTo(vSymbolEnd) <= 0).ToList();

                        }

                        list = balanceDetailsList;
                    }
                    else
                    {
                        list = list.Where(x => x.Account.Symbol.Contains(SearchData)).ToList();
                    }
                }

                //Group by symbol
                var details = list.Select(g =>
                                 new TempBalanceView
                                 {
                                     DbI = g.DbValueI,
                                     CrI = g.CrValueI,
                                     DbM = g.DbValueM,
                                     CrM = g.CrValueM,
                                     DbY = g.DbValueY,
                                     CrY = g.CrValueY,
                                     DbF = g.DbValueF,
                                     CrF = g.CrValueF,
                                     Symbol = g.Account.Symbol,
                                     CurrencyId = g.CurrencyId
                                 }).ToList();

                if (convertToAllCurrencies)
                {
                    foreach (var currency in details.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
                    {
                        var exchangeRate = _exchangeRatesRepository.GetExchangeRate(b.BalanceDate, currency, localCurrencyId);
                        foreach (var item in details.Where(f => f.CurrencyId == currency))
                        {
                            item.DbI = Math.Round(item.DbI * exchangeRate, 2);
                            item.CrI = Math.Round(item.CrI * exchangeRate, 2);
                            item.DbM = Math.Round(item.DbM * exchangeRate, 2);
                            item.CrM = Math.Round(item.CrM * exchangeRate, 2);
                            item.DbY = Math.Round(item.DbY * exchangeRate, 2);
                            item.CrY = Math.Round(item.CrY * exchangeRate, 2);
                            item.DbF = Math.Round(item.DbF * exchangeRate, 2);
                            item.CrF = Math.Round(item.CrF * exchangeRate, 2);
                        }
                    }
                }

                var count = details.Count();

                var sinteticeList = new List<BalanceDetailsView>();

                var curs = 0;
                for (int i = 0; i < count; ++i)
                {
                    try
                    {
                        var row = new BalanceDetailsView();
                        var account = GetAccountBySymbol(details[i].Symbol, accountList.ToList());

                        if (account is null)
                        {
                            throw new Exception("Contul " + details[i].Symbol + " nu se gaseste in planul de conturi");
                        }

                        row.Symbol = account.Symbol;
                        row.Name = account.AccountName;
                        row.AccountId = account.Id;

                        row.NivelRand = account.NivelRand;

                        row.DbValueI = details[i].DbI;
                        row.CrValueI = details[i].CrI;

                        OrganizeSolds(row, account.AccountTypes, true);

                        row.CrValueM = details[i].CrM;
                        row.CrValueY = details[i].CrY;
                        row.CurrencyId = details[i].CurrencyId;
                        row.DbValueM = details[i].DbM;
                        row.DbValueY = details[i].DbY;

                        OrganizeSolds(row, account.AccountTypes, false);

                        row.DbValueP = details[i].DbY - details[i].DbM; // rulaj precedent = RC - RL
                        row.DbValueT = details[i].DbY + details[i].DbI; // total cumulat = RC + SI
                        row.CrValueP = details[i].CrY - details[i].CrM;
                        row.CrValueT = details[i].CrY + details[i].CrI;

                        OrganizeSolds(row, account.AccountTypes, false);

                        // construiesc sumele totale pentru balanta cu 5 egalitati
                        row.DbValueSum = details[i].DbI + (details[i].DbY - details[i].DbM) + details[i].DbM;
                        row.CrValueSum = details[i].CrI + (details[i].CrY - details[i].CrM) + details[i].CrM;

                        OrganizeSolds(row, account.AccountTypes, false);

                        row.Id = i;
                        row.Synthetic = (account.SyntheticAccount != null) ? account.SyntheticAccount.Symbol : null;
                        row.IsSynthetic = false;// (account.SyntheticAccount != null) ? false : true;
                        row.accountType = account.AccountTypes;

                        curs++;

                        if (Math.Abs(row.CrValueI) + Math.Abs(row.DbValueI) + Math.Abs(row.CrValueM) + Math.Abs(row.DbValueM) + Math.Abs(row.CrValueY) + Math.Abs(row.CrValueY) +
                            Math.Abs(row.CrValueF) + Math.Abs(row.DbValueF) > 0)
                        {
                            det.Add(row);

                            // introduc sinteticele
                            var synthetic = row.Synthetic;
                            while (synthetic != null)
                            {
                                var accountSynthetic = GetAccountBySymbol(synthetic, accountList.ToList());
                                if (accountSynthetic == null)
                                {
                                    throw new Exception("Nu am identificat contul " + synthetic+ ". Verificati daca este cont activ!");
                                }

                                var countSynthetic = sinteticeList.Count(f => f.Symbol == synthetic);
                                if (countSynthetic == 0)
                                {
                                    var rowSynthetic = new BalanceDetailsView();
                                    rowSynthetic.Symbol = accountSynthetic.Symbol;
                                    rowSynthetic.IsSynthetic = true;// (accountSynthetic.SyntheticAccount != null) ? false : true;
                                    rowSynthetic.AccountId = accountSynthetic.Id;
                                    rowSynthetic.Name = accountSynthetic.AccountName;
                                    rowSynthetic.NivelRand = accountSynthetic.NivelRand;
                                    rowSynthetic.DbValueI = row.DbValueI;
                                    rowSynthetic.CrValueI = row.CrValueI;
                                    rowSynthetic.CrValueM = row.CrValueM;
                                    rowSynthetic.CrValueY = row.CrValueY;
                                    rowSynthetic.CurrencyId = row.CurrencyId;
                                    rowSynthetic.DbValueM = row.DbValueM;
                                    rowSynthetic.DbValueY = row.DbValueY;
                                    rowSynthetic.CrValueP = row.CrValueP;
                                    rowSynthetic.DbValueP = row.DbValueP;
                                    rowSynthetic.DbValueT = row.DbValueT;
                                    rowSynthetic.CrValueT = row.CrValueT;
                                    rowSynthetic.DbValueSum = row.DbValueSum;
                                    rowSynthetic.CrValueSum = row.CrValueSum;
                                    rowSynthetic.Synthetic = (accountSynthetic.SyntheticAccount != null) ? accountSynthetic.SyntheticAccount.Symbol : null;
                                    rowSynthetic.accountType = accountSynthetic.AccountTypes;
                                    rowSynthetic.TotalSum = true;
                                    sinteticeList.Add(rowSynthetic);
                                }
                                else
                                {
                                    var rowSynthetic = sinteticeList.FirstOrDefault(f => f.Symbol == synthetic);
                                    rowSynthetic.DbValueI += row.DbValueI;
                                    rowSynthetic.CrValueI += row.CrValueI;
                                    rowSynthetic.CrValueM += row.CrValueM;
                                    rowSynthetic.CrValueY += row.CrValueY;
                                    rowSynthetic.DbValueM += row.DbValueM;
                                    rowSynthetic.DbValueY += row.DbValueY;
                                    rowSynthetic.CrValueP += row.CrValueP;
                                    rowSynthetic.DbValueP += row.DbValueP;
                                    rowSynthetic.DbValueT += row.DbValueT;
                                    rowSynthetic.CrValueT += row.CrValueT;
                                    rowSynthetic.DbValueSum += row.DbValueSum;
                                    rowSynthetic.CrValueSum += row.CrValueSum;
                                }

                                synthetic = (accountSynthetic.SyntheticAccount != null) ? accountSynthetic.SyntheticAccount.Symbol : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                a.TotalCrI = 0;
                a.TotalDbI = 0;
                a.TotalCrM = 0;
                a.TotalDbM = 0;
                a.TotalCrF = 0;
                a.TotalDbF = 0;
                a.TotalCrC = 0;
                a.TotalCrP = 0; a.TotalCrSum = 0; a.TotalCrY = 0; a.TotalDbC = 0; a.TotalDbP = 0; a.TotalDbSum = 0; a.TotalDbY = 0;


                //totaluri
                a.TotalCrI = det.Sum(x => x.CrValueI);
                a.TotalDbI = det.Sum(x => x.DbValueI);
                a.TotalDbM = det.Sum(x => x.DbValueM);
                a.TotalCrM = det.Sum(x => x.CrValueM);
                a.TotalDbF = det.Sum(x => x.DbValueF);
                a.TotalCrF = det.Sum(x => x.CrValueF);
                a.TotalCrC = det.Sum(x => x.CrValueT);
                a.TotalDbC = det.Sum(x => x.DbValueT);
                a.TotalCrP = det.Sum(x => x.CrValueP);
                a.TotalDbP = det.Sum(x => x.DbValueP);
                a.TotalCrSum = det.Sum(x => x.CrValueSum);
                a.TotalDbSum = det.Sum(x => x.DbValueSum);
                a.TotalCrY = det.Sum(x => x.CrValueY);
                a.TotalDbY = det.Sum(x => x.DbValueY);

                // adaug sinteticele in balanta afisata
                foreach (var item in sinteticeList)
                {
                    OrganizeSolds(item, item.accountType, true);
                    OrganizeSolds(item, item.accountType, false);

                    det.Add(item);
                }


                a.Details = det.OrderBy(x => x.Symbol).ToList();
                // a.Type = type;

                //#region Totals
                //if (AddTotals)
                //{
                //    //adding subtotals
                //    if (a.Type == BalanceType.Analythic)
                //    {
                //        var subTotals = a.Details.GroupBy(p => new { p.Synthetic, p.CurrencyId/*, p.accountType*/ }).Select(g =>
                //                 new
                //                 {
                //                     DbI = g.Sum(x => x.DbValueI),
                //                     CrI = g.Sum(x => x.CrValueI),
                //                     DbM = g.Sum(x => x.DbValueM),
                //                     CrM = g.Sum(x => x.CrValueM),
                //                     DbY = g.Sum(x => x.DbValueY),
                //                     CrY = g.Sum(x => x.CrValueY),
                //                     DbF = g.Sum(x => x.DbValueF),
                //                     CrF = g.Sum(x => x.CrValueF),
                //                     Symbol = g.Key.Synthetic,
                //                     CurrencyId = g.Key.CurrencyId,
                //                     Name = g.Key.Synthetic
                //                     //AccountTypes = g.Key.accountType
                //                 });

                //        foreach (var d in subTotals)
                //        {
                //            try
                //            {
                //                var syntheticAccount = accountList.FirstOrDefault(f => f.Symbol.Split(".")[0] == d.Symbol);

                //                a.Details.Add(new BalanceDetailsView
                //                {
                //                    Name = d.Name,
                //                    CrValueF = d.CrF,
                //                    CrValueI = d.CrI,
                //                    CrValueM = d.CrM,
                //                    CrValueY = d.CrY,
                //                    DbValueF = d.DbF,
                //                    DbValueI = d.DbI,
                //                    DbValueM = d.DbM,
                //                    DbValueY = d.DbY,
                //                    Synthetic = d.Symbol,
                //                    Symbol = d.Symbol,
                //                    TotalSum = true,
                //                    accountType = syntheticAccount.AccountTypes
                //                });
                //            }
                //            catch (Exception ex)
                //            {
                //                throw new Exception("Nu exista sinteticul " + d.Symbol + " definit in planul de conturi. " + ex.Message);
                //            }
                //        }

                //        foreach (var d in a.Details.Where(x => x.TotalSum))
                //        {
                //            OrganizeSolds(d, d.accountType, true, type);

                //            OrganizeSolds(d, d.accountType, false, type);
                //        }
                //    }


                //    ///Adding general total
                //    var Total = a.Details.Where(x => x.TotalSum == (a.Type == BalanceType.Analythic ? true : false)).GroupBy(x => x.CurrencyId).Select(g => new
                //    {
                //        DbI = g.Sum(x => x.DbValueI),
                //        CrI = g.Sum(x => x.CrValueI),
                //        DbM = g.Sum(x => x.DbValueM),
                //        CrM = g.Sum(x => x.CrValueM),
                //        DbY = g.Sum(x => x.DbValueY),
                //        CrY = g.Sum(x => x.CrValueY),
                //        DbF = g.Sum(x => x.DbValueF),
                //        CrF = g.Sum(x => x.CrValueF),
                //        Name = "TOTAL GENERAL "
                //    }).ToArray()[0];

                //    a.Details.Add(new BalanceDetailsView
                //    {
                //        Name = Total.Name,
                //        CrValueF = Total.CrF,
                //        CrValueI = Total.CrI,
                //        CrValueM = Total.CrM,
                //        CrValueY = Total.CrY,
                //        DbValueF = Total.DbF,
                //        DbValueI = Total.DbI,
                //        DbValueM = Total.DbM,
                //        DbValueY = Total.DbY,
                //        Synthetic = "99999999999999",
                //        Symbol = Total.Name,
                //        TotalSum = true
                //    });
                //}

                //#endregion

                //a.Details = a.Details.OrderBy(x => x.Synthetic).ThenByDescending(x => x.TotalSum).ThenBy(x => x.Symbol).ToList();

                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Account GetAccountBySymbol(string accountSymbol, List<Account> listAccounts)
        {
            var account = new Account();
            account = listAccounts.Where(p => p.Symbol == accountSymbol).FirstOrDefault();

            return account;
        }

        public void OrganizeSolds(BalanceDetailsView b, AccountTypes typeofAccount, bool InitialSolds)
        {
            var DbValue = InitialSolds ? b.DbValueI : b.DbValueF;
            var CrValue = InitialSolds ? b.CrValueI : b.CrValueF;

            if (InitialSolds)
            {
                DbValue = b.DbValueI - b.CrValueI;
                CrValue = b.CrValueI - b.DbValueI;
            }
            else
            {
                DbValue = b.DbValueI - b.CrValueI + b.DbValueY - b.CrValueY;
                CrValue = b.CrValueI - b.DbValueI + b.CrValueY - b.DbValueY;
            }

            if (typeofAccount != AccountTypes.Bifunctional)
            {
                if (DbValue < 0)
                {
                    CrValue = Math.Abs(DbValue);
                    DbValue = 0;
                }

                if (CrValue < 0)
                {
                    DbValue = Math.Abs(CrValue);
                    CrValue = 0;
                }
            }
            else
            {
                if (DbValue > 0)
                    CrValue = 0;
                else
                {
                    CrValue = Math.Abs(DbValue);
                    DbValue = 0;
                }
            }


            if (InitialSolds)
            {
                b.DbValueI = DbValue;
                b.CrValueI = CrValue;
            }
            else
            {
                b.DbValueF = DbValue;
                b.CrValueF = CrValue;
            }
        }

        public void Compute(DateTime ComputeDate, bool _ClosingMonthOperations, int appClientId)
        {
            var _localCurrencyId = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.CurrencyId).FirstOrDefault();
            var BalanceList = Context.Balance.Where(f => f.Status == State.Active);
            //Check if date is ok 
            if (BalanceList.Where(x => x.BalanceDate == ComputeDate).ToList().Count != 0)
                throw new Exception("Exista o balanta cu aceeasi data!");

            //Date has to be bigger
            if (BalanceList.Where(x => x.BalanceDate >= ComputeDate).ToList().Count != 0)
                throw new Exception("Exista o balanta mai recenta decat data selectata!");


            //Get previous Balance Details
            var Mon = ComputeDate.AddMonths(-1);

            DateTime prevDate;
            try
            {
                prevDate = BalanceList.Where(x => x.BalanceDate.Month == Mon.Month && x.BalanceDate.Year == Mon.Year).SingleOrDefault().BalanceDate;
            }
            catch
            {
                throw new Exception("Nu am reusit sa identific balanta din luna precedenta!");
            }

            // verific daca exista operatii nevalidate intre cele doua date de balanta
            //var countOperNevalid = Context.Operations
            //                              .Count(f => f.State == State.Active && f.OperationStatus == OperationStatus.Unchecked
            //                                     && f.OperationDate <= ComputeDate && f.OperationDate > prevDate);
            //if (countOperNevalid != 0)
            //{
            //    throw new Exception("Exista operatii nevalidate in intervalul " + LazyMethods.DateToString(prevDate) + " - " + LazyMethods.DateToString(ComputeDate));
            //}



            if (_ClosingMonthOperations)
                ClosingMonthOperations(prevDate, ComputeDate, _localCurrencyId);

            var BD = GetBalanceDetails(prevDate);
            var tempBalList = new List<TemporaryBalance>();
            try
            {
                #region Get previous accounting balance and initials
                foreach (BalanceDetails bd in BD)
                {
                    var tempBal = new TemporaryBalance();
                    var YearStart = ComputeDate.Month == 1;
                    #region Initials
                    //Begining of the Year, Finals become Initials
                    //if (bd.Account.AccountTypes == AccountTypes.Active)
                    //{
                    //    tempBal.DbInitial = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);
                    //    tempBal.CrIntial = 0;
                    //}
                    //else if (bd.Account.AccountTypes == AccountTypes.Passive)
                    //{
                    //    tempBal.CrIntial = YearStart ? (bd.CrValueF - bd.DbValueF) : (bd.CrValueI - bd.DbValueI);
                    //    tempBal.DbInitial = 0;
                    //}
                    //else
                    //{
                    var sold = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);

                    if (sold >= 0)
                    {
                        tempBal.DbInitial = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);
                        tempBal.CrIntial = 0;
                    }
                    else
                    {
                        tempBal.CrIntial = YearStart ? (bd.CrValueF - bd.DbValueF) : (bd.CrValueI - bd.DbValueI);
                        tempBal.DbInitial = 0;
                    }
                    //}


                    #endregion

                    tempBal.CrYear = YearStart ? 0 : bd.CrValueY;
                    tempBal.DbYear = YearStart ? 0 : bd.DbValueY;

                    tempBal.CrMonth = tempBal.CrFinal = tempBal.DbFinal = tempBal.DbMonth = 0;

                    tempBal.Currency = bd.Currency;
                    tempBal.CurrencyId = bd.Currency.Id;
                    tempBal.Account = bd.Account;
                    tempBal.AccountId = bd.Account.Id;
                    tempBalList.Add(tempBal);
                }

                #endregion

                var LocalCurrency = Context.Currency.Where(x => x.Id == _localCurrencyId).FirstOrDefault();


                #region OperationsBetweenDates
                //Select operations between dates, with local currency

                //var list = Context.Operations.Where(x => x.OperationDate > prevDate && x.OperationDate <= ComputeDate && x.State == State.Active)
                //    .Include(f => f.OperationsDetails)
                //    .ThenInclude(f => f.Credit)
                //    .Include(f => f.OperationsDetails)
                //    .ThenInclude(f => f.Debit)
                //    .ToList();
                ///nu tin cont de moneda pentru RON . Aici le iau pe toate


                //foreach (var op in list)
                //{
                //    foreach (var opD in op.OperationsDetails)
                //    {
                //        try
                //        {
                //            var crCount = tempBalList.Where(x => x.Account == opD.Credit && LocalCurrency.Id == (x.Currency == null ? LocalCurrency.Id : x.Currency.Id)).Count();
                //            var dbCount = tempBalList.Where(x => x.Account == opD.Debit && LocalCurrency.Id == (x.Currency == null ? LocalCurrency.Id : x.Currency.Id)).Count();

                //            var t = new TemporaryBalance();
                //            t.Currency = LocalCurrency;

                //            if (crCount == 0)
                //            {
                //                t.Account = opD.Credit;
                //                t.DbInitial = t.CrIntial = 0;
                //            }
                //            else
                //            {
                //                t = tempBalList.Where(x => x.Account == opD.Credit && LocalCurrency.Id == (x.Currency == null ? LocalCurrency.Id : x.Currency.Id)).SingleOrDefault();
                //            }

                //            t.CrMonth = t.CrMonth + opD.Value;
                //            t.CrYear = t.CrYear + opD.Value;

                //            if (crCount == 0)
                //                tempBalList.Add(t);

                //            t = new TemporaryBalance();
                //            t.Currency = LocalCurrency;
                //            if (dbCount == 0)
                //            {
                //                t.Account = opD.Debit;
                //                t.DbInitial = t.CrIntial = 0;
                //            }
                //            else
                //            {
                //                t = tempBalList.Where(x => x.Account == opD.Debit && LocalCurrency.Id == (x.Currency == null ? LocalCurrency.Id : x.Currency.Id)).SingleOrDefault();
                //            }

                //            t.DbMonth = t.DbMonth + opD.Value;
                //            t.DbYear = t.DbYear + opD.Value;

                //            if (dbCount == 0)
                //                tempBalList.Add(t);
                //        }
                //        catch (Exception ex)
                //        {
                //            throw new Exception(ex.Message);
                //        }

                //    }
                //}
                #endregion

                #region OperationsBetweenDates_for_Currency
                //Select operations between dates, with local currency
                var list = Context.Operations.Include(f => f.OperationsDetails).ThenInclude(f => f.Debit)
                                             .Include(f => f.OperationsDetails).ThenInclude(f => f.Credit)
                                             .Include(f => f.Currency)
                                             .Where(x => x.OperationDate > prevDate && x.OperationDate <= ComputeDate && x.State == State.Active)
                                             .ToList();
                /// tin cont de moneda sa nu fie RON


                foreach (var op in list)
                {
                    try
                    {

                        foreach (var opD in op.OperationsDetails)
                        {
                            try
                            {
                                var crCount = tempBalList.Where(x => x.AccountId == opD.CreditId && op.Currency.Id == x.CurrencyId).Count();
                                var dbCount = tempBalList.Where(x => x.AccountId == opD.DebitId && op.Currency.Id == x.CurrencyId).Count();
                                var value = opD.Value;

                                var t = new TemporaryBalance();
                                t.Currency = op.Currency;
                                t.CurrencyId = op.CurrencyId;


                                if (crCount == 0)
                                {
                                    t.Account = opD.Credit;
                                    t.AccountId = opD.CreditId;
                                    t.DbInitial = t.CrIntial = 0;
                                }
                                else
                                {
                                    t = tempBalList.Where(x => x.AccountId == opD.CreditId && op.Currency.Id == x.CurrencyId).SingleOrDefault();
                                }

                                t.CrMonth = t.CrMonth + value;
                                t.CrYear = t.CrYear + value;

                                if (crCount == 0)
                                    tempBalList.Add(t);

                                var d = new TemporaryBalance();
                                d.Currency = op.Currency;
                                d.CurrencyId = op.CurrencyId;
                                if (dbCount == 0)
                                {
                                    d.Account = opD.Debit;
                                    d.AccountId = opD.DebitId;
                                    d.DbInitial = d.CrIntial = 0;
                                }
                                else
                                {
                                    d = tempBalList.Where(x => x.AccountId == opD.DebitId && op.CurrencyId == x.CurrencyId).SingleOrDefault();
                                }

                                d.DbMonth = d.DbMonth + value;
                                d.DbYear = d.DbYear + value;

                                if (dbCount == 0)
                                    tempBalList.Add(d);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                #endregion

                var balDetails = new List<BalanceDetails>();

                foreach (var o in tempBalList)
                {

                    o.DbFinal = o.DbInitial - o.CrIntial + o.DbYear - o.CrYear;
                    o.CrFinal = o.CrIntial - o.DbInitial + o.CrYear - o.DbYear;

                    //if (o.Account.AccountTypes != AccountTypes.Bifunctional)
                    //{
                    //    if (o.Account.AccountTypes == AccountTypes.Passive)
                    //    {
                    //        o.DbFinal = 0;
                    //    }
                    //    else
                    //    {
                    //        o.CrFinal = 0;
                    //    }
                    //}
                    //else
                    //{
                    if (o.DbFinal > 0)
                        o.CrFinal = 0;
                    else
                    {
                        o.CrFinal = Math.Abs(o.DbFinal);
                        o.DbFinal = 0;
                    }
                    //}

                    if (o.DbInitial != 0 || o.CrIntial != 0 || o.DbMonth != 0 || o.CrMonth != 0 || o.DbYear != 0 || o.CrYear != 0 || o.DbFinal != 0 || o.CrFinal != 0)
                    {
                        balDetails.Add(new BalanceDetails
                        {
                            AccountId = o.AccountId,
                            Account = o.Account,
                            CrValueF = o.CrFinal,
                            CrValueM = o.CrMonth,
                            CrValueI = o.CrIntial,
                            CrValueY = o.CrYear,
                            Currency = o.Currency,
                            DbValueF = o.DbFinal,
                            DbValueI = o.DbInitial,
                            DbValueM = o.DbMonth,
                            DbValueY = o.DbYear
                        });
                    }
                }

                var balance = new Balance();
                balance.BalanceDate = ComputeDate;
                balance.BalanceDetails = balDetails;
                balance.StartDate = prevDate;
                balance.Status = State.Active;

                Context.Balance.Add(balance);
                Context.SaveChanges();

                var balanceId = balance.Id;
                //var validList = BalanceCompValidList(balanceId, appClientId); TO DO
                var okValid = true;

                //foreach (var item in validList)
                //{
                //    if (!item.Ok)
                //    {
                //        okValid = false;
                //    }
                //}
                balance.OkValid = okValid;
                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void ClosingMonthOperations(DateTime startDate, DateTime endDate, int currencyID)
        {
            var symbol121 = "121";
            var docNumber = ""; // endDate.Month.ToString().PadLeft(2, '0');

            var currency = Context.Currency.Where(x => x.Id == currencyID).FirstOrDefault();

            var Account121 = _accountRepository.GetAccountBySymbol(symbol121);

            //Delete previous month closings
            var OpDel = Context.Operations.Where(x => x.ClosingMonth && x.DocumentDate == endDate && x.State == State.Active).ToList();
            if (OpDel.Count != 0)
            {
                foreach (var d in OpDel)
                {
                    _operationRepository.OperationDelete(d);
                }
            }

            //Select operations between dates, with local currency
            var listCredit = new List<OperationDetails>();
            listCredit = Context.Operations.Where(x => x.OperationDate > startDate && x.OperationDate <= endDate && x.State == State.Active)
                                     .SelectMany(x => x.OperationsDetails)
                                     .Where(x => x.Credit.Symbol.StartsWith("7")).ToList();

            var listDebit = Context.Operations.Where(x => x.OperationDate > startDate && x.OperationDate <= endDate && x.State == State.Active)
                                     .SelectMany(x => x.OperationsDetails)
                                     .Where(x => x.Debit.Symbol.StartsWith("6")).ToList();

            if (listCredit.Count == 0 && listDebit.Count == 0)
                return;

            var OpDetList = new List<OperationDetails>();
            var operation = new Operation();

            operation.Currency = currency;
            var doc = Context.DocumentType.Where(x => x.ClosingMonth).FirstOrDefault();
            if (doc == null)
            {
                throw new Exception("Nu ati specificat tipul documentului folosit la operatiile de inchidere prin contul de profit sau pierdere");
            }
            operation.DocumentType = doc;
            operation.DocumentNumber = docNumber;
            operation.DocumentDate = endDate;
            operation.OperationDate = endDate;
            operation.OperationStatus = OperationStatus.Checked;
            operation.State = State.Active;
            operation.ClosingMonth = true;

            int? detailNr = 1;


            var detailsCredit = listCredit.Where(x => x.Credit.Symbol.StartsWith("7")).GroupBy(p => new { p.Credit.Symbol }).Select(g =>
                              new
                              {
                                  Value = g.Sum(x => x.Value),
                                  ValueCurr = g.Sum(x => x.ValueCurr),
                                  Symbol = g.Key.Symbol
                              }).ToList();

            //var detailsCredit = listCredit.Where(x => x.Credit.SyntheticAccount.StartsWith("7")).ToList();

            var detailsDebit = listDebit.Where(x => x.Debit.Symbol.StartsWith("6")).GroupBy(p => new { p.Debit.Symbol }).Select(g =>
                             new
                             {
                                 Value = g.Sum(x => x.Value),
                                 ValueCurr = g.Sum(x => x.ValueCurr),
                                 Symbol = g.Key.Symbol
                             }).ToList();

            //   var detailsDebit = listDebit.Where(x => x.Debit.SyntheticAccount.StartsWith("6")).ToList();

            foreach (var opD in detailsCredit)
            {

                if (opD.Symbol.StartsWith("7"))
                {
                    var opNew = new OperationDetails();

                    opNew.Debit = _accountRepository.GetAccountBySymbol(opD.Symbol);
                    opNew.Credit = Account121;
                    opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                    opNew.Value = opD.Value;
                    opNew.ValueCurr = opD.ValueCurr;
                    if (doc.AutoNumber)
                    {
                        opNew.DetailNr = detailNr;
                        detailNr++;
                    }
                    OpDetList.Add(opNew);
                }
            }

            foreach (var opD in detailsDebit)
            {

                if (opD.Symbol.StartsWith("6"))
                {
                    var opNew = new OperationDetails();

                    opNew.Debit = Account121;
                    opNew.Credit = _accountRepository.GetAccountBySymbol(opD.Symbol);

                    opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                    opNew.Value = opD.Value;
                    opNew.ValueCurr = opD.ValueCurr;
                    if (doc.AutoNumber)
                    {
                        opNew.DetailNr = detailNr;
                        detailNr++;
                    }
                    OpDetList.Add(opNew);
                }
            }

            operation.OperationsDetails = OpDetList;
            if (OpDetList.Count > 0)
                Context.Operations.Add(operation);
            Context.SaveChanges();
        }

        protected List<BalanceDetails> GetBalanceDetails(DateTime date)
        {
            var b = Context.Balance.Include(f => f.BalanceDetails).ThenInclude(f => f.Account).Include(f => f.BalanceDetails).ThenInclude(f => f.Currency)
                                   .Where(x => x.BalanceDate == date && x.Status == State.Active).SingleOrDefault();

            return b.BalanceDetails.ToList();
        }

        public void DeleteBalance(int id)
        {
            try
            {
                var balance = Context.Balance.Include(f => f.BalanceDetails).FirstOrDefault(f => f.Id == id);
                if (Context.Balance.Where(x => x.BalanceDate > balance.BalanceDate && x.Status == State.Active).Count() > 0)
                    throw new Exception("Nu se poate sterge balanta! Exista o balanta incarcata pe luna urmatoare!");
                //Context.Balance.Remove(balance);

                //Delete previous month closings
                var OpDel = Context.Operations.Where(x => x.ClosingMonth && x.DocumentDate == balance.BalanceDate).ToList();
                if (OpDel.Count != 0)
                {
                    foreach (var d in OpDel)
                    {
                        //_operationRepository.OperationDelete(d, appClientId);
                        d.State = State.Inactive;
                    }
                }

                balance.Status = State.Inactive;
                Context.BalanceDetails.RemoveRange(balance.BalanceDetails);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime BalanceDateNextDay()
        {
            DateTime rez;

            try
            {
                var maxBalanceDate = Context.Balance.Where(f => f.Status == State.Active).Max(f => f.BalanceDate);
                if (maxBalanceDate == null)
                {
                    rez = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                else
                {
                    maxBalanceDate = maxBalanceDate.AddDays(1);
                    rez = new DateTime(maxBalanceDate.Year, maxBalanceDate.Month, maxBalanceDate.Day);
                }
            }
            catch (Exception ex)
            {
                rez = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            return rez;
        }

        public List<BalanceView> GetBalanceCurrency(int balanceId, string startAccount, string endAccount, bool convertToLocalCurrecy, bool convertAllCurrencies, int localCurrencyId, int? nivelRand)
        {
            var ret = new List<BalanceView>();
            var balance = GetBalanceById(balanceId);

            foreach (var item in balance.BalanceDetails.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
            {
                var currency = Context.Currency.FirstOrDefault(x => x.Id == item);

                var balView = ConvertBalanceToType(balance, "", item, convertToLocalCurrecy, convertAllCurrencies, localCurrencyId);

                if (nivelRand != null)
                {
                    balView.Details = balView.Details.Where(f => f.NivelRand <= nivelRand).ToList();
                }
                ret.Add(balView);
            }


            //var balance = balanceViews.Details.GroupBy(f => f.CurrencyId).ToList();
            //foreach (var item in balance)
            //{
            //    var balanceDetView = new List<BalanceDetailsView>();
            //    var balanceView = new List<BalanceView>();
            //    var currency = Context.Currency.FirstOrDefault(x => x.Id == item.Key)?.CurrencyName;
            //    foreach (var detail in item)
            //    {
            //        var det = new BalanceDetailsView
            //        {
            //            accountType = detail.accountType,
            //            CrValueF = detail.CrValueF,
            //            CrValueI = detail.CrValueI,
            //            CrValueM = detail.CrValueM,
            //            CrValueP = detail.CrValueP,
            //            CrValueSum = detail.CrValueSum,
            //            CrValueT = detail.CrValueT,
            //            CrValueY = detail.CrValueY,
            //            DbValueI = detail.DbValueI,
            //            DbValueM = detail.DbValueM,
            //            DbValueP = detail.DbValueP,
            //            DbValueSum = detail.DbValueSum,
            //            DbValueT = detail.DbValueT,
            //            DbValueY = detail.DbValueY,
            //            Name = detail.Name,
            //            Symbol = detail.Symbol,
            //            Analythic = detail.Analythic,
            //            Synthetic = detail.Synthetic,
            //            DbValueF = detail.DbValueF,
            //            TotalSum = detail.TotalSum,
            //            CurrencyId = detail.CurrencyId
            //        };
            //        balanceDetView.Add(det);

            //    }

            //    balanceView.Add(new BalanceView
            //    {
            //        AppClientId1 = balanceViews.AppClientId1,
            //        AppClientId2 = balanceViews.AppClientId2,
            //        AppClientName = balanceViews.AppClientName,
            //        BalanceDate = balanceViews.BalanceDate,
            //        TotalDbI = balanceDetView.Sum(x => x.DbValueI),
            //        TotalCrI = balanceDetView.Sum(x => x.DbValueI),
            //        TotalDbP = balanceDetView.Sum(x => x.DbValueP),
            //        TotalCrP = balanceDetView.Sum(x => x.CrValueP),
            //        TotalDbY = balanceDetView.Sum(x => x.DbValueY),
            //        TotalCrY = balanceDetView.Sum(x => x.CrValueY),
            //        TotalDbM = balanceDetView.Sum(x => x.DbValueM),
            //        TotalCrM = balanceDetView.Sum(x => x.CrValueM),
            //        TotalDbF = balanceDetView.Sum(x => x.DbValueF),
            //        TotalCrF = balanceDetView.Sum(x => x.DbValueF),
            //        TotalDbC = balanceDetView.Sum(x => x.DbValueT),
            //        TotalCrC = balanceDetView.Sum(x => x.CrValueT),
            //        TotalDbSum = balanceDetView.Sum(x => x.DbValueSum),
            //        TotalCrSum = balanceDetView.Sum(x => x.CrValueSum),
            //        Details = balanceDetView
            //    });

            //    var balanceToRet = new BalanceCurrencyView
            //    {
            //        CurrencyId = item.Key,
            //        CurrencyName = currency,
            //        BalanceView = balanceView
            //    };
            //    ret.Add(balanceToRet);
            //}
            return ret;
        }

        public Balance CreateTempBalance(DateTime ComputeDate, bool _ClosingMonthOperations, int appClientId)
        {
            var _localCurrencyId = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.CurrencyId).FirstOrDefault();
            var BalanceList = Context.Balance.Where(f => f.Status == State.Active).ToList();

            //Get previous Balance Details
            //var Mon = ComputeDate.AddMonths(-1);

            DateTime prevDate;
            try
            {
                prevDate = BalanceList.Where(x => x.BalanceDate <= ComputeDate).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            }
            catch
            {
                throw new Exception("Nu am reusit sa identific balanta din luna precedenta!");
            }

            // momentan nu e cazul
            //if (_ClosingMonthOperations)
            //    ClosingMonthOperations(prevDate, ComputeDate, _localCurrencyId);

            var BD = GetBalanceDetails(prevDate);
            var tempBalList = new List<TemporaryBalance>();
            try
            {
                #region Get previous accounting balance and initials
                foreach (BalanceDetails bd in BD)
                {
                    var tempBal = new TemporaryBalance();
                    var YearStart = ComputeDate.Month == 1;
                    #region Initials

                    var sold = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);

                    if (sold >= 0)
                    {
                        tempBal.DbInitial = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);
                        tempBal.CrIntial = 0;
                    }
                    else
                    {
                        tempBal.CrIntial = YearStart ? (bd.CrValueF - bd.DbValueF) : (bd.CrValueI - bd.DbValueI);
                        tempBal.DbInitial = 0;
                    }
                    //}


                    #endregion

                    tempBal.CrYear = YearStart ? 0 : bd.CrValueY;
                    tempBal.DbYear = YearStart ? 0 : bd.DbValueY;

                    tempBal.CrMonth = tempBal.CrFinal = tempBal.DbFinal = tempBal.DbMonth = 0;

                    tempBal.Currency = bd.Currency;
                    tempBal.CurrencyId = bd.Currency.Id;
                    tempBal.Account = bd.Account;
                    tempBal.AccountId = bd.Account.Id;
                    tempBalList.Add(tempBal);
                }

                #endregion

                var LocalCurrency = Context.Currency.Where(x => x.Id == _localCurrencyId).FirstOrDefault();

                #region OperationsBetweenDates_for_Currency
                //Select operations between dates, with local currency
                var list = Context.Operations.Include(f => f.OperationsDetails).ThenInclude(f => f.Debit)
                                             .Include(f => f.OperationsDetails).ThenInclude(f => f.Credit)
                                             .Include(f => f.Currency)
                                             .Where(x => x.OperationDate > prevDate && x.OperationDate <= ComputeDate && x.State == State.Active)
                                             .ToList();
                /// tin cont de moneda sa nu fie RON


                foreach (var op in list)
                {
                    try
                    {

                        foreach (var opD in op.OperationsDetails)
                        {
                            try
                            {
                                var crCount = tempBalList.Where(x => x.AccountId == opD.CreditId && op.Currency.Id == x.CurrencyId).Count();
                                var dbCount = tempBalList.Where(x => x.AccountId == opD.DebitId && op.Currency.Id == x.CurrencyId).Count();
                                var value = opD.Value;

                                var t = new TemporaryBalance();
                                t.Currency = op.Currency;
                                t.CurrencyId = op.CurrencyId;


                                if (crCount == 0)
                                {
                                    t.Account = opD.Credit;
                                    t.AccountId = opD.CreditId;
                                    t.DbInitial = t.CrIntial = 0;
                                }
                                else
                                {
                                    t = tempBalList.Where(x => x.AccountId == opD.CreditId && op.Currency.Id == x.CurrencyId).SingleOrDefault();
                                }

                                t.CrMonth = t.CrMonth + value;
                                t.CrYear = t.CrYear + value;

                                if (crCount == 0)
                                    tempBalList.Add(t);

                                var d = new TemporaryBalance();
                                d.Currency = op.Currency;
                                d.CurrencyId = op.CurrencyId;
                                if (dbCount == 0)
                                {
                                    d.Account = opD.Debit;
                                    d.AccountId = opD.DebitId;
                                    d.DbInitial = d.CrIntial = 0;
                                }
                                else
                                {
                                    d = tempBalList.Where(x => x.AccountId == opD.DebitId && op.CurrencyId == x.CurrencyId).SingleOrDefault();
                                }

                                d.DbMonth = d.DbMonth + value;
                                d.DbYear = d.DbYear + value;

                                if (dbCount == 0)
                                    tempBalList.Add(d);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                #endregion

                var balDetails = new List<BalanceDetails>();

                foreach (var o in tempBalList)
                {

                    o.DbFinal = o.DbInitial - o.CrIntial + o.DbYear - o.CrYear;
                    o.CrFinal = o.CrIntial - o.DbInitial + o.CrYear - o.DbYear;

                    if (o.Account.AccountTypes != AccountTypes.Bifunctional)
                    {
                        if (o.Account.AccountTypes == AccountTypes.Passive)
                        {
                            o.DbFinal = 0;
                        }
                        else
                        {
                            o.CrFinal = 0;
                        }
                    }
                    else
                    {
                        if (o.DbFinal > 0)
                            o.CrFinal = 0;
                        else
                        {
                            o.CrFinal = Math.Abs(o.DbFinal);
                            o.DbFinal = 0;
                        }
                    }

                    if (o.DbInitial != 0 || o.CrIntial != 0 || o.DbMonth != 0 || o.CrMonth != 0 || o.DbYear != 0 || o.CrYear != 0 || o.DbFinal != 0 || o.CrFinal != 0)
                    {
                        balDetails.Add(new BalanceDetails
                        {
                            AccountId = o.AccountId,
                            Account = o.Account,
                            CrValueF = o.CrFinal,
                            CrValueM = o.CrMonth,
                            CrValueI = o.CrIntial,
                            CrValueY = o.CrYear,
                            Currency = o.Currency,
                            CurrencyId = o.CurrencyId,
                            DbValueF = o.DbFinal,
                            DbValueI = o.DbInitial,
                            DbValueM = o.DbMonth,
                            DbValueY = o.DbYear
                        });
                    }
                }

                var balance = new Balance();
                balance.BalanceDate = ComputeDate;
                balance.BalanceDetails = balDetails;
                balance.StartDate = prevDate;
                balance.Status = State.Active;

                return balance;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceDetailsDto> GetBalanceAnyDate(DateTime computeDate, bool _ClosingMonthOperations, int appClientId, int localCurrencyId, bool convertToLocalCurrency)
        {
            var balance = Context.Balance.Include(f => f.BalanceDetails).ThenInclude(f => f.Account).Include(f => f.BalanceDetails).ThenInclude(f => f.Currency)
                                         .FirstOrDefault(f => f.BalanceDate == computeDate && f.Status == State.Active);
            if (balance == null)
            {
                balance = CreateTempBalance(computeDate, _ClosingMonthOperations, appClientId);
            }

            var balanceDetails = balance.BalanceDetails.Select(f => new BalanceDetailsDto
            {
                AccountId = f.AccountId,
                Account = f.Account.Symbol,
                CurrencyId = f.CurrencyId,
                Currency = f.Currency.CurrencyCode,
                DbValueI = f.DbValueI,
                CrValueI = f.CrValueI,
                DbValueM = f.DbValueM,
                CrValueM = f.CrValueM,
                DbValueY = f.DbValueY,
                CrValueY = f.CrValueY,
                DbValueF = f.DbValueF,
                CrValueF = f.CrValueF
            }).ToList();

            if (convertToLocalCurrency)
            {
                foreach (var currency in balanceDetails.Select(f => f.CurrencyId).Distinct().ToList())
                {
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(computeDate, currency, localCurrencyId);
                    foreach (var balanceItem in balanceDetails.Where(f => f.CurrencyId == currency))
                    {
                        balanceItem.DbValueI = Math.Round(balanceItem.DbValueI * exchangeRate, 2);
                        balanceItem.DbValueM = Math.Round(balanceItem.DbValueM * exchangeRate, 2);
                        balanceItem.DbValueY = Math.Round(balanceItem.DbValueY * exchangeRate, 2);
                        balanceItem.DbValueF = Math.Round(balanceItem.DbValueF * exchangeRate, 2);
                        balanceItem.CrValueI = Math.Round(balanceItem.CrValueI * exchangeRate, 2);
                        balanceItem.CrValueM = Math.Round(balanceItem.CrValueM * exchangeRate, 2);
                        balanceItem.CrValueY = Math.Round(balanceItem.CrValueY * exchangeRate, 2);
                        balanceItem.CrValueF = Math.Round(balanceItem.CrValueF * exchangeRate, 2);
                    }
                }
            }

            return balanceDetails;
        }

        public List<ContaOperationDetail> ContaOperationList(DateTime startDate, DateTime endDate, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency)
        {
            var operationsList = new List<ContaOperationDetail>();
            var operationChildList = new List<ContaOperationDetail>();

            operationsList = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit).Include(f => f.Credit)
                                           .Where(f => f.Operation.State == State.Active && startDate <= f.Operation.OperationDate && f.Operation.OperationDate <= endDate
                                                  && f.Operation.TenantId == tenantId)
                                           .Select(f => new ContaOperationDetail
                                           {
                                               OperationId = f.Operation.Id,
                                               OperationDetailId = f.Id,
                                               OperationDate = f.Operation.OperationDate,
                                               DebitId = f.DebitId,
                                               DebitSymbol = f.Debit.Symbol,
                                               CreditId = f.CreditId,
                                               CreditSymbol = f.Credit.Symbol,
                                               Valoare = f.Value,
                                               CurrencyId = f.Operation.CurrencyId,
                                               ParentId = f.Operation.OperationParentId
                                           })
                                           .Distinct()
                                           .ToList();
            if (!convertToLocalCurrency)
            {
                if (currencyId != 0) // daca currencyId = 0, atunci le iau pe toate fara sa mai fac conversii
                {
                    operationsList = operationsList.Where(f => f.CurrencyId == currencyId).ToList();
                }
            }

            if (convertToLocalCurrency)
            {
                var parentIdList = operationsList.Where(f => f.ParentId != null).Select(f => f.ParentId).ToList();

                foreach (var parentId in parentIdList)
                {
                    var operation = operationsList.FirstOrDefault(f => f.OperationId == parentId);
                    if (operation != null)
                    {
                        operationChildList.Add(operation);
                        operationsList.Remove(operation);
                    }
                }

                foreach (var item in operationChildList) //rosu
                {
                    var oper = operationsList.FirstOrDefault(f => f.ParentId == item.OperationId); //verde
                    var creditAccount = Context.Account.FirstOrDefault(f => f.Id == oper.CreditId);
                    if (creditAccount.Symbol.IndexOf("3722") >= 0)
                    {
                        oper.CreditId = item.CreditId;
                        oper.CreditSymbol = item.CreditSymbol;
                    }
                    var debitAccount = Context.Account.FirstOrDefault(f => f.Id == oper.DebitId);
                    if (debitAccount.Symbol.IndexOf("3721") >= 0)
                    {
                        oper.DebitId = item.DebitId;
                        oper.DebitSymbol = item.DebitSymbol;
                    }

                    if (oper.CurrencyId != localCurrencyId)
                    {
                        oper.ValoareValuta = oper.Valoare;
                        oper.Valoare = item.Valoare;
                        oper.CurrencyOrigId = oper.CurrencyId;
                        oper.CurrencyId = localCurrencyId;
                    }
                    else
                    {
                        oper.CurrencyOrigId = item.CurrencyId;
                        oper.ValoareValuta = item.Valoare;
                    }
                }

                // convertesc operatiile in valuta ramase la cursul valutar din data-1
                foreach (var origCurrencyId in operationsList.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
                {
                    foreach (var currencyOper in operationsList.Where(f => f.CurrencyId == origCurrencyId).ToList())
                    {
                        var operDate = currencyOper.OperationDate.AddDays(-1);
                        var exchangeRate = Context.ExchangeRates.Where(f => f.CurrencyId == currencyOper.CurrencyId && f.ExchangeDate <= operDate) // se ia cursul din data - 1
                                                        .OrderByDescending(f => f.ExchangeDate)
                                                        .FirstOrDefault();
                        currencyOper.CurrencyOrigId = currencyOper.CurrencyId;
                        currencyOper.CurrencyId = localCurrencyId;
                        currencyOper.ValoareValuta = currencyOper.Valoare;
                        currencyOper.Valoare = Math.Round(currencyOper.Valoare * exchangeRate.Value, 2);
                    }

                }
            }
            return operationsList;
        }

        public SolduriAccountDto GetSolduriAccount(DateTime data, int accountId, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency)
        {
            var ret = new SolduriAccountDto();
            var balance = Context.Balance.Include(f => f.BalanceDetails).FirstOrDefault(f => f.BalanceDate == data && f.Status == State.Active);
            if (balance == null)
            {
                balance = CreateTempBalance(data, false, tenantId);
            }

            // iau lista tuturor conturilor analitice ale contului account
            var accountList = _accountRepository.GetAllAnalythicsSintetic(accountId);
            var accountIdList = accountList.Select(f => f.Id).ToList();

            var balanceAccountList = balance.BalanceDetails.Where(f => accountIdList.Contains(f.AccountId)).ToList();

            if (!convertToLocalCurrency)
            {
                balanceAccountList = balanceAccountList.Where(f => f.CurrencyId == currencyId).ToList();
            }
            else
            {
                foreach (var currency in balanceAccountList.Select(f => f.CurrencyId).Distinct().ToList())
                {
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(data, currency, localCurrencyId);
                    foreach (var balanceItem in balanceAccountList.Where(f => f.CurrencyId == currency))
                    {
                        balanceItem.DbValueF = Math.Round(balanceItem.DbValueF * exchangeRate, 2);
                        balanceItem.CrValueF = Math.Round(balanceItem.CrValueF * exchangeRate, 2);
                    }
                }
            }

            var debitValue = balanceAccountList.Sum(f => f.DbValueF);
            var creditValue = balanceAccountList.Sum(f => f.CrValueF);

            ret.DebitValue = debitValue;
            ret.CreditValue = creditValue;

            return ret;
        }

        public SoldAccountDto GetSoldTypeAccount(DateTime data, int accountId, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency, string tipCont)
        {
            var ret = new SoldAccountDto();
            var sold = GetSolduriAccount(data, accountId, tenantId, currencyId, localCurrencyId, convertToLocalCurrency);
            var rezDb = sold.DebitValue;
            var rezCr = sold.CreditValue;

            if (tipCont == null || tipCont == "")
            {
                if (rezDb > rezCr)
                {
                    ret.TipSold = "D";
                    ret.Sold = rezDb - rezCr;
                }
                else
                {
                    ret.TipSold = "C";
                    ret.Sold = rezCr - rezDb;
                }
            }
            else
            {
                ret.Sold = (tipCont == "D" ? rezDb : rezCr);
                ret.TipSold = tipCont;
            }
            return ret;
        }
        public decimal GetSoldAccount(DateTime data, Account account, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency)
        {
            var sold = GetSolduriAccount(data, account.Id, tenantId, currencyId, localCurrencyId, convertToLocalCurrency);
            var rezDb = sold.DebitValue;
            var rezCr = sold.CreditValue;

            decimal ret = (account.AccountTypes == AccountTypes.Active || account.AccountTypes == AccountTypes.Bifunctional) ? (rezDb - rezCr) : (rezCr - rezDb);

            return ret;
        }

        public Balance BalantaZilnicaCalc(DateTime ComputeDate, int appClientId)
        {
            var _localCurrencyId = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.CurrencyId).FirstOrDefault();
            var BalanceList = Context.Balance.Where(f => f.Status == State.Active).ToList();

            //Get previous Balance Details
            //var Mon = ComputeDate.AddMonths(-1);

            DateTime prevDate;
            try
            {
                prevDate = BalanceList.Where(x => x.BalanceDate <= ComputeDate).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            }
            catch
            {
                throw new Exception("Nu am reusit sa identific balanta din luna precedenta!");
            }

            var BD = GetBalanceDetails(prevDate);
            var tempBalList = new List<TemporaryBalance>();
            try
            {
                #region Get previous accounting balance and initials
                foreach (BalanceDetails bd in BD)
                {
                    var tempBal = new TemporaryBalance();
                    var YearStart = ComputeDate.Month == 1;
                    #region Initials

                    var sold = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);

                    if (sold >= 0)
                    {
                        tempBal.DbInitial = YearStart ? (bd.DbValueF - bd.CrValueF) : (bd.DbValueI - bd.CrValueI);
                        tempBal.CrIntial = 0;
                    }
                    else
                    {
                        tempBal.CrIntial = YearStart ? (bd.CrValueF - bd.DbValueF) : (bd.CrValueI - bd.DbValueI);
                        tempBal.DbInitial = 0;
                    }
                    //}


                    #endregion

                    tempBal.CrYear = YearStart ? 0 : bd.CrValueY;
                    tempBal.DbYear = YearStart ? 0 : bd.DbValueY;

                    tempBal.CrMonth = tempBal.CrFinal = tempBal.DbFinal = tempBal.DbMonth = 0;

                    tempBal.Currency = bd.Currency;
                    tempBal.CurrencyId = bd.Currency.Id;
                    tempBal.Account = bd.Account;
                    tempBal.AccountId = bd.Account.Id;
                    tempBalList.Add(tempBal);
                }

                #endregion

                var LocalCurrency = Context.Currency.Where(x => x.Id == _localCurrencyId).FirstOrDefault();

                #region OperationsBetweenDates_for_Currency
                //Select operations between dates, with local currency
                var list = Context.OperationsDetails.Include(f => f.Operation).ThenInclude(f => f.Currency)
                                                    .Include(f => f.Debit)
                                                    .Include(f => f.Credit)
                                                    .Where(x => x.Operation.OperationDate > prevDate && x.Operation.OperationDate <= ComputeDate && x.Operation.State == State.Active)
                                                    .Select(f => new TempOperationDetails
                                                    {
                                                        Id = f.Id,
                                                        OperationId = f.Operation.Id,
                                                        CurrencyId = f.Operation.CurrencyId,
                                                        Currency = f.Operation.Currency,
                                                        CreditId = f.CreditId,
                                                        Credit = f.Credit,
                                                        DebitId = f.DebitId,
                                                        Debit = f.Debit,
                                                        Value = f.Value,
                                                        Details = f.Details
                                                    })
                                                    .ToList();

                // operatii temporare
                var startDate = prevDate.AddDays(1);

                var reevalPozitieValutaraTempOper = BalantaZilnicaReevaluarePozitieValutara(ComputeDate, appClientId, _localCurrencyId);
                list.AddRange(reevalPozitieValutaraTempOper);

                list = BalantaZilnicaRepartizareCheltuieli(list, startDate, ComputeDate, appClientId);

                list = BalantaZilnicaVenituriCheltuieli(list, appClientId, _localCurrencyId);

                foreach (var opD in list)
                {
                    try
                    {
                        var crCount = tempBalList.Where(x => x.AccountId == opD.CreditId && opD.Currency.Id == x.CurrencyId).Count();
                        var dbCount = tempBalList.Where(x => x.AccountId == opD.DebitId && opD.Currency.Id == x.CurrencyId).Count();
                        var value = opD.Value;

                        var t = new TemporaryBalance();
                        t.Currency = opD.Currency;
                        t.CurrencyId = opD.CurrencyId;


                        if (crCount == 0)
                        {
                            t.Account = opD.Credit;
                            t.AccountId = opD.CreditId;
                            t.DbInitial = t.CrIntial = 0;
                        }
                        else
                        {
                            t = tempBalList.Where(x => x.AccountId == opD.CreditId && opD.Currency.Id == x.CurrencyId).SingleOrDefault();
                        }

                        t.CrMonth = t.CrMonth + value;
                        t.CrYear = t.CrYear + value;

                        if (crCount == 0)
                            tempBalList.Add(t);

                        var d = new TemporaryBalance();
                        d.Currency = opD.Currency;
                        d.CurrencyId = opD.CurrencyId;
                        if (dbCount == 0)
                        {
                            d.Account = opD.Debit;
                            d.AccountId = opD.DebitId;
                            d.DbInitial = d.CrIntial = 0;
                        }
                        else
                        {
                            d = tempBalList.Where(x => x.AccountId == opD.DebitId && opD.CurrencyId == x.CurrencyId).SingleOrDefault();
                        }

                        d.DbMonth = d.DbMonth + value;
                        d.DbYear = d.DbYear + value;

                        if (dbCount == 0)
                            tempBalList.Add(d);
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                #endregion

                var balDetails = new List<BalanceDetails>();

                foreach (var o in tempBalList)
                {

                    o.DbFinal = o.DbInitial - o.CrIntial + o.DbYear - o.CrYear;
                    o.CrFinal = o.CrIntial - o.DbInitial + o.CrYear - o.DbYear;

                    if (o.Account.AccountTypes != AccountTypes.Bifunctional)
                    {
                        if (o.Account.AccountTypes == AccountTypes.Passive)
                        {
                            o.DbFinal = 0;
                        }
                        else
                        {
                            o.CrFinal = 0;
                        }
                    }
                    else
                    {
                        if (o.DbFinal > 0)
                            o.CrFinal = 0;
                        else
                        {
                            o.CrFinal = Math.Abs(o.DbFinal);
                            o.DbFinal = 0;
                        }
                    }

                    if (o.DbInitial != 0 || o.CrIntial != 0 || o.DbMonth != 0 || o.CrMonth != 0 || o.DbYear != 0 || o.CrYear != 0 || o.DbFinal != 0 || o.CrFinal != 0)
                    {
                        balDetails.Add(new BalanceDetails
                        {
                            AccountId = o.AccountId,
                            Account = o.Account,
                            CrValueF = o.CrFinal,
                            CrValueM = o.CrMonth,
                            CrValueI = o.CrIntial,
                            CrValueY = o.CrYear,
                            Currency = o.Currency,
                            CurrencyId = o.CurrencyId,
                            DbValueF = o.DbFinal,
                            DbValueI = o.DbInitial,
                            DbValueM = o.DbMonth,
                            DbValueY = o.DbYear
                        });
                    }
                }

                var balance = new Balance();
                balance.BalanceDate = ComputeDate;
                balance.BalanceDetails = balDetails;
                balance.StartDate = prevDate;
                balance.Status = State.Active;
                balance.TenantId = appClientId;

                return balance;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<TempOperationDetails> BalantaZilnicaRepartizareCheltuieli(List<TempOperationDetails> OpDetList, DateTime startDate, DateTime dataEnd, int appClientId)
        {
            try
            {
                var ret = new List<TempOperationDetails>();

                // iau veniturile
                var incomeList = OpDetList.Where(f => f.Credit.Symbol.IndexOf("7") == 0)
                                          .GroupBy(f => f.Credit.ActivityTypeId)
                                          .Select(f => new { ActivityType = f.Key, Value = f.Sum(g => g.Value) })
                                          .ToList();
                var totalIncome = incomeList.Sum(f => f.Value);

                //--------------------
                var activityTypesList = incomeList.Where(f => f.ActivityType != null).Select(f => f.ActivityType).Distinct().ToList();
                var nrActivityType = activityTypesList.Count;
                bool amVenituri = (nrActivityType != 0);
                nrActivityType = (nrActivityType == 0) ? 1 : nrActivityType;

                int[] vActivityType = new int[nrActivityType];
                decimal[] vIncomeProc = new decimal[nrActivityType];
                decimal incomeProcSum = 0;
                decimal[] vExpenseValue = new decimal[nrActivityType];
                var mainActivityId = Context.ActivityTypes.FirstOrDefault(f => f.MainActivity && f.Status == State.Active).Id;

                int index = 0;
                // calculez procentul veniturilor pentru activitatile neprincipale
                foreach (var activityType in activityTypesList.Where(f => f != mainActivityId))
                {
                    vActivityType[index] = activityType.Value;
                    decimal incomeProc = incomeList.FirstOrDefault(f => f.ActivityType == activityType.Value).Value / totalIncome;
                    incomeProcSum += incomeProc;
                    vIncomeProc[index] = incomeProc;
                    index++;
                }
                //calculez procentul veniturilor pentru activitatea principala
                vActivityType[index] = mainActivityId;
                vIncomeProc[index] = 1 - incomeProcSum;

                // iau cheltuielile care nu au activitate stabilita
                var expenseList = OpDetList.Where(f => f.Debit.Symbol.IndexOf("6") == 0 && f.Debit.ActivityTypeId == null)
                                           .ToList();
                // parcurg cheltuielile si le impart in functie de procentul veniturilor pe fiecare activitate in parte
                foreach (var operDetail in expenseList)
                {
                    // stornez operatia inregistrata
                    var operDetailStorno = new TempOperationDetails
                    {
                        OperationId = operDetail.OperationId,
                        DebitId = operDetail.DebitId,
                        Debit = operDetail.Debit,
                        CreditId = operDetail.CreditId,
                        Credit = operDetail.Credit,
                        Details = "Storno - " + operDetail.Details,
                        Value = -1 * operDetail.Value,
                        CurrencyId = operDetail.CurrencyId,
                        Currency = operDetail.Currency

                    };
                    ret.Add(operDetailStorno);

                    decimal totalValue = operDetail.Value;
                    decimal remainingValue = totalValue;
                    for (int i = 0; i <= index; i++)
                    {
                        decimal value = 0;
                        if (index != i) // nu e activitatea principala
                        {
                            value = Math.Round(vIncomeProc[i] * totalValue, 2);
                            remainingValue -= value;
                        }
                        else
                        {
                            value = remainingValue;
                        }

                        var activityType = Context.ActivityTypes.FirstOrDefault(f => f.Id == vActivityType[i]);
                        var account = Context.Account.Include(f => f.SyntheticAccount)
                                                     .Where(f => f.TenantId == appClientId && f.AccountStatus && f.SyntheticAccount.Id == operDetail.DebitId && f.ActivityTypeId == vActivityType[i]
                                                            && f.ComputingAccount && f.AccountStatus)
                                                     .FirstOrDefault();
                        int accountId = 0;
                        if (account == null)
                        {
                            var sinteticAccount = Context.Account.FirstOrDefault(f => f.Id == operDetail.DebitId);

                            // construiesc simbolul contului nou
                            var maxAccount = Context.Account.Where(f => f.SyntheticAccountId == sinteticAccount.Id).OrderByDescending(f => f.Symbol).FirstOrDefault();
                            string newSymbol = sinteticAccount.Symbol;
                            if (maxAccount == null)
                            {
                                newSymbol += "." + "1".PadLeft(2, '0');
                            }
                            else
                            {
                                var maxSymbol = maxAccount.Symbol;
                                var substrSymbol = maxSymbol.Substring(newSymbol.Length + 1);
                                if (substrSymbol.IndexOf(".") >= 0)
                                {
                                    substrSymbol = substrSymbol.Substring(0, substrSymbol.IndexOf("."));
                                    var nextItem = int.Parse(substrSymbol);
                                    nextItem++;
                                    newSymbol += "." + nextItem.ToString().PadLeft(2, '0');
                                }
                            }

                            account = new Account
                            {
                                Symbol = newSymbol,
                                AccountName = sinteticAccount.AccountName + " - " + activityType.ActivityName,
                                CurrencyId = sinteticAccount.CurrencyId,
                                AccountFuncType = sinteticAccount.AccountFuncType,
                                AccountStatus = true,
                                AccountTypes = sinteticAccount.AccountTypes,
                                ActivityTypeId = activityType.Id,
                                ComputingAccount = true,
                                Status = State.Active,
                                SyntheticAccountId = sinteticAccount.Id,
                                TaxStatus = sinteticAccount.TaxStatus,
                                TenantId = sinteticAccount.TenantId
                            };
                            Context.Account.Add(account);
                            Context.SaveChanges();
                            accountId = account.Id;
                        }
                        else
                        {
                            accountId = account.Id;
                        }

                        var operDetailActivity = new TempOperationDetails
                        {
                            OperationId = operDetail.OperationId,
                            DebitId = accountId,
                            Debit = account,
                            CreditId = operDetail.CreditId,
                            Credit = operDetail.Credit,
                            Details = activityType.ActivityName + " - " + operDetail.Details,
                            Value = value,
                            CurrencyId = operDetail.CurrencyId,
                            Currency = operDetail.Currency

                        };
                        ret.Add(operDetailActivity);
                    }

                }
                OpDetList.AddRange(ret);
                return OpDetList;

            }
            catch (Exception ex)
            {
                throw new Exception("Eroare repartizare cheltuieli " + ex.ToString());
            }
        }

        public List<TempOperationDetails> BalantaZilnicaReevaluarePozitieValutara(DateTime dataEnd, int appClientId, int localCurrencyId)
        {
            try
            {
                var OpDetList = new List<TempOperationDetails>();
                var localCurrency = Context.Currency.FirstOrDefault(f => f.Id == localCurrencyId);

                // conturi pozitie de schimb
                var accountPozitieList = Context.Account.Include(f => f.AnalyticAccounts).Include(f => f.SyntheticAccount)
                                                        .Where(f => f.TenantId == appClientId && f.ComputingAccount && (f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli || f.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni))
                                                        .OrderBy(f => f.Symbol)
                                                        .ToList();

                foreach (var pozitie in accountPozitieList) // pentru toate conturile de pozitie de schimb
                {
                    var soldPozitie = GetSoldTypeAccount(dataEnd, pozitie.Id, appClientId, pozitie.CurrencyId, localCurrencyId, false, "");
                    //if (soldPozitie.Sold != 0)
                    //{
                    var currency = Context.Currency.FirstOrDefault(f => f.Id == pozitie.CurrencyId);
                    // identific tipul functiei contului pentru contravaloare
                    var contravaloareFuncType = AccountFuncType.ContravaloarePozitieSchimbCheltuieli;
                    if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                    {
                        contravaloareFuncType = AccountFuncType.ContravaloarePozitieSchimbOperatiuni;
                    }
                    // identific contul de contravaloare
                    var contravaloare = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == contravaloareFuncType
                                                                            && f.ActivityTypeId == pozitie.ActivityTypeId && f.AccountName.IndexOf(currency.CurrencyCode) >= 0);
                    // iau soldul contravalorii
                    var soldContravaloare = GetSoldTypeAccount(dataEnd, contravaloare.Id, appClientId, contravaloare.CurrencyId, localCurrencyId, false, "");

                    var exchangeRates = _exchangeRatesRepository.GetExchangeRate(dataEnd, pozitie.CurrencyId, localCurrencyId);
                    // sold pozitie in localcurrency
                    var soldPozitieLocalCurrency = Math.Round(soldPozitie.Sold * exchangeRates, 2);

                    if (soldContravaloare.Sold != soldPozitieLocalCurrency)
                    {

                        var operDetail = new TempOperationDetails
                        {
                            Value = Math.Abs(soldPozitieLocalCurrency - soldContravaloare.Sold),
                            CurrencyId = localCurrencyId,
                            Currency = localCurrency
                        };
                        int debitId = 0, creditId = 0;
                        Account debit = new Account();
                        Account credit = new Account();

                        bool amVenit = true;
                        if (soldPozitie.TipSold == "C")
                        {
                            if (soldPozitieLocalCurrency > soldContravaloare.Sold)
                            {
                                amVenit = true;
                            }
                            else
                            {
                                amVenit = false;
                            }
                        }
                        else
                        {
                            if (soldPozitieLocalCurrency > soldContravaloare.Sold)
                            {
                                amVenit = false;
                            }
                            else
                            {
                                amVenit = true;
                            }
                        }


                        if (amVenit) // am diferenta pozitiva => am venit
                        {
                            // identific tipul functiei contului pentru venit
                            var venitFuncType = AccountFuncType.VenituriDiferenteCursValutarCheltuieli;
                            if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                            {
                                venitFuncType = AccountFuncType.VenituriDiferenteCursValutarOperatiuni;
                            }
                            // identific contul de venit
                            var venitAccount = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == venitFuncType && f.ActivityTypeId == pozitie.ActivityTypeId);

                            debitId = contravaloare.Id;
                            debit = contravaloare;
                            creditId = venitAccount.Id;
                            credit = venitAccount;
                        }
                        else
                        {
                            // identific tipul functiei contului pentru cheltuiala
                            var cheltuialatFuncType = AccountFuncType.CheltuieliDiferenteCursValutarCheltuieli;
                            if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                            {
                                cheltuialatFuncType = AccountFuncType.CheltuieliDiferenteCursValutarOperatiuni;
                            }
                            // identific contul de venit
                            var cheltuialaAccount = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == cheltuialatFuncType && f.ActivityTypeId == pozitie.ActivityTypeId);

                            debitId = cheltuialaAccount.Id;
                            debit = cheltuialaAccount;
                            creditId = contravaloare.Id;
                            credit = contravaloare;
                        }
                        operDetail.DebitId = debitId;
                        operDetail.Debit = debit;
                        operDetail.CreditId = creditId;
                        operDetail.Credit = credit;
                        operDetail.Details = "Reevaluare pozitie valutara";

                        OpDetList.Add(operDetail);
                    }
                    //}
                }

                return OpDetList;
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare reevaluare pozitie valutara " + ex.ToString());
            }
        }

        public List<TempOperationDetails> BalantaZilnicaVenituriCheltuieli(List<TempOperationDetails> OpDetList, int appClientId, int localCurrencyId)
        {
            try
            {
                var localCurrency = Context.Currency.FirstOrDefault(f => f.Id == localCurrencyId);
                // preiau operatiile
                //Select operations between dates, with local currency
                var listCredit = OpDetList.Where(x => x.Credit.Symbol.StartsWith("7")).ToList();

                var listDebit = OpDetList.Where(x => x.Debit.Symbol.StartsWith("6")).ToList();

                if (listCredit.Count == 0 && listDebit.Count == 0)
                    return OpDetList;

                var detailsCredit = listCredit.Where(x => x.Credit.Symbol.StartsWith("7")).GroupBy(p => new { p.Credit.Symbol }).Select(g =>
                                  new
                                  {
                                      Value = g.Sum(x => x.Value),
                                      Symbol = g.Key.Symbol
                                  }).ToList();

                var detailsDebit = listDebit.Where(x => x.Debit.Symbol.StartsWith("6")).GroupBy(p => new { p.Debit.Symbol }).Select(g =>
                                 new
                                 {
                                     Value = g.Sum(x => x.Value),
                                     Symbol = g.Key.Symbol
                                 }).ToList();

                // venituri
                foreach (var opD in detailsCredit)
                {

                    if (opD.Symbol.StartsWith("7"))
                    {
                        if (opD.Value != 0)
                        {
                            var opNew = new TempOperationDetails();

                            opNew.Debit = _accountRepository.GetAccountBySymbol(opD.Symbol);
                            opNew.DebitId = opNew.Debit.Id;
                            opNew.Credit = Context.Account.FirstOrDefault(f => f.ComputingAccount && f.AccountFuncType == AccountFuncType.ProfitSauPierdere
                                                                          && f.ActivityTypeId == opNew.Debit.ActivityTypeId && f.CurrencyId == localCurrencyId);
                            if (opNew.Credit == null)
                            {
                                throw new Exception("Nu am identificat contul de profit sau pierdere pornind de la simbolul " + opNew.Credit.Symbol);
                            }

                            opNew.CreditId = opNew.Credit.Id;
                            opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                            opNew.Value = opD.Value;
                            opNew.Currency = localCurrency;
                            opNew.CurrencyId = localCurrencyId;
                            OpDetList.Add(opNew);
                        }
                    }
                }

                // cheltuieli
                foreach (var opD in detailsDebit)
                {

                    if (opD.Symbol.StartsWith("6"))
                    {
                        if (opD.Value != 0)
                        {
                            var opNew = new TempOperationDetails();

                            opNew.Credit = _accountRepository.GetAccountBySymbol(opD.Symbol);
                            opNew.CreditId = opNew.Credit.Id;
                            opNew.Debit = Context.Account.FirstOrDefault(f => f.ComputingAccount && f.AccountFuncType == AccountFuncType.ProfitSauPierdere
                                                                              && f.ActivityTypeId == opNew.Credit.ActivityTypeId && f.CurrencyId == localCurrencyId);
                            if (opNew.Debit == null)
                            {
                                throw new Exception("Nu am identificat contul de profit sau pierdere pornind de la simbolul " + opNew.Credit.Symbol);
                            }

                            opNew.DebitId = opNew.Debit.Id;
                            opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                            opNew.Value = opD.Value;
                            opNew.Currency = localCurrency;
                            opNew.CurrencyId = localCurrencyId;
                            OpDetList.Add(opNew);
                        }
                    }
                }
                return OpDetList;

            }
            catch (Exception ex)
            {
                throw new Exception("Eroare inchidere venituri si cheltuieli " + ex.ToString());
            }

        }

        public DateTime LastBalanceDay(DateTime date)
        {
            try
            {
                var balance = Context.Balance.Where(f => f.Status == State.Active && f.BalanceDate <= date).OrderByDescending(f => f.BalanceDate).FirstOrDefault();
                if (balance == null)
                {
                    throw new Exception("Nu am identificat o balanta anterioara datei " + date.ToShortDateString());
                }
                return balance.BalanceDate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Balance> GetAllIncludingBalanceDetails()
        {
            var ret = Context.Balance.Include(f => f.BalanceDetails).ThenInclude(g => g.Account)
                                     .Include(f=>f.BalanceDetails).ThenInclude(f=>f.Currency);

            return ret;
        }
    }
}


public class TemporaryBalance
{
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public decimal DbInitial { get; set; }
    public decimal CrIntial { get; set; }

    public decimal DbMonth { get; set; }
    public decimal CrMonth { get; set; }

    public decimal DbYear { get; set; }
    public decimal CrYear { get; set; }

    public decimal DbFinal { get; set; }
    public decimal CrFinal { get; set; }
}

public class TempBalanceView
{
    public string Symbol { get; set; }
    public int CurrencyId { get; set; }
    public decimal DbI { get; set; }
    public decimal CrI { get; set; }

    public decimal DbM { get; set; }
    public decimal CrM { get; set; }

    public decimal DbY { get; set; }
    public decimal CrY { get; set; }

    public decimal DbF { get; set; }
    public decimal CrF { get; set; }

    public decimal DbP { get; set; }
    public decimal CrP { get; set; }
    public decimal DbT { get; set; }
    public decimal CrT { get; set; }
}

public class TempOperationDetails
{
    public int Id { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public virtual decimal Value { get; set; }
    public virtual string Details { get; set; }
    public int DebitId { get; set; }
    public virtual Account Debit { get; set; }
    public int CreditId { get; set; }
    public virtual Account Credit { get; set; }
    public int OperationId { get; set; }
}