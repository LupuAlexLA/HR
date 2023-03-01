using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Conta.Lichiditate.Dto
{
    public class LichidConfigDto
    {   
        public int Id { get; set; }
        public string DenumireRand { get; set; }
        public string CodRand { get; set; }
        public bool EDinConta { get; set; }
        public string FormulaConta { get; set; }
        public string FormulaTotal { get; set; }
        public string TipInstrument { get; set; }
        public int? LichidBenziId { get; set; }
        public int OrderView { get; set; }  
        public virtual State State { get; set; }
        public int TenantId { get; set; }
    }

    public class LichidBenziDto
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
        public virtual State State { get; set; }
        public int TenantId { get; set; }
    }
}
