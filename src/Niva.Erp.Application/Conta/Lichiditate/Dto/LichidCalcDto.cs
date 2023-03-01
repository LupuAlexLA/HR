using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Lichiditate.Dto
{
    public class LichidSavedBalanceListDto
    {
        public int Id { get; set; }
        public DateTime SavedBalanceDate { get; set; }
        public string SavedBalanceDesc { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcInitDto
    {
        public List<LichidCalcSavedBalancetDto> LichidCalcSavedBalanceList { get; set; }
        public bool ShowLichidType { get; set; }    
        public bool ShowDetails { get; set; }
        public bool ShowLichidCalcSavedBalanceList { get; set; }
        public bool ShowLichidCalcCurrList { get; set; }
        public bool ShowLichidCalcList { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcSavedBalancetDto
    {
        public int SavedBalanceId { get; set; }
        public DateTime SavedBalanceDate { get; set; }
        public string SavedBalanceDesc { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcListDetDto
    {
        public int LichidConfigId { get; set; }
        public int LichidCalcId { get; set; }
        public string Descriere { get; set; }
        public bool RandTotal { get; set; } 
        public decimal ValoareBanda1 { get; set; }
        public decimal ValoareBanda2 { get; set; }
        public decimal ValoareBanda3 { get; set; }
        public decimal ValoareBanda4 { get; set; }
        public decimal ValoareBanda5 { get; set; }
        public decimal TotalActualiz { get; set; }
        public decimal TotalInit { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcDetDto
    {
        public int LichidCalcId { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
    }

    public class LichidCalcCurrListDto
    {
        public int LichidConfigId { get; set; }
        public int LichidCalcId { get; set; }
        public string Descriere { get; set; }
        public bool RandTotal { get; set; }
        public decimal ValoareRON { get; set; }
        public decimal ValoareEUR{ get; set; }
        public decimal ValoareUSD { get; set; }
        public decimal ValoareGBP { get; set; }
        public decimal ValoareAlteMonede { get; set; }
        public decimal TotalActualiz { get; set; }
        public decimal TotalInit { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidCalcCurrDetDto
    {
        public int LichidCalcCurrId { get; set; }   
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
    }
}
