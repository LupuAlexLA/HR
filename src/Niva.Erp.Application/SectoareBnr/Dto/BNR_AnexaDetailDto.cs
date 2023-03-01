using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.SectoareBnr.Dto
{
    public class BNR_AnexaDetailDto
    {
        public int Id { get; set; }
        public string NrCrt { get; set; }
        public string DenumireRand { get; set; }
        public string CodRand { get; set; }
        public bool EDinConta { get; set; }
        public string FormulaConta { get; set; }
        public string FormulaTotal { get; set; }
        public string FormulaCresteri { get; set; }
        public string FormulaReduceri { get; set; }
        public string TipTitlu { get; set; }
        public string TipInstrument { get; set; }
        public bool Sectorizare { get; set; }
        public int? AnexaId { get; set; }
    }

    public class AnexaBnrEditDto
    {
        public int Id { get; set; }
        public string NrCrt { get; set; }
        public string DenumireRand { get; set; }
        public bool EDinConta { get; set; }
        public string FormulaConta { get; set; }
        public string FormulaTotal { get; set; }
        public string TipInstrument { get; set; }
        public int? DurataMinima { get; set; }
        public int? DurataMaxima { get; set; }
        public bool Sectorizare { get; set; }

        public virtual State State
        {
            get;
            set;
        }
        public int TenantId { get; set; }

    }
}
