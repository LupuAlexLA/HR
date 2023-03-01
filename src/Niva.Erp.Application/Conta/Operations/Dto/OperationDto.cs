using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Operations
{
    public class OperationSearchDto 
    {
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public string Account1 { get; set; }
        public string Account2 { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? OperationTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public decimal? Value { get; set; }
        public int? OperationId { get; set; }
        //public int? OperationStatus { get; set; }
        public string Explication { get; set; }

        public IList<OperationDTO> Operations { get; set; }
    }

    public class OperationDTO
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public bool ClosingMonth { get; set; }
        public IList<OperationDetailsDTO> OperationsDetails { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        public int? OperationTypeId { get; set; }
        public string OperationType { get; set; }

        //public bool OperationStatus { get; set; }

        //public bool OperationStatusDB { get; set; }
        public int? OperationDefinitionId { get; set; }

        // public OperationDefinition OperationDefinition { get; set; }
        public int AppClientId { get; set; }
        public virtual decimal Value { get; set; }
        public virtual decimal ValueCurr { get; set; }
        public bool ShowDetail { get; set; }
        public bool ExternalOperation { get; set; }
    }

    public class OperationDetailsDTO
    {
        public int Id { get; set; }
        public virtual decimal Value { get; set; }
        public virtual decimal ValueCurr { get; set; }
        public virtual decimal VAT { get; set; }
        public virtual string Details { get; set; }
        public int DebitId { get; set; }
        public string Debit { get; set; }
        public string DebitName { get; set; }
        public int CreditId { get; set; }
        public string Credit { get; set; }
        public string CreditName { get; set; }
        public int? DetailNr { get; set; }
        public int? OperGenerateId { get; set; }
    }

    public partial class OperationEditDto
    {
        public virtual int Id { get; set; }

        public virtual DateTime OperationDate { get; set; }

        public virtual string DocumentNumber { get; set; }

        public virtual DateTime DocumentDate { get; set; }

        public virtual bool ClosingMonth { get; set; }

        public int DocumentTypeId { get; set; }

        public int CurrencyId { get; set; }

        public int LocalCurrencyId { get; set; }
        public int? OperationTypeId { get; set; }

        public decimal ExchangeRate { get; set; }

        //public virtual OperationStatus OperationStatus { get; set; }

        public int? OperationDefinitionId { get; set; }

        public int? OperationParentId { get; set; }

        public List<OperationEditDetailsDto> OperationDetails { get; set; }

        public List<OperationLast10DTO> Last10 { get; set; }
    }

    public class OperationEditDetailsDto
    {
        public virtual int Id { get; set; }

        public virtual decimal Value { get; set; }

        public virtual decimal ValueCurr { get; set; }

        public virtual decimal VAT { get; set; }

        public virtual string Details { get; set; }

        public int DebitId { get; set; }

        public string DebitName { get; set; }
        public int CreditId { get; set; }
        public string CreditName { get; set; }
        public int IdOrd { get; set; }
        public int? DetailNr { get; set; }
        public int? OperGenerateId { get; set; }

        public bool ShowInvoiceDetails { get; set; }

        public int? InvoiceElementsDetailsId { get; set; }
        public int? InvoiceElementsDetailsCategoryId { get; set; }  
    }

    public class OperationLast10DTO
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public bool ClosingMonth { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }

        public int? OperationTypeId { get; set; }
        public string OperationType { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        //public bool OperationStatus { get; set; }
        //public bool OperationStatusDB { get; set; }
        public int? OperationDefinitionId { get; set; }
        public virtual decimal Value { get; set; }
        public virtual decimal ValueCurr { get; set; }
        public bool ShowDetail { get; set; }
    }

  public class SoldOperationDto
    {
        public decimal SoldInitial { get; set; }
        public DateTime CurrentDate { get; set; }
        public string SoldInitialType { get; set; }
    }

    public class OperationDefinitionDto
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual State Status { get; set; }
        public int DocumentTypeId { get; set; } 
        public int CurrencyId { get; set; }
        public virtual IList<OperationDefinitionDetailsDto> OperationDetails { get; set; }
        public int TenantId { get; set; }
    }

    public class OperationDefinitionDetailsDto
    {
        public int Id { get; set; } 
        public virtual string Observations { get; set; }
        public int DebitId { get; set; }
        public String DebitName { get; set; }
        public int CreditId { get; set; }
        public String CreditName { get; set; }
        public int TenantId { get; set; }
        public int OperationDefinitionId { get; set; }
    }
}
