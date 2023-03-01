using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectListDto
    {
        public virtual int Id { get; set; }

        [StringLength(1000)]
        public virtual string Name { get; set; }
        public virtual int InventoryNr { get; set; }

        public virtual decimal PriceUnit { get; set; }

        public virtual decimal InventoryValue { get; set; }

        public DateTime OperationDate { get; set; }
        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentType { get; set; }

        public string CategoryName { get; set; }
        public virtual int CategoryId { get; set; }
        public string ThirdParty { get; set; }

        public string Invoice { get; set; }

        public bool Processed { get; set; }
    }

    public class InvObjectAddDirectDto
    {
        public int Id { get; set; }

        public DateTime OperationDate { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public InvObjectOperType OperationType { get; set; }

        public int? InvoiceId { get; set; }

        public bool FinishAdd { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public int InventoryNr { get; set; }

        public int Quantity { get; set; }

        public decimal InventoryValue { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int? InvCategoryId { get; set; }

        public int? InvAccountId { get; set; }

        public int? ExpenseAccountId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public bool Processed { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }

        public string ThirdPartyName { get; set; }
    }

    public class InvObjectAddDto
    {
        public DateTime OperationDate { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public InvObjectOperType OperationType { get; set; }

        public int? InvoiceId { get; set; }

        public IList<InvObjectAddInvoiceDetailDto> InvoiceDetail { get; set; }

        public List<InvObjectAddDetailDto> InvObjects { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool ShowForm3 { get; set; }

        public bool FinishAdd { get; set; }
    }

    public class InvObjectAddInvoiceDetailDto
    {
        public int InvoiceId { get; set; }
        public int? ActivityTypeId { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string InvObjectName { get; set; }

        public int Quantity { get; set; }

        public decimal InvValue { get; set; }

        public int? StorageInId { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }
    }

    public class InvObjectAddDetailDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public int InventoryNr { get; set; }

        public int Quantity { get; set; }

        public decimal InventoryValue { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int? InvCategoryId { get; set; }

        public int? InvObjectAccountId { get; set; }

        public int? ExpenseAccountId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }
    }


    public class InvObjectsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InventoryNr { get; set; }
    }
}
