using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class SoldContCurentModel
    {
        public string AppClientName { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }

        public string TipPerioada { get; set; }
        public List<SoldDetail> SoldDetails { get; set; }   
    }

    public class SoldDetail
    {
        public int? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal SoldValuta { get; set; }
        public decimal SoldEchivalent { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string IC { get; set; }
        public decimal RulajDb { get; set; }
        public decimal RulajCr { get; set; }
        public decimal ComisionPerceput { get; set; }
        public decimal DobandaIncasata { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
