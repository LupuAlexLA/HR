using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.HR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP_AvailableSum : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("InvoiceElementsDetailsCategory")]
        public virtual int InvoiceElementsDetailsCategoryId { get; set; }
        public virtual InvoiceElementsDetailsCategory InvoiceElementsDetailsCategory { get; set; }

        [ForeignKey("Departament")]
        public int? DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }

        public decimal SumApproved { get; set; }
        public decimal SumAllocated { get; set; }
        public decimal Rest { get; set; }
        public int ApprovedYear { get; set; }
        public State State { get; set; }
        public int TenantId { get ; set ; }
    }
}
