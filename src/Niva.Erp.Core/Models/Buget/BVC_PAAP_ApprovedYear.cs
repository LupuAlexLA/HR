using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP_ApprovedYear : AuditedEntity<int>, IMustHaveTenant
    {
        public int Year { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
