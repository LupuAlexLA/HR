using Abp.Application.Services.Dto;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class GarantieDto : EntityDto
    {
        public int Id { get; set; }

        public string TipGarantie { get; set; }
        public int DocumentNr { get; set; }
        public string LegalPerson { get; set; } // Partener Garantie

        public string GarantieAccount { get; set; }
        public string Currency { get; set; }
        public string GarantieTip { get; set; }
        public string GarantieCeGaranteaza { get; set; }
        public decimal SumaGarantiei { get; set; }
        public decimal Sold { get; set; }
        public string Mentiuni { get; set; }
        public string CeGaranteaza { get; set; }
        public string GarantiePrimitaDataEnum { get; set; }
        public DateTime StartDateGarantie { get; set; }

        public DateTime EndDateGarantie { get; set; }

        public DateTime DocumentDate { get; set; }

    }
    public class GarantieEditDto
    {
        public int Id { get; set; }

        public string TipGarantie { get; set; }
        public int DocumentNr { get; set; }

        public virtual int? GarantieAccountId { get; set; }

        public decimal SumaGarantiei { get; set; }
        public string Mentiuni { get; set; }

        public DateTime StartDateGarantie { get; set; }

        public DateTime EndDateGarantie { get; set; }
        public DateTime DocumentDate { get; set; }

        public virtual int ImprumutId { get; set; }
        public virtual int CurrencyId { get; set; }
        public virtual int LegalPersonId { get; set; }
        public virtual int GarantieTipId { get; set; }
        public virtual int GarantieCeGaranteazaId { get; set; }
        public TipGarantiePrimitaDataEnum GarantiePrimitaDataEnum { get; set; }
        public bool OkDelete { get; set; }

    }

    public class OperatieGarantieDto 
    {
        public int Id { get; set; }
        public int GarantieId { get; set; }
        public decimal Suma { get; set; }
        public decimal Sold { get; set; }
        public TipOperatieGarantieEnum TipOperatieGarantieEnum { get; set; }
        public DateTime DataOperatiei { get; set; }
        public int TenantId { get; set; }

    }
}
