using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Niva.Erp.Authorization.Roles;
using Niva.Erp.Authorization.Users;
using Niva.Erp.MultiTenancy;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using System.Linq;

namespace Niva.Erp.Authorization
{
    public class ExternalUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> ClaimsList { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }  // in cazul ca se inactiveaza userul si apoi reincearca cu link-ul din istoric browser
        public int idAngajat { get; set; }
    }

    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private readonly IAbpSession _session;
        private readonly IPermissionManager _permissionManager;
        private readonly IConfiguration _configuration;
        private readonly string fgdbApiUrl;
        public ILogger _logger { get; set; }


        public LogInManager(
            IAbpSession session,
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher,
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory,
             IPermissionManager permissionManager,
             IConfiguration configuration,
               ILogger Logger
)
            : base(
                  userManager,
                  multiTenancyConfig,
                  tenantRepository,
                  unitOfWorkManager,
                  settingManager,
                  userLoginAttemptRepository,
                  userManagementConfig,
                  iocResolver,
                  passwordHasher,
                  roleManager,
                  claimsPrincipalFactory)
        {
            _permissionManager = permissionManager;
            _session = session;
            _configuration = configuration;
            _logger = Logger;
            fgdbApiUrl = _configuration["App:FgdbUrl"];
        }

        public override Task<AbpLoginResult<Tenant, User>> LoginAsync(UserLoginInfo login, string tenancyName = null)
        {
            return base.LoginAsync(login, tenancyName);
        }
         
        public async Task<AbpLoginResult<Tenant, User>> LoginAsync(string token)
        {
            try
            {
                var externalUser = GetExternalUsersApi(token);
                externalUser.IsActive = true; 
                var tenant = await TenantRepository.SingleAsync(t => t.TenancyName == "Default");
                //using (_session.Use(tenant.Id, null))
                //{
                //UserManager.InitializeOptions(tenant.Id);
                var user = await UserManager.FindByNameAsync(externalUser.UserName);
                if (user == null)
                {
                    _logger.Info("External login new user: " + externalUser.UserName);
                    user = new User() { UserName = externalUser.UserName, TenantId = tenant.Id, EmailAddress = externalUser.Email, IsEmailConfirmed = true, IsActive = externalUser.IsActive, Name = externalUser.Name, Surname = externalUser.Surname, Password = token, AngajatId = externalUser.idAngajat };
                    try
                    {

                        var identity = await UserManager.CreateAsync(user);
                        this.UnitOfWorkManager.Current.SaveChanges();
                        if (!identity.Succeeded)
                        {
                            return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin);
                    }
                }
                _logger.Info("userID: " + user.Id);
                user.IsActive =   externalUser.IsActive;
                user.AngajatId = externalUser.idAngajat;
                await UserManager.UpdateAsync(user);
                UnitOfWorkManager.Current.SaveChanges();
                await UpdateUserPermissionsAsync(user, externalUser.ClaimsList);
                UnitOfWorkManager.Current.SaveChanges();
                user = await UserManager.FindByNameAsync(externalUser.UserName);
                var ret = await base.CreateLoginResultAsync(user, tenant);
                return ret;
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("External login error", ex);
            }
        }

        async Task UpdateUserPermissionsAsync(User user, List<string> permissions)
        {
            try
            {
                var appDefinedPermisions = _permissionManager.GetAllPermissions();

                //await UserManager.ResetAllPermissionsAsync(user);
                UnitOfWorkManager.Current.SaveChanges();
                var grantedPermissions = appDefinedPermisions.Where(s => permissions.Contains(s.Name)).ToList();
                await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
            }
            catch (Exception)
            {
                _logger.Info("UpdateUserPermissionsAsync error" + user.Id);
                throw;
            }
        }
        ExternalUser GetExternalUsersApi(string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl + @"/api/external/getUser/" + token);
            ExternalUser user;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                var serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    user = JsonConvert.DeserializeObject<ExternalUser>(responseBody);
                }
            }
            return user;
        }
    }
}
