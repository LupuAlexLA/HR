using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.AutoOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Niva.Erp.Models.Conta.TaxProfit
{
    public class TaxProfitConfig : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime ConfigDate { get; set; }

        public IList<TaxProfitConfigDet> ConfigDet { get; set; }

       
        public State State { get; set; }

       
        public int TenantId   { get; set; }
    }

    public class TaxProfitConfigDet : AuditedEntity<int> 
    {
        [ForeignKey("TaxProfitConfig")]
        public int TaxProfitConfigId { get; set; }
        public TaxProfitConfig TaxProfitConfig { get; set; }

        public int IdRow { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string RowNr { get; set; }

        public bool TotalRow { get; set; }

        public bool FontBold { get; set; }

        public TaxProfitConfigSource FormulaSource { get; set; }

        [StringLength(2000)]
        public string Col1 { get; set; }

        [StringLength(2000)]
        public string Col2 { get; set; }

        public int DecimalNrCol1 { get; set; }

        public int DecimalNrCol2 { get; set; }

        public bool NegativeValue { get; set; }

        public State State { get; set; }

        public int OrderView { get; set; }

        public TaxProfitElement? AutoOperationElement { get; set; }
 
    }

    public enum TaxProfitConfigSource
    {
        Rulaje,
        [Description("Alte surse")]
        AlteSurse
    }

    public enum TaxProfitOperType : int
    {
        Constituire
    }
}
