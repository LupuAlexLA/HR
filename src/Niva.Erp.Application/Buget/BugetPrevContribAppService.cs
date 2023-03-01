using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetPrevContribAppService : IApplicationService
    {
        List<BugetPrevContribListDto> GetContribList();
        BugetPrevContribAddDto GetContribById(int? contribId);
        void SaveContrib(BugetPrevContribAddDto contrib);
        void DeleteContrib(int contribId);
    }
    public class BugetPrevContribAppService : ErpAppServiceBase, IBugetPrevContribAppService
    {
        public IRepository<BVC_BugetPrevContributie> _bugetPrevContribRepository;

        public BugetPrevContribAppService(IRepository<BVC_BugetPrevContributie> bugetPrevContribRepository)
        {
            _bugetPrevContribRepository = bugetPrevContribRepository;
        }

        [AbpAuthorize("Buget.BVC.Prevazut.ContribAlteIncasari")]
        public void DeleteContrib(int contribId)
        {
            try
            {
                var contrib = _bugetPrevContribRepository.GetAll().FirstOrDefault(f => f.Id == contribId);
                _bugetPrevContribRepository.Delete(contrib);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.ContribAlteIncasari")]
        public BugetPrevContribAddDto GetContribById(int? contribId)
        {
            try
            {


                BugetPrevContribAddDto contribDto;

                if (contribId == 0)// INSERT
                {
                    contribDto = new BugetPrevContribAddDto
                    {
                        ActivityTypeId = null,
                        CurrencyId = null,
                        DataIncasare = LazyMethods.Now()
                    };
                }
                else
                {
                    var contribDB = _bugetPrevContribRepository.GetAllIncluding(f => f.ActivityType, f => f.BankAccount, f => f.Currency).FirstOrDefault(f => f.Id == contribId);
                    contribDto = ObjectMapper.Map<BugetPrevContribAddDto>(contribDB);
                }
                return contribDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.ContribAlteIncasari")]
        public List<BugetPrevContribListDto> GetContribList()
        {
            try
            {
                var list = _bugetPrevContribRepository.GetAllIncluding(f => f.ActivityType, f => f.BankAccount, f => f.BankAccount.LegalPerson, f => f.Currency)
                                                      .OrderBy(f => f.DataIncasare)
                                                      .ToList();
                var ret = ObjectMapper.Map<List<BugetPrevContribListDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.ContribAlteIncasari")]
        public void SaveContrib(BugetPrevContribAddDto contrib)
        {
            try
            {
                var contribDb = ObjectMapper.Map<BVC_BugetPrevContributie>(contrib);
                var appClient = GetCurrentTenant();

                if (contribDb.Id == 0)
                {
                    _bugetPrevContribRepository.Insert(contribDb);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    contribDb.TenantId = appClient.Id;
                    _bugetPrevContribRepository.Update(contribDb);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
