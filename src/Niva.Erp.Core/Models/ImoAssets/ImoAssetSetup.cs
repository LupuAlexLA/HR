using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Niva.Erp.Models.ImoAsset
{
    public class ImoAssetSetup : AuditedEntity<int>, IMustHaveTenant
    {
        public bool ReserveDepreciation { get; set; }

     

        public State State { get; set; }
      
        public int TenantId { get; set; }
    }
}
