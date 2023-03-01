using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Models.Conta.Lichiditate
{
    public class LichidBenziCurr : AuditedEntity<int>, IMustHaveTenant
    {
        public string Descriere { get; set; }
        public int? CurrencyId { get; set; }
        public bool Other { get; set; }
        public virtual State State { get; set; }    
        public int TenantId { get; set; }
    }
}
