using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class AnexaReportModel
    {
        public string AppClientName { get; set; }
        public int SavedBalanceId { get; set; }
        public DateTime SavedBalanceDate { get; set; }
        public List<Anexa1Model> AnexaA1Details { get; set; }
        public List<Anexa1Model> AnexaB1Details { get; set; }
        public List<Anexa2Model> Anexa2Details { get; set; }
        public List<Anexa3Model> Anexa3Details { get; set; }
        public List<Anexa4Model> Anexa4Details { get; set; }    
    }

    public class Anexa1Model
    {
        public int AnexaDetailId { get; set; }
        public int OrderView { get; set; }
        public string NrCrt { get; set; }
        public string CodRand { get; set; }

        [Display(Name = "Banca centrala")]
        public decimal Sector_S121 { get; set; }

        [Display(Name = "Societati care accepta depozite, exclusiv banca centrala (S122)")]
        public decimal Sector_S122 { get; set; }

        [Display(Name = "Administratia centrala (S1311)")]
        public decimal Sector_S1311 { get; set; }

        [Display(Name = "Administratiile locale (S1313)")]
        public decimal Sector_S1313 { get; set; }

        [Display(Name = " Administratiile de securitate sociala (S1314)")]
        public decimal Sector_S1314 { get; set; }

        [Display(Name = "Fonduri de piata monetara (S123)")]
        public decimal Sector_S123 { get; set; }
        [Display(Name = "Fonduri de investitii, altele decat fondurile de piata monetara (S124)")]

        public decimal Sector_S124 { get; set; }
        [Display(Name = "Auxiliari financiari (S126)")]

        public decimal Sector_S126 { get; set; }
        [Display(Name = "Societati de asigurare  (S128)")]

        public decimal Sector_S128 { get; set; }
        [Display(Name = "Fonduri de pensii (S129)")]

        public decimal Sector_S129 { get; set; }
        [Display(Name = "Societati nefinanciare (S11)")]

        public decimal Sector_S11 { get; set; }
        [Display(Name = "Gospodariile populatiei (S14)")]

        public decimal Sector_S14 { get; set; }
        [Display(Name = "Institutii fara scop lucrativ in serviciul gospodariilor populatiei (S15)")]

        public decimal Sector_S15 { get; set; }
        [Display(Name = "zona euro (S211)")]

        public decimal Sector_S211 { get; set; }
        [Display(Name = "zona UE - non euro (S212)")]

        public decimal Sector_S212 { get; set; }
        [Display(Name = "zona non UE (S22)")]

        public decimal Sector_S22 { get; set; }
        [Display(Name = "Alti intermediari financiari, exclusiv societatile de asigurare si fondurile de pensii (S125)")]

        public decimal Sector_S125 { get; set; }

        public string AnexaDetailName { get; set; }
        public string InstrumentFinanciar { get; set; }
        public decimal Total { get; set; }
    }

    public class Anexa2Model
    {
        public int OrderView { get; set; }
        public string NrCrt { get; set; }
        public DateTime OperationDate { get; set; } 
        public string AnexaDetailName { get; set; }
        public decimal Valoare { get; set; }
    }

    public class Anexa3Model
    {
        public int OrderView { get; set; }
        public string NrCrt { get; set; }
        public string Indicator { get; set; }
        public decimal Valoare { get; set; }
    }

    public class Anexa4Model
    {
        public int OrderView { get; set; }
        public string NrCrt { get; set; }
        public string DenumireRand { get; set; }
        public decimal ValoareCresteri { get; set; }
        public decimal ValoareReduceri { get; set; }
    }
}
