using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic.Casierii
{
    public class Exchange : AuditedEntity<int>, IMustHaveTenant
    {
        public ExchangeOperType ExchangeOperType { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        public virtual Currency Curency { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OperationDate { get; set; } // data tranzactie

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExchangeDate { get; set; } // data valuta 
        public decimal Value { get; set; }

        public decimal ExchangedValue { get; set; }
        [ForeignKey("BankAccountLei")]
        public int BankAccountLeiId { get; set; }
        public virtual BankAccount BankAccountLei { get; set; }

        [ForeignKey("BankAccountValuta")]
        public int BankAccountValutaId { get; set; }
        public virtual BankAccount BankAccountValuta { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("Operation")]
        public int? ContaOperationId { get; set; }
        public virtual Operation Operation { get; set; }

        public int TenantId { get ; set; }

        public decimal ExchangeRate { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public State State { get; set; }
    }

    public enum ExchangeOperType : int
    {
        [Description("Cumpar lei")]
        CumparLei,
        [Description("Cumpar valuta")]
        CumparValuta
    }

    public enum ExchangeType: int
    {
        Operatiuni,
        Cheltuieli,
        Imprumuturi
    }
}
