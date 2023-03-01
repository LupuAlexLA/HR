using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Newtonsoft.Json;
using Niva.Conta.Nomenclatures;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Conta.Nomenclatures
{
    public class CreatePersonDto { };
    public interface IPersonAppService : IApplicationService
    {
        PersonListDto PersonSetupList();
        List<PersonListDto> SearchPerson(string search);

        GetBankOutput BankSetupList();
        PersonInitForm InitForm();
        List<PersonListDto> SearchPersonList(PersonInitForm person);
        GetPersonOutput PersonList();

        GetPersonOutput PersonListSearch(string search);

        GetCurrencyOutput CurrencyList();

        GetCurrencyOutput ForeignCurrencyList();

        GetCountryOutput CountryList();

        GetRegionOutput RegionList(int id);

        GetBankOutput BankList();

        GetThirdPartyOutput ThirdPartyList();

        List<ThirdPartyListDDDto> ThirdPartyDDList();

        GetThirdPartyAccOutput ThirdPartyAccList(int thirdPartyId);

        List<ThirdPartyAccListDDDto> AppClientBankAccountList();

        GetThirdPartyAccOutput ThirdPartyAccSetupList();

        ThirdPartyAccEditDto GetBankAccSetupById(int id);

        void SaveThirdPartySetupAcc(ThirdPartyAccEditDto thirdParty);

        void DeleteThirdPartySetupAcc(int bankAccountId);

        ThirdPartyAccEditDto GetBankAccById(int id);

        PersonEditDto GetPersonById(int id);

        PersonEditDto SavePerson(PersonEditDto person);

        void DeletePerson(int id);

        void SaveThirdPartyAcc(ThirdPartyAccEditDto thirdParty);

        ExchangeRateModelDto ExchangeRateInit();

        ExchangeRateModelDto SearchExchangeRates(ExchangeRateModelDto model);

        List<BankListDto> BanksForThirdParty(int thirdPartyId);

        List<ThirdPartyAccListDto> BankAccountsForThirdParty(int thirdPartyId, int? bankId, int currencyId);

        List<ThirdPartyListDto> ThirdPartyListDD(string search);
        GetThirdPartyOutput ThirdPartySearch(string search);
        GetThirdPartyOutput ThirdPartySearchForDecont(string search);

        List<BankAccountDto> BankAccountsForExchangeValuta(int bankValutaId, int currencyId);
        List<BankAccountDto> BankAccountsForExchangeLei(int bankLeiId);
        List<BankListDto> BankForExchangeLei();
        List<BankListDto> BankForExchangeValuta(int? currencyId);
        Task<DateFirmaAnaf> AnafDateFirma(int cui);
        void ExchangeRateDetele(int Id);
        void ExchangeRateAdd(ExchangeRateDto exchangeRate);

        void ActualizarePerson();

        string GetCurrentAppClientName();

        GetThirdPartyOutput SearchPersonForInvObjectByInput(string search);
    }

    public class GetPersonOutput
    {
        public List<PersonListDto> GetPerson { get; set; }
    }

    public class GetCurrencyOutput
    {
        public List<CurrencyListDto> GetCurrency { get; set; }
    }

    public class GetCountryOutput
    {
        public List<CountryListDto> GetCountry { get; set; }
    }

    public class GetRegionOutput
    {
        public List<RegionListDto> GetRegion { get; set; }
    }

    public class GetBankOutput
    {
        public List<BankListDto> GetBank { get; set; }
    }

    public class GetThirdPartyOutput
    {
        public List<ThirdPartyListDto> GetThirdParty { get; set; }
    }

    public class GetThirdPartyAccOutput
    {
        public List<ThirdPartyAccListDto> GetThirdPartyAcc { get; set; }
    }

    public class PersonAppService : ErpAppServiceBase, IPersonAppService
    {

        IPersonRepository _personRepository;
        IRepository<LegalPerson> _legalPersonRepository;
        IRepository<NaturalPerson> _naturalPersonRepository;
        IRepository<ExchangeRates> _exchangeRatesRepository;
        IAngajatiExternManager _angajatiExternManager;
        IDepartamentRepository _departamentRepository;
        IRepository<SalariatiDepartamente> _salariatiDepartamenteRepository;

        public PersonAppService(IPersonRepository personRepository, IRepository<LegalPerson> legalPersonRepository, IRepository<NaturalPerson> naturalPersonRepository,
            IRepository<ExchangeRates> exchangeRatesRepository, IAngajatiExternManager angajatiExternManager, IDepartamentRepository departamentRepository,
            IRepository<SalariatiDepartamente> salariatiDepartamenteRepository)
        {

            _personRepository = personRepository;
            _legalPersonRepository = legalPersonRepository;
            _naturalPersonRepository = naturalPersonRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _angajatiExternManager = angajatiExternManager;
            _departamentRepository = departamentRepository;
            _salariatiDepartamenteRepository = salariatiDepartamenteRepository;
        }

        // Third Party Setup List
        //    [AbpAuthorize("General.ListaConturi.Acces")]
        public GetThirdPartyAccOutput ThirdPartyAccSetupList()
        {
            //get current tennant!
            var appClient = GetCurrentTenant();
            int thirdPartyId = (int)appClient.LegalPersonId;

            var _thirdPartyAcc = _personRepository.ThirdPartyAccList(thirdPartyId).OrderBy(f => f.Bank.LegalPerson.Name).ThenBy(f => f.Currency.CurrencyName);

            var ret = new GetThirdPartyAccOutput { GetThirdPartyAcc = ObjectMapper.Map<List<ThirdPartyAccListDto>>(_thirdPartyAcc) };

            return ret;
        }

        public ThirdPartyAccEditDto GetBankAccSetupById(int id)
        {
            var _bank = _personRepository.GetBankAccById(id);
            var ret = ObjectMapper.Map<ThirdPartyAccEditDto>(_bank);
            return ret;
        }

        public async Task<DateFirmaAnaf> AnafDateFirma(int cui)
        {



            try
            {
                var payload = new FirmaCuiData
                {
                    cui = cui,
                    data = DateTime.Now.ToString("yyyy'-'MM'-'dd")
                };



                string stringPayload = System.Text.Json.JsonSerializer.Serialize(new List<FirmaCuiData>() { payload });
                // stringPayload = "[" + stringPayload + "]";


                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                var httpResponse = await httpClient.PostAsync("https://webservicesp.anaf.ro/PlatitorTvaRest/api/v6/ws/tva", httpContent);


                String responseString = await httpResponse.Content.ReadAsStringAsync();


                DateFirmaAnaf DateFirma = new DateFirmaAnaf();



                DateFirma = JsonConvert.DeserializeObject<DateFirmaAnaf>(responseString);



                return DateFirma;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Nereusit", ex.Message);
            }
        }

        [AbpAuthorize("General.ListaConturi.Modificare")]
        public void SaveThirdPartySetupAcc(ThirdPartyAccEditDto thirdParty)
        {
            try
            {
                var _bank = ObjectMapper.Map<BankAccount>(thirdParty);

                var appClient = GetCurrentTenant();
                int thirdPartyId = (int)appClient.LegalPersonId;
                _bank.PersonId = thirdPartyId;

                //_bank.ThirdPartyId = appClient.LegalPersonId;

                if (_bank.Id == 0)
                {
                    _personRepository.BankAccInsertOrUpdate(_bank); // INSERT
                }
                else
                {
                    _personRepository.BankAccInsertOrUpdate(_bank); //UPDATE
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("General.ListaConturi.Modificare")]
        public void DeleteThirdPartySetupAcc(int bankAccountId)
        {
            try
            {
                _personRepository.BankAccDelete(bankAccountId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // Person Setup List
        //   [AbpAuthorize("General.DateSocietate.Acces")]
        public PersonListDto PersonSetupList()
        {
            var appClient = GetCurrentTenant();
            LegalPerson _person;
            if (appClient.LegalPersonId == null)
            {
                _person = new LegalPerson();
                _personRepository.InsertAndGetId(_person);
                appClient.LegalPerson = _person;
            }
            else
            {
                _person = _legalPersonRepository.Single(f => f.Id == appClient.LegalPersonId);
            }
            var ret = ObjectMapper.Map<PersonListDto>(_person);
            return ret;
        }

        // Bank Setup List        
        public GetBankOutput BankSetupList()
        {
            //int selectedAppClientId = GetCurrentAppClientId();
            //var appClient = _appClientRepository.Get(selectedAppClientId);
            var _bank = _personRepository.BankList().OrderBy(f => f.LegalPerson.Name);

            var ret = new GetBankOutput { GetBank = ObjectMapper.Map<List<BankListDto>>(_bank) };
            return ret;
        }

        // Person List
        public GetPersonOutput PersonList()
        {
            //int selectedAppClientId = GetCurrentAppClientId();
            var _person = _personRepository.GetAll()
                //.Where(f => f.DefinedById == selectedAppClientId)
                .ToList()
                .OrderBy(f => f.FullName);

            var ret = new GetPersonOutput { GetPerson = ObjectMapper.Map<List<PersonListDto>>(_person) };
            return ret;
        }

        public GetPersonOutput PersonListSearch(string search)
        {
            //int selectedAppClientId = GetCurrentAppClientId();
            var _person = _personRepository.GetAll()
                //.Where(f => f.DefinedById == selectedAppClientId)
                .ToList()
                .OrderBy(f => f.FullName)
                .ToList();
            if (search != "" && search != null)
            {
                _person = _person.Where(f => f.FullName.ToUpper().IndexOf(search.ToUpper()) >= 0).ToList();
            }

            var ret = new GetPersonOutput { GetPerson = ObjectMapper.Map<List<PersonListDto>>(_person) };
            return ret;
        }

        public void CreatePerson(CreatePersonDto createPersonDto)
        {
            var person = new LegalPerson();
            _personRepository.Insert(person);
        }

        // CurrencyList    
        public GetCurrencyOutput CurrencyList()
        {
            var _currency = _personRepository.CurrencyList();
            var ret = new GetCurrencyOutput { GetCurrency = ObjectMapper.Map<List<CurrencyListDto>>(_currency) };
            return ret;
        }

        // CurrencyList    
        public GetCurrencyOutput ForeignCurrencyList()
        {
            //int selectedAppClientId = GetCurrentAppClientId();
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId;

            var _currency = _personRepository.CurrencyList().Where(f => f.Id != localCurrencyId);
            var ret = new GetCurrencyOutput { GetCurrency = ObjectMapper.Map<List<CurrencyListDto>>(_currency) };
            return ret;
        }

        // Country List    
        public GetCountryOutput CountryList()
        {
            var _country = _personRepository.CountryList();
            var ret = new GetCountryOutput { GetCountry = ObjectMapper.Map<List<CountryListDto>>(_country) };
            return ret;
        }


        // Region List    
        public GetRegionOutput RegionList(int id)
        {
            var _region = _personRepository.RegionList(id);
            var ret = new GetRegionOutput { GetRegion = ObjectMapper.Map<List<RegionListDto>>(_region) };
            return ret;
        }

        // Bank List        
        public GetBankOutput BankList()
        {

            var _bank = _personRepository.BankList().OrderBy(f => f.LegalPerson.Name);

            var ret = new GetBankOutput { GetBank = ObjectMapper.Map<List<BankListDto>>(_bank) };
            return ret;
        }

        // Third Party List        
        public GetThirdPartyOutput ThirdPartyList()
        {


            // Trebuie sa facem .ToList() pentru ca FullName nu este implicit o coloana din BD.
            // FullName = FirstName + " " + LastName
            var _thirdParty = _personRepository.ThirdPartyList().ToList().OrderBy(f => f.FullName);

            var ret = new GetThirdPartyOutput { GetThirdParty = ObjectMapper.Map<List<ThirdPartyListDto>>(_thirdParty) };
            return ret;
        }

        public GetThirdPartyOutput ThirdPartySearch(string search)
        {
            //int selectedAppClientId = GetCurrentAppClientId();

            // Trebuie sa facem .ToList() pentru ca FullName nu este implicit o coloana din BD.
            // FullName = FirstName + " " + LastName

            var ret = new GetThirdPartyOutput { GetThirdParty = null };

            if (search != null)
            {
                var _thirdParty = _personRepository.ThirdPartyList().ToList().Where(s => s.FullName.ToUpper().Contains(search.ToUpper())).OrderBy(f => f.FullName);

                ret = new GetThirdPartyOutput { GetThirdParty = ObjectMapper.Map<List<ThirdPartyListDto>>(_thirdParty) };
            }

            return ret;
        }


        public GetThirdPartyOutput ThirdPartySearchForDecont(string search)
        {
            try
            {
                var ret = new GetThirdPartyOutput { GetThirdParty = null };

                if (search != null)
                {
                    var _thirdParty = _personRepository.ThirdPartyList().ToList().Where(s => s.FullName.ToUpper().Contains(search.ToUpper()) && s.IsEmployee == true).OrderBy(f => f.FullName);

                    ret = new GetThirdPartyOutput { GetThirdParty = ObjectMapper.Map<List<ThirdPartyListDto>>(_thirdParty) };
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ThirdPartyListDDDto> ThirdPartyDDList()
        {
            //int selectedAppClientId = GetCurrentAppClientId();

            // Trebuie sa facem .ToList() pentru ca FullName nu este implicit o coloana din BD.
            // FullName = FirstName + " " + LastName
            var _thirdParty = _personRepository.ThirdPartyList().ToList().OrderBy(f => f.FullName);

            var ret = ObjectMapper.Map<List<ThirdPartyListDDDto>>(_thirdParty);
            return ret;
        }

        // Third Party List        
        //[AbpAuthorize("General.Persoane.Acces")]
        public GetThirdPartyAccOutput ThirdPartyAccList(int thirdPartyId)
        {
            var _thirdPartyAcc = _personRepository.ThirdPartyAccList(thirdPartyId).ToList().OrderBy(f => f.BankId);

            var ret = new GetThirdPartyAccOutput { GetThirdPartyAcc = ObjectMapper.Map<List<ThirdPartyAccListDto>>(_thirdPartyAcc) };
            return ret;
        }

        public List<ThirdPartyAccListDDDto> AppClientBankAccountList()
        {
            var thirdPartyId = GetCurrentTenant().LegalPersonId;
            var ret = _personRepository.BankAccountsForThirdParty(thirdPartyId.Value).ToList()
                                .Select(f => new ThirdPartyAccListDDDto { Id = f.Id, Account = f.Bank.LegalPerson.FullName + " - " + f.IBAN })
                                .ToList();
            return ret;
        }

        //[AbpAuthorize("General.Persoane.Acces")]
        public PersonEditDto GetPersonById(int id)
        {
            //var p = _personRepository.Single(p => p.Id == id);

            var _naturalPerson = new NaturalPerson();
            var _legalPerson = new LegalPerson();
            var ret = new PersonEditDto();
            _naturalPerson = _personRepository.GetNaturalPersonById(id);
            if (_naturalPerson != null) // e natural Person
            {
                ret = ObjectMapper.Map<PersonEditDto>(_naturalPerson);
                ret.PersonType = "NP";
            }
            else
            {
                _legalPerson = _personRepository.GetLegalPersonById(id);
                ret = ObjectMapper.Map<PersonEditDto>(_legalPerson);
                ret.PersonType = "LP";
            }

            return ret;
        }

        [AbpAuthorize("General.Persoane.Modificare")]
        public PersonEditDto SavePerson(PersonEditDto person)
        {
            try
            {

                var count = _personRepository.GetAll().Count(f => f.Id1 == person.Id1 && f.Id != person.Id);
                if (count != 0)
                {
                    throw new UserFriendlyException("Eroare", "Exista o alta persoana introdusa care are CNP/CIF: " + person.Id1);
                }

                if (person.IsNaturalPerson == true) // Natural Person
                {
                    if (person.FirstName == null || person.LastName == null)
                    {
                        throw new UserFriendlyException("Eroare", "Numele si prenumele sunt obigatorii");
                    }
                    var _person = ObjectMapper.Map<NaturalPerson>(person);
                    if (_person.Id == 0)
                    {

                        _personRepository.Insert(_person);
                        CurrentUnitOfWork.SaveChanges();
                        var thirdParty = new ThirdPartyEditDto
                        {
                            PersonId = _person.Id,
                            IsClient = true,
                            IsOther = true,
                            IsProvider = true
                        };
                        person = ObjectMapper.Map<PersonEditDto>(_person);
                    }
                    else
                    {
                        var oldPerson = _naturalPersonRepository.Get(_person.Id);
                        ObjectMapper.Map<PersonEditDto, Person>(person, oldPerson);
                        person = ObjectMapper.Map<PersonEditDto>(oldPerson);
                    }

                }
                else //Legal Person
                {
                    if (person.Name == null)
                    {
                        throw new UserFriendlyException("Eroare", "Numele este obigatoriu");
                    }
                    var _person = ObjectMapper.Map<LegalPerson>(person);

                    if (_person.Id == 0)
                    {

                        _personRepository.Insert(_person);
                        CurrentUnitOfWork.SaveChanges();
                        var thirdParty = new ThirdPartyEditDto
                        {
                            PersonId = _person.Id,
                            IsClient = true,
                            IsOther = true,
                            IsProvider = true
                        };

                        person = ObjectMapper.Map<PersonEditDto>(_person);

                    }
                    else
                    {
                        var oldPerson = _legalPersonRepository.Get(_person.Id);
                        ObjectMapper.Map<PersonEditDto, Person>(person, oldPerson);
                        //_personRepository.Update(_person);
                        person = ObjectMapper.Map<PersonEditDto>(oldPerson);
                    }
                }
                return person;
            }
            catch (Exception ex)
            {
                var errText = ex.Message + " " + ((Abp.UI.UserFriendlyException)ex).Details;
                throw new UserFriendlyException("Eroare", errText);
            }

        }

        //[AbpAuthorize("General.Persoane.Acces")]
        public ThirdPartyAccEditDto GetBankAccById(int id)
        {
            var _bank = _personRepository.GetBankAccById(id);
            var ret = ObjectMapper.Map<ThirdPartyAccEditDto>(_bank);
            return ret;
        }

        [AbpAuthorize("General.Persoane.Modificare")]
        public void SaveThirdPartyAcc(ThirdPartyAccEditDto thirdParty)
        {
            try
            {
                var _bank = ObjectMapper.Map<BankAccount>(thirdParty);

                // _bank.PersonId = GetCurrentTenant().LegalPersonId.Value;
                _bank.PersonId = thirdParty.ThirdPartyId;
                if (_bank.Id == 0)
                {
                    _personRepository.BankAccInsertOrUpdate(_bank); // INSERT
                }
                else
                {
                    _personRepository.BankAccInsertOrUpdate(_bank); //UPDATE
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString
                    ());
            }
        }

        //[AbpAuthorize("General.CursValutar.Acces")]
        public ExchangeRateModelDto ExchangeRateInit()
        {
            var ret = new ExchangeRateModelDto();
            var currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ret.StartDate = currDate.AddMonths(-1);
            ret.EndDate = currDate;

            ret = SearchExchangeRates(ret);
            return ret;
        }

        public ExchangeRateModelDto SearchExchangeRates(ExchangeRateModelDto model)
        {
            var list = _exchangeRatesRepository.GetAllIncluding(f => f.Currency)
                                               .Where(f => f.ExchangeDate >= model.StartDate && f.ExchangeDate <= model.EndDate
                                                      && f.CurrencyId == (model.CurrencyId ?? f.CurrencyId))
                                               .Select(f => new ExchangeRateDto { Id = f.Id, ExchangeDate = f.ExchangeDate, CurrencyCode = f.Currency.CurrencyCode, Value = f.Value })
                                               .OrderByDescending(f => f.ExchangeDate).ThenBy(f => f.CurrencyCode)
                                               .ToList();
            model.ExchangeList = list;
            return model;
        }

        [AbpAuthorize("General.CursValutar.Modificare")]
        public void ExchangeRateAdd(ExchangeRateDto exchangeRate)
        {
            var exchangeRateDb = _exchangeRatesRepository.GetAll().Select(f => new { f.ExchangeDate, CurrencyId = (int?)f.CurrencyId }).ToList();

            if (exchangeRate.Id == 0)
            {
                if (!exchangeRateDb.Contains(new { exchangeRate.ExchangeDate, exchangeRate.CurrencyId }))
                {
                    var ret = ObjectMapper.Map<ExchangeRates>(exchangeRate);
                    _exchangeRatesRepository.Insert(ret);
                }
                else
                {
                    throw new UserFriendlyException("Valuta deja introdusa la data de:" + exchangeRate.ExchangeDate);
                }
            }

            else
            {
                var ret = ObjectMapper.Map<ExchangeRates>(exchangeRate);
                _exchangeRatesRepository.Update(ret);
            }

            CurrentUnitOfWork.SaveChanges();
        }

        [AbpAuthorize("General.CursValutar.Modificare")]
        public void ExchangeRateDetele(int Id)
        {
            _exchangeRatesRepository.Delete(Id);
            CurrentUnitOfWork.SaveChanges();
        }

        [AbpAuthorize("General.CursValutar.Modificare")]
        public ExchangeRateDto ExchangeRateId(int id)
        {
            var exchangeRate = _exchangeRatesRepository.Get(id);
            var ret = ObjectMapper.Map<ExchangeRateDto>(exchangeRate);

            return ret;

        }
        public List<BankListDto> BanksForThirdParty(int thirdPartyId)
        {
            var ret = _personRepository.BanksForThirdParty(thirdPartyId).ToList()
                                       .OrderBy(f => f.LegalPerson.Name)
                                       .Select(f => new BankListDto { Id = f.Id, BankName = f.LegalPerson.Name })
                                       .ToList();

            return ret;
        }

        public List<ThirdPartyAccListDto> BankAccountsForThirdParty(int thirdPartyId, int? bankId, int currencyId)
        {
            var ret = new List<ThirdPartyAccListDto>();
            if (bankId == null)
            {
                return ret;
            }
            ret = _personRepository.BankAccountsForThirdParty(thirdPartyId, bankId.Value)
                                       .Where(f => f.CurrencyId == currencyId)
                                       .OrderBy(f => f.IBAN)
                                       .Select(f => new ThirdPartyAccListDto { Id = f.Id, Iban = f.IBAN })
                                       .ToList();

            return ret;
        }

        public List<ThirdPartyListDto> ThirdPartyListDD(string search)
        {
            string _search = search ?? "";
            //int selectedAppClientId = GetCurrentAppClientId();
            var _thirdParty = _personRepository.ThirdPartyList()
                                 .ToList()
                                 .Where(g => String.Concat(g.Id1, ".", g.FullName).ToUpper().StartsWith(_search.ToUpper()))
                                 .ToList()
                                 .OrderBy(f => f.FullName).ToList()
                                 .Take(50);
            var ret = ObjectMapper.Map<List<ThirdPartyListDto>>(_thirdParty);

            var list = _personRepository.ThirdPartyList().ToList();
            var list1 = list.Where(g => String.Concat(g.Id1, ".", g.FullName).ToUpper().StartsWith(_search.ToUpper())).ToList();


            return ret;
        }

        public string GetCurrentAppClientName()
        {
            string rez = "";

            try
            {
                var appClient = GetCurrentTenant();
                rez = appClient.LegalPerson.FullName;

                return rez;
            }
            catch
            {
                return rez;
            }
        }

        public List<PersonListDto> SearchPerson(string search)
        {
            try
            {
                var _search = search ?? "";
                var _personList = _personRepository.GetAll().Where(f => f.Id1.Contains(_search.ToUpper()));
                var _list = _personList.ToList();
                var ret = ObjectMapper.Map<List<PersonListDto>>(_list);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("General.Persoane.Modificare")]
        public void DeletePerson(int id)
        {
            try
            {
                _personRepository.VerifyPersonDelete(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BankAccountDto> BankAccountsForExchangeValuta(int bankValutaId, int currencyId)
        {
            var appClient = GetCurrentTenant();
            var bankAccounts = new List<BankAccount>();
            if (currencyId == appClient.LocalCurrencyId)
            {
                bankAccounts = _personRepository.BankAccountsForExchange(appClient.Id).Where(f => f.CurrencyId != currencyId && f.BankId == bankValutaId).ToList();
            }
            else
            {
                bankAccounts = _personRepository.BankAccountsForExchange(appClient.Id).Where(f => f.CurrencyId == currencyId && f.BankId == bankValutaId).ToList();

            }
            var ret = ObjectMapper.Map<List<BankAccountDto>>(bankAccounts);
            return ret;
        }

        public List<BankAccountDto> BankAccountsForExchangeLei(int bankLeiId)
        {
            var appClient = GetCurrentTenant();
            var localCurrencyId = 1;
            var bankAccounts = _personRepository.BankAccountsForExchange(appClient.Id).Where(f => f.BankId == bankLeiId && f.CurrencyId == localCurrencyId && f.PersonId == appClient.LegalPersonId).ToList();
            var ret = ObjectMapper.Map<List<BankAccountDto>>(bankAccounts);
            return ret;
        }

        public List<BankListDto> BankForExchangeLei()
        {
            try
            {
                var appClient = GetCurrentTenant();
                var bankList = _personRepository.BankExchangeLeiList().Where(f => f.TenantId == appClient.Id).Select(f => new BankListDto { Id = f.Id, BankName = f.LegalPerson.Name }).ToList();

                return bankList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BankListDto> BankForExchangeValuta(int? currencyId)
        {
            try
            {
                var bankListDto = new List<BankListDto>();
                var appClient = GetCurrentTenant();
                if (currencyId != null)
                {
                    var bankListDb = _personRepository.BankExchangeValutaList(currencyId.Value).Where(f => f.TenantId == appClient.Id).ToList();
                    bankListDto = ObjectMapper.Map<List<BankListDto>>(bankListDb);
                }

                return bankListDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void ActualizarePerson()
        {
            try
            {
                var appClient = GetCurrentTenant();
                var angajatiExternList = _angajatiExternManager.GetAngajatiList();

                foreach (var item in angajatiExternList.Where(f => f.IdCompartiment != 0).Select(f => new { f.IdCompartiment, f.Compartiment }).Distinct())
                {
                    var compartiment = _departamentRepository.GetAll().FirstOrDefault(f => f.IdExtern == item.IdCompartiment);
                    if (compartiment != null)
                    {
                        _departamentRepository.Update(compartiment);
                    }
                    else
                    {
                        var newCompartiment = new Departament()
                        {
                            IdExtern = item.IdCompartiment,
                            Name = item.Compartiment,
                            TenantId = appClient.Id
                        };
                        _departamentRepository.Insert(newCompartiment);
                    }

                    CurrentUnitOfWork.SaveChanges();
                }

                foreach (var personExtern in angajatiExternList)
                {
                    var compartiment = _departamentRepository.GetAll().Where(f => f.IdExtern == personExtern.IdCompartiment).FirstOrDefault();
                    int compartimentId = 0;
                    if (compartiment != null)
                    {
                        compartimentId = compartiment.Id;
                    }
                    var person = _naturalPersonRepository.GetAll().FirstOrDefault(f => f.IdPersonal == personExtern.Id);
                    if (person != null)
                    {
                        person.Id1 = personExtern.Cnp;
                        person.Id2 = personExtern.CI;
                        person.FirstName = personExtern.Prenume;
                        person.LastName = personExtern.Nume;

                        CurrentUnitOfWork.SaveChanges();

                        if (compartimentId != 0)
                        {
                            var salariatDep = _salariatiDepartamenteRepository.GetAllIncluding(f => f.Person, f => f.Departament).FirstOrDefault(f => f.PersonId == person.Id);

                            if (salariatDep != null)
                            {
                                _salariatiDepartamenteRepository.Update(salariatDep);
                            }
                            else
                            {
                                salariatDep = new SalariatiDepartamente()
                                {
                                    PersonId = person.Id,
                                    DepartamentId = compartimentId
                                };

                                _salariatiDepartamenteRepository.Insert(salariatDep);
                                CurrentUnitOfWork.SaveChanges();
                            }
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        var naturalPerson = new NaturalPerson
                        {
                            IdPersonal = personExtern.Id,
                            Id1 = personExtern.Cnp,
                            Id2 = personExtern.CI,
                            FirstName = personExtern.Prenume,
                            LastName = personExtern.Nume,
                            IsEmployee = true,
                            TenantId = appClient.Id
                        };

                        _naturalPersonRepository.Insert(naturalPerson);
                        CurrentUnitOfWork.SaveChanges();

                        if (compartimentId != 0)
                        {
                            // salvez PersonId si CompartimentId in table SalariatiDepartamente
                            var salariatiDep = new SalariatiDepartamente()
                            {
                                PersonId = naturalPerson.Id,
                                DepartamentId = compartimentId
                            };

                            _salariatiDepartamenteRepository.Insert(salariatiDep);
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }

        }

        //[AbpAuthorize("General.Persoane.Acces")]
        public PersonInitForm InitForm()
        {
            var ret = new PersonInitForm
            {
                PersonList = new List<PersonListDto>()
            };

            ret.PersonList = SearchPersonList(ret);

            return ret;
        }

        public List<PersonListDto> SearchPersonList(PersonInitForm person)
        {
            try
            {
                var list = _personRepository.GetAll().ToList().OrderBy(f => f.FullName).ToList();

                if ((person.Id1 != null) && (person.Id1 != ""))
                {
                    list = list.Where(f => f.Id1 == person.Id1).ToList();
                }
                if ((person.FullName != null) && (person.FullName != ""))
                {
                    list = list.Where(f => f.FullName.ToUpper().IndexOf(person.FullName.ToUpper()) >= 0).ToList();
                }

                var ret = ObjectMapper.Map<List<PersonListDto>>(list);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public GetThirdPartyOutput SearchPersonForInvObjectByInput(string search)
        {
            var ret = new GetThirdPartyOutput { GetThirdParty = null };

            if (search != null)
            {
                var _thirdParty = _personRepository.ThirdPartyList().ToList().Where(s => s.FullName.ToUpper().Contains(search.ToUpper()) && s.IsEmployee == true).OrderBy(f => f.FullName);

                ret = new GetThirdPartyOutput { GetThirdParty = ObjectMapper.Map<List<ThirdPartyListDto>>(_thirdParty) };
            }

            return ret;
        }
    }
}