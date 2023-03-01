using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;

namespace Niva.Erp.Models.Economic.Casierii.Cupiuri
{
    public class CupiuriInit : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime OperationDate { get; set; }
        public State State { get; set; }

        public int TenantId { get; set; }
    }
}
