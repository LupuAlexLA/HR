 
namespace Niva.Erp.Models.ImoAsset
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    
    using Niva.Erp.Models.Conta;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class ImoAssetClassCode : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Code { get; set; } 
        [ForeignKey("AssetAccount")]
        public int? AssetAccountId { get; set; }
        public Account AssetAccount { get; set; }

        [ForeignKey("DepreciationAccount")]
        public int? DepreciationAccountId { get; set; }
        public Account DepreciationAccount { get; set; }

        [ForeignKey("ExpenseAccount")]
        public int? ExpenseAccountId { get; set; }
        public Account ExpenseAccount { get; set; }

        [ForeignKey("ClassCodeParrent")]
        public int? ClassCodeParrentId { get; set; }
        public ImoAssetClassCode ClassCodeParrent { get; set; }

        public int DurationMin { get; set; }

        public int DurationMax { get; set; }

        public State State { get; set; } 
        public int TenantId { get; set; }
    }
}

