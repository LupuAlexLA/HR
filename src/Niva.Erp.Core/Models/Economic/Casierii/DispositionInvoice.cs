using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic.Casierii
{
    public class DispositionInvoice : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Disposition")]
        public int DispositionId { get; set; }
        public virtual Disposition Disposition { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
        public virtual Invoices Invoice { get; set; }

        public decimal PayedValue { get; set; }
        public DateTime OperationDate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
