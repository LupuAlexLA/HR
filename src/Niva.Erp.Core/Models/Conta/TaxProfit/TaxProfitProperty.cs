using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.Conta.TaxProfit
{
    public class TaxProfitProperty : AuditedEntity<int>, IMustHaveTenant
    {
       

        public DateTime PropertyDate { get; set; }

        public TaxProfitPropertyType PropertyType { get; set; }

        public TaxProfitPickCondition? PickCondition { get; set; }

        public IList<TaxProfitPropertyDet> PropertyDet { get; set; }

        public State State { get; set; }
 
        public int TenantId { get; set; }
    }

    public class TaxProfitPropertyDet : AuditedEntity<int> 
    {
        [ForeignKey("TaxProfitProperty")]
        public int? TaxProfitPropertyId { get; set; }
        public virtual TaxProfitProperty TaxProfitProperty { get; set; }

        public TaxProfitPropertyElem PropertyElem { get; set; }

        public decimal Value { get; set; }
 
    }
    
    public enum TaxProfitPropertyElem
    {
        [Description("Cifra de afaceri")]
        CifraDeAfaceri,
        [Description("Impozit pe profit datorat")]
        ImpozitPeProfitDatorat,
        [Description("Rezultat si cheltuieli de protocol")]
        RezultatSiCheltuieliDeProtocol
    }

    public enum TaxProfitPropertyType
    {
        Sponsorizare,
        Protocol
    }

    public enum TaxProfitPickCondition
    {
        Minim,
        Maxim
    }
}
