using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IActivityTypeAppService : IApplicationService
    {
        List<ActivityTypeDto> ActivityTypeList();

        void SaveActivityType(ActivityTypeDto activityType);
        ActivityTypeDto ActivityTypeEditInit(int activityTypeId);
        void DeleteActivityType(int activityTypeId);

    }
    public class ActivityTypeAppService : ErpAppServiceBase, IActivityTypeAppService
    {
        IRepository<ActivityType> _activityTypeRepository;
        IAccountRepository _accountRepository;

        public ActivityTypeAppService(IRepository<ActivityType> activityTypeRepository, IAccountRepository accountRepository)
        {
            _activityTypeRepository = activityTypeRepository;
            _accountRepository = accountRepository;
        }
        
        //[AbpAuthorize("Admin.Conta.TipActivitate.Acces")]
        public ActivityTypeDto ActivityTypeEditInit(int activityTypeId)
        {
            var ret = new ActivityTypeDto();
            ActivityType _activityType;
            if (activityTypeId == 0)
            {
                _activityType = new ActivityType();
                _activityType.Status = State.Active;
            }
            else
            {
                _activityType = _activityTypeRepository.FirstOrDefault(f => f.Id == activityTypeId);
            }

            ret = ObjectMapper.Map<ActivityTypeDto>(_activityType);
            return ret;
        }
        
        //[AbpAuthorize("Admin.Conta.TipActivitate.Acces")]
        public List<ActivityTypeDto> ActivityTypeList()
        {
            var _activityTypes = _activityTypeRepository.GetAll().Where(f => f.Status == State.Active);

            var ret = ObjectMapper.Map<List<ActivityTypeDto>>(_activityTypes);
            return ret;
        }
        
        //[AbpAuthorize("Admin.Conta.TipActivitate.Acces")]
        public void DeleteActivityType(int activityTypeId)
        {
            try
            {
                var activityType = _activityTypeRepository.FirstOrDefault(f => f.Id == activityTypeId);
                var count = _accountRepository.GetAllIncluding(f => f.ActivityType).Count(f => f.ActivityTypeId == activityTypeId);
                if (count != 0)
                {
                    throw new Exception("Tipul de activitate nu poate fi sters, deoarece este folosit la definirea planului de cont");
                }
                else
                {
                    activityType.Status = State.Inactive;
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
      
        //[AbpAuthorize("Admin.Conta.TipActivitate.Acces")]
        public void SaveActivityType(ActivityTypeDto activityType)
        {
            // check existing activity type
            var count = _activityTypeRepository.GetAll().Count(f => f.Status == State.Active &&
                                                                    f.ActivityName == activityType.Name &&
                                                                    f.Id != activityType.Id);

            if (count != 0)
                throw new UserFriendlyException("Eroare", "Este definit un al tip de activitate cu acelasi nume");

            try
            {
                var _activityType = ObjectMapper.Map<ActivityType>(activityType);
                var appClient = GetCurrentTenant();

                _activityType.TenantId = appClient.Id;

                if (_activityType.Id == 0)
                {
                    _activityTypeRepository.Insert(_activityType); //INSERT
                }
                else
                {
                    _activityTypeRepository.Update(_activityType); // UPDATE
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
