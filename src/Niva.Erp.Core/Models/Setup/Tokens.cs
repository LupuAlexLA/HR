using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Setup
{
    public class Tokens : AuditedEntity<int>, IMustHaveTenant
    {
        public virtual string Token { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public int TenantId { get; set; }
    }
}
