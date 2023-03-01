using Abp.EntityFramework;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace Niva.EntityFramework.Repositories.Nomenclatures
{
    public class PersonRepository : ErpRepositoryBase<Person, int>, IPersonRepository
    {
        public PersonRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public IQueryable<Currency> CurrencyList()
        {
            var ret = Context.Currency.OrderBy(f => f.CurrencyCode);
            return ret;
        }

        public IQueryable<Country> CountryList()
        {
            var ret = Context.Country.OrderBy(f => f.CountryName);
            return ret;
        }

        public IQueryable<Region> RegionList(int id)
        {
            //var ret = Context.Region.Include(f => f.Country).Where(f => f.Country.Id == id).OrderBy(f => f.RegionName);
            var ret = Context.Region.Include(f => f.Country).Where(f => f.Country.Id == id).OrderBy(f => f.RegionName);
            return ret;
        }

        public IQueryable<Issuer> BankList()
        {
            var ret = Context.Issuer.Where(f=>f.IssuerType == IssuerType.Banca).Include(f => f.LegalPerson);
                            //.Where(f => f.LegalPerson.DefinedById == selectedAppClientId);            
            return ret;
        }

        public IQueryable<Person> ThirdPartyList()
        {
            var ret = Context.Persons;
                            //.Where(f => f.Person.DefinedById == selectedAppClientId);
            return ret;
        }

        public IQueryable<BankAccount> ThirdPartyAccList(int thirdPartyId)
        {
            var ret = Context.BankAccount.Include(f => f.Person)
                                        .Include(f => f.Bank.LegalPerson)
                                        .Include(f => f.Currency)
                                        .Where(f => f.PersonId == thirdPartyId);
            return ret;
        }

        public IQueryable<BankAccount> AppClientBankAccountList(int appClientId)
        {
            throw new Exception("use BanksForThirdParty using thirdparty id");
            //   var thirdPartyIdAppClient = Context.Clients.FirstOrDefault(f => f.Id == appClientId).LegalPersonId;
            //var ret = Context.BankAccount.Include(f => f.ThirdParty).Include(f=>f.Bank).Include(f => f.Bank.LegalPerson)
            //                 .Where(f => f.ThirdPartyId == thirdPartyIdAppClient);
            //return ret;
        }

        public void BankAccInsertOrUpdate(BankAccount bankObj)
        {
            var count = Context.BankAccount.Count(f => f.Id == bankObj.Id);
            BankAccount bankAccount;
            if (count == 0) // INSERT
            {
                bankAccount = new BankAccount
                {
                    IBAN = bankObj.IBAN,
                    BankId = bankObj.BankId,
                    PersonId = bankObj.PersonId,
                    CurrencyId = bankObj.CurrencyId
                };
                Context.BankAccount.Add(bankAccount);
            }
            else // UPDATE
            {
                bankAccount = Context.BankAccount.FirstOrDefault(f => f.Id == bankObj.Id);
                bankAccount.IBAN = bankObj.IBAN;
                bankAccount.BankId = bankObj.BankId;
                bankAccount.PersonId = bankObj.PersonId;
                bankAccount.CurrencyId = bankObj.CurrencyId;
            }
            Context.SaveChanges(); //SAVING ....
        }

        public void BankAccDelete(int bankAccountId)
        {
            var count = Context.Account.Count(f => f.BankAccountId == bankAccountId);
            if (count != 0)
            {
                throw new Exception("Contul bancar nu poate fi sters deoarece este folosit la definirea conturilor contabile");
            }

            var bankAccount = Context.BankAccount.FirstOrDefault(f => f.Id == bankAccountId);
            Context.BankAccount.Remove(bankAccount);
            Context.SaveChanges();
        }

        public BankAccount GetBankAccById(int id)
        {
            BankAccount bankAccount;
            bankAccount = Context.BankAccount.FirstOrDefault(f => f.Id == id);
            return bankAccount;
        }

        public NaturalPerson GetNaturalPersonById (int id)
        {
            var ret = Context.NaturalPersons.FirstOrDefault(f => f.Id == id);
            return ret;
        }

        public LegalPerson GetLegalPersonById(int id)
        {
            var ret = Context.LegalPersons.FirstOrDefault(f => f.Id == id);
            return ret;
        }

        /*public void LegalPersonInsertOrUpdate(LegalPerson p)
        {
            var count = Context.LegalPersons.Count(f => f.Id == p.Id);
            LegalPerson person;
            if (count == 0) // INSERT
            {
                person = new LegalPerson
                {

                    person = person.Id1,
                    person = person.Id2,
                    person = person.AddressStreet,
                    person = person.AddressNo,
                    person = person.AddressBlock,
                    person = person.AddressFloor,
                    person = person.AddressApartment,
                    person = person.AddressZipCode,
                    person = person.AddressCountryId,
                    person = person.Name,
                    person = person.DefinedById,
                    person = person.AddressLocality
                };
                Context.LegalPersons.Add(person);
            }
            else // UPDATE
            {
                bankAccount = Context.BankAccount.FirstOrDefault(f => f.Id == bankObj.Id);
            }
            Context.SaveChanges(); //SAVING ....
        }

        public void NaturalPersonInsertOrUpdate(NaturalPerson p)
        {
            NaturalPerson person;
        }*/

        public IQueryable<Issuer> BanksForThirdParty(int thirdPartyId)
        {
            var bankIdList = Context.BankAccount.Where(f => f.PersonId == thirdPartyId).Select(f => f.BankId).ToList();
            var ret = Context.Issuer.Include(f => f.LegalPerson).Where(f => f.IssuerType == IssuerType.Banca && bankIdList.Contains(f.Id));
            var list = ret.ToList();
            return ret;
        }

        public IQueryable<BankAccount> BankAccountsForThirdParty(int thirdPartyId, int bankId)
        {
            var ret = Context.BankAccount.Where(f => f.PersonId == thirdPartyId && f.BankId == bankId);
            return ret;
        }

        public IQueryable<BankAccount> BankAccountsForThirdParty(int thirdPartyId)
        {
            var ret = Context.BankAccount.Include(f => f.Bank.LegalPerson).Where(f => f.PersonId == thirdPartyId );
            return ret;
        }

        public void VerifyPersonDelete(int personId)
        {
            int count = 0;
            count = Context.BankAccount.Include(f => f.Bank).Count(f => f.PersonId == personId);
            if (count != 0)
            {
                throw new Exception("Persoana nu poate fi stearsa, deoarece este folosita la definirea conturilor bancare");
            }

            count = Context.Issuer.Count(f => f.LegalPersonId == personId);
            if (count != 0)
            {
                throw new Exception("Persoana nu poate fi stearsa, deoarece este folosita la definirea emitentilor");

            }

            count = Context.Account.Count(f => f.ThirdPartyId == personId);
            if (count != 0)
            {
                throw new Exception("Persoana nu poate fi stearsa, deoarece este folosita la definirea planului de conturi");
            }

            var person = Context.Persons.FirstOrDefault(f => f.Id == personId);
            Context.Persons.Remove(person);
            Context.SaveChanges();
        }

        public int GetPersonTenantId(int tenantId)
        {
            int ret = 0;
            var tenant = Context.Tenants.FirstOrDefault(f => f.Id == tenantId);
            ret = tenant.LegalPersonId.Value;
            return ret;
        }

        public IQueryable<BankAccount> BankAccountsForExchange(int appClientId)
        {
            var ret = Context.BankAccount.Include(f => f.Bank).Include(f => f.Currency).Where(f => f.TenantId == appClientId);
            return ret;
        }

        public int GetLocalCurrency(int tenantId)
        {
            int ret = 0;
            var tenant = Context.Tenants.FirstOrDefault(f => f.Id == tenantId);
            ret = tenant.LocalCurrencyId.Value;
            return ret;
        }

        public IQueryable<Issuer> BankExchangeLeiList()
        {
            var localCurrencyId = 1;
            var bankIdList = Context.BankAccount.Include(f => f.Currency).Where(f => f.CurrencyId == localCurrencyId).Select(f => f.BankId).ToList();
            var issuerList = Context.Issuer.Include(f => f.LegalPerson).Where(f => f.IssuerType == IssuerType.Banca && bankIdList.Contains(f.Id));

            return issuerList;
        }

        public IQueryable<Issuer> BankExchangeValutaList(int currencyId)
        {
            var bankIdList = Context.BankAccount.Include(f => f.Currency).Where(f => f.CurrencyId == currencyId).Select(f => f.BankId).ToList();
            var issuerList = Context.Issuer.Include(f => f.LegalPerson).Where(f => f.IssuerType == IssuerType.Banca && bankIdList.Contains(f.Id));

            return issuerList;
        }

        public Departament GetUserDeptId(long userId)
        {
            Departament rez = new Departament();
            var user = Context.Users.FirstOrDefault(f => f.Id == userId);
            if (user.AngajatId != 0)
            {
                var person = Context.Persons.FirstOrDefault(f => f.IdPersonal == user.AngajatId);
                if (person != null)
                {
                    var dept = Context.SalariatiDepartamente.FirstOrDefault(f => f.PersonId == person.Id);
                    if (dept != null)
                    {
                        rez = Context.Departament.FirstOrDefault(f=>f.Id == dept.DepartamentId);
                    }
                }
            }

            return rez;
        }
    }
}
