using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public class PrepaymentsRegistruReport
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime RegDate { get; set; }
        public PrepaymentType PrepaymentType { get; set; }
        public string ReportName { get; set; }
        public int ModCalc { get; set; }
        public List<PrepaymentsRegistruDetails> RegDetails { get; set; }
    }

    public class PrepaymentsRegistruDetails
    {
        public string PrepaymentName { get; set; }
        public decimal PrepaymentValue { get; set; }
        public decimal MonthlyDepreciation { get; set; }
        public decimal Depreciation { get; set; } // amortizare cumulata
        public decimal RemainingPrepaymentValue { get; set; } // val ramasa
        public int Duration { get; set; }
        public int RemainingDuration { get; set; }
        public string SyntheticAccount { get; set; }
        public string AnalyticAccount { get; set; }
        public string AccountName { get; set; }
        public string AccountForGroup { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNr { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime DepreciationStartDate { get; set; }
        public string ThirdParty { get; set; }
    }
}
