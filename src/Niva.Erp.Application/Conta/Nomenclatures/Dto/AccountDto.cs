
using AutoMapper;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;

namespace Niva.Conta.Nomenclatures
{
    public class AccountListDDDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int CurrencyId { get; set; }
    }

    public class AccountListFormDto
    {
        public string SearchAccount { get; set; }
        public bool AccountStatus { get; set; }

        public List<AccountListDto> Accounts { get; set; }
    }


    public class AccountListDto
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public virtual string SyntheticAccount { get; set; }

        public virtual string AccountName { get; set; }

        public virtual string AccountType { get; set; }

        public virtual string Currency { get; set; }

        public virtual string ActivityType { get; set; }
        public virtual string AccountFuncTypeName { get; set; }

        public virtual string ThirdParty { get; set; }
        public virtual bool AccountStatus { get; set; }
        public string DenumireSector { get; set; }
        public int? NivelRand { get; set; }
        public DateTime DataValabilitate { get; set; }

    }

    public class AccountEditDto
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string SyntheticAccount { get; set; }

        public int? SyntheticAccountId { get; set; }

        public virtual string AccountName { get; set; }

        public virtual string ExternalCode { get; set; }

        public virtual int CurrencyId { get; set; }

        public virtual int? ActivityTypeId { get; set; }

        public virtual int? ThirdPartyId { get; set; }

        public virtual int AccountType { get; set; }

        public virtual int AccountFuncType { get; set; }

        public bool ComputingAccount { get; set; }

        public virtual int? BankAccountId { get; set; }

        public virtual int TaxStatus { get; set; }

        public bool AccountStatus { get; set; }
        public int? SectorBnrId { get; set; }
        public int? NivelRand { get; set; }
        public DateTime DataValabilitate { get; set; }
    }

    public class AccountDeductibilityForm
    {
        public string AccountSearch { get; set; }

        public int AccountId { get; set; }

        public bool ShowList { get; set; }

        public bool ShowEdit { get; set; }

        public int AppClientId { get; set; }

        public string AccountName { get; set; }

        public List<AccountDeductibilityDto> DeductibilityList { get; set; }

        public AccountDeductibilityDto DeductibilityEdit { get; set; }
    }

    public class AccountDeductibilityDto
    {
        public int Id { get; set; }

        public DateTime PropertyDate { get; set; }
        public AccountTaxPropertyType PropertyType { get; set; }

        public int PropertyTypeId { get; set; }

        public decimal PropertyValue { get; set; }
        public string PropertyValueStr { get; set; }

        public int? AccountNededId { get; set; }

        public string AccountNeded { get; set; }
    }

    public class AccountHistoryListDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string AccountType { get; set; }
        public virtual string Currency { get; set; }
        public virtual string ActivityType { get; set; }
        public virtual string AccountFuncTypeName { get; set; }
        public virtual string ThirdParty { get; set; }
        public virtual bool AccountStatus { get; set; }
        public DateTime DataValabilitate { get; set; }
    }


    public class AccountMapProfile : Profile
    {
        public AccountMapProfile()
        {
            CreateMap<Account, AccountEditDto>()
                .ForMember(t => t.AccountType, opts => opts.MapFrom(d => d.AccountTypes))
                .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId))
                .ForMember(t => t.AccountFuncType, opts => opts.MapFrom(d => d.AccountFuncType))
                .ForMember(t => t.TaxStatus, opts => opts.MapFrom(d => d.TaxStatus))
                .ForMember(t => t.SyntheticAccount, opts => opts.MapFrom(d => (d.SyntheticAccount == null ? null : d.SyntheticAccount.Symbol)));

            CreateMap<AccountEditDto, Account>()
                  .ForMember(t => t.AccountTypes, opts => opts.MapFrom(d => d.AccountType))
                  .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId))
                  .ForMember(t => t.ThirdPartyId, opts => opts.MapFrom(d => d.ThirdPartyId))
                  .ForMember(t => t.AccountFuncType, opts => opts.MapFrom(d => d.AccountFuncType))
                  .ForMember(t => t.TaxStatus, opts => opts.MapFrom(d => d.TaxStatus))
                  .ForMember(t => t.SyntheticAccountId, opts => opts.Ignore())
                  .ForMember(t => t.SyntheticAccount, opts => opts.Ignore());

            CreateMap<AccountHistory, AccountHistoryListDto>()
                 .ForMember(t => t.AccountType, opts => opts.MapFrom(d => d.AccountTypes.ToString()))
                 .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                 .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                 .ForMember(t => t.ActivityType, opts => opts.MapFrom(d => d.ActivityType.ActivityName.ToString()))
                 .ForMember(t => t.AccountFuncTypeName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.AccountFuncType)));
        }
    }
}
