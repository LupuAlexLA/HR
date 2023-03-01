using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvStorage : AuditedEntity<int>
    {
        [StringLength(1000)]
        public string Name { get; set; }


        //[ForeignKey("InvAccount")]
        //public int InvAccountId { get; set; }
        //public Account InvAccount { get; set; }

        //[ForeignKey("ExpenseAccount")]
        //public int ExpenseAccountId { get; set; }
        //public Account ExpenseAccount { get; set; }

        //[ForeignKey("ExtraAccount")]
        //public int ExtraAccountId { get; set; }
        //public Account ExtraAccount { get; set; }

        public bool CentralStorage { get; set; }

        public State State { get; set; }
     
    }
}
