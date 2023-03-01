using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
    public class AccountTaxProperty : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public DateTime PropertyDate { get; set; }
        public AccountTaxPropertyType PropertyType { get; set; }

        public decimal PropertyValue { get; set; }

        [ForeignKey("AccountNeded")]
        public int? AccountNededId { get; set; }
        public virtual Account AccountNeded { get; set; }

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int TenantId { get ; set; }
    }

    public enum AccountTaxPropertyType
    {
        Nedeductibil,
        Deductibil
    }
}
