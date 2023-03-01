using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectInventariereInitDto
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<InvObjectInventariereListDto> InvObjectInventariereList { get; set; }
    }

    public class InvObjectInventariereListDto
    {
        public int Id { get; set; }
        public DateTime DataInventariere { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class InvObjectInventariereEditDto
    {
        public int Id { get; set; }
        public DateTime DataInventariere { get; set; }

        public List<InvObjectInventariereDetDto> InvObjectInventariereDetails { get; set; }

        public int TenantId { get; set; }
    }

    public class InvObjectInventariereDetDto
    {
        public int Id { get; set; }

        public int InvObjectInventariereId { get; set; }

        public string Description { get; set; }
        public DateTime OperationDate { get; set; }

        public int InventoryNumber { get; set; }

        public Decimal InventoryValue { get; set; }


        public decimal StockScriptic { get; set; }

        public decimal StockFaptic { get; set; }

        public int InvObjectStockId { get; set; }

        public int InvObjectItemId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }
        public int TenantId { get; set; }
    }
}
