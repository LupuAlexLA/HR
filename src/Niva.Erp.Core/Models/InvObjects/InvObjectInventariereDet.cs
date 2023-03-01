using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectInventariereDet : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("InvObjectStock")]
        public int? InvObjectStockId { get; set; }
        public InvObjectStock InvObjectStock { get; set; }

        public Decimal StockFaptic { get; set; } // se introduce de catre utilizator

        [ForeignKey("InvObjectItem")]
        public int? InvObjectItemId { get; set; }
        public InvObjectItem InvObjectItem { get; set; }

        [ForeignKey("InvObjectInventariere")]
        public virtual int InvObjectInventariereId { get; set; }
        public virtual InvObjectInventariere InvObjectInventariere { get; set; }

        public int TenantId { get; set; }
    }
}
