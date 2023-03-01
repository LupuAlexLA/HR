using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public class ZileLibere : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime ZiLibera { get; set; }
        public int TenantId { get; set; }
    }
}
