using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.ImoAsset;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.ImoAssets
{
    public class ImoInventariereDet : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("ImoAssetStock")]
        public int? ImoAssetStockId { get; set; }
        public ImoAssetStock ImoAssetStock { get; set; }        
        public Decimal StockFaptic { get; set; } // se introduce de catre utilizator
        [ForeignKey("ImoAssetItem")]
        public int? ImoAssetItemId { get; set; }
        public ImoAssetItem ImoAssetItem { get; set; }

        [ForeignKey("ImoInventariere")]
        public virtual int ImoInventariereId { get; set; }
        public virtual ImoInventariere ImoInventariere { get; set; }
        public int TenantId { get; set; }
    }
}
