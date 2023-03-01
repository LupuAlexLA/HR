using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.Conta.SituatiiFinanciare
{
    public class SitFinanCalc : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public SavedBalance SavedBalance { get; set; }

        [ForeignKey("SitFinanRapRow")]
        public int SitFinanRapRowId { get; set; }
        public SitFinanRapConfig SitFinanRapRow { get; set; }

        public decimal? Val1 { get; set; }

        public decimal? Val2 { get; set; }

        public decimal? Val3 { get; set; }

        public decimal? Val4 { get; set; }

        public decimal? Val5 { get; set; }

        public decimal? Val6 { get; set; }

        public bool Calculated { get; set; }

        public bool Validated { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }

        public int TenantId { get; set; }
    }

    public class SitFinanCalcDetails : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public SavedBalance SavedBalance { get; set; }

        [ForeignKey("SitFinanRapRow")]
        public int SitFinanRapRowId { get; set; }
        public SitFinanRapConfig SitFinanRapRow { get; set; }

        public int ColumnId { get; set; }

        [StringLength(1000)]
        public string ElementDet { get; set; }

        public decimal Val { get; set; }
 
        public int TenantId { get; set; }
    }

    public class SitFinanCalcFormulaDet : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public SavedBalance SavedBalance { get; set; }

        [ForeignKey("SitFinanRapRow")]
        public int SitFinanRapRowId { get; set; }
        public SitFinanRapConfig SitFinanRapRow { get; set; }

        public int ColumnId { get; set; }

        [StringLength(1000)]
        public string Formula { get; set; }

        [StringLength(1000)]
        public string FormulaVal { get; set; }
 
        public int TenantId { get; set; }
    }

    public class SitFinanCalcNote : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public SavedBalance SavedBalance { get; set; }

        [ForeignKey("SitFinanRap")]
        public int SitFinanRapId { get; set; }
        public SitFinanRap SitFinanRap { get; set; }

        public string BeforeNote { get; set; }

        public string AfterNote { get; set; }
        public int TenantId { get; set; }
    }
}
