using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BalantaReportModel
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime BalanceDate { get; set; }
        public string BalanceType { get; set; }
        public string CurrencyCode { get; set; }
        public bool ShowTotalClasa { get; set; }
        public string SymbolSearch { get; set; }
        public IList<BalantaReportModelDetails> Details { get; set; }
    }

    public class BalantaReportModelDetails
    {
        public string Synthetic { get; set; }
        public string Symbol { get; set; }
        public string SyntheticAccount { get; set; }
        public string AnalyticAccount { get; set; }
        public string AccountName { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public decimal DbValueI { get; set; }
        public decimal CrValueI { get; set; }
        public decimal DbValueM { get; set; }
        public decimal CrValueM { get; set; }
        public decimal DbValueY { get; set; }
        public decimal CrValueY { get; set; }
        public decimal DbValueF { get; set; }
        public decimal CrValueF { get; set; }
        public IList<BalantaReportModelDetailsSubReport> DetailsSubReport { get; set; }
    }

    public class BalantaReportModelDetailsSubReport
    {
        public string Synthetic { get; set; }
        public string Symbol { get; set; }
        public string SyntheticAccount { get; set; }
        public string AnalyticAccount { get; set; }
        public string AccountName { get; set; }
        public decimal DbValueI { get; set; }
        public decimal CrValueI { get; set; }
        public decimal DbValueM { get; set; }
        public decimal CrValueM { get; set; }
        public decimal DbValueY { get; set; }
        public decimal CrValueY { get; set; }
        public decimal DbValueF { get; set; }
        public decimal CrValueF { get; set; }
    }
}
