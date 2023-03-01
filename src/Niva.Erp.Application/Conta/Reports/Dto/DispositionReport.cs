using System;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class DispositionModel
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public string HeaderParams { get; set; }
        public string FooterParams { get; set; }
        public decimal Sum { get; set; }
        public string SumInWords { get; set; }
        public string Description { get; set; } // pentru Motivul incasarii/platii
        public string Name { get; set; }
        public string CI { get; set; }
        public string Serie_CI { get; set; }
        public string Nr_CI { get; set; }

        public int DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? PlataSum { get; set; }
        public string PlataCurrencyCode { get; set; }
        public int? NrChitanta { get; set; }
        public string NumePrenume { get; set; }
        public string TipDoc { get; set; }
        public string ActIdentitate { get; set; }

    }
}
