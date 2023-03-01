using Niva.Erp.Models.Buget;
using System;

namespace Niva.Erp.Buget.Dto
{
    public class BugetPrevContribListDto
    {
        public int Id { get; set; }
        public DateTime DataIncasare { get; set; }
        public string TipIncasare { get; set; }
        public string BankName { get; set; }
        public decimal Value { get; set; }
        public string CurrencyName { get; set; }
        public string ActivityTypeName { get; set; }
        public string Descriere { get; set; }
        public int TenantId { get; set; }

    }

    public class BugetPrevContribAddDto
    {
        public int Id { get; set; }
        public DateTime DataIncasare { get; set; }
        public BVC_BugetPrevContributieTipIncasare TipIncasare { get; set; }
        public int? BankId { get; set; }
        public decimal Value { get; set; }
        public int? CurrencyId { get; set; }
        public int? ActivityTypeId { get; set; }
        public string Descriere { get; set; }
        public int TenantId { get; set; }
    }
}
