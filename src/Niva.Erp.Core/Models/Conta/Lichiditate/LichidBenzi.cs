using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Models.Conta.Lichiditate
{
    public class LichidBenzi : AuditedEntity<int>, IMustHaveTenant
    {
        public string Descriere { get; set; }
        public int? DurataMinima { get; set; }
        public int? DurataMaxima { get; set; }
        public int? DurataInLuniMinima { get; set; }
        public int? DurataInLuniMaxima { get; set; }
        public virtual State State { get; set; }
        public int TenantId { get; set; }
    }
}
