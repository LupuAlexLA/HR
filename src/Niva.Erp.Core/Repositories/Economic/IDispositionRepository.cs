using Abp.Domain.Repositories;
using Niva.Erp.Models.Economic.Casierii;
using System;

namespace Niva.Erp.Repositories.Economic
{
    public interface IDispositionRepository: IRepository<Disposition, int>
    {
        decimal SoldPrec(DateTime previousDate, int currencyId);
        int GetNextNumber(DateTime date);
        void InsertOrUpdateV(Disposition disp);
    }
}
