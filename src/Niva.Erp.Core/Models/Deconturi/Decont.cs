using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Deconturi
{
    public class Decont : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public DateTime DecontDate { get; set; }

        public string Description { get; set; }

        public int DecontNumber { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public int? DiurnaLegalaValue { get; set; }
        public int? DiurnaZi { get; set; }
        public int? DiurnaImpozabila { get; set; }
        public decimal? NrZile { get; set; }
        public decimal TotalDiurnaAcordata { get; set; }
        public decimal TotalDiurnaImpozabila { get; set; }

        public DecontType DecontType { get; set; }

        [ForeignKey("ThirdParty")]
        public int? ThirdPartyId { get; set; }
        public Person ThirdParty { get; set; }

        [ForeignKey("DiurnaLegala")]
        public int? DiurnaLegalaId { get; set; }
        public virtual DiurnaLegala DiurnaLegala { get; set; }

        public State State { get; set; }

        public virtual IList<InvoiceDetails> InvoiceDetails { get; set; }
        public int TenantId { get; set; }
        public virtual ScopDeplasareType ScopDeplasareType { get; set; }

        [ForeignKey("DocumentType")]
        public int? DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual int FileDocId { get; set; }
        public decimal TotalDiurnaAcordataFileDoc { get; set; }

        [ForeignKey("Operation")]
        public int? OperationId { get; set; }
        public virtual Operation Operation { get; set; }
    }

    public enum DecontType
    {
        Card,
        Casa,
        [Description("Cont personal")]
        ContPersonal
    }
}
