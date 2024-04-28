using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models
{
    public class Studiu : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public byte[] Poza { get; set; }
        public string Titlu { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public Stare Stare { get; set; }
        [ForeignKey("Owner")]
        public long? OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
