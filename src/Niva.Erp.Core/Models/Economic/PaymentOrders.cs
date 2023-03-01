using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Economic
{
    public class PaymentOrders : AuditedEntity<int> ,IMustHaveTenant
    {
        public int OrderNr { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Value { get; set; }

        [StringLength(1000)]
        public string WrittenValue { get; set; }

        [ForeignKey("PayerBankAccount")]
        public int PayerBankAccountId { get; set; }
        public BankAccount PayerBankAccount { get; set; }

        [ForeignKey("Beneficiary")]
        public int BeneficiaryId { get; set; }
        public Person Beneficiary { get; set; }

        [ForeignKey("BenefAccount")]
        public int BenefAccountId { get; set; }
        public BankAccount BenefAccount { get; set; }

        [StringLength(1000)]
        public string PaymentDetails { get; set; }

        //[ForeignKey("Invoice")]
        //public int? InvoiceId { get; set; }
        //public Invoices Invoice { get; set; }
        public virtual IList<PaymentOrderInvoice> PaymentOrderInvoices { get; set; }

        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("FgnOperationsDetail")]
        public virtual int? FgnOperDetailId { get; set; }
        public virtual ForeignOperationsDetails FgnOperationsDetail { get; set; }

        public int? Div { get; set; }

        public OperationStatus Status { get; set; }

        public DateTime StatusDate { get; set; }

        public State State { get; set; }

      
        public int TenantId { get; set; }
    }
}
