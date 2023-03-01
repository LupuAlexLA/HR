using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public class AccountRelation : AuditedEntity<int> , IMustHaveTenant
    {
        [Required]
        public virtual string DebitRoot { get; set; }
        [Required]
        public virtual string CreditRoot { get; set; }
        public int TenantId { get; set; }
    }
}
