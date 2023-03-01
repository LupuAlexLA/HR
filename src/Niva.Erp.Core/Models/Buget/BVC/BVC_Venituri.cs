using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Buget
{
    public class BVC_VenitTitlu : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }
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
        public List<BVC_VenitTitluBVC> VenituriBVC { get; set; }
        public List<BVC_VenitTitluCF> VenituriCF { get; set; }
        public decimal ProcentDobanda { get; set; }
        public bool Reinvestit { get; set; }
        public bool Selectat { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_VenitTitluBVC : AuditedEntity<int>
    {
        [ForeignKey("BVC_VenitTitlu")]
        public int BVC_VenitTitluId { get; set; }
        public virtual BVC_VenitTitlu BVC_VenitTitlu { get; set; }
        public DateTime DataDobanda { get; set; }
        public decimal ValoarePlasament { get; set; }
        public decimal DobandaLuna { get; set; } // valoareDobandaLuna
        public decimal DobandaCumulataPrec { get; set; } // valoareDobandaCumulataPanaLaLuna
    }

    public class BVC_VenitTitluCF : AuditedEntity<int>
    {
        [ForeignKey("BVC_VenitTitlu")]
        public int BVC_VenitTitluId { get; set; }
        public virtual BVC_VenitTitlu BVC_VenitTitlu { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal ValoarePlasament { get; set; }
        public decimal DobandaTotala { get; set; } // valoareDobandaCumulataPanaLaLuna
        public DateTime DataReinvestire { get; set; }
        public decimal SumaReinvestita { get; set; }
        public virtual List<BVC_VenitTitluCFReinv> BVC_VenitTitluCFReinv { get; set; }
    }

    public class BVC_VenitTitluCFReinv : AuditedEntity<int>
    {
        [ForeignKey("BVC_VenitTitluCF")]
        public int BVC_VenitTitluCFId { get; set; }
        public virtual BVC_VenitTitluCF BVC_VenitTitluCF { get; set; }
        public DateTime DataReinvestire { get; set; }
        public decimal SumaReinvestita { get; set; }
        public decimal ProcDobanda { get; set; }
        public bool MainValue { get; set; }
        public decimal SumaIncasata { get; set; }   

        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
            
        public decimal CursValutarEstimat { get; set; }

        public List<BVC_VenitCheltuieli> VenitCheltuieli { get; set; }
    }

    public class BVC_VenitTitluParams : AuditedEntity<int>
    {
        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }

        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
    }

    public class BVC_VenitCheltuieli : AuditedEntity<int>
    {
        [ForeignKey("BVC_VenitTitluCFReinv")]
        public int BVC_VenitTitluCFReinvId { get; set; }
        public virtual BVC_VenitTitluCFReinv BVC_VenitTitluCFReinv { get; set; }

        [ForeignKey("BVC_BugetPrevRandValue")]
        public int BVC_BugetPrevRandValueId { get; set; }
        public virtual BVC_BugetPrevRandValue BVC_BugetPrevRandValue { get; set; }

        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }
        public decimal Value { get; set; }
    }

    public class BVC_VenitBugetPrelim : AuditedEntity<int>
    {

        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }
        public DateTime? DataUltBalanta { get; set; }

        [ForeignKey("BVC_BugetPrev")]
        public int? BVC_BugetPrevId { get; set; }
        public virtual BVC_BugetPrev BVC_BugetPrev { get; set; }
        public PreliminatCalculType PreliminatCalculType { get; set; }
        public List<BVC_VenitProcRepartiz> VenitProcRepartiz { get; set; }

    }

    public class BVC_VenitProcRepartiz : AuditedEntity<int>
    {

        [ForeignKey("BVC_VenitBugetPrelim")]
        public int BVC_VenitBugetPrelimId { get; set; }
        public virtual BVC_VenitBugetPrelim BVC_VenitBugetPrelim { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public decimal ProcRepartiz { get; set; }
        public decimal VenitRepartiz { get; set; }
        public decimal VenitRepartizBVC { get; set; }
    }

    public enum VenitType
    {
        Plasamente,
        Contributii
    }

    public enum PreliminatCalculType
    {
        Manual,
        [Description("Ultima balanta")]
        UltimaBalanta,
        [Description("Preliminare venituri")]
        [Display(Name = "Preliminare venituri")]
        PreliminareVenituri
    }
}
