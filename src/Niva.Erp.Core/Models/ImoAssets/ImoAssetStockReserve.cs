using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.ImoAsset
{
    public class ImoAssetStockReserve : AuditedEntity<int>, IMustHaveTenant
    {
        public decimal TranzDeprecReserve { get; set; }

        public decimal DeprecReserve { get; set; }

        public decimal TranzReserve { get; set; }

        public decimal Reserve { get; set; }

        public decimal ExpenseReserve { get; set; }

        [ForeignKey("ImoAssetStock")]
        public int? ImoAssetStockId { get; set; }
        public ImoAssetStock ImoAssetStock { get; set; }

        [ForeignKey("ImoAssetOperDetail")]
        public int? ImoAssetOperDetailId { get; set; }
        public ImoAssetOperDetail ImoAssetOperDetail { get; set; }
       
        public int TenantId { get; set; }
    }
}
