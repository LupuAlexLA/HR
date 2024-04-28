using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models
{
    public class MasuratoareInterpretare : AuditedEntity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("Masuratoare")]
        public int MasuratoareId { get; set; }
        public virtual Masuratoare Masuratoare { get; set; }
        public string Expresie1 { get; set; }
        public string DescriereRezultat1 { get; set; }
        public Culoare Culoare1 { get; set; }
        public string Expresie2 { get; set; }
        public string DescriereRezultat2 { get; set; }
        public Culoare Culoare2 { get; set; }
        public Stare Stare { get; set; }
    }
}
