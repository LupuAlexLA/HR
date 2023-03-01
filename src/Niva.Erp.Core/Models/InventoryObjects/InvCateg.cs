using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvCateg : AuditedEntity<int>,IMustHaveTenant
    {
        [StringLength(1000)]
        public string Name { get; set; }
        public int TenantId { get; set; }
    }
}
