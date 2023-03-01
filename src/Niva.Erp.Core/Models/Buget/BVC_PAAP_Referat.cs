using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP_Referat : AuditedEntity<int>, IMustHaveTenant
    {
        public string Name { get; set; }

        [ForeignKey("BVC_PAAP")]
        public int BVC_PAAP_Id { get; set; }
        public virtual BVC_PAAP BVC_PAAP { get; set; }

        public State State { get; set; }

        public DateTime OperationDate { get; set; }
        public decimal Suma { get; set; }
            
        public int TenantId { get; set; }
    }
}
