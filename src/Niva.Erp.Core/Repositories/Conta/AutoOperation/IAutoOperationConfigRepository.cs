using Abp.Domain.Repositories;
using Niva.Erp.Models.AutoOperation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Conta.AutoOperation
{
    public interface IAutoOperationConfigRepository : IRepository<AutoOperationConfig, int>
    {
        void SaveConfigToDb(List<AutoOperationConfig> configList);
    }
}
