using System;

namespace Niva.Erp.SectoareBnr.Dto
{
    public class BNR_RaportareDto
    {
        public int Id { get; set; }
        public int SavedBalanceId { get; set; }
        public DateTime SavedDate { get; set; }
        public string BalanceName { get; set; }
        public int AnexaId { get; set; }
        public int TenantId { get; set; }
    }

    public class BNR_RaportareRowDto
    {
        public int Id { get; set; }
        public int BNR_RaportareId { get; set; }
        public int AnexaDetailId { get; set; }
        public string AnexaDetailName { get; set; }
        public int? SectorId { get; set; }
        public string SectorName { get; set; }
        public decimal Valoare { get; set; }
        public int OrderView { get; set; }
    }

    public class BNR_Detalii
    {
        public string AnexaDetailName { get; set; }
        public string SectorName { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public int OrderView { get; set; }

    }

    public class BNR_RaportareRowDetailsDto
    {
        public int Id { get; set; }
        public int BNR_RaportareRandId { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
    }

    public class PlasamentBNRDto
    {
        public string idplasament { get; set; }
        public string tipPlasament { get; set; }
        public DateTime startDate { get; set; }
        public DateTime dataEmisiune { get; set; }
        public DateTime maturityDate { get; set; }
        public decimal durataAn
        {
            get
            {
                return (maturityDate.AddDays(-1).Year - startDate.Year - 1) +
                        (((maturityDate.AddDays(-1).Month > startDate.Month) ||
                        ((maturityDate.AddDays(-1).Month == startDate.Month) && (maturityDate.AddDays(-1).Day >= startDate.Day))) ? 1 : 0);
            }
        }

        public decimal durataAnEmis
        {
            get
            {
                //return (maturityDate.AddDays(-1).Year - dataEmisiune.Year - 1) +
                //        (((maturityDate.AddDays(-1).Month > dataEmisiune.Month) ||
                //        ((maturityDate.AddDays(-1).Month == dataEmisiune.Month) && (maturityDate.AddDays(-1).Day >= dataEmisiune.Day))) ? 1 : 0);
                return (decimal)((maturityDate - dataEmisiune).TotalDays / 365);
            }
        }

        public decimal valoarePlasament { get; set; }
        public string codStatistica { get; set; }

    }
}
