using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP_State : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("BVC_PAAP")]
        public int BVC_PAAP_Id { get; set; }
        public virtual BVC_PAAP BVC_PAAP { get; set; }

        [ForeignKey("CotaTVA")]
        public int? CotaTVA_Id { get; set; }
        public virtual CotaTVA CotaTVA { get; set; }
        public DateTime OperationDate { get; set; }
        public PAAP_State Paap_State { get; set; }
        public string Comentarii { get; set; }
        public decimal ValoareEstimataFaraTvaLei { get; set; }
        public decimal ValoareTotalaValuta { get; set; }
        public decimal ValoareTotalaLei { get; set; }
        public State State { get; set; }
        public int TenantId { get; set ; }
    }
}
