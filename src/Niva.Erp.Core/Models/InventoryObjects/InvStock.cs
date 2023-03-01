using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvStock : Entity<int>, IAudited
    {
        [ForeignKey("InvProduct")]
        public int InvProductId { get; set; }
        public InvProduct InvProduct { get; set; }

        [ForeignKey("InvStorage")]
        public int InvStorageId { get; set; }
        public InvStorage InvStorage { get; set; }

        public DateTime StockDate { get; set; }

        public int StockQuantity { get; set; }

        public int DischargeQuantity { get; set; }

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
