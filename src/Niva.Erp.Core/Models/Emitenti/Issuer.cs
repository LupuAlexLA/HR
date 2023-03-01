using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.SectoareBnr;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Emitenti
{
    public partial class Issuer : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("LegalPerson")]
        public virtual int LegalPersonId { get; set; }
        public virtual LegalPerson LegalPerson { get; set; }


        [StringLength(1000)]
        public virtual string IbanAbrv { get; set; }

        [StringLength(1000)]
        public virtual string Bic { get; set; }

        public IssuerType IssuerType { get; set; }

        [ForeignKey("BNR_Sector")]
        public int? BNR_SectorId { get; set; }
        public virtual BNR_Sector BNR_Sector { get; set; }

        public int TenantId { get; set; }
    }


    public enum IssuerType
    {
        Societate,
        Banca,
        IFN,
        SAI,
        [Description("Institutie de interes public")]
        InstitutieDeInteresPublic
    }
}
