using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;

namespace Niva.Erp.Models.Conta
{
    public class CotaTVA : AuditedEntity<int>, IMustHaveTenant
    {
        public decimal VAT { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
