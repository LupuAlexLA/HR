using Abp.Domain.Repositories;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Economic
{
    public interface IContractsRepository : IRepository<Contracts, int>
    {

        void InsertContractState(int contractId, DateTime operationDate, Contract_State state, string comentariu);


    }
}
