using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvProduct :  AuditedEntity<int>  
    {
        [StringLength(1000)]
        public string Name { get; set; }

        public decimal PriceUnit { get; set; }

        public int InventoryNr { get; set; }

        [ForeignKey("InvUM")]
        public int InvUmId { get; set; }

        public InvUM InvUM { get; set; }

        [ForeignKey("InvCateg")]
        public int InvCategId { get; set; }
        public InvCateg InvCateg { get; set; }

       
        public State State { get; set; }

        
    }
}
