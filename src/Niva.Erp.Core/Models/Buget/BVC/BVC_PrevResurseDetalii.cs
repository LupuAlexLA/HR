using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget.BVC
{
    public class BVC_PrevResurseDetalii : AuditedEntity, IMustHaveTenant
    {
        public string IdPlasament { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public decimal ProcentDobanda { get; set; }
        public decimal NrZilePlasament { get; set; }
        public decimal SumaInvestita { get; set; }

        [ForeignKey("BVC_PrevResurse")]
        public int BVC_PrevResurseId { get; set; }
        public BVC_PrevResurse BVC_PrevResurse { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public int TenantId { get; set; }
        public decimal CursValutar { get; set; }
        public decimal SumaInRon { get; set; }
    }
}
