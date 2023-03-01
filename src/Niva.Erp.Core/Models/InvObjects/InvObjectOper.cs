using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectOper : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime OperationDate { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public InvObjectOperType InvObjectsOperType { get; set; }

        [ForeignKey("InvObjectsStoreIn")]
        public int? InvObjectsStoreInId { get; set; }
        public InvStorage InvObjectsStoreIn { get; set; }

        [ForeignKey("InvObjectsStoreOut")]
        public int? InvObjectsStoreOutId { get; set; }
        public InvStorage InvObjectsStoreOut { get; set; }

        [ForeignKey("PersonStoreIn")]
        public int? PersonStoreInId { get; set; }
        public virtual Person PersonStoreIn { get; set; }

        [ForeignKey("PersonStoreOut")]
        public int? PersonStoreOutId { get; set; }
        public virtual Person PersonStoreOut { get; set; }

        public List<InvObjectOperDetail> OperDetails { get; set; }

        public bool Processed { get; set; }

        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }
        public Invoices Invoice { get; set; }

        public State State { get; set; }

        public int TenantId { get; set; }
    }
}
