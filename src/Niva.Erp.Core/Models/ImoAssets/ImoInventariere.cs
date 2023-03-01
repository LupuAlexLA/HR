using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Models.ImoAssets
{
    public class ImoInventariere : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime DataInventariere { get; set; }
        public virtual IList<ImoInventariereDet> ImoInventariereDetails { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
