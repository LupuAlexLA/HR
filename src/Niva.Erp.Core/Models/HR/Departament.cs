using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Models.HR
{
    public class Departament : AuditedEntity<int>, IMustHaveTenant
    {
        public string Name { get; set; }
        public State State { get; set; }
        public int? IdExtern { get; set; }
        public int TenantId { get; set; }
    }
}
