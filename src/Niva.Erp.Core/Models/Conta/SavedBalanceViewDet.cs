using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
    public class SavedBalanceViewDet: Entity<int>    
    {
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [Display(Name = "Sold initial creditor")]
        public decimal CrValueI { get; set; }

        [Display(Name = "Sold initial debitor")]
        public decimal DbValueI { get; set; }

        [Display(Name = "Rulaj luna credit")]
        public decimal CrValueM { get; set; }

        [Display(Name = "Rulaj luna debit")]
        public decimal DbValueM { get; set; }

        [Display(Name = "Rulaj an credit")]
        public decimal CrValueY { get; set; }

        [Display(Name = "Rulaj an debit")]
        public decimal DbValueY { get; set; }

        [Display(Name = "Sold final creditor")]
        public decimal CrValueF { get; set; }

        [Display(Name = "Sold final debitor")]
        public decimal DbValueF { get; set; }

        [Display(Name = "Rulaj precedent debitor")]
        public decimal DbValueP { get; set; }

        [Display(Name = "Rulaj precedent creditor")]
        public decimal CrValueP { get; set; }

        public int? NivelRand { get; set; }

        public bool IsSynthetic { get; set; }
        public string Cont { get; set; }
        public string Denumire { get; set; }
        public bool IsConverted { get; set; }   

        public int TenantId { get; set; }
    }


    public enum CurrencyType : int
    {
        RonEchivalentRon,
        Valuta
    }
}

