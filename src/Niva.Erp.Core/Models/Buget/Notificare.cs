using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.HR;
using System;

namespace Niva.Erp.Models.Buget
{
    public class Notificare : AuditedEntity<int>, IMustHaveTenant
    {
        public virtual int? DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }
        public virtual int? IdPersonal { get; set; }    
        public StareNotificare StareNotificare { get; set; }
        public string Mesaj { get; set; }
        public int UserVizualizareId { get; set; }
        public DateTime DataVizualizare { get; set; }   
        public int TenantId { get; set; }
    }
    public enum StareNotificare
    {
        Citita,     
        Necitita
    }
}
