using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Niva.Erp.Models.Conta
{
    public class SetupStergOperExterna : AuditedEntity<int>, IMustHaveTenant
    {
        public bool AllowDeletion{ get; set; }
        public int TenantId { get; set; }
    }
}
