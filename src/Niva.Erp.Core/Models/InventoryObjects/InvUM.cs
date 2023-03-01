using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvUM : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Abrv { get; set; }
        public State State { get; set; }     
        public int TenantId { get; set; }
    }
}
