using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public class TranzactiiFonduri : AuditedEntity<int>, IMustHaveTenant
    {
        public int Numar { get; set; }
        public DateTime Data { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public decimal Suma { get; set; }
        public string Fel_doc { get; set; }
        public string Nr_doc { get; set; }
        public DateTime Data_doc { get; set; }
        public string Explicatie { get; set; }
        public string Tip { get; set; }
        public string Nota { get; set; }

        public int TenantId { get; set; }
    }

    
}
