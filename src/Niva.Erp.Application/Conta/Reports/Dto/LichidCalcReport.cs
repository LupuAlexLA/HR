using Niva.Erp.Conta.Lichiditate.Dto;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class LichidCalcModel
    {
        public string Description { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime CalcDate { get; set; }
        public List<LichidCalcListDetDto> LichidCalcList { get; set; }
        public List<LichidCalcCurrListDto> LichidCalcCurrList { get; set; } 
    }
}
