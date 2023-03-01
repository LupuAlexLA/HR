using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public class ImoAssetClassCodeListDDDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }
    }

    public class ImoAssetClassCodeListDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Code { get; set; }

        [StringLength(1000)]
        public string AssetAccount { get; set; }

        [StringLength(1000)]
        public string DepreciationAccount { get; set; }

        [StringLength(1000)]
        public string ExpenseAccount { get; set; }

        [StringLength(1000)]
        public string ClassCodeParrent { get; set; }

        public int DurationMin { get; set; }

        public int DurationMax { get; set; }
    }

    public class ImoAssetClassCodeEditDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Code { get; set; }

        public int? AssetAccountId { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int? ExpenseAccountId { get; set; }

        public int? ClassCodeParrentId { get; set; }

        public int DurationMin { get; set; }

        public int DurationMax { get; set; }
    }
}
