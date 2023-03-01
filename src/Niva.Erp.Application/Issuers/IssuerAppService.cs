using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.Nomenclatures.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Emitenti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Niva.Erp.Issuers
{
    public interface IIssuerAppService : IApplicationService
    {
        void SaveIssuer(IssuerDto issuerDto);
        IssuerDto GetIssuerByPersonId(int id);
        List<IssuerListDto> GetIssuerList();
        void DeleteIssuer(int id);

        PersonIssuerEditDto PersonIssuerInit(int? issuerId);
        PersonIssuerEditDto ShowForm(PersonIssuerEditDto personIssuerEdit, int formNr);
        void Save(PersonIssuerEditDto personIssuerEdit);
        PersonIssuerEditDto GetPersonIssuerByPersonId(int personId);
        void ActualizeazaIssuer();
    }
    public class IssuerAppService : ErpAppServiceBase, IIssuerAppService
    {
        IPersonRepository _personRepository;
        IRepository<LegalPerson> _legalPersonRepository;
        IIssuerRepository _issuerRepository;
        IEmitentiExternManager _emitentExternManager;
        IRepository<BNR_Sector> _bnrSectorRepository;

        public IssuerAppService(IIssuerRepository issuerRepository, IPersonRepository personRepository, IRepository<LegalPerson> legalPersonRepository, IEmitentiExternManager emitentExternManager,
                                IRepository<BNR_Sector> bnrSectorRepository)
        {
            _issuerRepository = issuerRepository;
            _personRepository = personRepository;
            _legalPersonRepository = legalPersonRepository;
            _emitentExternManager = emitentExternManager;
            _bnrSectorRepository = bnrSectorRepository;
        }

        public IssuerDto GetIssuerByPersonId(int id)
        {
            var issuerDb = _issuerRepository.FirstOrDefault(f => f.LegalPersonId == id);
            var ret = ObjectMapper.Map<IssuerDto>(issuerDb);

            return ret;
        }

        //[AbpAuthorize("General.Emitenti.Acces")]
        public List<IssuerListDto> GetIssuerList()
        {
            var _issuers = _issuerRepository.GetAllIncluding(f => f.BNR_Sector).ToList();

            var _persons = _personRepository.GetAllIncluding();

            var issuerList = new List<IssuerListDto>();
            foreach (var item in _issuers)
            {
                foreach (var person in _persons)
                {
                    if (item.LegalPersonId == person.Id)
                    {
                        var issuer = new IssuerListDto()
                        {
                            FullName = person.FullName,
                            Id = item.Id,
                            Id1 = person.Id1,
                            Id2 = person.Id2,
                            IssuerType = item.IssuerType.ToString(),
                            CodStatistic = item.BNR_Sector != null ? item.BNR_Sector.Denumire : ""
                        };
                        issuerList.Add(issuer);
                    }
                }
            }
            return issuerList.OrderBy(f => f.FullName).ToList();
        }

        public PersonIssuerEditDto ShowForm(PersonIssuerEditDto personIssuerEdit, int formNr)
        {
            personIssuerEdit.ShowForm1 = (formNr == 1); // cautare persoana
            personIssuerEdit.ShowForm2 = (formNr == 2); // adaugare persoana daca nu exista
            personIssuerEdit.ShowForm3 = (formNr == 3); // adaugare emitent
            return personIssuerEdit;
        }

        public PersonIssuerEditDto PersonIssuerInit(int? issuerId)
        {
            try
            {
                PersonIssuerEditDto personIssuer;
                if ((issuerId ?? 0) == 0)
                {
                    personIssuer = new PersonIssuerEditDto();
                    personIssuer.PersonType = "LP";

                    personIssuer.IssuerDetails = new IssuerDetailsDto();
                    personIssuer = ShowForm(personIssuer, 1);
                }
                else // EDIT
                {
                    var issuer = _issuerRepository.GetAllIncluding(f => f.BNR_Sector).FirstOrDefault(f => f.Id == issuerId);
                    var issuerDetails = ObjectMapper.Map<IssuerDetailsDto>(issuer);

                    var person = _personRepository.GetAllIncluding(f => f.AddressRegion).FirstOrDefault(f => f.Id == issuer.LegalPersonId);
                    personIssuer = ObjectMapper.Map<PersonIssuerEditDto>(person);
                    personIssuer.PersonType = "LP";

                    personIssuer.IssuerDetails = issuerDetails;

                    personIssuer = ShowForm(personIssuer, 2);
                }
                return personIssuer;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void SaveIssuer(IssuerDto issuerDto)
        {
            try
            {
                var issuer = ObjectMapper.Map<Issuer>(issuerDto);
                var appClient = GetCurrentTenant();
                issuer.LegalPersonId = (int)appClient.LegalPersonId;

                _issuerRepository.InsertOrUpdate(issuer);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("General.Emitenti.Modificare")]
        public void Save(PersonIssuerEditDto personIssuerEdit)
        {
            try
            {
                if (personIssuerEdit.Id1 != null)
                {
                    var count = _personRepository.GetAll().Count(f => f.Id1 == personIssuerEdit.Id1 &&
                                                                f.Id != personIssuerEdit.Id);
                    if (count != 0)
                    {
                        throw new Exception("Exista o alta persoana introdusa care are CNP/CIF: " + personIssuerEdit.Id1);
                    }
                }

                var _person = ObjectMapper.Map<LegalPerson>(personIssuerEdit);

                if (_person.Id == 0)
                {
                    _personRepository.Insert(_person);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    var oldPerson = _legalPersonRepository.Get(_person.Id);
                    ObjectMapper.Map<PersonIssuerEditDto, Person>(personIssuerEdit, oldPerson);
                }

                var issuer = ObjectMapper.Map<Issuer>(personIssuerEdit.IssuerDetails);
                issuer.LegalPersonId = _person.Id;

                if (issuer.Id == 0)
                {

                    _issuerRepository.IssuerInsertOrUpdate(issuer); // INSERT
                }
                else
                {
                    _issuerRepository.IssuerInsertOrUpdate(issuer); // UPDATE
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        [AbpAuthorize("General.Emitenti.Modificare")]
        public void DeleteIssuer(int id)
        {
            try
            {
                var _issuer = _issuerRepository.FirstOrDefault(f => f.Id == id);
                _issuerRepository.Delete(_issuer);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public PersonIssuerEditDto GetPersonIssuerByPersonId(int personId)
        {
            try
            {
                var person = _personRepository.GetLegalPersonById(personId);


                var ret = ObjectMapper.Map<PersonIssuerEditDto>(person);
                ret.PersonType = "LP";

                var issuer = _issuerRepository.FirstOrDefault(f => f.LegalPersonId == person.Id);

                if (issuer == null)
                {
                    ret.IssuerDetails = new IssuerDetailsDto();
                }
                else
                {
                    var issuerDto = ObjectMapper.Map<IssuerDetailsDto>(issuer);
                    ret.IssuerDetails = issuerDto;
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void ActualizeazaIssuer()
        {
            try
            {
                var appClient = GetCurrentTenant();
                var issuerExternList = _emitentExternManager.GetIssuerList();

                foreach (var item in issuerExternList)
                {
                    var countryAbvr = item.CodTara == "ROU" ? "ROM" : "";
                    var country = _personRepository.CountryList().FirstOrDefault(f => f.CountryAbrv == countryAbvr);
                    var person = _legalPersonRepository.GetAll().Where(f => f.IdBanci == item.Id/* && f.Name == item.DenumireBanca*/).FirstOrDefault();
                    int personId = 0;
                    if (person != null)
                    {
                        personId = person.Id;
                        person.Name = item.DenumireBanca;
                        person.AddressCountryId = country?.Id;

                        CurrentUnitOfWork.SaveChanges();

                    }
                    else
                    {
                        var legalPerson = new LegalPerson()
                        {
                            Name = item.DenumireBanca,
                            IdBanci = item.Id,
                            TenantId = appClient.Id,
                            IsVATPayer = false,
                            IsVATCollector = false,

                        };

                        _legalPersonRepository.Insert(legalPerson);
                        CurrentUnitOfWork.SaveChanges();
                        personId = legalPerson.Id;
                    }

                    var issuer = _issuerRepository.GetAllIncluding(f => f.LegalPerson)
                                                  .Where(f => f.LegalPersonId == personId)// f.LegalPerson.Name == item.DenumireBanca && f.IssuerType == IssuerType.Banca)
                                                  .FirstOrDefault();

                    var bnrSector = _bnrSectorRepository.GetAll().FirstOrDefault(f => f.Sector == item.CodStatistica);
                    if (bnrSector == null)
                    {
                        throw new Exception("Nu am identificat sectorul cu codul " + item.CodStatistica);
                    }

                    if (issuer != null)
                    {
                        //_legalPersonRepository.Update(issuer.LegalPerson);
                        //CurrentUnitOfWork.SaveChanges();

                        issuer.Bic = item.Swift;
                        issuer.IbanAbrv = item.Bic;
                        issuer.IssuerType = IssuerType.Banca;
                        issuer.BNR_SectorId = bnrSector.Id;

                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        //var legalPerson = _legalPersonRepository.GetAll().Where(f => f.Name == item.DenumireBanca).FirstOrDefault();
                        var newIssuer = new Issuer()
                        {
                            Bic = item.Swift,
                            IbanAbrv = item.Bic,
                            TenantId = appClient.Id,
                            IssuerType = IssuerType.Banca,
                            LegalPersonId = personId,
                            BNR_SectorId = bnrSector.Id
                        };

                        _issuerRepository.Insert(newIssuer);
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
