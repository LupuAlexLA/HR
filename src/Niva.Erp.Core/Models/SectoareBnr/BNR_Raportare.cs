using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.SectoareBnr
{
    public class BNR_Raportare : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("BNR_Anexa")]
        public int AnexaId { get; set; }
        public virtual BNR_Anexa BNR_Anexa { get; set; }

        public List<BNR_RaportareRand> BNR_RaportareRanduri { get; set; }   
        public int TenantId { get; set; }
    }

    public class BNR_RaportareRand: AuditedEntity<int>
    {
        [ForeignKey("BNR_Raportare")]
        public int BNR_RaportareId { get; set; }
        public virtual BNR_Raportare BNR_Raportare { get; set; }

        [ForeignKey("BNR_AnexaDetail")]
        public int AnexaDetailId { get; set; }
        public virtual BNR_AnexaDetail BNR_AnexaDetail { get; set; }

        [ForeignKey("BNR_Sector")]
        public int? SectorId { get; set; }
        public virtual BNR_Sector BNR_Sector { get; set; }

        public decimal Valoare { get; set; }

        public List<BNR_RaportareRandDetail> BNR_RaportareRandDetails { get; set; }

    }

    public class BNR_RaportareRandDetail: AuditedEntity<int>
    {
        [ForeignKey("BNR_RaportareRand")]
        public int BNR_RaportareRandId { get; set; }
        public virtual BNR_RaportareRand BNR_RaportareRand { get; set; }

        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
    }
}
