namespace Niva.Erp.Models.Conta
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SavedBalance : AuditedEntity<int>, IMustHaveTenant
    {
        [MaxLength(200)]
        public virtual string BalanceName { get; set; }

        public virtual DateTime SaveDate { get; set; }

        public virtual bool ExternalSave { get; set; }
        public bool IsDaily { get; set; }

        public virtual IList<SavedBalanceDetails> SavedBalanceDetails { get; set; }
        public int TenantId { get; set; }
    }
}

