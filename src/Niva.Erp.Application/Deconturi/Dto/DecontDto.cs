using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Deconturi.Dto
{
    public class DecontInitForm
    {
        public int? ThirdPartyId { get; set; }
        public string ThirdPartyName { get; set; }  
        public DateTime DecontStartDate { get; set; }
        public DateTime DecontEndDate { get; set; }
        public DateTime? DelegatieStartDate { get; set; }
        public DateTime? DelegatieEndDate { get; set; } 
        public int? DiurnaLegalaId { get; set; }
        public int? DecontTypeId { get; set; }
        public int? ScopDeplasareTypeId { get; set; }
        public string DocumentType { get; set; }
        public List<DecontListDto> DecontList { get; set; } 
    }

    public class DecontListDto
    {
        public int Id { get; set; }
        public int DecontNumber { get; set; }
        public DateTime DecontDate { get; set; }
        public string ThirdParty { get; set; }
        public int? Diurna { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string DecontType { get; set; }
        public string ScopDeplasareTypeName { get; set; }
        public string DocumentName { get; set; }
        public decimal TotalDiurnaAcordata { get; set; }
        public decimal TotalDiurnaImpozabila { get; set; }
    }

    public class DecontEditDto
    {
        public int Id { get; set; }
        public int DecontNumber { get; set; }
        public DateTime DecontDate { get; set; }
        public int CurrencyId { get; set; }
        public int ThirdPartyId { get; set; }
        public string ThirdPartyName { get; set; }
        public int? DiurnaZi { get; set; }
        public int? DiurnaImpozabila { get; set; }
        public int? DiurnaLegala { get; set; }
        public int? DiurnaLegalaId { get; set; } 
        public decimal TotalDiurnaAcordata { get; set; }
        public decimal? NrZile { get; set; }
        public decimal TotalDiurnaImpozabila { get; set; }
        public DateTime? DateStart { get; set; }    
        public DateTime? DateEnd { get; set; }
        public int? DecontTypeId { get; set; }
        public string DecontType { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }
        public int ScopDeplasareTypeId { get; set; }
        public string DocumentType { get; set; }
        public decimal TotalDiurnaAcordataFileDoc { get; set; }
        public int? OperationId { get; set; }
    }
}
