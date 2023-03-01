using System;
using System.Collections.Generic;

namespace Niva.Erp.Buget
{
    public class BugetRealizatDto
    {
        public int Id { get; set; } 
        public int? BVC_BalRealizatId { get; set; } 
        public DateTime SavedBalanceDate { get; set; }
        public string SavedBalanceDescription { get; set; }
        public int BVC_Tip { get; set; }
        public string BVC_TipStr { get; set; }
    }

    public class RealizatAddDispoDto
    {
        public int Id { get; set; }

        public string Descriere { get; set; }
    }

    public class BugetRealizatRowDto
    {
        public int Id { get; set; }
        public string DenumireRand { get; set; }
        public bool Bold { get; set; }
        public decimal ValoareCuReferat { get; set; }
        public decimal ValoareFaraReferat { get; set; }

    }

    public class BugetRealizatSavedBalanceDateDto   
    {
        public int Id { get; set; }
        public int? BalRealizatId { get; set; }  
        public string Descriere { get; set; }
    }

    public class BugetBalRealizatSavedBalanceDateDto
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
    }


    public class BugetRealizatRowDetailDto
    {
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public string CurrencyName { get; set; }
    }

    public class BVC_RealizatCalcDto
    {
        public List<BVC_RealizatRandDto> RanduriCalc { get; set; }
        public List<BVC_RealizatExceptiiDto> Exceptii { get; set; }
    }

    public class BVC_RealizatRandDto
    {
        public int Id { get; set; }
        public int BVC_RealizatId { get; set; }
        public int BVC_FormRandId { get; set; }
        public virtual decimal ValoareCuReferat { get; set; }
        public virtual decimal ValoareFaraReferat { get; set; }
        public List<BVC_RealizatRandDetailsDto> BVC_RealizatRandDetails { get; set; }
    }

    public class BVC_RealizatRandDetailsDto
    {
        public int Id { get; set; }
        
        public virtual string Descriere { get; set; }

        public virtual decimal Valoare { get; set; }
        public int CurrencyId { get; set; }
    }

    public class BVC_RealizatExceptiiDto
    {
        public int Id { get; set; }

        public virtual string Descriere { get; set; }

        public virtual decimal Valoare { get; set; }
    }

    public class BugetRealizatCalcTotalDto
    {
        public int RandId { get; set; }
        public decimal Valoare { get; set; }
        public bool Calculat { get; set; }
    }

    public class BVC_BalRealizatRandDto
    {
        public int Id { get; set; }
        public int BVC_BalRealizatId { get; set; }
        public int BVC_FormRandId { get; set; }
        public int ActivityTypeId { get; set; }
        public virtual decimal Valoare { get; set; }
        public List<BVC_BalRealizatRandDetailsDto> BVC_BalRealizatRandDetails { get; set; }
    }

    public class BVC_BalRealizatRandDetailsDto
    {
        public int Id { get; set; }

        public virtual string Descriere { get; set; }

        public virtual decimal Valoare { get; set; }
    }

    public class BVC_BalRealizatCalcDto 
    {
        public List<BVC_BalRealizatRandDto> RanduriCalc { get; set; }
        public List<BVC_RealizatExceptiiDto> Exceptii { get; set; }
    }

    public class BugetBalRealizatRowDto
    {
        public int Id { get; set; }
        public string DenumireRand { get; set; }
        public bool Bold { get; set; }
        public decimal Valoare { get; set; }
        public string ActivityTypeName { get; set; }
        public int ActivityTypeId { get; set; } 
    }

    public class BugetBalRealizatRowDetailDto
    {
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public string CurrencyName { get; set; }
    }

    public class BugetBalRealizatCalcTotalDto
    {
        public int RandId { get; set; }
        public decimal Valoare { get; set; }
        public bool Calculat { get; set; }  
        public int ActivityTypeId { get; set; } 
    }
}