using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public class DocumentType : AuditedEntity<int> ,IMustHaveTenant
    {
        [Required]
        public virtual string TypeName { get; set; }
        [Required]
        public virtual string TypeNameShort { get; set; }
        public virtual bool Editable { get; set; } 
        public virtual bool AutoNumber { get; set; } 
        public virtual bool ClosingMonth { get; set; }
        public int TenantId { get; set; }
    }
}
