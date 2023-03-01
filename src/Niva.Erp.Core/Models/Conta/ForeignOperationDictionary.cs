using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
    public enum FODictionaryType: int {
        Toate,
        Incasare,
        Plata
    }
    public class ForeignOperationDictionary : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public DateTime OperationDate { get; set; }
        public string Expression { get; set; }

        public State State { get; set; }
        public FODictionaryType FODictionaryType { get; set; }
        public int TenantId { get; set; }
    }
}
