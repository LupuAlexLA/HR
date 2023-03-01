using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic.Casierii.Cupiuri
{
    public class CupiuriDetails : AuditedEntity<int>, IMustHaveTenant
    {
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }

        [ForeignKey("CupiuriItem")]
        public int CupiuriItemId { get; set; }
        public virtual CupiuriItem CupiuriItem { get; set; }
    }
}