using Niva.Erp.Models.Conta.Enums;
using System;

namespace Niva.Erp.Buget.Dto
{
    public class BugetPrevDobandaReferintaListDto
    {
        public int Id { get; set; }
        public int FormularId { get; set; }
        public int An { get; set; }
        public string PlasamentName { get; set; }
        public decimal Procent { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }
        public string CurrencyName { get; set; }
        public string ActivityTypeName { get; set; }    
    }   

    public class BugetPrevDobandaReferintaEditDto
    {
        public int Id { get; set; }
        public int FormularId { get; set; }
        public int? PlasamentType { get; set; }
        public decimal Procent { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }
        public int? CurrencyId { get; set; }
        public int? ActivityTypeId { get; set; }    
    }
}
