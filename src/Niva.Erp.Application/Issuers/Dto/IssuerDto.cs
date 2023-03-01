using Niva.Erp.Models.Emitenti;
using System;
using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Conta.Nomenclatures.Dto
{
    public class IssuerDto
    {
        public int Id { get; set; }
        public string IbanAbrv { get; set; }
        public string Bic { get; set; }
        public int IssuerType { get; set; }
        public int LegalPersonId { get; set; }
        public int TenantId { get; set; }
    }

    public class IssuerListDto
    {
        public int Id { get; set; }
        public string IssuerType { get; set; }
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string FullName { get; set; }
        public string CodStatistic { get; set; }    
    }

    public class PersonIssuerEditDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Id1 { get; set; }

        [StringLength(1000)]
        public string Id2 { get; set; }

        [StringLength(1000)]
        public string AddressStreet { get; set; }

        [StringLength(1000)]
        public string AddressNo { get; set; }

        [StringLength(1000)]
        public string AddressBlock { get; set; }

        [StringLength(1000)]
        public string AddressFloor { get; set; }

        [StringLength(1000)]
        public string AddressApartment { get; set; }

        [StringLength(1000)]
        public string AddressZipCode { get; set; }

        [StringLength(1000)]
        public string AddressLocality { get; set; }

        public int DefinedById { get; set; }

        public int? AddressRegionId { get; set; }

        public int? AddressCountryId { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string FirstName { get; set; }

        [StringLength(1000)]
        public string LastName { get; set; }

        public string PersonType { get; set; }

        public bool IsNaturalPerson { get { return (PersonType == "NP"); } }

        public bool IsVATPayer { get; set; }

        public DateTime? StartDateVATPayment { get; set; }

        public bool IsVATCollector { get; set; }
        public DateTime? VATCollectedStartDate { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool ShowForm3 { get; set; }

        public IssuerDetailsDto IssuerDetails { get; set; }
    }

    public class IssuerDetailsDto
    {
        public int Id { get; set; }
        public IssuerType IssuerType { get; set; }
        public string IbanAbrv { get; set; }
        public string Bic { get; set; }
        public int? BNR_SectorId { get; set; }
    }
}
