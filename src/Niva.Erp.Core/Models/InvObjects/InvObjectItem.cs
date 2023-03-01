using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectItem : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string Name { get; set; }
        public DateTime OperationDate { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        [ForeignKey("PrimDocumentType")]
        public int? PrimDocumentTypeId { get; set; }
        public DocumentType PrimDocumentType { get; set; }

        [StringLength(1000)]
        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        [ForeignKey("ThirdParty")]
        public int? ThirdPartyId { get; set; }
        public Person ThirdParty { get; set; }

        [ForeignKey("InvoiceDetails")]
        public int? InvoiceDetailsId { get; set; }
        public InvoiceDetails InvoiceDetails { get; set; }

        public decimal PriceUnit { get; set; }

        public int InventoryNr { get; set; }

        public int Quantity { get; set; }

        public decimal InventoryValue { get; set; }

        public InvObjectOperType OperationType { get; set; }


        [ForeignKey("InvStorage")]
        public int? InvObjectStorageId { get; set; }
        public InvStorage InvStorage { get; set; }

        [ForeignKey("InvCateg")]
        public int InvCategId { get; set; }
        public InvCateg InvCateg { get; set; }

        [ForeignKey("InvObjectAccount")]
        public int? InvObjectAccountId { get; set; }
        public Account InvObjectAccount { get; set; }

        [ForeignKey("ExpenseAccount")]
        public int? ExpenseAccountId { get; set; }
        public Account ExpenseAccount { get; set; }

        public State State { get; set; }

        public bool Processed { get; set; }

        public int TenantId { get; set; }
    }
}
