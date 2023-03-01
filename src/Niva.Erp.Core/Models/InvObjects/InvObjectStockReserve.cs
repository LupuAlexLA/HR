using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectStockReserve : AuditedEntity<int>, IMustHaveTenant
    {
        public decimal TranzDeprecReserve { get; set; }

        public decimal DeprecReserve { get; set; }

        public decimal TranzReserve { get; set; }

        public decimal Reserve { get; set; }

        public decimal ExpenseReserve { get; set; }

        [ForeignKey("InvObjectStock")]
        public int? InvObjectStockId { get; set; }
        public InvObjectStock InvObjectStock { get; set; }

        [ForeignKey("InvObjectOperDetail")]
        public int? InvObjectOperDetailId { get; set; }
        public InvObjectOperDetail InvObjectOperDetail { get; set; }
        public int TenantId { get; set; }
    }
}