using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic.Dto
{
    public class CupiuriListDto
    {
        public int Id { get; set; }

        public DateTime OperationDate { get; set; }
    }

    public class CupiuriForm
    {
        public int Id { get; set; }
        public int CupiuriInitId { get; set; }
        public DateTime OperationDate { get; set; }
        public List<CupiuriItemDto> Cupiuri { get; set; }

    }

    public class CupiuriItemDto
    {
        public int? CurrencyId { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }
        public List<CupiuriDetailsDto> CupiuriDetails { get; set; }
        public decimal Total
        {
            get; set;
        }
        public decimal Sold { get; set; }
    }

    public class CupiuriDetailsDto
    {
        public int? Quantity { get; set; }

        public decimal Value { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

    }
}
