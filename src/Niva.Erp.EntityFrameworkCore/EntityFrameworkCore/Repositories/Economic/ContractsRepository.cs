using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Economic
{
    public class ContractsRepository : ErpRepositoryBase<Contracts, int>, IContractsRepository
    {
        public ContractsRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider) { }
        public void InsertContractState(int contractId, DateTime operationDate, Contract_State state, string comentariu)
        {
            var Contract = Context.Contracts.FirstOrDefault(f => f.Id == contractId);
            var ContractState = new Contracts_State()
            {
                ContractsId = contractId,
                OperationDate = operationDate,
                Contract_State = state,
                Comentarii = comentariu,

            };
            Context.Contracts_State.Add(ContractState);
            UnitOfWorkManager.Current.SaveChanges();
        }
    }
}
