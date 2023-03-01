using Niva.Erp.Buget.Dto;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Nomenclatures.Dto
{
    public class CotaTVAListDto
    {
        public int Id { get; set; }
        public decimal VAT { get; set; }
        public DateTime  StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public State State { get; set; }    
        public int TenantId { get; set; }

    }

    

    public class CotaTVAEditDto
    {
        public int Id { get; set; }
        public decimal VAT { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class PaapIdCotaTVAListDto
    {
        public int PaapId { get; set; }
        public IList<CotaTVAListDto> CotaTVAList { get; set; }
}
}
