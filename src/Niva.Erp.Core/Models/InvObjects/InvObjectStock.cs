using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectStock : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("InvObjectItem")]
        public int InvObjectItemId { get; set; }
        public InvObjectItem InvObjectItem { get; set; }

        public InvObjectOperType OperType { get; set; }

        public DateTime StockDate { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        [ForeignKey("Storage")]
        public int StorageId { get; set; }
        public InvStorage Storage { get; set; }

        public decimal TranzInventoryValue { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal TranzFiscalInventoryValue { get; set; }

        public decimal FiscalInventoryValue { get; set; }

        public decimal TranzDeprec { get; set; }

        public decimal Deprec { get; set; }

        public decimal TranzFiscalDeprec { get; set; }

        public decimal FiscalDeprec { get; set; }

        public bool InConservare { get; set; }

        public decimal MonthlyDepreciation { get; set; }

        public decimal MonthlyFiscalDepreciation { get; set; }

        public IList<InvObjectStockReserve> InvObjectStockReserve { get; set; }

        [ForeignKey("InvObjectItemPF")]
        public int? InvObjectItemPFId { get; set; }
        public InvObjectItem InvObjectItemPF { get; set; }

        [ForeignKey("InvObjectOperDet")]
        public int? InvObjectOperDetId { get; set; }
        public InvObjectOperDetail InvObjectOperDet { get; set; }

        [ForeignKey("InvObjectAccount")]
        public int? InvObjectAccounttId { get; set; }
        public Account InvObjectAccount { get; set; }
        public int TenantId { get; set; }
    }
}
