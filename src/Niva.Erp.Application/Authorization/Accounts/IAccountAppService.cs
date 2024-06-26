﻿using System.Threading.Tasks;
using Abp.Application.Services;
using Niva.Erp.Authorization.Accounts.Dto;

namespace Niva.Erp.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
