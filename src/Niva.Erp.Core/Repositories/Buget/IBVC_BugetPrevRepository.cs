using Abp.Domain.Repositories;
using Newtonsoft.Json;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_BugetPrevRepository : IRepository<BVC_BugetPrev, int>
    {
        void InsertBVC_BugetPrevStatus(int bugetPrevId, DateTime statusDate, BVC_Status state);
        void BVC_PrevAmortizariMF(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        void BVC_PrevAmortizariCA(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        void BVC_PrevPAAP(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        void BVC_CashFlowPAAP(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        //void BVC_PrevActive(List<ActiveBugetBVCDto> activeBugetList, int bugetPrevId);
        void BVC_PrevContributii(int bugetPrevId);
        void BVC_PrevCreante(int bugetPrevId);
        void BVC_CashFlowContributii(int bugetPrevId);
        void BVC_CashFlowCreante(int bugetPrevId);
        void BVC_PrevIncasari(int bugetPrevId, BVC_BugetPrevContributieTipIncasare tipIncasare);
        void BVC_CashFlowIncasari(int bugetPrevId, BVC_BugetPrevContributieTipIncasare tipIncasare);
        void BVC_PrevSalarizare(SalarizareBVCDto salarizareBVC, int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        void BVC_CashFlowSalarizare(SalarizareCFDto salarizareCF, int bugetPrevId, List<VenitRepartizProc> repartizareVenituri);
        void BVC_PrevTotaluri(int bugetPrevId);
        void BVC_PrevCheltuieli(int bugetPrevId);
        void BVC_CashFlowCheltuieli(int bugetPrevId);
        public List<BugetPrevAllDepartmentsDto> GetBugetPrevDetails(int? departamentId, int bugetPrevId, string month);
        void DesfacFormula(string formula, out List<string> semneList, out List<int> codRandList);
        List<BugetPrevazutModel> GetBugetPrevValues(BVC_FormRand rand, DateTime dataStart, DateTime dataEnd, int bugetPrevId);
        List<BugetPrevazutModel> ComputePrevazutTrimestrial(List<BugetPrevazutModel> prevazutList);
        List<BugetPrevazutModel> ComputePrevazutLunar(List<BugetPrevazutModel> randuriList);
        List<BugetPrevazutModel> ComputePrevazutTotal(List<BugetPrevazutModel> randuriList);
    }

    public class ActiveBugetBVCDto
    {
        public string idplasament { get; set; }
        public DateTime data { get; set; }
        public decimal valoarePlasament { get; set; }
        public decimal valoareDobandaLuna { get; set; }
        public decimal valoareDobandaCumulataPanaLaLuna { get; set; }
        public string tipPlasament { get; set; }
        public string tipFond { get; set; }
        public string moneda { get; set; }
        public DateTime maturityDate { get; set; }
        public DateTime startDate { get; set; }
        public decimal procentDobanda { get; set; }
    }

    public class ActiveBugetPrepareAddDto
    {
        public int ActivityType { get; set; }
        public DateTime DataLuna { get; set; }
        public decimal Valoare { get; set; }
        public int? RandCheltuialaId { get; set; }

    }

    public class ActiveCashFlowPrepareAddDto
    {
        public int ActivityType { get; set; }
        public DateTime DataOper { get; set; }
        public DateTime DataLuna { get; set; }
        public decimal Valoare { get; set; }
        public int? RandCheltuialaId { get; set; }
    }

    public class ActiveBugetCalcTotalDto
    {
        public int RandId { get; set; }
        public int ActivityType { get; set; }
        public DateTime DataLuna { get; set; }
        public decimal Valoare { get; set; }
        public int DepartamentId { get; set; }
        public bool Calculat { get; set; }
    }

    public class SalarizareBVCDto
    {
        public int Anul { get; set; }
        public int Versiune { get; set; }
        public List<SalarizareBVCLuniDto> Luni { get; set; }
    }

    public class SalarizareBVCLuniDto
    {
        public int Luna { get; set; }
        public decimal CheltIndemnCS { get; set; }
        public decimal cheltFdSalarii { get; set; }
        public decimal cheltContrib { get; set; }
        public decimal cheltFdSocial { get; set; }
    }

    public class SalarizareCFDto
    {
        public int Anul { get; set; }
        public int Versiune { get; set; }
        public List<SalarizareCFLuniDto> Luni { get; set; }
    }

    public class SalarizareCFLuniDto
    {
        public int Luna { get; set; }
        public DateTime DataPlatii { get; set; }
        public string TipPlata { get; set; }
        public decimal Valoare { get; set; }
    }

    public class VenitRepartizProc
    {
        public int ActivityTypeId { get; set; }
        public decimal ProcRepartizat { get; set; }
    }

    public class BugetPrevAllDepartmentsDto
    {
        public int FormRandId { get; set; }
        public string Descriere { get; set; }
        public int OrderView { get; set; }
        public decimal Valoare { get; set; }
        public bool Validat { get; set; }
    }

    public class BugetPrevazutModel
    {
        public int Id { get; set; }
        public int TipRand { get; set; }
        public int OrderView { get; set; }
        public string DenumireRand { get; set; }
        public DateTime DataLuna { get; set; }  
        public decimal Valoare { get; set; }
        public int ActivityTypeId { get; set; }
        public int An { get; set; }
    }

    public class SalarizareCFRealizatiDto
    {
        [JsonProperty("dataPlatii")]
        public DateTime DataPlatii { get; set; }
        [JsonProperty("tipPlata")]
        public string TipPlata { get; set; }
        [JsonProperty("valoare")]
        public decimal Valoare { get; set; }
    }
}
