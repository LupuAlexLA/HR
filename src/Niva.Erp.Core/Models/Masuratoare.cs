using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models
{
    public class Masuratoare : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Denumire { get; set; }
        [ForeignKey("Tara")]
        public int TaraId { get; set; }
        public virtual Tara Tara { get; set; }
        [ForeignKey("Judet")]
        public int JudetId { get; set; }
        public Judet Judet { get; set; }
        public string Localitate { get; set; }
        public string Adresa { get; set; }
        public decimal Peak { get; set; }
        public decimal Average { get; set; }
        public decimal Latitudine { get; set; }
        public decimal Longitudine { get; set; }
        public Stare Stare { get; set; }
        [ForeignKey("Owner")]
        public long? OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
