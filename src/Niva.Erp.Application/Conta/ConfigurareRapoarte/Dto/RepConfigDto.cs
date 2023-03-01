using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.ConfigurareRapoarte.Dto
{
    public class RepConfigInitDto
    {
        public int Id { get; set; }
        public string RapDate { get; set; }
    }

    public class RepConfigEditDto 
    {
        public int Id { get; set; }
        public DateTime RapDate { get; set; }
        public int TenantId { get; set; }
    }

    public class ReportConfigForm
    {
        public int RepConfigId { get; set; }
        public DateTime RepDate { get; set; }
        public List<RepConfigDto> ReportList { get; set; }
    }

    public class RepConfigDto
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public string ReportSymbol { get; set; }
        public int RepConfigId { get; set; }
    }

    public class ReportConfigFormulaForm
    {
        public int RepConfigId { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportSymbol { get; set; }
        public List<ConfigFormulaDto> ConfigFormulaList { get; set; }

    }

    public class ConfigFormulaDto
    {
        public string RowName { get; set; }

        public decimal RowCode { get; set; }

        public string RowNr { get; set; }

        public int OrderView { get; set; }

        public string FormulaSold { get; set; }

        public string FormulaRulaj { get; set; }

        public bool Bold { get; set; }
        public bool TotalRow { get; set; }
    }

    public class CalcRapListDto
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public int OrderView { get; set; }
    }
}
