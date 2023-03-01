using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Niva.Erp.Roles.Dto;
using Niva.Erp.Users.Dto;

namespace Niva.Erp.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
    }
}
