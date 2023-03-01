using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class RegistruInventarReport
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime ReportDate { get; set; }
        public List<RegistruInventarItem> RegistruInventarList { get; set; }
    }

    public class RegistruInventarItem
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public decimal Value { get; set; }
        public decimal InventoryValue { get; set; }
    }
}
