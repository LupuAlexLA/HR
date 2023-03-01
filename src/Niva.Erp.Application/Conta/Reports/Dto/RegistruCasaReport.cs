using Niva.Erp.Economic.Dto;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class RegistruCasaModel
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime StartDate { get; set; }
        public Decimal SoldPrec { get; set; }
        public Decimal SoldCurr { get; set; }
        public string CurrencyName { get; set; }
        public int TenantId { get; set; }
        public List<DispositionListDto> Dispositions { get; set; }

    }

}
