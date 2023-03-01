using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetPrevAutoAppService
    {
        List<BugetPrevAutoValueListDto> BugetPrevAutoList();
        BugetPrevAutoValueAddDto EditBugetPrevAuto(int butePrevAutoId);
        void SaveBugetPrevAuto(BugetPrevAutoValueAddDto form);
        void DeleteBugetPrevAuto(int bugetPrevAutoId);

    }
    public class BugetPrevAutoAppService : ErpAppServiceBase, IBugetPrevAutoAppService
    {
        IRepository<BVC_BugetPrevAutoValue> _bugetPrevAutoValueRepository;

        public BugetPrevAutoAppService(IRepository<BVC_BugetPrevAutoValue> bugetPrevAutoValueRepository)
        {
            _bugetPrevAutoValueRepository = bugetPrevAutoValueRepository;
        }

        [AbpAuthorize("Buget.BVC.Prevazut.ConfigValori")]
        public List<BugetPrevAutoValueListDto> BugetPrevAutoList()
        {
            try
            {
                var appClient = GetCurrentTenant();
                var bugetPrevAutoValueList = _bugetPrevAutoValueRepository.GetAllIncluding(f => f.Departament)
                                                                          .Where(f => f.TenantId == appClient.Id)
                                                                          .ToList();
            
                var ret = ObjectMapper.Map<List<BugetPrevAutoValueListDto>>(bugetPrevAutoValueList);
                
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void DeleteBugetPrevAuto(int bugetPrevAutoId)
        {
            try
            {
                var bugetPRevAutoValue = _bugetPrevAutoValueRepository.GetAll().FirstOrDefault(f => f.Id == bugetPrevAutoId);
                _bugetPrevAutoValueRepository.Delete(bugetPRevAutoValue);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public BugetPrevAutoValueAddDto EditBugetPrevAuto(int bugetPrevAutoId)
        {
            try
            {
                var appClient = GetCurrentTenant();
                BugetPrevAutoValueAddDto bugetPrevAuto;

                if (bugetPrevAutoId == 0)
                {
                    bugetPrevAuto = new BugetPrevAutoValueAddDto
                    {
                        DepartamentId = null,
                        TipRandId = null,
                        TipRandVenitId = null
                    };
                }
                else
                {
                    var bugetPrevAutoDb = _bugetPrevAutoValueRepository.GetAllIncluding(f => f.Departament)
                                                                       .FirstOrDefault(f => f.Id == bugetPrevAutoId && f.TenantId == appClient.Id);
                    bugetPrevAuto = ObjectMapper.Map<BugetPrevAutoValueAddDto>(bugetPrevAutoDb);
                }

                return bugetPrevAuto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void SaveBugetPrevAuto(BugetPrevAutoValueAddDto form)
        {
            try
            {
                var bugetPrevAutoValueDb = ObjectMapper.Map<BVC_BugetPrevAutoValue>(form);
                if (bugetPrevAutoValueDb.Id == 0) //INSERT
                {
                    _bugetPrevAutoValueRepository.Insert(bugetPrevAutoValueDb);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    var appClient = GetCurrentTenant();
                    bugetPrevAutoValueDb.TenantId = appClient.Id;
                    _bugetPrevAutoValueRepository.Update(bugetPrevAutoValueDb);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
