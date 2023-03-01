using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic.Casierii
{
    public enum OperationType
    {
        Plata,
        Incasare,
        Depunere,
        Retragere,
        SoldInitial
    }

    public enum DispositionState
    {
        Finalizata,
        Validata, 
        Nevalidata,
        Anulata
    }

    public class Disposition : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("ThirdParty")]
        public virtual int? ThirdPartyId { get; set; }
        public virtual Person ThirdParty { get; set; }

        public virtual OperationType OperationType { get; set; }

        public virtual int DispositionNumber { get; set; }
        public virtual DateTime DispositionDate { get; set; }
        public virtual Decimal Value { get; set; }

        public virtual int? InvoiceElementsDetailsId { get; set; }
        public virtual InvoiceElementsDetails InvoiceElementsDetails { get; set; }

        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public virtual string Description { get; set; }

        [ForeignKey("DocumentType")]
        public virtual int? DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual string DocumentNumber { get; set; }
        public virtual DateTime? DocumentDate { get; set; }
        public virtual List<DispositionInvoice> DispositionInvoices { get; set; }  
        public virtual Decimal SumOper { get; set; }

        [ForeignKey("BankAccount")]
        public virtual int? BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

        [ForeignKey("Operation")]
        public virtual int? OperationId { get; set; }
        public virtual Operation Operation { get; set; }

        public int TenantId { get; set; }

        public virtual State State { get; set; }
        public int? NrChitanta { get; set; }

        public virtual string NumePrenume { get; set; }
        public virtual string TipDoc { get; set; }
        public virtual string ActIdentitate { get; set; }

    }
}
