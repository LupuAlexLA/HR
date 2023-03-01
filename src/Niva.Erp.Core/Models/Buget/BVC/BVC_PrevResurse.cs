using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget.BVC
{
    public class BVC_PrevResurse : AuditedEntity, IMustHaveTenant
    {
        public string Descriere { get; set; }
        public int OrderView { get; set; }
        public decimal Suma { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("BugetPrev")]
        public int BugetPrevId { get; set; }
        public virtual BVC_BugetPrev BugetPrev { get; set; }
        public int TenantId { get; set; }
    }
}
