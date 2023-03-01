using Niva.Erp.Models.Economic.Casierii.Cupiuri;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class DeclaratieCasierModel
    {
        public DateTime DataStart { get; set; }
        public DateTime DataDecizie { get; set; }
        public string NumeCasier { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string OperationType { get; set; }
        public decimal CupiuriTotal { get; set; }
        public decimal CupiuriSold { get; set; }
        public List<DeclaratieCasierDetails> DetailsIn { get; set; } // lista detalii intrare(Incasare)
        public List<DeclaratieCasierDetails> DetailsOut { get; set; } // lista detalii iesire(Plata)
        public List<CupiuriDeclaratieCaserieDetails> CupiuriDetails { get; set; } 
    }

    public class DeclaratieCasierDetails
    {

        public DateTime DispositionDate { get; set; }
        public int DispositionNumber { get; set; }
        public string DocumentTypeName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int UM { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
    }

    public class CupiuriDeclaratieCaserie
    {
        public int? CurrencyId { get; set; }
        public int TenantId { get; set; }
        public List<CupiuriDeclaratieCaserieDetails> CupiuriDetails { get; set; }
        public decimal Total
        {
            get; set;
        }
        public decimal Sold { get; set; }
    }

    public class CupiuriDeclaratieCaserieDetails
    {
        public int? Quantity { get; set; }

        public decimal Value { get; set; }
        public int TenantId { get; set; }

    }




}
