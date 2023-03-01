using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.Contracts
{
    public class InvoiceElementsDetailsCategory : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string CategoryElementDetName { get; set; }
        public virtual State State { get; set; }

        public virtual CategoryType CategoryType { get; set; }

        public int TenantId { get; set; }
    }

    public enum CategoryType: int
    {
        Cheltuieli,
        Venituri,
        [Description("Plati incasari")]
        PlatiIncasari
    }
}
