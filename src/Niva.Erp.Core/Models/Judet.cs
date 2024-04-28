using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models
{
    public class Judet : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Denumire { get; set; }
        public string Abreviere { get; set; }
        public Stare Stare { get; set; }
    }
}
