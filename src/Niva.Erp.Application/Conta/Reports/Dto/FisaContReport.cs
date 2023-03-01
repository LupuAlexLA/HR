using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class FisaContModel
    {
        public int TenantId { get; set; }

        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? AccountId { get; set; }

        public string AccountSymbol { get; set; }

        public string AccountName { get; set; }

        public AccountTypes AccountType { get; set; }

        public int? DocumentTypeId { get; set; }

        public int LocalCurrencyId { get; set; }

        public string LocalCurrency { get; set; }

        public int? CurrencyId { get; set; }

        public string Currency { get; set; }

        public string Parameters { get; set; }

        public int? CorespAccountId { get; set; }

        public string CorespAccountName { get; set; }

        public decimal SoldInitial { get; set; }

        public string SoldInitialType { get; set; }

        public decimal SoldPrecedent { get; set; }

        public string SoldPrecedentType { get; set; }

        public decimal TotalPrecDebit { get; set; }


        public decimal TotalPrecCredit { get; set; }


        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal? SoldFinal { get; set; }

        public string SoldFinalType { get; set; }

        public bool ShowDetails { get; set; }

        public List<FisaContModelDetails> OperationsDetail { get; set; }

        public List<FisaContModelDetails> OperationsDetailSelection { get; set; }

        public List<CorespAccount> CorespAccountList { get; set; }
    }

    public class FisaContModelDetails
    {
        public DateTime OperationDate { get; set; }

        public string DocumentTypeShortName { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public int CorespAccountId { get; set; }

        public string CorespAccountSymbol { get; set; }

        public string CorespAccountName { get; set; }

        public decimal DebitValue { get; set; }


        public decimal CreditValue { get; set; }



        public decimal? Sold { get; set; }

        public string ValueType { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public string OperationDetailsObservations { get; set; }
    }

    public enum ValueTypeEnum
    {
        D,
        C
    }

    public class CorespAccount
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
