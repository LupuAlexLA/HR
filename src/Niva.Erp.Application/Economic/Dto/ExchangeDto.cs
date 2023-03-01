using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Economic.Dto
{
    public class ExchangeInitDto {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public int? OperationTypeId { get; set; }
        public int? CurrencyId { get; set; }
        public List<ExchangeListDto> ExchangeList { get; set; }  

    }

    public class ExchangeListDto
    {
        public int Id { get; set; }
        public string ExchangeOperType { get; set; }
        public string OperationType { get; set; }
        public string BankAccountLei { get; set; }
        public string BankAccountValuta { get; set; }

        public DateTime OperationDate { get; set; }
        public decimal Value { get; set; }
        public decimal ExchangeRate { get; set; }
        public string CurrencyName { get; set; }        
        public State State { get; set; }
    }

    public class ExchangeEditDto
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; } // data tranzactie
        public DateTime ExchangeDate { get; set; } // data valuta

        public decimal Value { get; set; }  // suma in lei
        public decimal ExchangedValue { get; set; } // suma in valuta
        public int? ExchangeOperType { get; set; }
        public int? OperationType { get; set; }
        public int? CurrencyId { get; set; }
        public string LocalCurrencyCode { get; set; }
        public string CurrencyCode { get; set; } 
        public int? BankAccountLeiId { get; set; }
        public int? BankAccountValutaId { get; set; }

        public int? BankLeiId { get; set; }
        public int? BankValutaId { get; set; }
        public int? ActivityTypeId { get; set; }
        public decimal ExchangeRate { get; set; }
        public int? ContaOperationId { get; set; }
        public int TenantId { get; set; }
    }
}
