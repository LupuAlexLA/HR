using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic
{
    public class PaymentOrderInvoice : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("PaymentOrder")]
        public int PaymentOrderId { get; set; }
        public virtual PaymentOrders PaymentOrder { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
        public virtual Invoices Invoice { get; set; }

        public decimal PayedValue { get; set; }
        public DateTime OperationDate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
