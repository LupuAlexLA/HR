using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Conta.Operation
{
    public interface IForeignOperationRepository : IRepository<ForeignOperation, int>
    {
        void DeleteNC(int foreignOperId);

        void UpdateFgnOperationDetail(ForeignOperationsDetails detail);
        void DeleteFO(int foreignOperationId);
    }
}
