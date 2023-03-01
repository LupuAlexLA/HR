using Abp.Authorization;
using Niva.Erp.Authorization.Roles;
using Niva.Erp.Authorization.Users;

namespace Niva.Erp.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
