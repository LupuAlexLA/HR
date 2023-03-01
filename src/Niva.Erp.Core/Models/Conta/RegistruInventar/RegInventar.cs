using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta.RegistruInventar
{
    public class RegInventar : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal Value { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
