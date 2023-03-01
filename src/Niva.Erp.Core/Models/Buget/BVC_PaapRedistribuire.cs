using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PaapRedistribuire : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("PaapCarePrimeste")]
        public int PaapCarePrimesteId { get; set; } // achizitie care primeste 
        public BVC_PAAP PaapCarePrimeste { get; set; }  
        public int TenantId { get; set; }
        public decimal SumaPlatita { get; set; }
        public DateTime DataRedistribuire { get; set; }
        public virtual IList<BVC_PaapRedistribuireDetalii> PaapRedistribuireDetalii { get; set; }   
    }

    public class BVC_PaapRedistribuireDetalii: AuditedEntity<int>
    {
        [ForeignKey("PaapCarePierde")]
        public int PaapCarePierdeId { get; set; }
        public BVC_PAAP PaapCarePierde { get; set; }    
        public decimal SumaPierduta { get; set; }

        [ForeignKey("BVC_PaapRedistribuire")]
        public virtual int BVC_PaapRedistribuireId { get; set; }
        public virtual BVC_PaapRedistribuire BVC_PaapRedistribuire { get; set; }
    }
}
