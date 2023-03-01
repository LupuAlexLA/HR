using System.Threading.Tasks;
using Niva.Erp.Configuration.Dto;

namespace Niva.Erp.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
