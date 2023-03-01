using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr.Dto
{
   public class BNR_BalanceListDto
    {
        public int Id { get; set; }
        public DateTime BalanceDate { get; set; }
        public string BalanceDesc { get; set; }

        public int TenantId { get; set; }
    }

    public class BNR_SectorCalculatListDto
    {
        public int Id { get; set; }
        public int BalanceId { get; set; }
        public DateTime BalanceDate { get; set; }
        public string BalanceDesc { get; set; }
        public int TenantId { get; set; }
        public bool AllowCompute { get; set; }
      //  public bool AllowCompute { get { return (BNR_Rows.All(f => f.IsSectored == true)); } }
        public List<BNR_SectorRowCalcListDto> BNR_Rows { get; set; }

    }

    public class BNR_SectorRowCalcListDto
    {
        public int BalanceId { get; set; }  
        public int Id { get; set; }
        public string NrCrt { get; set; }
        public int AnexaId { get; set; }
        public int AnexaDetailId { get; set; }
        public bool Sectorizare { get; set; }
        public string Denumire { get; set; }
        public int TenantId { get; set; }
        public bool IsSectored { get { return (BNR_RowDetails.All(f => f.SectorId != null) || !Sectorizare); } }
        public List<BNR_SectorRowCalValDto> BNR_RowDetails { get; set; }    
    }

    public class BNR_SectorRowCalValDto
    {
        public int RowId { get; set; }
        public int? SectorId { get; set; }
        public string SectorCod { get; set; }
        public string Denumire { get; set; }
        public decimal Value { get; set; }
        //public List<BNR_SectorDetailDto> Details { get; set; }
    }

    public class BNR_SectorDetailDto
    {
        public int SavedBalanceId { get; set; }
        public int AnexaDetailId { get; set; }
        public string Account { get; set; }
        public string Denumire { get; set; }
        public int? SectorId { get; set; }
        public int TenantId { get; set; }
        public decimal SoldDb { get; set; }
        public decimal SoldCr { get; set; }
        public decimal Value { get; set; }

    }
}
