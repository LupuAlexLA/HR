using Abp.Application.Services.Dto;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class RataDto : EntityDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public TipRata TipRata { get; set; }
        public int NumarOrdinDePlata { get; set; }
        public DateTime DataPlataRata { get; set; }
        public decimal SumaPrincipal { get; set; }
        public decimal SumaDobanda { get; set; }
        public decimal SumaPlatita { get; set; }
        public decimal ProcentDobanda { get; set; }
        public virtual string Currency { get; set; }
        
        public bool IsValid { get; set; }
        public decimal Sold { get; set; }
        public virtual int ImprumutId { get; set; }
        public virtual int CurrencyId { get; set; }
        public virtual int? ContaOperationDetailId { get; set; }

    }

    public class RataEditDto 
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public TipRata TipRata { get; set; }
        public TipDobanda TipDobanda { get; set; }
        public int NumarOrdinDePlata { get; set; }
        public DateTime DataPlataRata { get; set; }
        public decimal SumaPrincipal { get; set; }
        public decimal SumaDobanda { get; set; }
        public decimal SumaPlatita { get; set; }
        public decimal ProcentDobanda { get; set; }
        public decimal Sold { get; set; }
        public bool IsValid { get; set; }
        public virtual int ImprumutId { get; set; }
        public virtual int CurrencyId { get; set; }
        public virtual int? ContaOperationDetailId { get; set; }
        public bool OkDelete { get; set; }

    }
}
