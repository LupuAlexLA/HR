using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BVC_Report
    {
        public string Titlu { get; set; }
        public int AnBuget { get; set; }
        public string Parameters { get; set; }
        public string AppClientName { get; set; }
        public string Frecventa { get; set; }
        public List<BugetPrevazutModel> BugetDetails { get; set; }
    }

    public class BVC_Realizat_Report
    {
        public string Titlu { get; set; }
        public string AppClientName { get; set; }
        public int An { get; set; }
        public DateTime OperationDate { get; set; }
        public List<BVC_RealizatDetail> Details { get; set; }
    }

    public class BVC_RealizatDetail
    {
        public string DenumireRand { get; set; }
        public decimal ValoareRealizat { get; set; }
        public decimal ValoarePrevazut { get; set; }
        public decimal ValoareDiferenta { get; set; }
        public decimal Procent { get; set; }
    }

    public class BVC_BalRealizat_Report
    {
        public string Titlu { get; set; }
        public string AppClientName { get; set; }
        public string TipBuget { get; set; }
        public int An { get; set; }
        public int ActivityTypeName { get; set; }
        public DateTime OperationDate { get; set; }

        [DefaultValue(false)]
        public bool HideTotalVenituri { get; set; }

        public List<BVC_BalRealizatDetail> Details { get; set; }
        public List<VenitDinFond> VenitDinFonduri { get; set; }
    }

    public class BVC_BalRealizatDetail
    {
        public string DenumireRand { get; set; }
        public decimal ValoareBalRealizat { get; set; }
        public decimal ValoarePrevazut { get; set; }
        public decimal ValoareDiferenta { get; set; }
        public decimal Procent { get; set; }
        public decimal? Aprobat { get; set; }
        public decimal? RamasDeRealizat { get; set; }
    }

    public class VenitDinFond
    {
        public string DenumireRand { get; set; }
        public decimal ValoareFondBalRealizat { get; set; }
        public decimal ValoareFondPrevazut { get; set; }
        public decimal ValoareFondDiferenta { get; set; }
        public decimal Procent { get; set; }
        public decimal? AprobatFond { get; set; }
        public decimal? FondRamasDeRealizat { get; set; }
    }
}