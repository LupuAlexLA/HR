using Abp.Domain.Repositories;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Repositories.Conta
{
    public interface IBalanceRepository : IRepository<Balance, int>
    {
        BalanceView GetBalanceDetails(int balanceId, Boolean addTotals, string searchData, int idCurrency, bool convertToLocalCurrency, bool convertToAllCurrencies, int localCurrencyId, int? nivelRand);

        Balance GetBalanceById(int id);

        BalanceView ConvertBalanceToType(Balance b, string SearchData, int IdCurrency, bool convertToLocalCurrency, bool convertToAllCurrencies, int localCurrencyId);

        void Compute(DateTime ComputeDate, bool _ClosingMonthOperations, int appClientId);

        void DeleteBalance(int id);

        Account GetAccountBySymbol(string accountSymbol, List<Account> listAccounts);

        void OrganizeSolds(BalanceDetailsView b, AccountTypes typeofAccount, bool InitialSolds);

        DateTime BalanceDateNextDay();
       
        List<BalanceView> GetBalanceCurrency(int balanceId, string startAccount, string endAccount, bool ConvertToLocalCurrecy, bool convertToAllCurrencies, int localCurrencyId, int?nivelRand);

        Balance CreateTempBalance(DateTime ComputeDate, bool _ClosingMonthOperations, int appClientId);
        List<BalanceDetailsDto> GetBalanceAnyDate(DateTime computeDate, bool _ClosingMonthOperations, int appClientId, int localCurrencyId, bool convertToLocalCurrency);

        List<ContaOperationDetail> ContaOperationList(DateTime startDate, DateTime endDate, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency);

        SolduriAccountDto GetSolduriAccount(DateTime data, int accountId, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency);
        SoldAccountDto GetSoldTypeAccount(DateTime data, int accountId, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency, string tipCont);
        decimal GetSoldAccount(DateTime data, Account account, int tenantId, int currencyId, int localCurrencyId, bool convertToLocalCurrency);
        Balance BalantaZilnicaCalc(DateTime ComputeDate, int appClientId);
        DateTime LastBalanceDay(DateTime date);
        IQueryable<Balance> GetAllIncludingBalanceDetails();
    }

    public class SoldAccountDto
    {
        public string TipSold { get; set; }
        public decimal Sold { get; set; }
    }

    public class SolduriAccountDto
    {
        public decimal DebitValue { get; set; }
        public decimal CreditValue { get; set; }
    }

    public class ContaOperationDetail
    {
        public int OperationId { get; set; }
        public int OperationDetailId { get; set; }
        public DateTime OperationDate { get; set; }
        public int DebitId { get; set; }
        public string DebitSymbol { get; set; }
        public int CreditId { get; set; }
        public string CreditSymbol { get; set; }
        public decimal Valoare { get; set; }
        public decimal ValoareValuta { get; set; }
        public int CurrencyId { get; set; }
        public int? ParentId { get; set; }
        public int CurrencyOrigId { get; set; }
    }

    public class BalanceDetailsDto
    {
        public int AccountId { get; set; }
        public virtual string Account { get; set; }
        public int CurrencyId { get; set; }
        public virtual string Currency { get; set; }
        public virtual decimal CrValueI { get; set; }
        public virtual decimal DbValueI { get; set; }
        public virtual decimal CrValueM { get; set; }
        public virtual decimal DbValueM { get; set; }
        public virtual decimal CrValueY { get; set; }
        public virtual decimal DbValueY { get; set; }
        public virtual decimal CrValueF { get; set; }
        public virtual decimal DbValueF { get; set; }
    }
}
