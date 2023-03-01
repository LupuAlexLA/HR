using Abp.EntityFrameworkCore;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation
{
    public class OperGenerateRepository : ErpRepositoryBase<OperGenerate, int>, IOperGenerateRepository
    {
        public OperGenerateRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            
        }

    }
}
