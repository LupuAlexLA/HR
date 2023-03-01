using System;

namespace Niva.Erp.Conta.Nomenclatures.Dto
{
    public class DiurnaListDto
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public DateTime DataValabilitate { get; set; }
        public int Value { get; set; }
        public string DiurnaTypeName { get; set; }
        public int TenantId { get; set; }
    }

    public class DiurnaEditDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public int CurrencyId { get; set; }
        public int Value { get; set; }
        public DateTime DataValabilitate { get; set; }
        public virtual int? DiurnaTypeId { get; set; }
        public int TenantId { get; set; }
    }
}
