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
    public class InvGestObjects : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime OperationDate { get; set; }

        [ForeignKey("InvProduct")]
        public int InvProductId { get; set; }
        public InvProduct InvProduct { get; set; }

        [ForeignKey("InvStorage")]
        public int InvStorageId { get; set; }
        public InvStorage InvStorage { get; set; }

        public int TranzStockQuantity { get; set; }

        public int StockQuantity { get; set; }

        public int TranzDischargeQuantity { get; set; }

        public int DischargeQuantity { get; set; }

        public decimal TranzInvStValue { get; set; }

        public decimal InvStValue { get; set; }

        public decimal TranzInvDischValue { get; set; }

        public decimal InvDischValue { get; set; }

        [ForeignKey("InvOperation")]
        public int InvOperationId { get; set; }
        public InvOperation InvOperation { get; set; }

       
      
        public int TenantId { get; set; }
    }
}
