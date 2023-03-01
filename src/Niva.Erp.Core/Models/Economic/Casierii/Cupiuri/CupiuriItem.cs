using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic.Casierii.Cupiuri
{
    public class CupiuriItem : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("CupiuriInit")]
        public int CupiuriInitId { get; set; }
        public virtual CupiuriInit CupiuriInit { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public IList<CupiuriDetails> CupiuriDetails { get; set; }
        public int TenantId { get; set; }

        public State State { get; set; }
    }
}
