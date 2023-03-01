using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.InventoryObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvOperationDetail : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("InvOperation")]
        public int InvOperationId { get; set; }
        public InvOperation InvOperation { get; set; }

        [ForeignKey("InvProduct")]
        public int InvProductId { get; set; }
        public InvProduct InvProduct { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("InvoiceDetails")]
        public int? InvoiceDetailsId { get; set; }
        public InvoiceDetails InvoiceDetails { get; set; }
        public int TenantId { get; set; }
    }
}
