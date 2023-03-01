using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.ImoAsset
{
    public class ImoAssetStockModerniz : AuditedEntity<int>, IMustHaveTenant
    {
        public decimal TranzDeprecModerniz { get; set; }

        public decimal DeprecModerniz { get; set; }

        public decimal TranzModerniz { get; set; }

        public decimal Moderniz { get; set; }

        public decimal ExpenseModerniz { get; set; }

        [ForeignKey("ImoAssetStock")]
        public int? ImoAssetStockId { get; set; }
        public ImoAssetStock ImoAssetStock { get; set; }

        [ForeignKey("ImoAssetOperDetail")]
        public int? ImoAssetOperDetailId { get; set; }
        public ImoAssetOperDetail ImoAssetOperDetail { get; set; }

        public int TenantId { get; set; }
    }
}
