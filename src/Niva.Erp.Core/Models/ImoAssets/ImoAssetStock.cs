 
namespace Niva.Erp.Models.ImoAsset
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public partial class ImoAssetStock : AuditedEntity<int>, IMustHaveTenant
    {
        
        [ForeignKey("ImoAssetItem")]
        public int ImoAssetItemId { get; set; }
        public ImoAssetItem ImoAssetItem { get; set; }

        public ImoAssetOperType OperType { get; set; }


        public DateTime StockDate { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        [ForeignKey("Storage")]
        public int StorageId { get; set; }
        public ImoAssetStorage Storage { get; set; }

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

        public IList<ImoAssetStockReserve> ImoAssetStockReserve { get; set; }

        public IList<ImoAssetStockModerniz> ImoAssetStockModerniz { get; set; }

        [ForeignKey("ImoAssetItemPF")]
        public int? ImoAssetItemPFId { get; set; }
        public ImoAssetItem ImoAssetItemPF { get; set; }

        [ForeignKey("ImoAssetOperDet")]
        public int? ImoAssetOperDetId { get; set; }
        public ImoAssetOperDetail ImoAssetOperDet { get; set; }

        [ForeignKey("AssetAccount")]
        public int? AssetAccountId { get; set; }
        public Account AssetAccount { get; set; }

        public int TenantId { get; set; }
    }
}

