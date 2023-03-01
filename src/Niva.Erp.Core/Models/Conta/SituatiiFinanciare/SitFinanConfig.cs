using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.Conta.SituatiiFinanciare
{
    public class SitFinan : AuditedEntity<int> ,IMustHaveTenant
    {
        public virtual DateTime RapDate { get; set; }

        public virtual State State { get; set; }
  
        public int TenantId { get; set; }
    }

    public class SitFinanRap : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string ReportName { get; set; }

        [StringLength(10)]
        public string ReportSymbol { get; set; }

        public int OrderView { get; set; }

        public int NrCol { get; set; }

        public bool PerioadaEchivalenta { get; set; }

        public virtual State State { get; set; }

        [ForeignKey("SitFinan")]
        public int SitFinanId { get; set; }
        public SitFinan SitFinan { get; set; }     
        public int TenantId { get; set; }
    }

    public class SitFinanRapConfig : AuditedEntity<int> ,IMustHaveTenant
    {
        [StringLength(1000)]
        public string RowName { get; set; }

        public decimal RowCode { get; set; }

        [StringLength(10)]
        public string RowNr { get; set; }

        [StringLength(10)]
        public string RowNota { get; set; } 

        public int OrderView { get; set; }

        [StringLength(1000)]
        public string Col1 { get; set; }

        [StringLength(1000)]
        public string Col2 { get; set; }

        [StringLength(1000)]
        public string Col3 { get; set; }

        [StringLength(1000)]
        public string Col4 { get; set; }

        [StringLength(1000)]
        public string Col5 { get; set; }

        [StringLength(1000)]
        public string Col6 { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }
        public SitFinanRowModCalc SitFinanRowModCalc { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }
        public int TenantId { get; set; }
    }

    public class SitFinanRapConfigCol : AuditedEntity<int>, IMustHaveTenant
    {
        public int ColumnNr { get; set; }

        [StringLength(1000)]
        public string ColumnName { get; set; }

        public int? ColumnModCalc { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }
 
        public int TenantId { get; set; }
    }

    public class SitFinanRapConfigNote : AuditedEntity<int>, IMustHaveTenant
    {
        public string BeforeNote { get; set; }

        public string AfterNote { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }
 
        public int TenantId { get; set; }
    }

    public class SitFinanRapConfigCorel : AuditedEntity<int>, IMustHaveTenant
    {
        public decimal OrderView { get; set; }

        [StringLength(1000)]
        public string DescribeCol1 { get; set; }

        [StringLength(1000)]
        public string FormulaCol1 { get; set; }

        [StringLength(1000)]
        public string DescribeCol2 { get; set; }

        [StringLength(1000)]
        public string FormulaCol2 { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }
        public int TenantId { get; set; }
    }

    public class SitFinanRapFluxConfig : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public SitFinanFluxRowType SitFinanFluxRowType { get; set; }
        public int TenantId { get; set; }
    }


    public enum SitFinanRowModCalc
    {
        [Description("Valori balanta")]
        ValoriBalanta,
        Rulaje
    }

    public enum SitFinanFluxRowType
    {
        ContCash,
        Exceptii
    }

}
