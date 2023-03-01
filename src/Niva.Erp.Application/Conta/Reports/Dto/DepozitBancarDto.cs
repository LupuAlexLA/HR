using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class DepozitBancarDto
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime BalanceDate { get; set; }

        public string ReportName { get; set; }
        public IList<DepozitBancarDetailDto> Details { get; set; }
        public decimal TotalDepozit { get; set; }
        public decimal ConturiCurente { get; set; }
        public decimal TotalDepozitConturi { get; set; }
    }

    public class DepozitBancarDetailDto
    {
        public string IdPlasament { get; set; }    
        public string DenumireBanca { get; set; }
        public decimal SumaInvestita { get; set; }
        public int MaximRezidual { get; set; }
        public decimal Dobanda { get; set; }

    }
}
