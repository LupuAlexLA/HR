using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class ComisionDto
    {
        public int Id { get; set; }

        public int? ImprumutId { get; set; }
        public string TipComision { get; set; }
        public string Description { get; set; }
        public string TipValoareComision { get; set; }

        public decimal ValoareComision { get; set; }
        public string ModCalculComision { get; set; }
        public string TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }

    }
    public class ComisionEditDto
    {
        public int Id { get; set; }

        public virtual int? ImprumutId { get; set; }
        
        public TipComision TipComision { get; set; }
        public string Description { get; set; }
        public TipValoareComision TipValoareComision { get; set; }

        public decimal ValoareComision { get; set; }
        public ModCalculComision ModCalculComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }

        public bool OkDelete { get; set; }
    }

    public class ComisionV2Dto
    {
        public int Id { get; set; }

        public int? ImprumutId { get; set; }
        public string TipComision { get; set; }
        public string Descriere { get; set; }
        public string TipValoareComision { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public decimal ValoareComision { get; set; }
        public string ModCalculComision { get; set; }
        public string TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }

    }
    public class ComisionV2EditDto
    {
        public int Id { get; set; }
        public int? ImprumutId { get; set; }
        public TipComisionV2 TipComision { get; set; }
        public string Descriere { get; set; }
        public TipValoareComision TipValoareComision { get; set; }

        public decimal ValoareComision { get; set; }
        public ModCalculComision ModCalculComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

        public bool OkDelete { get; set; }
    }
}
