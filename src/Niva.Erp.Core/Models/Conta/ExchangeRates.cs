using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public class ExchangeRates : AuditedEntity
    {
        public virtual DateTime ExchangeDate { get; set; }
        public virtual Decimal Value { get; set; }
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
