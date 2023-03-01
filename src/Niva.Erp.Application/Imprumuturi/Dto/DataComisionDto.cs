using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class DataComisionDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public DateTime DataPlataComision { get; set; }
        public decimal SumaComision { get; set; }
        public TipValoareComision TipValoareComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }
        public virtual int? TragereId { get; set; }
        public virtual int? ImprumutId { get; set; }
        public virtual int? ComisionId { get; set; }
        public decimal ValoareComision { get; set; }
        public State State { get; set; }
        
        public bool IsValid { get; set; }
    }
    public class DataComisionEditDto
    {
        public int Id { get; set; }
        public virtual int? ComisionId { get; set; }  
        public virtual int? ImprumutId { get; set; }
        public virtual int? TragereId { get; set; } 
        public virtual int? ContaOperationId { get; set; }
        public virtual int? ContaOperationDetailId { get; set; }
        public int Index { get; set; }
        public DateTime DataPlataComision { get; set; }
        public decimal SumaComision { get; set; }
        public TipValoareComision TipValoareComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }

        public decimal ValoareComision { get; set; }
        
        
        public bool IsValid { get; set; }
        public bool OkDelete { get; set; }
    }
}
