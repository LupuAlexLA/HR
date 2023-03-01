 
namespace Niva.Erp.Models.PrePayments

{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using Niva.Erp.Models.Conta.Enums;
    using Niva.Erp.Models.Contracts;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class Prepayment : AuditedEntity<int>, IMustHaveTenant
    {
        public PrepaymentType PrepaymentType { get; set; }

        public virtual decimal PrepaymentValue { get; set; }

        public virtual decimal PrepaymentVAT { get; set; }

        [StringLength(1000)]
        public virtual string Description { get; set; }

        public virtual DateTime PaymentDate { get; set; }

        public virtual int DurationInMonths { get; set; }

        public virtual decimal? MontlyDepreciation { get; set; }

        public virtual decimal? MontlyDepreciationVAT { get; set; }

        [StringLength(1000)]
        public virtual string Observations { get; set; }

        public virtual DateTime DepreciationStartDate { get; set; }

        public virtual DateTime? EndDate { get; set; } // data de iesire din gestiune

        public virtual DateTime? EndDateChelt { get; set; } // data finalizarii calculului amoritizarii

       

        [ForeignKey("InvoiceDetails")]
        public int? InvoiceDetailsId { get; set; }
        public virtual InvoiceDetails InvoiceDetails { get; set; }

        [ForeignKey("PrimDocumentType")]
        public int? PrimDocumentTypeId { get; set; }
        public DocumentType PrimDocumentType { get; set; }

        [StringLength(1000)]
        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        [ForeignKey("PrepaymentAccount")]
        public int? PrepaymentAccountId { get; set; }
        public Account PrepaymentAccount { get; set; }

        [ForeignKey("DepreciationAccount")]
        public int? DepreciationAccountId { get; set; }
        public Account DepreciationAccount { get; set; }

        [ForeignKey("PrepaymentAccountVAT")]
        public int? PrepaymentAccountVATId { get; set; }
        public Account PrepaymentAccountVAT { get; set; }

        [ForeignKey("DepreciationAccountVAT")]
        public int? DepreciationAccountVATId { get; set; }
        public Account DepreciationAccountVAT { get; set; }

        [ForeignKey("ThirdParty")]
        public int? ThirdPartyId { get; set; }
        public Person ThirdParty { get; set; }

        public int? RemainingDuration { get; set; }

        public decimal? Depreciation { get; set; }

        public decimal? DepreciationVAT { get; set; }

        public bool UnreceiveInvoice { get; set; }

        public State State { get; set; }

        public bool Processed { get; set; }

        public bool ProcessedOut { get; set; }
 
        public int TenantId { get; set; }
    }

    public enum PrepaymentType : int
    {
        CheltuieliInAvans,
        VenituriInAvans
    }
}

