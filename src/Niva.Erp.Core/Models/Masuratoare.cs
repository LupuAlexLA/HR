using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models
{
    public class Masuratoare : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Denumire { get; set; }
        public decimal Peak { get; set; }
        public decimal Average { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
