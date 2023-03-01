using AutoMapper;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Emitenti;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Niva.Conta.Nomenclatures
{
    public class PersonInitForm {
        public string Id1 { get; set; }
        public string FullName { get; set; }
        public List<PersonListDto> PersonList { get; set; } 
    }

    public class PersonListDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Id1 { get; set; }

        [StringLength(1000)]
        public string Id2 { get; set; }

        [StringLength(1000)]
        public string FullName { get; set; }

        public bool IsEmployee { get; set; }
        public int IdPersonal { get; set; }
    }

    public class PersonEditDto
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
        //public bool IsNaturalPerson { get; set; }

        public bool IsVATPayer { get; set; }

        public DateTime? StartDateVATPayment { get; set; }

        public bool IsVATCollector { get; set; }
        public DateTime? VATCollectedStartDate { get; set; }


    }

    public class BankListDto
    {

        public int Id { get; set; }

        [StringLength(1000)]
        public string IbanAbrv { get; set; }

        [StringLength(1000)]
        public string Bic { get; set; }

        public string BankName { get; set; }

        public int IssuerId { get; set; }

    }

    public class BankEditDto
    {

        public int LegalPersonId { get; set; }

        [StringLength(1000)]
        public string IbanAbrv { get; set; }

        [StringLength(1000)]
        public string Bic { get; set; }

        public string BankName { get; set; }

        //public bool Add { get; set; }

    }

    public class ThirdPartyListDto
    {

        public int PersonId { get; set; }

        public bool IsClient { get; set; }

        public bool IsProvider { get; set; }

        public bool IsOther { get; set; }

        public string FullName { get; set; }
    }

    public class ThirdPartyListDDDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ThirdPartyEditDto
    {

        public int PersonId { get; set; }

        public bool IsClient { get; set; }

        public bool IsProvider { get; set; }

        public bool IsOther { get; set; }

    }

    public class ThirdPartyAccListDto
    {

        public int Id { get; set; }

        public string BankName { get; set; }

        public string Iban { get; set; }

        public string Currency { get; set; }

        public int ThirdPartyId { get; set; }
    }

    public class ThirdPartyAccListDDDto
    {
        public int Id { get; set; }

        public string Account { get; set; }
    }

    public class ThirdPartyAccEditDto
    {
        public int Id { get; set; }

        public string Iban { get; set; }

        public int BankId { get; set; }

        public int ThirdPartyId { get; set; }

        public int CurrencyId { get; set; }
    }

    public class CurrencyListDto
    {

        public int Id { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public int Status { get; set; }

    }

    public class ExchangeRateModelDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? CurrencyId { get; set; }

        public List<ExchangeRateDto> ExchangeList { get; set; }

    }

    public class ExchangeRateDto
    {
        public int Id { get; set; }

        public DateTime ExchangeDate { get; set; }

        public string CurrencyCode { get; set; }
        public int? CurrencyId { get; set; }

        public decimal Value { get; set; }
    }

    public class CountryListDto
    {
        public int Id { get; set; }

        public string CountryName { get; set; }
    }

    public class RegionListDto
    {
        public int Id { get; set; }

        public string RegionName { get; set; }

        public string Country { get; set; }
    }

    public class BankAccountDto
    {
        public int Id { get; set; }
        public int BankId { get; set; }

        public string Iban { get; set; }

        public string Currency { get; set; }
    }

    

    public class PersonMapProfile : Profile
    {
        public PersonMapProfile()
        {
            CreateMap<PersonListDto, Person>();
            CreateMap<Person, PersonListDto>();
            CreateMap<PersonEditDto, Person>();

            CreateMap<NaturalPerson, Person>();
            CreateMap<NaturalPerson, PersonEditDto>()
                .ForMember(t => t.Name, opts => opts.Ignore())
                .ForMember(t => t.PersonType, opts => opts.Ignore());

            CreateMap<LegalPerson, PersonEditDto>()
                .ForMember(t => t.FirstName, opts => opts.Ignore())
                .ForMember(t => t.LastName, opts => opts.Ignore())
                .ForMember(t => t.PersonType, opts => opts.Ignore())
                .ForMember(t => t.DefinedById, opts => opts.MapFrom(d => d.CreatorUserId))
                .ForMember(t => t.VATCollectedStartDate, opts => opts.MapFrom(d => d.VATCollectedStartDate));

            CreateMap<PersonEditDto, LegalPerson>()
                .ForMember(t => t.CreatorUserId, opts => opts.MapFrom(d => d.DefinedById));


            CreateMap<PersonEditDto, NaturalPerson>();



            CreateMap<Issuer, ThirdPartyAccEditDto>()
                .ForMember(t => t.BankId, opts => opts.MapFrom(d => d.Id));

            CreateMap<ThirdPartyAccListDto, BankAccount>()
                .ForMember(t => t.PersonId, opts => opts.MapFrom(d => d.ThirdPartyId));

            CreateMap<BankAccount, ThirdPartyAccEditDto>()
                .ForMember(t => t.ThirdPartyId, opts => opts.MapFrom(d => d.PersonId));

            //TOFIX
            CreateMap<Person, ThirdPartyListDto>()
                 .ForMember(t => t.FullName, opts => opts.MapFrom(d => d.FullName));

            CreateMap<Person, ThirdPartyListDDDto>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.FullName));
            CreateMap<ThirdPartyListDDDto, Person>();


            //CreateMap<ThirdPartyEditDto, Person>();

            CreateMap<PersonEditDto, Country>();
            CreateMap<CountryListDto, Country>();
            CreateMap<Country, CountryListDto>();
            CreateMap<CountryListDto, Region>()
                .ForMember(t => t.Country, opts => opts.MapFrom(d => d.Id));

            CreateMap<RegionListDto, Region>();
            CreateMap<Region, RegionListDto>();

            CreateMap<CurrencyListDto, Currency>();
            CreateMap<Currency, CurrencyListDto>();
            CreateMap<ThirdPartyAccEditDto, Currency>();

            CreateMap<BankAccountDto, BankAccount>();
            CreateMap<BankAccount, BankAccountDto>();
        }
    }
}
