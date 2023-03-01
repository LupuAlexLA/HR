using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class CursValutarBNRModel
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public List<CursValutarBNRDetails> Details { get; set; }
    }

    public class CursValutarBNRDetails
    {
        public int? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal ValoarePrecedenta { get; set; }
        public decimal ValoareCurenta { get; set; }
        public decimal Variatie { get; set; }

    }

}
