using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Repositories.Conta.Nomenclatures
{
    public interface IPersonRepository : IRepository<Person, int>
    {
        IQueryable<Issuer> BankList();

        IQueryable<Currency> CurrencyList();

        IQueryable<Country> CountryList();

        IQueryable<Region> RegionList(int id);

        IQueryable<Person> ThirdPartyList();

        IQueryable<BankAccount> ThirdPartyAccList(int thirdPartyId);

        IQueryable<BankAccount> AppClientBankAccountList(int appClientId);

        void BankAccInsertOrUpdate(BankAccount bankObj);

        void BankAccDelete(int bankAccountId);
        void VerifyPersonDelete(int personId);

        BankAccount GetBankAccById(int id);

        NaturalPerson GetNaturalPersonById(int id);

        LegalPerson GetLegalPersonById(int id);

        IQueryable<Issuer> BanksForThirdParty(int thirdPartyId);

        IQueryable<BankAccount> BankAccountsForThirdParty(int thirdPartyId, int bankId);
        IQueryable<BankAccount> BankAccountsForThirdParty(int thirdPartyId);

        int GetPersonTenantId(int tenantId);
        IQueryable<BankAccount> BankAccountsForExchange(int appClientId);

        int GetLocalCurrency(int tenantId);
        IQueryable<Issuer> BankExchangeLeiList();
        IQueryable<Issuer> BankExchangeValutaList(int currencyId);
        Departament GetUserDeptId(long userId);
    }
}
