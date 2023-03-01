using Abp.Domain.Repositories;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Models.Administration;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Repositories
{
    public interface IOperationRepository : IRepository<Operation, int>
    {
        bool VerifyClosedMonth(DateTime date);

        bool OperationCheck(Operation operation, OperationType operationType, int? localCurId);
         IQueryable<Operation> GetAllIncludingOperationDetails();
        IQueryable<User> GetUsers();
    }

}
