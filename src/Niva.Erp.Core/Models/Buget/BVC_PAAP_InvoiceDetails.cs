using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP_InvoiceDetails : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [ForeignKey("InvoiceDetails")]
        public int InvoiceDetailsId { get; set; }
        public virtual InvoiceDetails InvoiceDetails { get; set; }

        [ForeignKey("BVC_PAAP")]
        public int BVC_PAAPId { get; set; }
        public virtual BVC_PAAP BVC_PAAP { get; set; }
    }
}
