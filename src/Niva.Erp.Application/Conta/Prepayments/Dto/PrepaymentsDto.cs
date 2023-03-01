using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public class PrepaymentsListDto
    {
        public int Id { get; set; }
        public PrepaymentType PrepaymentType { get; set; }

        public virtual decimal PrepaymentValue { get; set; }

        public virtual decimal PrepaymentVAT { get; set; }

        [StringLength(1000)]
        public virtual string Description { get; set; }

        public virtual DateTime PaymentDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual int DurationInMonths { get; set; }

        public virtual decimal? MontlyCharge { get; set; }

        [StringLength(1000)]
        public virtual string Observations { get; set; }

        public virtual DateTime DepreciationStartDate { get; set; }

        public int InvoiceDetailsId { get; set; }

        public string ThirdParty { get; set; }

        public string Invoice { get; set; }

        public State State { get; set; }

        public bool Processed { get; set; }

        public bool ProcessedOut { get; set; }

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }


    public class PrepaymentsAddDto
    {
        public DateTime OperationDate { get; set; }
        public PrepaymentType PrepaymentType { get; set; }

        public int? InvoiceId { get; set; }
        public bool InregVAT { get; set; }
        public int DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public bool UnreceiveInvoice { get; set; }

        public IList<PrepaymentsAddInvoiceDetailDto> InvoiceDetail { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool FinishAdd { get; set; }

    }

    public class PrepaymentsAddInvoiceDetailDto
    {
        public int InvoiceId { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public string DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string PrepaymentName { get; set; }

        public int Quantity { get; set; }

        public decimal Value { get; set; }

        public decimal VAT { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }

        public int? PrepaymentAccountId { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int? PrepaymentAccountVATId { get; set; }

        public int? DepreciationAccountVATId { get; set; }

        public int? DurationInMonths { get; set; }

        public DateTime DepreciationStartDate { get; set; }

        public DateTime? EndDateChelt { get; set; }
    }

    public class PrepaymentsAddDirectDto
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual DateTime? EndDateChelt { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentType { get; set; }

        public int? InvoiceId { get; set; }

        public bool FinishAdd { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public DateTime DepreciationStartDate { get; set; }

        public int DurationInMonths { get; set; }

        public decimal? Depreciation { get; set; }

        public decimal? DepreciationVAT { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int? PrepaymentAccountId { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int? PrepaymentAccountVATId { get; set; }

        public int? DepreciationAccountVATId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public bool Processed { get; set; }

        public bool ProcessedOut { get; set; }

        public int? ThirdPartyId { get; set; }

        public string ThirdPartyName { get; set; }

        public PrepaymentType PrepaymentType { get; set; }

        public decimal PrepaymentValue { get; set; }

        public decimal PrepaymentVAT { get; set; }

        public int? RemainingDuration { get; set; }

        public decimal? MontlyDepreciation { get; set; }

        public decimal? MontlyDepreciationVAT { get; set; }

        public bool UnreceiveInvoice { get; set; }

    }

    public class PrepaymentsDDDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime? DocumentDate { get; set; }
    }

    public class PrepaymentsExitDto
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public string PrimDocumentType { get; set; }

        public bool FinishAdd { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public bool ProcessedOut { get; set; }

        public string ThirdPartyName { get; set; }

        public PrepaymentType PrepaymentType { get; set; }

        public decimal PrepaymentValue { get; set; }
    }

}
