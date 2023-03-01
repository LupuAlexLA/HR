
namespace Niva.Erp.Models.Contracts
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class InvoiceDetails : Entity<int>, IAudited
    {
        [StringLength(1000)]
		public virtual string Element { get; set; }

        public virtual int Quantity { get; set; }

        public virtual Decimal Value { get; set; }

        public virtual Decimal VAT { get; set; }

        public virtual Decimal ProcVAT { get; set; }

        [ForeignKey("CotaTVA")]
        public int? CotaTVA_Id { get; set; }
        public virtual CotaTVA CotaTVA { get; set; }

        public virtual int? DurationInMonths { get; set; }

        public virtual State State { get; set; }

        [ForeignKey("InvoiceElementsDetails")]
        public virtual int InvoiceElementsDetailsId { get; set; }
        public virtual InvoiceElementsDetails InvoiceElementsDetails { get; set; }

        [ForeignKey("DebitorAccount")]
        public virtual int? DebitorAccountId { get; set; }
        public virtual Account DebitorAccount { get; set; }

        [ForeignKey("CreditorAccount")]
        public virtual int? CreditorAccountId { get; set; }
        public virtual Account CreditorAccount { get; set; }

        [ForeignKey("Invoices")]
        public virtual int InvoicesId { get; set; }
        public virtual Invoices Invoices { get; set; }

        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }

        public bool UsedInGest { get; set; } // se foloseste la mijloace fixe, daca e true => nu se afiseaza la adaugarea mijloacelor fixe => se folosete la modernizari

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public decimal? ValoareTotalaDetaliu { get; set; }
        public virtual DateTime? DataStartAmortizare { get; set; }   
    }
}

