using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class DetaliuSoldReportDto
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime StartDate { get; set; }

        public int? CurrencyId { get; set; }

        public string Currency { get; set; }

        public string AccountName { get; set; } 
        public IList<SoldDetailBase> SoldDetails { get; set; } = new List<SoldDetailBase>();
    }

    public class SoldDetailBase
    {
        public int? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public decimal SoldValuta { get; set; }
        public decimal SoldEchivalent { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal DebitValue { get; set; }
        public decimal CreditValue { get; set; }
    }

}