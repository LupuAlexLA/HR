using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Economic.Dto
{
    public class DispositionListDto
    {
        public int Id { get; set; }
        public int ThirdPartyId { get; set; }
        public string ThirdPartyName { get; set; }
        public int OperationTypeId { get; set; }    
        public string OperationType { get; set; }
        public DateTime OperationDate { get; set; }
        public int DispositionNumber { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? InvoiceId { get; set; }

        public int? DispositionOperStatus { get; set; }
        public Decimal SumOper { get; set; }
        public State State { get; set; }
    }

    public class DispositionEditDto
    {
        public int Id { get; set; }
        public int ThirdPartyId { get; set; }
        public string ThirdPartyName { get; set; }
        public OperationType OperationType { get; set; }
        public int? OperationTypeId { get; set; }
        public int DispositionNumber { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }

        public int? CategoryElementId { get; set; }
        public int? ElementId { get; set; }

        public int? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; }
        public Decimal SumOper { get; set; }

        public int? OperationId { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        public int? NrChitanta { get; set; }
        public string NumePrenume { get; set; }
        public string TipDoc { get; set; }
        public string ActIdentitate { get; set; }

        public List<InvoiceListSelectableDto> InvoiceList { get; set; }
    }

    public class DepositListDto
    {
        public int Id { get; set; }
        public string OperationType { get; set; }
        public int DispositionNumber { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; }

        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }

        public Decimal SumOper { get; set; }
        public State State { get; set; }
        public int? BankAccountId { get; set; }
        public string BankName { get; set; }
        public string BankAcount { get; set; }
    }

    public class DepositEditDto
    {
        public int Id { get; set; }
        public OperationType OperationType { get; set; }
        public DateTime OperationDate { get; set; }
        public int DispositionNumber { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; }

        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }

        public Decimal SumOper { get; set; }
        public State State { get; set; }
        public int BankAccountId { get; set; }
        public int BankId { get; set; }
        public int TenantId { get; set; }

    }

    public class SoldInitialDto
    {
        public int Id { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public Decimal SumOper { get; set; }
        public string CurrencyName { get; set; }
    }

    public class SoldInitialEditDto
    {
        public int Id { get; set; }
        public string OperationType { get; set; }
        public DateTime DispositionDate { get; set; }
        public Decimal Value { get; set; }
        public Decimal SumOper { get; set; }
        public int CurrencyId { get; set; }
        public int TenantId { get; set; }
    }


}
