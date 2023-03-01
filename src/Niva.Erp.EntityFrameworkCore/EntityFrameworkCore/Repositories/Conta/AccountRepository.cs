using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.DbFunctions;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Niva.EntityFramework.Repositories.Nomenclatures
{
    public class AccountRepository : ErpRepositoryBase<Account, int>, IAccountRepository
    {
        string[] chartStart = { "1", "2", "3", "4" };

        public AccountRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public List<Account> AccountList(int currentTenantId, string searchFilter, bool accountStatus)
        {
            //string searchSynthetic = "", searchAnalythic = "";
            if (searchFilter == null) searchFilter = "";
            //try
            //{
            //    string[] search = searchFilter.Split('.');
            //    try
            //    {
            //        searchSynthetic = search[0];
            //    }
            //    catch
            //    {

            //    }
            //    try
            //    {
            //        searchAnalythic = search[1];
            //    }
            //    catch
            //    {

            //    }
            //}
            //catch
            //{

            //}

            var accounts = Context.Account.Include(f => f.Currency)
                                          .Include(f => f.ThirdParty)
                                          .Include(f => f.ActivityType)
                                          .Include(f => f.SyntheticAccount)
                                          .Include(f => f.AnalyticAccounts)
                                          .Include(f => f.BNR_Sector)
                                          .Where(f => f.Status == State.Active && f.TenantId == currentTenantId &&
                                                ((f.Symbol.StartsWith(searchFilter)) ||
                                                  f.AccountName.ToUpper().IndexOf(searchFilter.ToUpper()) >= 0))
                                  .OrderBy(xa => xa.Symbol);
            //.ThenBy(xa => xa.AnalyticAccount == null ? "" : MyDbFunctions.Right("000000000000000000000000000000" + xa.AnalyticAccount, 30));

            if (accountStatus == true)
                accounts = accounts.Where(f => f.AccountStatus == accountStatus).OrderBy(xa => xa.Symbol);
            var ret = accounts.ToList();
            return ret;
        }

        public IQueryable<Account> AccountList()
        {
            var x = Context.Account
                .Include(f => f.ThirdParty)
                .Include(f => f.Currency)
                .Include(f => f.SyntheticAccount)
                .Include(f => f.AnalyticAccounts)
                .Where(f => f.Status == State.Active && f.AccountStatus)
                .OrderBy(xa => xa.Symbol);
            //  ThenBy(xa => xa.AnalyticAccount == null ? "" : MyDbFunctions.Right("000000000000000000000000000000" + xa.AnalyticAccount, 30));
            return x;
        }

        public IQueryable<Account> AccountListForReports()
        {
            var x = Context.Account
                .Include(f => f.ThirdParty)
                .Include(f => f.Currency)
                .Include(f => f.SyntheticAccount)
                .Include(f => f.AnalyticAccounts)
                .Where(f => f.Status == State.Active)
                .OrderBy(xa => xa.Symbol);
            //  ThenBy(xa => xa.AnalyticAccount == null ? "" : MyDbFunctions.Right("000000000000000000000000000000" + xa.AnalyticAccount, 30));
            return x;
        }

        public string VerifyAccountDelete(int accountId)
        {
            // throw new NotImplementedException("Decomenteaza codul din metoda"); // TODO
            var ret = "OK";
            int count = 0;
            count = Context.ImoAssetItem.Count(f => f.State == State.Active && (f.AssetAccountId == accountId || f.DepreciationAccountId == accountId || f.ExpenseAccountId == accountId));
            if (count != 0)
            {
                ret = "Nu puteti sterge acest cont deoarece este folosit la definirea mijloacelor fixe";
            }
            count = Context.ImoAssetClassCode.Count(f => f.State == State.Active && (f.AssetAccountId == accountId || f.DepreciationAccountId == accountId || f.ExpenseAccountId == accountId));
            if (count != 0)
            {
                ret = "Nu puteti sterge acest cont deoarece este folosit la definirea codurilor de clasificare pentru mijloacele fixe";
            }
            //count = Context.InvStorage.Count(f => f.State == State.Active && (f.ExtraAccountId == accountId || f.InvAccountId == accountId || f.ExpenseAccountId == accountId));
            //if (count != 0)
            //{
            //    ret = "Nu puteti sterge acest cont deoarece este folosit la definirea gestiunilor pentru obiectele de inventar";
            //}
            count = Context.OperationsDetails.Include(f => f.Operation).Count(f => f.Operation.State == State.Active && (f.CreditId == accountId || f.DebitId == accountId));
            if (count != 0)
            {
                ret = "Nu puteti sterge acest cont deoarece a fost folosit la inregistrarea operatiilor contabile";
            }
            count = Context.BalanceDetails.Include(f => f.Balance).Count(f => f.Balance.Status == State.Active && f.AccountId == accountId);
            if (count != 0)
            {
                ret = "Nu puteti sterge acest cont deoarece face parte dintr-o balanta calculata";
            }
            count = Context.SavedBalanceDetails.Include(f => f.SavedBalance).Count(f => f.AccountId == accountId);
            if (count != 0)
            {
                ret = "Nu puteti sterge acest cont deoarece face parte dintr-o balanta salvata";
            }

            return ret;
        }

        public IQueryable<Account> AccountListV()
        {
            var x = Context.Account
                .Include(f => f.ThirdParty)
                .Include(f => f.Currency)
                .Where(f => f.Status == State.Active && f.AccountStatus)
                .OrderBy(xa => xa.Symbol);
            return x;
        }

        public IQueryable<Account> AccountListAll()
        {
            var x = Context.Account
                .Include(f => f.ThirdParty)
                .Include(f => f.Currency)
                .Where(f => f.Status == State.Active && f.AccountStatus)
                .OrderBy(xa => xa.Symbol);
            return x;
        }

        //public List<Account> GetAllAccounts(string PrefixOrName, int selectedAppClient)
        //{

        //    if (String.IsNullOrEmpty(PrefixOrName))
        //        return AccountListV(selectedAppClient).ToList();
        //    else
        //    {
        //        var list = new List<Account>();
        //        int n;
        //        bool isNumeric = int.TryParse(PrefixOrName.Replace(".", ""), out n);

        //        var accounts = AccountListV(selectedAppClient).AsEnumerable();
        //        if (isNumeric)
        //            accounts = accounts.Where(x => x.SyntheticAccount.StartsWith(PrefixOrName.Split('.')[0]));
        //        //var count = accounts.Count();

        //        foreach (var account in accounts)
        //        {
        //            var symbol = account.Symbol;

        //            if (symbol.IndexOf(PrefixOrName) == 0)
        //                list.Add(account);
        //            else
        //                if (account.AccountName.ToUpper().IndexOf(PrefixOrName.ToUpper()) >= 0)
        //                list.Add(account);
        //        }

        //        return list.OrderBy(x => decimal.Parse(x.SyntheticAccount)).ThenBy(x => x.AnalyticAccount ?? "0000000").ToList();
        //    }
        //}

        public List<Account> GetAllAccounts(string PrefixOrName)
        {

            if (String.IsNullOrEmpty(PrefixOrName))
                return AccountListAll().ToList();
            else
            {
                var list = new List<Account>();
                int n;
                bool isNumeric = int.TryParse(PrefixOrName.Replace(".", ""), out n);

                var accounts = AccountListAll().AsEnumerable();
                if (isNumeric)
                    accounts = accounts.Where(x => x.Symbol.StartsWith(PrefixOrName));
                //var count = accounts.Count();

                foreach (var account in accounts)
                {
                    var symbol = account.Symbol;

                    if (symbol.IndexOf(PrefixOrName) == 0)
                        list.Add(account);
                    else
                        if (account.AccountName.ToUpper().IndexOf(PrefixOrName.ToUpper()) >= 0)
                        list.Add(account);
                }

                return list.OrderBy(x => x.Symbol).ToList();
            }
        }

        public List<Account> GetAllSyntheticAccounts(string PrefixOrName)
        {
            if (PrefixOrName.Length > 2)
            {
                var list = GetAllAccounts(PrefixOrName).Where(f => (f.AnalyticAccounts.Count == 0 && f.SyntheticAccountId == null) || f.AnalyticAccounts.Count != 0).Take(50);
                return list.ToList();
            }
            else
                return null;
        }


        public List<Account> InvoiceElementAccountList(string prefix, string accountTypeTC)
        {
            throw new NotImplementedException("Decomenteaza codul din metoda");
            //    InvoiceElementAccountType accountType = (accountTypeTC == "T"
            //                                            ? InvoiceElementAccountType.ContTert
            //                                            : (accountTypeTC == "C" ? InvoiceElementAccountType.ContCorespondent : InvoiceElementAccountType.ContCheltuialaAmortizare));
            //    var elementAccounts = Context.InvoiceElementAccounts
            //                             .Include(f => f.Account)
            //                             .Where(f => f.InvoiceElementAccountType == accountType)
            //                             .ToList().Select(f => f.Account.SyntheticAccount);


            //    var accounts = GetAllAccounts(prefix );
            //    var accounts1 = accounts.Where(f => elementAccounts.Contains(f.SyntheticAccount));


            //    var list = new List<Account>();
            //    if (accountType == InvoiceElementAccountType.ContTert) // am nevoie de conturi sintetice
            //    {
            //        list = accounts1.Where(f => f.AnalyticAccount == null).ToList();
            //    }
            //    else
            //    {
            //        //var allAccounts = AccountListV.ToArray();
            //        //list = accounts1.Where(f => ComputingAccount(f, allAccounts)).ToList();
            //        list = accounts1.Where(f => f.ComputingAccount).ToList();
            //    }

            //    return list;
        }

        public Account GetAccountBySymbol(string accountSymbol)
        {
            var accountList = AccountList();

            var account = GetAccountBySymbol(accountSymbol, accountList.ToList());

            return account;
        }

        public Account GetAccountBySymbol(string accountSymbol, List<Account> listAccounts)
        {
            //throw new NotImplementedException("Decomenteaza codul din metoda");
            var synthetic = LazyMethods.GetSynthetic(accountSymbol);
            var analythic = LazyMethods.GetAnalythic(accountSymbol);

            var account = new Account();
            account = listAccounts.Where(p => p.Symbol == accountSymbol).FirstOrDefault();

            return account;
        }

        public Account GetAccountById(int? accountId)
        {
            Account account = new Account();
            account = Context.Account.FirstOrDefault(p => p.Id == accountId);
            return account;
        }

        public Account GetAccountFromString(string symbol)
        {
            var x = Context.Account.FirstOrDefault(f => f.Symbol == symbol && f.Status == State.Active && f.AccountStatus);
            return x;
        }

        public IQueryable<Account> GetAccounts(string synthetic, string includeSynthetic)
        {
            var x = Context.Account.Include(f => f.SyntheticAccount)
                                   .Where(f => f.Status == State.Active && f.AccountStatus && (f.SyntheticAccount.Symbol == synthetic || f.Symbol == synthetic))
                    .OrderBy(f => f.Symbol);
            if (includeSynthetic == "N")
            {
                x = x.Where(f => f.Symbol != synthetic).OrderBy(f => f.Symbol);
            }

            return x;
        }


        public bool VerifyAccount(Account account)
        {
            var _account = GetAccountById(account.Id);

            var computing = _account.ComputingAccount;// ComputingAccount(_account, appClientId);
            if (computing)
                return true;
            else
                return false;
        }

        public bool VerifyAccount(Account account, Account[] list)
        {
            var _account = GetAccountById(account.Id);

            var computing = _account.ComputingAccount; //ComputingAccount(_account, list);
            if (computing)
                return true;
            else
                return false;
        }

        public IQueryable<AccountRelation> AccountRelationList()
        {
            throw new NotImplementedException("Decomenteaza codul din metoda");
            //var x = Context.AccountRelation

            //    .OrderBy(f => f.DebitRoot).ThenBy(f => f.CreditRoot);
            //return x;
        }

        public bool VerifyAccountsRelation(int idDebit, int idCredit)
        {
            var accountRel = AccountRelationList();
            return VerifyAccountsRelation(idDebit, idCredit, accountRel.ToList());
        }

        public bool VerifyAccountsRelation(int idDebit, int idCredit, List<AccountRelation> listAccounts)
        {
            var debit = GetAccountById(idDebit);
            var credit = GetAccountById(idCredit);

            var count = listAccounts.Where(x => debit.Symbol.IndexOf(x.DebitRoot) == 0 && credit.Symbol.IndexOf(x.CreditRoot) == 0).Count();

            if (count != 0)
                return true;
            else
                return false;
        }

        public List<Account> GetAllAnalythics_old(int accountId)
        {
            var account = Context.Account.Include(f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == accountId);
            var ret = new List<Account>();

            if (account.SyntheticAccountId != null)
            {
                ret.Add(account);
            }
            else
            {
                var accountList = Context.Account
                                     .Where(f => f.TenantId == account.TenantId && f.SyntheticAccountId == account.Id)
                                     .ToList()
                                     .OrderBy(f => f.Symbol);
                foreach (var item in accountList)
                {
                    ret.Add(item);
                }
            }
            return ret;
        }

        public List<Account> GetAllAnalythics(int accountId)
        {
            var account = Context.Account.Include(f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == accountId);
            var ret = new List<Account>();

            if (account.AnalyticAccounts.Count != 0)
            {
                foreach (var analytic in account.AnalyticAccounts)
                {
                    var listAnalitc = GetAllAnalythics(analytic.Id);
                    ret.AddRange(listAnalitc);
                }
            }
            else
            {
                ret.Add(account);
            }
            return ret;
        }

        public List<Account> GetAllAnalythicsSintetic(int accountId)
        {
            var account = Context.Account.Include(f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == accountId);
            var ret = new List<Account>();

            if (account.AnalyticAccounts.Count != 0)
            {
                foreach (var analytic in account.AnalyticAccounts)
                {
                    var listAnalitc = GetAllAnalythicsSintetic(analytic.Id);
                    ret.AddRange(listAnalitc);
                }
                ret.Add(account); // il adaug si pe el in cazul in care am solduri sau operatii inregistrate in sintetic
            }
            else
            {
                ret.Add(account);
            }
            return ret;
        }

        public List<Account> GetAllNededAccount(int appClientId)
        {
            var ret = new List<Account>();

            var taxNededList = Context.AccountTaxProperties
                .Where(f => f.TenantId == appClientId)
                .GroupBy(f => f.AccountId)
                .Select(f => f.Max(x => x.Id))
                .ToList();

            var accountList = Context.AccountTaxProperties
                .Where(f => taxNededList.Contains(f.Id) && f.PropertyType == AccountTaxPropertyType.Nedeductibil)
               .Select(f => f.AccountId);

            ret = Context.Account.Where(f => f.TenantId == appClientId && accountList.Contains(f.Id) && f.ComputingAccount)
                .ToList()
                .OrderBy(f => f.Symbol)
                .ToList();

            return ret;
        }

        // Lista conturilor analitice de ultim nivel fara fond
        public List<Account> GetAnalythicsWithoutActivityType()
        {
            var localCurrencyId = 1;
            var accounts = Context.Account.Include(f => f.AnalyticAccounts)
                                         .Include(f => f.ActivityType)
                                         .Where(f => f.Status == State.Active && /*f.ActivityType == null &&*/ f.AnalyticAccounts.Count() == 0)
                                         .ToList()
                                         .Where(f => f.Symbol.StartsWith('1') || f.Symbol.StartsWith('2') || f.Symbol.StartsWith('3') || f.Symbol.StartsWith('4') || f.Symbol.StartsWith('5') || f.Symbol.StartsWith('9'))
                                         .OrderBy(f => f.Symbol)
                                         .ToList();

            var ret = new List<Account>();
            // grupez conturile in functie de sintetic
            var grouppedAccounts = accounts.GroupBy(f => new { f.SyntheticAccountId }).Select(f => new { f.Key.SyntheticAccountId }).ToList();

            foreach (var account in grouppedAccounts)
            {
                var _list = accounts.Where(f => f.SyntheticAccountId == account.SyntheticAccountId).Where(f => f.CurrencyId != localCurrencyId).Count();
                if (_list > 0) // daca gaseste conturi in valuta, adauga in lista sinteticul 
                {
                    var syntheticAccount = Context.Account.FirstOrDefault(f => f.Id == account.SyntheticAccountId);
                    var count = Context.RegInventarExceptiiEliminare.Count(f => f.AccountId == syntheticAccount.Id);
                    if (count != 0) // contul are definita exceptie => raman analiticele
                    {
                        ret.AddRange(accounts.Where(f => f.SyntheticAccountId == account.SyntheticAccountId).ToList());
                    }
                    else
                    {
                        ret.Add(syntheticAccount);
                    }
                }
                else
                {
                    ret.AddRange(accounts.Where(f => f.SyntheticAccountId == account.SyntheticAccountId).ToList());
                }
            }

            return ret;
        }

        // lista conturi exceptate de la eliminare pentru Registu inventar
        public List<Account> GetAccountRegInventarExceptEliminare()
        {
            var localCurrencyId = 1;
            var accounts = Context.Account.Include(f => f.AnalyticAccounts)
                                         .Include(f => f.ActivityType)
                                         .Where(f => f.Status == State.Active && f.ActivityType == null && f.AnalyticAccounts.Count() == 0)
                                         .ToList()
                                         .Where(f => f.Symbol.StartsWith('1') || f.Symbol.StartsWith('2') || f.Symbol.StartsWith('3') || f.Symbol.StartsWith('4'))
                                         .OrderBy(f => f.Symbol)
                                         .ToList();

            var ret = new List<Account>();
            // grupez conturile in functie de sintetic
            var grouppedAccounts = accounts.Where(f => f.SyntheticAccountId != null).GroupBy(f => new { f.SyntheticAccountId }).Select(f => new { f.Key.SyntheticAccountId }).ToList();

            foreach (var account in grouppedAccounts)
            {
                var syntheticAccount = Context.Account.FirstOrDefault(f => f.Id == account.SyntheticAccountId);
                ret.Add(syntheticAccount);
            }

            return ret;
        }
    }
}
