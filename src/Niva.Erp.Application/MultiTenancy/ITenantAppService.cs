using Abp.Application.Services;
using Niva.Erp.MultiTenancy.Dto;

namespace Niva.Erp.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

