using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BonConsumModel
    {
        public int DocumentNumber { get; set; }
        public DateTime OperationDate { get; set; }
        public int TenantId { get; set; }
        public string PersonStoreInName { get; set; }
        public string PersonStoreOutName { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }

        public List<BonConsumDetail> BonConsumDetails { get; set; }
    }

    public class BonConsumDetail
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal InventoryValue { get; set; }
    }
}
