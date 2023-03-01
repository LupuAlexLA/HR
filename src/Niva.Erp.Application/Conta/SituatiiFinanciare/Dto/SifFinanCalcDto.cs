using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.SituatiiFinanciare
{
    public class SitFinanCalcForm
    {
        public bool IsDailyBalance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SitFinanBalanceList> BalanceList { get; set; }
    }

    public class SitFinanBalanceList
    {
        public int Id { get; set; }

        public string BalanceName { get; set; }

        public DateTime SaveDate { get; set; }

        public bool CalcSitFinan { get; set; }
    }

    public class SitFinanCalcReportForm
    {
        public int BalanceId { get; set; }

        public DateTime SaveDate { get; set; }

        public string BalanceName { get; set; }

        public List<SitFinanCalcReportList> ReportList { get; set; }

        public int SelReportId { get; set; }

        public string SelReportName { get; set; }

        public int SelReportNrCol { get; set; }

        public List<SitFinanCalcReportDetList> ReportDetList { get; set; }

        public SitFinanCalcVal ValueDetails { get; set; }

        public SitFinanCalcDetNote DetNote { get; set; }

        public bool ShowReport { get; set; }

        public bool ShowValueDetail { get; set; }

        public bool ShowNota { get; set; }
    }

    public class SitFinanCalcReportList
    {
        public int Id { get; set; }

        public string ReportName { get; set; }
    }

    public class SitFinanCalcReportDetList
    {
        public int CalcRowId { get; set; }

        public int RowId { get; set; }

        public string RowName { get; set; }

        public string RowNr { get; set; }
        public string RowNota { get; set; } 

        public int OrderView { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public decimal? Val1 { get; set; }

        public decimal? Val2 { get; set; }

        public decimal? Val3 { get; set; }

        public decimal? Val4 { get; set; }

        public decimal? Val5 { get; set; }

        public decimal? Val6 { get; set; }

    }

    public class SitFinanCalcVal
    {
        public int RowId { get; set; }

        public int ColumnId { get; set; }

        public string RowName { get; set; }

        public string Formula { get; set; }

        public string FormulaVal { get; set; }

        public List<SitFinanCalcValDet> DetailValueList { get; set; }
    }

    public class SitFinanCalcValDet
    {
        public string ElementDet { get; set; }

        public decimal Val { get; set; }
    }

    public class SitFinanCalcDetNote
    {
        public int Id { get; set; }

        public string BeforeNote { get; set; }

        public string AfterNote { get; set; }
    }

    public class SitFinanCalcDetail
    {
        public int RowId { get; set; }
        public string RowName { get; set; }
        public string ElementDet { get; set; }
        public string Formula { get; set; }
        public decimal Valoare { get; set; }
        public int OrderView { get; set; }
    }
}
