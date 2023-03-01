using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Models.SectoareBnr
{
    public class BNR_Sector : AuditedEntity<int>, IMustHaveTenant
    {
        public string Sector { get; set; }
        public string Denumire { get; set; }
        public State State { get; set; }    
        public int TenantId { get; set; }
    }
}
