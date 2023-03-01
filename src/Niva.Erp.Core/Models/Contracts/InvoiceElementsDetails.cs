 
namespace Niva.Erp.Models.Contracts
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class InvoiceElementsDetails : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public virtual string Description { get; set; }

        public virtual InvoiceElementsType InvoiceElementsType { get; set; }

        [StringLength(1000)]
        public virtual string ThirdPartyAccount { get; set; }

        [StringLength(1000)]
        public virtual string AmortizationAccount { get; set; }

        [StringLength(1000)]
        public virtual string ExpenseAmortizAccount { get; set; }

        [StringLength(1000)]
        public virtual string CorrespondentAccount { get; set; }

        public virtual State State { get; set; }

        [ForeignKey("InvoiceElementsDetailsCategory")]
        public virtual int? InvoiceElementsDetailsCategoryId { get; set; }
        public virtual InvoiceElementsDetailsCategory InvoiceElementsDetailsCategory { get; set; }

        public int TenantId { get; set; }
    }
}

