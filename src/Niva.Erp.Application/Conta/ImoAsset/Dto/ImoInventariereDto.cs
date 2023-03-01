using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.ImoAsset.Dto
{
    public class ImoInventariereInitDto
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<ImoInventariereListDto> ImoInventariereList { get; set; }
    }

    public class ImoInventariereListDto
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public State State { get; set; }
    }

    public class ImoInventariereEditDto
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public List<ImoInventariereDetailDto> ImoInventariereDetails { get; set; }
    }

    public class ImoInventariereDetailDto
    {
        public int Id { get; set; }
        public int ImoInventariereId { get; set; }

        public string Description { get; set; }
        public DateTime? UseStartDate { get; set; }

        public int InventoryNumber { get; set; }

        public Decimal InventoryValue { get; set; }

        public Decimal RemainingValue { get; set; }

        public decimal StockScriptic { get; set; }

        public decimal StockFaptic { get; set; }

        public int ImoAssetStockId { get; set; }

        public int ImoAssetItemId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }
        public int TenantId { get; set; }
    }
}
