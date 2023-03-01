using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.Conta
{
    public class OperationTypes : AuditedEntity<int>, IMustHaveTenant
    {
        [Required]
        public virtual string Name { get; set; }
        public int TenantId { get; set; }

        public virtual State State { get; set; }
    }
}
