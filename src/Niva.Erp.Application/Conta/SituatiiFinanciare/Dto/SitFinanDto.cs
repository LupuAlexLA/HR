using Niva.Erp.Models.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.SituatiiFinanciare
{
    public class SitFinanForm
    {
        public SitFinanConfigDto SitFinan { get; set; }

        public List<SitFinanDto> SitFinanList { get; set; }

        public bool ShowList { get; set; }

        public bool ShowEdit { get; set; }
    }

    public class SitFinanDto
    {
        public int Id { get; set; }

        public virtual DateTime RapDate { get; set; }
    }

    public class SitFinanDDDto
    {
        public int Id { get; set; }

        public virtual string RapDate { get; set; }
    }

    public class SitFinanConfigDto
    {
        public int Id { get; set; }

        public virtual DateTime RapDate { get; set; }

        public bool CopyReport { get; set; }

        public int? PrevDateId { get; set; }
    }

    public class SitFinanReportForm
    {
        public int SitFinanId { get; set; }

        public DateTime SitFinanDate { get; set; }

        public List<SitFinanRapDto> SitFinanRapList { get; set; }

    }

    public class SitFinanRapDto
    {
        public int Id { get; set; }

        public string ReportName { get; set; }

        public string ReportSymbol { get; set; }

        public int OrderView { get; set; }

        public int NrCol { get; set; }

        public bool PerioadaEchivalenta { get; set; }

        public int SitFinanId { get; set; }

        public SitFinanRowModCalc SitFinanRowModCalc { get; set; }
    }

    public class SitFinanFormuleForm
    {
        public int SitFinanId { get; set; }

        public int ReportId { get; set; }

        public string ReportName { get; set; }

        public int NrCol { get; set; }

        public string ReportSymbol { get; set; }

        public List<SitFinanRapConfigColDto> ConfigColList { get; set; }

        public List<SitFinanRapConfigDto> ConfigFormulaList { get; set; }
    }

    public class SitFinanRapConfigDto
    {
        public int Id { get; set; }
        public string RowName { get; set; }

        public decimal RowCode { get; set; }

        public string RowNr { get; set; }

        public string RowNota { get; set; } 

        public int OrderView { get; set; }

        public string Col1 { get; set; }

        public string Col2 { get; set; }

        public string Col3 { get; set; }

        public string Col4 { get; set; }

        public string Col5 { get; set; }

        public string Col6 { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }

        public SitFinanRowModCalc SitFinanRowModCalc { get; set; }
    }

    public class SitFinanRapConfigColDto
    {
        public int Id { get; set; }

        public int ColumnNr { get; set; }

        public string ColumnName { get; set; }

        public int? ColumnModCalc { get; set; }

    }

    public class SitFinanRapConfigNoteDto
    {
        public int SitFinanId { get; set; }

        public int ReportId { get; set; }

        public string ReportName { get; set; }

        public string ReportSymbol { get; set; }

        public int Id { get; set; }

        public string BeforeNote { get; set; }

        public string AfterNote { get; set; }

    }
    public class SitFinanRapFluxConfigDto
    { 
        public int Id { get; set; }
        public int SitFinanRapId { get; set; }
        
        public string Debit { get; set; }
        public string Credit { get; set; }
        public SitFinanFluxRowType SitFinanFluxRowType { get; set; }
        public int TenantId { get; set; }
    }
}