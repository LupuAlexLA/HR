using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.HR
{
    public class SalariatiDepartamente : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        [ForeignKey("Departament")]
        public int DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
