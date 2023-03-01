using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.ImoAsset;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{

    public partial class AccountConfig : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string Symbol { get; set; }

        //[ForeignKey("SyntheticAccount")]
        //public int SyntheticAccountId { get; set; }
        //public virtual Account SyntheticAccount { get; set; }

        [StringLength(1000)]
        public virtual string AccountRad { get; set; }

        [StringLength(1000)]
        public string Sufix { get; set; }
        public bool IncludPunct { get; set; }
        public bool IncludMoneda { get; set; }
        public bool IncludId1 { get; set; }
        public bool ExactAccount { get; set; }
        public int? NrCaractere { get; set; }
        public DateTime ValabilityDate { get; set; }

        [StringLength(1000)]
        public virtual string Description { get; set; }
        public bool ThirdPartyAccount { get; set; }
        public virtual State Status { get; set; }

        [ForeignKey("ImoAssetStorage")]
        public int? ImoAssetStorageId { get; set; }
        public virtual ImoAssetStorage ImoAssetStorage { get; set; }  
        public int TenantId { get; set; }

        public virtual ScopDeplasareType? ScopDeplasareType { get; set; }
        public virtual DiurnaType? DiurnaType { get; set; }

        [ForeignKey("ActivityType")]
        public virtual int? ActivityTypeId { get; set; } // tip fond
        public virtual ActivityType ActivityType { get; set; }
    }
}
