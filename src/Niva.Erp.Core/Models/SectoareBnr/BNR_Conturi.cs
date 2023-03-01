using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.SectoareBnr
{
    public class BNR_Conturi : AuditedEntity<int>, IMustHaveTenant
    {
        //public string Cont { get; set; }
        public string ContBNR { get; set; }
        public decimal SoldDb { get; set; }
        public decimal SoldCr { get; set; }
        public decimal Value { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("BNR_AnexaDetail")]
        public int AnexaDetailId { get; set; }
        public virtual BNR_AnexaDetail BNR_AnexaDetail { get; set; }

        [ForeignKey("BNR_Sector")]
        public int? BNR_SectorId { get; set; }
        public virtual BNR_Sector BNR_Sector { get; set; }
    }
}
