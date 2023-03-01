using Abp.Application.Services;

namespace Niva.Erp.Conta.Balance
{
    public interface IInventoryBalanceAppService: IApplicationService
    {

    }

    public class InventoryBalanceAppService: ErpAppServiceBase, IInventoryBalanceAppService
    {

    }
}
