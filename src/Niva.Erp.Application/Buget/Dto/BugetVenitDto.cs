using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Buget
{
    public class BugetTitluriDto
    {
        public string IdPlasament { get; set; }
        public string TipPlasamentStr { get; set; }
        public VenitType VenitType { get; set; }
        public string VenitTypeStr { get; set; }
        public BVC_PlasamentType TipPlasament { get; set; }
        public virtual string TipFond { get; set; }
        public virtual int ActivityTypeId { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public decimal ValoarePlasament { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal ProcentDobanda { get; set; }
    }

    public class BugetTitluriDDDto
    {
        public int FormularBVCId { get; set; }
        public int AnBVC { get; set; }
    }

    public class BugetTitluriViewDto
    {
        public int Id { get; set; }
        public string IdPlasament { get; set; }
        public BVC_PlasamentType TipPlasament { get; set; }
        public string TipPlasamentStr { get; set; }
        public VenitType VenitType { get; set; }
        public string VenitTypeStr { get; set; }
        public virtual int ActivityTypeId { get; set; }
        public virtual string ActivityType { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public decimal ValoarePlasament { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal ProcentDobanda { get; set; }
        public bool Selectat { get; set; }
        public bool Reinvestit { get; set; }
    }

    public class BugetTitluriBVCViewList
    {
        public int Id { get; set; }
        public DateTime DataDobanda { get; set; }
        public decimal ValoarePlasament { get; set; }
        public decimal DobandaLuna { get; set; }
        public decimal DobandaCumulataPrec { get; set; }
    }

    public class BugetTitluriCFViewList
    {
        public int Id { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal ValoarePlasament { get; set; }
        public decimal DobandaTotala { get; set; }
        public DateTime DataReinvestire { get; set; }
        public decimal SumaReinvestita { get; set; }
    }

    public class BugetReinvest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class BugetReinvestIncasari
    {
        public int Id { get; set; }
        public string IdPlasament { get; set; }
        public string TipPlasamentStr { get; set; }
        public BVC_PlasamentType TipPlasament { get; set; }
        public virtual string ActivityType { get; set; }
        public virtual int ActivityTypeId { get; set; }
        public VenitType VenitType { get; set; }
        public string VenitTypeStr { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal ValoarePlasament { get; set; }
        public decimal DobandaTotala { get; set; }
        public decimal ValoareIncasata { get; set; }
        public decimal ValoareIncasataRon { get; set; }
        public DateTime DataReinvestire { get; set; }
        public decimal SumaReinvestita { get; set; }
        public virtual List<BVC_VenitTitluCFReinvDto> BVC_VenitTitluCFReinv { get; set; }
    }

    public class TabelIncasari
    {
        public DateTime Date { get; set; }
        public bool Reinvestit { get; set; }
        public string VenitTypeStr { get; set; }

        public string IdPlasament { get; set; }

        public virtual string ActivityType { get; set; }

        public string Currency { get; set; }

        public string TipPlasamentStr { get; set; }
        public decimal ValoareIncasata { get; set; }
        public decimal ValoareReinvestita { get; set; }

        public decimal ProcDobanda { get; set; }

    }

    public class BVC_VenitTitluCFReinvDto
    {
        public int Id { get; set; }
        public int BVC_VenitTitluCFId { get; set; }
        public DateTime DataReinvestire { get; set; }
        public decimal SumaReinvestita { get; set; }
        public decimal ProcDobanda { get; set; }
        public bool MainValue { get; set; }
        public decimal SumaIncasata { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal CursValutar { get; set; }    

        public bool Delete { get; set; }
    }

    public class BugetReinvestPlati
    {
        public int Id { get; set; }
        public DateTime DataPlatii { get; set; }
        public string Descriere { get; set; }
        public decimal ValoarePlata { get; set; }
        public decimal ValoarePlatita { get; set; }
    }

    public class BugetTitluriCFViewCurrenciesList
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public decimal ValoareIncasata { get; set; }    
        public decimal ValoareReinvestita { get; set; }
        public decimal ValoarePlasament { get; set; }
        public virtual string ActivityType { get; set; }
        public virtual int ActivityTypeId { get; set; }
        public decimal ValoareIncasataLei { get; set; }
        public decimal ValoareReinvestitaLei { get; set; }
        public decimal ValoarePlasamentLei { get; set; }

    }
}
