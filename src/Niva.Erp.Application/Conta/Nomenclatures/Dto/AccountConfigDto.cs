using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Conta.Nomenclatures
{
    public class AccountConfigDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public virtual string AccountRad { get; set; }
        public string Sufix { get; set; }
        public bool IncludPunct { get; set; }
        public bool IncludMoneda { get; set; }
        public bool IncludId1 { get; set; }
        public bool ExactAccount { get; set; }
        public int? NrCaractere { get; set; }
        public bool ThirdPartyAccount { get; set; }
        public DateTime ValabilityDate { get; set; }
        public virtual string Description { get; set; }
        public virtual int? ImoAssetStorageId { get; set; }
        public virtual string ImoAssetStorage { get; set; }
        public int AppClientId { get; set; }
        public virtual int? ScopDeplasareType { get; set; }
        public virtual int? DiurnaType { get; set; }
        public virtual int? ActivityTypeId { get; set; }
        public virtual string ActivityType { get; set; }
    }
}
