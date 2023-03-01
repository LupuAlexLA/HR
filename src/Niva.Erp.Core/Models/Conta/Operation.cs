 
namespace Niva.Erp.Models.Conta
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.AutoOperation;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class Operation : AuditedEntity<int>, IMustHaveTenant
    {
        [Required]
        public virtual DateTime OperationDate { get; set; }

        public virtual string DocumentNumber { get; set; }

        public virtual DateTime DocumentDate { get; set; }

        public virtual bool ClosingMonth { get; set; }

        public virtual IList<OperationDetails> OperationsDetails { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }

        [ForeignKey("OperationTypes")]
        public int? OperationTypeId { get; set; }
        public virtual OperationTypes OperationType { get; set; } // tip operatie contabila

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public virtual OperationStatus OperationStatus { get; set; }

        public bool ExternalOperation { get; set; } // true = operatie introdusa din alt modul => nu poate fi modificata din modulul operatii

        public virtual State State { get; set; }

        [ForeignKey("OperationDefinition")]
        public int? OperationDefinitionId { get; set; }
        public virtual OperationDefinition OperationDefinition { get; set; }

        [ForeignKey("OperGenerate")]
        public int? OperGenerateId { get; set; }
        public virtual OperGenerate OperGenerate { get; set; }

        [ForeignKey("OperationParent")]
        public int? OperationParentId { get; set; }
        public virtual Operation OperationParent { get; set; }

        public int TenantId { get; set; }
    }

    public enum OperationStatus : int
    {
        Unchecked,
        Checked,
    }

    public enum TipPerioadaSold : int
    {
        Zilnic,
        Lunar
    }
}

