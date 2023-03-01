using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
 
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvOperation : AuditedEntity<int>,IMustHaveTenant
    {
        public DateTime OperationDate { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public InvOperationType OperationType { get; set; }
               
        [ForeignKey("StorageOut")]
        public int? StorageOutId { get; set; }
        public InvStorage StorageOut { get; set; }

        [ForeignKey("StorageIn")]
        public int? StorageInId { get; set; }
        public InvStorage StorageIn { get; set; }

        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }
        public Invoices Invoice { get; set; }


        public State State { get; set; }

        public IList<InvOperationDetail> OperationDetail { get; set; }

        public bool Processed { get; set; }
        public int TenantId { get; set; }
    }
}
