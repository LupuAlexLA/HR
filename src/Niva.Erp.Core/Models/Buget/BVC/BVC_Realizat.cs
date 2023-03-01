using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Buget
{
    public class BVC_Realizat : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalanceId")]
        public int SavedBalanceId { get; set; }
        [Required]
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("BVC_Formular")]
        public int BVC_FormularId { get; set; }
        [Required]
        public virtual BVC_Formular BVC_Formular { get; set; }
        public BVC_Tip BVC_Tip { get; set; }
        public List<BVC_RealizatRand> BVC_RealizatRanduri { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_RealizatRand : AuditedEntity<int>
    {
        [ForeignKey("BVC_Realizat")]
        public int BVC_RealizatId { get; set; }
        [Required]
        public virtual BVC_Realizat BVC_Realizat { get; set; }

        [ForeignKey("BVC_FormRand")]
        public int BVC_FormRandId { get; set; }
        [Required]
        public virtual BVC_FormRand BVC_FormRand { get; set; }

        public virtual decimal ValoareCuReferat { get; set; }
        public virtual decimal ValoareFaraReferat { get; set; } 
        public List<BVC_RealizatRandDetails> BVC_RealizatRandDetails { get; set; }
    }


    public class BVC_RealizatRandDetails : AuditedEntity<int>
    {
        [ForeignKey("BVC_RealizatRand")]
        public int BVC_RealizatRandId { get; set; }
        [Required]
        public virtual BVC_RealizatRand BVC_RealizatRand { get; set; }

        public virtual string Descriere { get; set; }

        public virtual decimal Valoare { get; set; }

        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
    }

    public class BVC_RealizatExceptii : AuditedEntity<int>
    {
        public virtual string Descriere { get; set; }
        public virtual decimal Valoare { get; set; }
    }

    public class BVC_BalRealizat : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalanceId")]
        public int SavedBalanceId { get; set; }
        [Required]
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("BVC_Formular")]
        public int BVC_FormularId { get; set; }
        [Required]
        public virtual BVC_Formular BVC_Formular { get; set; }
        public BVC_Tip BVC_Tip { get; set; }
        public List<BVC_BalRealizatRand> BVC_BalRealizatRanduri { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_BalRealizatRand : AuditedEntity<int>
    {
        [ForeignKey("BVC_BalRealizat")]
        public int BVC_BalRealizatId { get; set; }
        [Required]
        public virtual BVC_BalRealizat BVC_BalRealizat { get; set; }

        [ForeignKey("BVC_FormRand")]
        public int BVC_FormRandId { get; set; }
        [Required]
        public virtual BVC_FormRand BVC_FormRand { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }

        public virtual decimal Valoare { get; set; }
        public List<BVC_BalRealizatRandDetails> BVC_BalRealizatRandDetails { get; set; }
    }


    public class BVC_BalRealizatRandDetails : AuditedEntity<int>
    {
        [ForeignKey("BVC_BalRealizatRand")]
        public int BVC_BalRealizatRandId { get; set; }
        [Required]
        public virtual BVC_BalRealizatRand BVC_BalRealizatRand { get; set; }

        public virtual string Descriere { get; set; }

        public virtual decimal Valoare { get; set; }
    }
}
