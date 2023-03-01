using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta.Lichiditate
{
    public class LichidCalc : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("LichidConfig")]
        public int LichidConfigId { get; set; }
        public virtual LichidConfig LichidConfig { get; set; }

        [ForeignKey("LichidBenzi")]
        public int LichidBenziId { get; set; }
        public virtual LichidBenzi LichidBenzi { get; set; }
        public decimal Valoare { get; set; }
        public IList<LichidCalcDet> LichidCalcDet { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcDet : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("LichidCalc")]
        public int LichidCalcId { get; set; }
        public virtual LichidCalc LichidCalc { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcCurr : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("LichidConfig")]
        public int LichidConfigId { get; set; }
        public virtual LichidConfig LichidConfig { get; set; }

        [ForeignKey("LichidBenziCurr")]
        public int LichidBenziCurrId { get; set; }
        public virtual LichidBenziCurr LichidBenziCurr { get; set; }

        public decimal Valoare { get; set; }
        public IList<LichidCalcCurrDet> LichidCalcCurrDet { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcCurrDet : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("LichidCalcCurr")]
        public int LichidCalcCurrId { get; set; }
        public virtual LichidCalcCurr LichidCalcCurr { get; set; }

        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public int TenantId { get; set; }
    }

    public enum LichidCalcType: int
    {
        [Description("Lichiditate pe benzi")]
        LichidBenzi,
        [Description("Lichiditate pe moneda")]
        LichidValuta,
    }

}
