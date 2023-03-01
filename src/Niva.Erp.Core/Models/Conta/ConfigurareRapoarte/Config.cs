using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta.ConfigurareRapoarte
{
    public class ReportInit : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime RapDate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class Report : AuditedEntity<int>, IMustHaveTenant
    {
        public string ReportName { get; set; }

        [StringLength(10)]
        public string ReportSymbol { get; set; }
        public State State { get; set; }

        [ForeignKey("ReportInit")]
        public int ReportInitId { get; set; }

        public ReportInit ReportInit { get; set; }
        public int TenantId { get; set; }
    }

    public class ReportConfig : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string RowName { get; set; }

        public decimal RowCode { get; set; }

        [StringLength(10)]
        public string RowNr { get; set; }

        public int OrderView { get; set; }

        [StringLength(1000)]
        public string FormulaSold { get; set; } 

        [StringLength(1000)]
        public string FormulaRulaj { get; set; } 

        public bool Bold { get; set; }
        public bool TotalRow { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public Report Report { get; set; }
        public int TenantId { get; set; }
    }

    public class ReportCalc
    {
        public string AppClientName { get; set; }
        public string ReportTitle { get; set; }
        public string SubTitle { get; set; }
        public List<ReportCalcItem> ReportCalcItems { get; set; }

    }
    public class ReportCalcItem
    {
        public DateTime ReportDate { get; set; }
        public string RowName { get; set; }
        public decimal RowValue { get; set; }
        public decimal? RowCode { get; set; }
        public int OrderView { get; set; }
        public int ReportConfigRowId { get; set; }
        public bool Bold { get; set; }  
        public bool Calculat { get; set; }
    }
}
