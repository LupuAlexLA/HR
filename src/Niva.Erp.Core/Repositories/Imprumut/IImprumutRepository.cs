using Abp.Domain.Repositories;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Imprumuturi
{
    public interface IImprumutRepository : IRepository<Imprumut, int>
    {
        void GenerateDobandaOper(DateTime Data, int operGenId , int localCurrencyId);
        void GenerateDobandaConta(Imprumut Imprumut, DateTime Data, int operGenId, int localCurrencyId);
    }
}
