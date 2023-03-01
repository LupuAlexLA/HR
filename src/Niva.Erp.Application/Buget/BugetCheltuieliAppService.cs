using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Buget
{
    public interface IBugetCheltuieliAppService : IApplicationService
    {
        List<BugetCheltuieliDto> GetCheltuieliList();
        List<BugetFormRandDto> CheltuieliDisponobileList(BugetCheltuieliEditDto cheltuiala);
        BugetCheltuieliEditDto GetCheltuieliById(int? cheltId);
        void SaveCheltuieli(BugetCheltuieliEditDto chelt);
        void DeleteCheltuiala(int contribId);
    }
    public class BugetCheltuieliAppService : ErpAppServiceBase, IBugetCheltuieliAppService
    {
        public IRepository<BVC_Cheltuieli> _bugetCheltuieliRepository;
        public IRepository<BVC_FormRand> _formRandRepository;

        public BugetCheltuieliAppService(IRepository<BVC_Cheltuieli> bugetCheltuieliRepository, IRepository<BVC_FormRand> formRandRepository)
        {
            _bugetCheltuieliRepository = bugetCheltuieliRepository;
            _formRandRepository = formRandRepository;
        }

        public List<BugetFormRandDto> CheltuieliDisponobileList(BugetCheltuieliEditDto cheltuiala)
        {
            var cheltuieliInregistrate = _bugetCheltuieliRepository.GetAll().Select(f => f.BVC_FormRandId).ToList();
            var ret = _formRandRepository.GetAll().Where(f => f.IsTotal == false && !cheltuieliInregistrate.Contains(f.Id)).ToList();
            ret.Add(_formRandRepository.GetAll().Where(f => f.IsTotal == false && f.Id == cheltuiala.BVC_FormRandId).FirstOrDefault());

            return ObjectMapper.Map<List<BugetFormRandDto>>(ret);
        }


        public void DeleteCheltuiala(int contribId)
        {
            try
            {
                var contrib = _bugetCheltuieliRepository.GetAll().FirstOrDefault(f => f.Id == contribId);
                _bugetCheltuieliRepository.Delete(contrib);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }


        public BugetCheltuieliEditDto GetCheltuieliById(int? cheltId)
        {
            try
            {
                BugetCheltuieliEditDto Cheltdto;

                if (cheltId == 0)// INSERT
                {
                    Cheltdto = new BugetCheltuieliEditDto
                    {
                        DataIncasare = LazyMethods.Now()
                    };
                }
                else
                {
                    var ret = _bugetCheltuieliRepository.GetAllIncluding(f => f.ActivityType, f => f.Currency, f=> f.BVC_FormRand, f => f.Departament).FirstOrDefault(f => f.Id == cheltId);
                    Cheltdto = ObjectMapper.Map<BugetCheltuieliEditDto>(ret);
                }
                return Cheltdto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        
        public List<BugetCheltuieliDto> GetCheltuieliList()
        {
            try
            {
                var list = _bugetCheltuieliRepository.GetAllIncluding(f => f.ActivityType, f => f.BVC_FormRand , f => f.Currency, f=> f.Departament)
                                                      .OrderBy(f => f.DataIncasare)
                                                      .ToList();

                var ret = ObjectMapper.Map<List<BugetCheltuieliDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        
        public void SaveCheltuieli(BugetCheltuieliEditDto chelt)
        {
            try
            {
                var cheltDb = ObjectMapper.Map<BVC_Cheltuieli>(chelt);
                var appClient = GetCurrentTenant();

                if (cheltDb.Id == 0)
                {
                    _bugetCheltuieliRepository.Insert(cheltDb);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    cheltDb.TenantId = appClient.Id;
                    _bugetCheltuieliRepository.Update(cheltDb);
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

