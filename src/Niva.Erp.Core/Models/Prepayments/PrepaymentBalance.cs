 
namespace Niva.Erp.Models.PrePayments
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using System;
	using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class PrepaymentBalance : AuditedEntity<int>, IMustHaveTenant
    {
		public virtual DateTime ComputeDate { get; set; }

        public PrepaymentOperType OperType { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        public decimal TranzDeprec { get; set; }

        public decimal Deprec { get; set; }

        public decimal TranzDeprecVAT { get; set; }

        public decimal DeprecVAT { get; set; }

        public decimal TranzPrepaymentValue { get; set; }

        public decimal PrepaymentValue { get; set; }

        public decimal TranzPrepaymentVAT { get; set; }

        public decimal PrepaymentVAT { get; set; }

        public virtual decimal MontlyCharge { get; set; }

        public virtual decimal MontlyChargeVAT { get; set; }

        [ForeignKey("Prepayment")]
        public int PrepaymentId { get; set; }
        public virtual Prepayment Prepayment { get; set; }

        [ForeignKey("PrepaymentPF")]
        public int? PrepaymentPFId { get; set; }
        public Prepayment PrepaymentPF { get; set; }

      

        [ForeignKey("OperationDetail")]
        public int? OperationDetailId { get; set; }
        public virtual OperationDetails OperationDetail { get; set; }
 
        public int TenantId { get; set; }
    }

    public enum PrepaymentOperType : int
    {
        Constituire,
        [Description("Amortizare lunara")]
        AmortizareLunara,
        Iesire
    }
}

