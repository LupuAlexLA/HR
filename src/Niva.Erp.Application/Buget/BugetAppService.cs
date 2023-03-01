using Abp.Application.Services;

namespace Niva.Erp.Buget
{
    public interface IBugetAppService : IApplicationService
    {

    }

    public class BugetAppService : ErpAppServiceBase, IBugetAppService
    {
    }
}
