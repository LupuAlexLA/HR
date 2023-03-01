using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetPrevDobandaRefAppService : IApplicationService
    {
        List<BugetPrevDobandaReferintaListDto> GetDobandaRefList();
        BugetPrevDobandaReferintaEditDto GetDobandaRefById(int dobandaRefId);
        void SaveDobandaRef(BugetPrevDobandaReferintaEditDto dobandaRef);
        void DeleteDobandaRef(int dobandaRefId);
    }
    public class BugetPrevDobandaRefAppService : ErpAppServiceBase, IBugetPrevDobandaRefAppService
    {

        IRepository<BVC_DobandaReferinta> _bvcDobandaReferintaRepository;
        IRepository<BVC_Formular> _bugetConfigRepository;

        public BugetPrevDobandaRefAppService(IRepository<BVC_DobandaReferinta> bvcDobandaReferintaRepository, IRepository<BVC_Formular> bugetConfigRepository)
        {
            _bvcDobandaReferintaRepository = bvcDobandaReferintaRepository;
            _bugetConfigRepository = bugetConfigRepository;
        }

        [AbpAuthorize("Buget.BVC.Prevazut.DobanziReferinta")]
        public void DeleteDobandaRef(int dobandaRefId)
        {
            try
            {
                var dobandaRef = _bvcDobandaReferintaRepository.FirstOrDefault(f => f.Id == dobandaRefId);
                _bvcDobandaReferintaRepository.Delete(dobandaRef);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.DobanziReferinta")]
        public BugetPrevDobandaReferintaEditDto GetDobandaRefById(int dobandaRefId)
        {
            try
            {
                BugetPrevDobandaReferintaEditDto dobandaRef;
                if (dobandaRefId == 0)
                {
                    var formularId = _bugetConfigRepository.GetAll().OrderByDescending(f => f.AnBVC).FirstOrDefault().Id;

                    dobandaRef = new BugetPrevDobandaReferintaEditDto
                    {
                        PlasamentType = null,
                        FormularId = formularId,
                        State = State.Active,
                        DataStart = DateTime.Now,
                        DataEnd = DateTime.Now
                    };
                }
                else
                {
                    var dobandaRefDb = _bvcDobandaReferintaRepository.FirstOrDefault(f => f.Id == dobandaRefId);
                    dobandaRef = ObjectMapper.Map<BugetPrevDobandaReferintaEditDto>(dobandaRefDb);
                }

                return dobandaRef;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.DobanziReferinta")]
        public List<BugetPrevDobandaReferintaListDto> GetDobandaRefList()
        {
            try
            {
                var list = _bvcDobandaReferintaRepository.GetAllIncluding(f => f.Formular, f=>f.Currency, f=> f.ActivityType).OrderByDescending(f => f.Formular.AnBVC).ThenByDescending(f => f.DataStart);
                var ret = ObjectMapper.Map<List<BugetPrevDobandaReferintaListDto>>(list);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.DobanziReferinta")]
        public void SaveDobandaRef(BugetPrevDobandaReferintaEditDto dobandaRef)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var dobandaRefDb = ObjectMapper.Map<BVC_DobandaReferinta>(dobandaRef);
                if (dobandaRefDb.Id == 0)
                {
                    dobandaRefDb.TenantId = appClient.Id;
                    _bvcDobandaReferintaRepository.Insert(dobandaRefDb);
                }
                else
                {
                    _bvcDobandaReferintaRepository.Update(dobandaRefDb);
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
