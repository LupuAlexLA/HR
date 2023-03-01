using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectInventariere : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime DataInventariere { get; set; }
        public virtual IList<InvObjectInventariereDet> InvObjectInventariereDetails { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
