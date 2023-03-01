using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.Conta
{
    public class ActivityType : AuditedEntity<int>, IMustHaveTenant
    {
        [Required]
        public string ActivityName { get; set; }
        public State Status { get; set; }
        public int TenantId { get; set; }
        public bool MainActivity { get; set; }
    }
}
