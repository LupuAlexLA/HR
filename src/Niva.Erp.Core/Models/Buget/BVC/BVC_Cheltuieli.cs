using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DevExpress.Data.Mask;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.HR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Buget.BVC 
{
    public class BVC_Cheltuieli : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime DataIncasare { get; set; }
        public decimal Value { get; set; }

        [ForeignKey("BVC_FormRand")]
        public int BVC_FormRandId { get; set; }
        public virtual BVC_FormRand BVC_FormRand { get; set;}

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        
        public int TenantId { get; set; }

        [ForeignKey("Departament")]
        public int DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }
        public string Descriere { get; set; }
    }

}
