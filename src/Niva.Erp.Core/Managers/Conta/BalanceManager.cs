using Abp.Domain.Services;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta;

namespace Niva.Erp.Managers.Conta
{
    public class BalanceManager: DomainService
    {
        ISavedBalanceRepository _savedBalanceRepository;

        public BalanceManager(ISavedBalanceRepository savedBalanceRepository)
        {
            _savedBalanceRepository = savedBalanceRepository;
        }

        public SavedBalance AddSavedBalance(Balance balance, string balanceName, bool balantaZilnica)
        {
           var savedBalance = _savedBalanceRepository.AddSavedBalance(balance, balanceName, balantaZilnica);
            return savedBalance;
        }
    }
}
