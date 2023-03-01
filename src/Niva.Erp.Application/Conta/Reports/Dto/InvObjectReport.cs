using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class InvObjectReportModel
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime InvObjectDate { get; set; }
        public List<InvObjectReportDetail> InvObjectDetails { get; set; }
    }

    public class InvObjectReportDetail
    {
        public string InvObjectName { get; set; }
        public int InventoryNr { get; set; }    
        public DateTime DocumentDate { get; set; }
        public DateTime? InUseDate { get; set; }
        public string ThirdParty { get; set; }
        public int? StorageId { get; set; }
        public string Storage { get; set; }
        public decimal Value { get; set; }
        public string InUse { get; set; }
        public string InvObjectAccount { get; set; }
        public string AccountForGroup { get; set; }
        public string ExpenseAccount { get; set; }  

    }
}
