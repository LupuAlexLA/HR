using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BonTransferModel
    {
        public int DocumentNumber { get; set; }
        public DateTime OperationDate { get; set; }
        public string StorageInName { get; set; }
        public string StorageOutName { get; set; }
        public string PersonStoreInName { get; set; }
        public string PersonStoreOutName { get; set; }
        public int TenantId { get; set; }
        public string Parameters { get; set; }
        public List<BonTransferDetail> BonTransferDetails { get; set; }
    }

    public class BonTransferDetail
    {
        public string Name { get; set; }
        public int InventoryNumber { get; set; }
        public int Quantity { get; set; }
        public decimal InventoryValue { get; set; }
    }
}
