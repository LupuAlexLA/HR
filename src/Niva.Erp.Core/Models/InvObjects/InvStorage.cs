using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.InvObjects
{
    public class InvStorage : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string StorageName { get; set; }

        public bool CentralStorage { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
