using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class TragereDto
    {
        public int Id { get; set; }
        public TipTragere TipTragere { get; set; }
        public string TipTragereString { get; set; }
        public DateTime DataTragere { get; set; }
        public decimal SumaDisponibila { get; set; }
        public decimal SumaTrasa { get; set; }
        public decimal Dobanda { get; set; }
        public decimal SumaImprumutata { get; set; }
        public decimal Comision { get; set; }
        private decimal comisionSum;
        public decimal ComisionSum { get { return Comisions != null ? Comisions.Sum(item => item.SumaComision) : 0; } set { this.comisionSum = value; } }

        public virtual int CurrencyId { get; set; }
        public string CurrencyS { get; set; }


        public State State { get; set; }
        
        public virtual int? ImprumutId { get; set; }
       
        public int TenantId { get; set; }
        public virtual List<DataComisionDto> Comisions { get; set; }

        
    }

    public class DataTragereDto
    {
        public int Id { get; set; }
        public DateTime DataTragere { get; set; }
    }
}
