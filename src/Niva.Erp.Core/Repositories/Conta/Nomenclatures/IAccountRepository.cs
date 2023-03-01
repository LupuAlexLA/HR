using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Repositories.Conta.Nomenclatures
{
    public interface IAccountRepository : IRepository<Account, int>
    {
        List<Account> AccountList(int currentTenantId,  string searchFilter, bool accountStatus);

        List<Account> InvoiceElementAccountList(string prefix, string accountTypeTC );

        IQueryable<Account> AccountList( );

        IQueryable<Account> AccountListForReports();

        List<Account> GetAllSyntheticAccounts(string PrefixOrName );

        string VerifyAccountDelete(int accountId);
        Account GetAccountById(int? accountId);

        Account GetAccountBySymbol(string accountSymbol );

        Account GetAccountBySymbol(string accountSymbol, List<Account> listAccounts);

        Account GetAccountFromString( string symbol);

        IQueryable<Account> GetAccounts(string synthetic, string includeSynthetic );

        IQueryable<AccountRelation> AccountRelationList();

        List<Account> GetAllAnalythics(int accountId);
        List<Account> GetAllAnalythicsSintetic(int accountId);
        List<Account> GetAllNededAccount(int appClientId);

        List<Account> GetAnalythicsWithoutActivityType();
        List<Account> GetAccountRegInventarExceptEliminare();
    }
}
