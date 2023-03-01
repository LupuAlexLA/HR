using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Buget
{
    public class ExchangeRateForecast : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Currency")]
        public virtual int? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public decimal ValoareEstimata { get; set; }
        public int TenantId { get; set; }
        public int Year { get; set; }
        public State State { get; set; }
    }
}
