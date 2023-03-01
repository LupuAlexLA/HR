using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Buget.Dto
{
    public class ExchangeRateForecastDto
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal ValoareEstimata { get; set; }
    }

    public class ExchangeRateForecastEditDto
    {
        public int Id { get; set; }
        public virtual int? CurrencyId { get; set; }

        public decimal ValoareEstimata { get; set; }
        public int TenantId { get; set; }
        public int Year { get; set; }
        public State State { get; set; }
    }
}
