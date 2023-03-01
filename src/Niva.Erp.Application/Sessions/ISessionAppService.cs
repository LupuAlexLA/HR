using System.Threading.Tasks;
using Abp.Application.Services;
using Niva.Erp.Sessions.Dto;

namespace Niva.Erp.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
