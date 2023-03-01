using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.HR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
 
namespace Niva.Erp.Models.Buget
{
    public class BVC_BugetPrev : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }
        public DateTime DataBuget { get; set; }
        public string Descriere { get; set; }
        public BVC_Tip BVC_Tip { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public virtual IList<BVC_BugetPrevStatus> StatusList { get; set; }
        public BVC_Status Status
        {
            get { return StatusList.Where(f => f.BugetPrevId == Id).OrderByDescending(f => f.Id).Select(f => f.Status).First(); }
        }
        public List<BVC_BugetPrevRand> PrevRanduri { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public enum BVC_Tip
    {
        BVC,
        CashFlow
    }

    public class BVC_BugetPrevRand : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        [Required]
        public virtual BVC_BugetPrev BugetPrev { get; set; }

        [ForeignKey("FormRand")]
        public int FormRandId { get; set; }
        [Required]
        public virtual BVC_FormRand FormRand { get; set; }

        [ForeignKey("Departament")]
        public int DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }

        public virtual IList<BVC_BugetPrevRandValue> ValueList { get; set; }
        public decimal Value
        {
            get { return ValueList.Where(f => f.BugetPrevRandId == Id).Sum(f => f.Value); }
        }

        public bool Validat { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_BugetPrevStatus : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        [Required]
        public virtual BVC_BugetPrev BugetPrev { get; set; }
        public BVC_Status Status { get; set; }
        public DateTime StatusDate { get; set; }
        public int TenantId { get; set; }
    }

    public enum BVC_Status
    {
        Preliminat,
        Inregistrat,
        [Description("Avizat CD")]
        AvizatCD,
        [Description("Avizat CS")]
        AvizatCS,
        [Description("Aprobat BNR")]
        AprobatBNR
    }

    public class BVC_BugetPrevRandValue : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrevRand")]
        public int BugetPrevRandId { get; set; }
        [Required]
        public virtual BVC_BugetPrevRand BugetPrevRand { get; set; }
        public DateTime DataOper { get; set; }
        public DateTime DataLuna { get; set; }
        public string Description { get; set; }
        public BVC_PrevValueType ValueType { get; set; }
        public decimal Value { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }
        public int TenantId { get; set; }
        public bool EsteDinVenituri { get; set; }

    }

    public enum BVC_PrevValueType
    {
        Manual,
        Dobanzi,
        DiferentaCursValutar,
        Contributii,
        Creante,
        PAAP,
        Salarii,
        Amortizari,
        Altele,
        Cheltuieli
    }

    public class BVC_BugetPrevAutoValue : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Departament")]
        public int DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }
        public BVC_RowType TipRand { get; set; }
        public virtual BVC_RowTypeIncome? TipRandVenit { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_BugetPrevContributie : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime DataIncasare { get; set; }
        public decimal Value { get; set; }

        public BVC_BugetPrevContributieTipIncasare TipIncasare { get; set; }

        [ForeignKey("BankAccount")]
        public int? BankId { get; set; }
        public virtual Issuer BankAccount { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public int TenantId { get; set; }
        public string Descriere { get; set; }
    }

    public class BVC_BugetPrevStatCalcul : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        [Required]
        public virtual BVC_BugetPrev BugetPrev { get; set; }
        public BVC_BugetPrevElemCalc ElemCalc { get; set; }
        public bool StatusCalc { get; set; }
        public string Message { get; set; }
        public int TenantId { get; set; }
    }

    public enum BVC_BugetPrevElemCalc
    {
        Contributii,
        Creante,
        Dobanzi,
        AmortizariMijFixe,
        AmortizariCheltAvans,
        PAAP,
        Salarizare,
        Altele
    }

    public enum BVC_BugetPrevContributieTipIncasare
    {
        Contributii, 
        Creante, 
        [Description("Comision lichidator")]
        ComisionLichidator, 
        Altele 
    }

    public class BVC_BugetPrevSumeReinvest : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        [Required]
        public virtual BVC_BugetPrev BugetPrev { get; set; }
        public string IdPlasament { get; set; }
        public BVC_PlasamentType TipPlasament { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public decimal ValoarePlasament { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public VenitType VenitType { get; set; }
        public decimal ProcentDobanda { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_BugetPrevTitluriValab : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        [Required]
        public virtual BVC_BugetPrev BugetPrev { get; set; }
        public string IdPlasament { get; set; }
        public BVC_PlasamentType TipPlasament { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public decimal ValoarePlasament { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public VenitType VenitType { get; set; }
        public decimal ProcentDobanda { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
