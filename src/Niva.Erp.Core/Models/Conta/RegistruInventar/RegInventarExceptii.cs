using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta.RegistruInventar
{
    public class RegInventarExceptii: AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        [StringLength(1000)]
        public string Formula { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
