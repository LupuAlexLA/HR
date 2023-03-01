using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class SoldFurnizoriDebitoriModel
    {
        public string AppClientName { get; set; }
        public string AppClientAddress { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }

        public string ThirdPartyName { get; set; }
        public string ThirdPartyAddress { get; set; }
        public string ThirdPartyId1 { get; set; }
        public DateTime OperationDate { get; set; }

        public List<SoldFurnizoriDebitoriDetails> Details { get; set; }
    }

    public class SoldFurnizoriDebitoriDetails
    {
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public decimal Value { get; set; }
        public decimal RestPlata { get; set; }
    }
}
