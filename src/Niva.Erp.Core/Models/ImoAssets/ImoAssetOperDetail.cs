using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Niva.Erp.Models.ImoAsset
{
    public class ImoAssetOperDetail : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("ImoAssetItem")]
        public int ImoAssetItemId { get; set; }
        public ImoAssetItem ImoAssetItem { get; set; }

        public int Quantity { get; set; }

        public Decimal InvValueModif { get; set; }

        public Decimal FiscalValueModif { get; set; }

        public int DurationModif { get; set; }

        public Decimal DeprecModif { get; set; }

        public Decimal FiscalDeprecModif { get; set; }

        [ForeignKey("ImoAssetOper")]
        public int ImoAssetOperId { get; set; }
        public ImoAssetOper ImoAssetOper { get; set; }

        [ForeignKey("InvoiceDetail")]
        public int? InvoiceDetailId { get; set; }
        public InvoiceDetails InvoiceDetail { get; set; }

        [ForeignKey("OldAssetAccount")]
        public int? OldAssetAccountId { get; set; }
        public Account OldAssetAccount { get; set; }

        [ForeignKey("OldAssetAccountInUse")]
        public int? OldAssetAccountInUseId { get; set; }
        public Account OldAssetAccountInUse { get; set; }

        [ForeignKey("OldDepreciationAccount")]
        public int? OldDepreciationAccountId { get; set; }
        public Account OldDepreciationAccount { get; set; }

        [ForeignKey("OldExpenseAccount")]
        public int? OldExpenseAccountId { get; set; }
        public Account OldExpenseAccount { get; set; }

        [ForeignKey("NewAssetAccount")]
        public int? NewAssetAccountId { get; set; }
        public Account NewAssetAccount { get; set; }

        [ForeignKey("NewAssetAccountInUse")]
        public int? NewAssetAccountInUseId { get; set; }
        public Account NewAssetAccountInUse { get; set; }

        [ForeignKey("NewDepreciationAccount")]
        public int? NewDepreciationAccountId { get; set; }
        public Account NewDepreciationAccount { get; set; }

        [ForeignKey("NewExpenseAccount")]
        public int? NewExpenseAccountId { get; set; }
        public Account NewExpenseAccount { get; set; }  

        public State State { get; set; }
      
        public int TenantId { get; set; }
    }
}
