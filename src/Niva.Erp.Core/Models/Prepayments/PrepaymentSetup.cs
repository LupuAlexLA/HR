using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.PrePayments
{
    public class PrepaymentsDurationSetup : AuditedEntity<int>, IMustHaveTenant
    {
        public PrepaymentType PrepaymentType { get; set; }

        public PrepaymentDurationCalc PrepaymentDurationCalc { get; set; }

       
        public int TenantId { get; set; }
    }

    public class PrepaymentsDecDeprecSetup : AuditedEntity<int>, IMustHaveTenant
    {
        public PrepaymentType PrepaymentType { get; set; }

        public int DecimalAmort { get; set; }
        public int TenantId { get; set; }
    }

    public enum PrepaymentDurationCalc : int
    {
        Lunar,
        Zilnic
    }


}
