using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectOperDetail : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("InvObjectItem")]
        public int InvObjectItemId { get; set; }
        public InvObjectItem InvObjectItem { get; set; }

        public int Quantity { get; set; }

        public Decimal InvValueModif { get; set; }

        [ForeignKey("InvObjectOper")]
        public int InvObjectOperId { get; set; }
        public InvObjectOper InvObjectOper { get; set; }

        [ForeignKey("InvoiceDetail")]
        public int? InvoiceDetailId { get; set; }
        public InvoiceDetails InvoiceDetail { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }
}