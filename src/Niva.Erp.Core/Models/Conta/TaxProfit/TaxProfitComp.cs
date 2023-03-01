using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta.TaxProfit
{

    public class TaxProfitComp : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime ComputeDate { get; set; }

        public IList<TaxProfitCompDet> ComputingDet { get; set; }

        public IList<TaxProfitCompExpense> CompExpense { get; set; }

        [ForeignKey("ContaOperation")]
        public int? ContaOperationId { get; set; }
        public Operation ContaOperation { get; set; }
        public int TenantId { get; set; }
    }

    public class TaxProfitCompDet : AuditedEntity<int>
    {
        [ForeignKey("TaxProfitComp")]
        public int TaxProfitCompId { get; set; }
        public TaxProfitComp TaxProfitComp { get; set; }

        [ForeignKey("TaxProfitConfigDet")]
        public int TaxProfitConfigDetId { get; set; }
        public TaxProfitConfigDet TaxProfitConfigDet { get; set; }

        public decimal? Col1 { get; set; }

        public decimal? Col2 { get; set; }

        public bool Calculated1 { get; set; }

        public bool Calculated2 { get; set; }

       
        public int TenantId { get; set; }
    }

    public class TaxProfitCompStruct : AuditedEntity<int>
    {
        [ForeignKey("TaxProfitComp")]
        public int TaxProfitCompId { get; set; }
        public TaxProfitComp TaxProfitComp { get; set; }

        [ForeignKey("TaxProfitConfigDet")]
        public int TaxProfitConfigDetId { get; set; }
        public TaxProfitConfigDet TaxProfitConfigDet { get; set; }

        public int ColumnId { get; set; }

        [StringLength(1000)]
        public string Details { get; set; }

        public decimal? Value { get; set; }
         
    }

    public class TaxProfitCompValueForm : AuditedEntity<int>
    {
        [ForeignKey("TaxProfitComp")]
        public int TaxProfitCompId { get; set; }
        public TaxProfitComp TaxProfitComp { get; set; }

        [ForeignKey("TaxProfitConfigDet")]
        public int TaxProfitConfigDetId { get; set; }
        public TaxProfitConfigDet TaxProfitConfigDet { get; set; }

        public int ColumnId { get; set; }

        [StringLength(1000)]
        public string Formula { get; set; }

        [StringLength(1000)]
        public string FormulaVal { get; set; }
 
    }

    public class TaxProfitCompExpense : AuditedEntity<int>
    {
        [ForeignKey("TaxProfitComp")]
        public int TaxProfitCompId { get; set; }
        public TaxProfitComp TaxProfitComp { get; set; }

        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public decimal TotRulaje { get; set; }

        public decimal ProcDeduct { get; set; } // afisez procentul de deductibilitate 

        public decimal CheltNededVenitNeimp { get; set; } // Cheltuieli nedeductibile aferente veniturilor neimpozabile alocate direct

        public decimal CheltNededNotRow32 { get; set; } // Cheltuieli nedeductibile  care nu sunt de natura rd.32 din Declaratia 101

        public decimal CheltDedVenitNeimp { get; set; } // Cheltuieli deductibile alocate direct unor tranzactii cu venituri impozabile

        public decimal CheltDedVenitImp { get; set; } // Cheltuieli deductibile de alocat veniturilor neimpozabile
 
    }

    public class ExpenseDetails
    {   //todo de mutat in apllication, metodele din repository care returneaza altceva decat IEntity sunt de pus in Application!
        public string Detail { get; set; }

        public decimal Value { get; set; }
    }

}