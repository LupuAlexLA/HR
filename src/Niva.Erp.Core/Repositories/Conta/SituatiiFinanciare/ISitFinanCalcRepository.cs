using Abp.Domain.Repositories;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories.Conta.Lichiditate;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Conta.SituatiiFinanciare
{
    public interface ISitFinanCalcRepository : IRepository<SitFinanCalc, int>
    {
        void DeleteCalcRapoarte(int balanceId);

        void DeleteCalcRaport(int balanceId, int raportId);

        void CalcRapoarte(int balantaId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues);

        void CalcRaport(int balanceId, int raportId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues);

        void CalculRaportTotalOld(int balanceId, int raportId);

        void CalcRaportNotaOld(int balantaId, int raportId);

        string GetDateFromFormula(string formula, DateTime rapDate);

        string GetCalcColumnText(string formula, int balanceId, int raportId);

        List<SitFinanCalcDetailTemp> GetSitFinanRapRows(int columnId, int balanceId, int reportId);

        void ComputeReportDetails(DateTime startDate, DateTime endDate, bool isDailyBalance, int raportId, SitFinanRapConfigCol column, out SitFinanRaport ret);
    }


    public class SitFinanCalcDetailTemp
    {
        public int Id { get; set; }
        public string RowName { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string ElementDet { get; set; }
        public decimal Val { get; set; }
        public int OrderView { get; set; }
    }

    public class SitFinanExternalValues
    {
        public SitFinanExternalValuesType ValueType { get; set; }
        public decimal Value { get; set; }
    }

    public enum SitFinanExternalValuesType
    {
        NrMediuSalariatiPerioada,
        NrSalariatiData
    }
}
