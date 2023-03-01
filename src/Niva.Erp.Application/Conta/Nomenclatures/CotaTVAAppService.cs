using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Conta.Nomenclatures.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface ICotaTVAAppService : IApplicationService
    {
        List<CotaTVAListDto> GetTVAList();
        List<CotaTVAListDto> GetTVAListByYear(DateTime time);
        CotaTVAEditDto GetTVAById(int cotaId);
        void SaveCotaTVA(CotaTVAEditDto cota);
        void DeleteCotaTVA(int cotaId);

        void SaveCoteTVAForPaap(List<PaapDto> paapList);

        PaapDto CalculateValueByTVA(int paapId, int cotaTVAId);
        Dictionary<int, List<CotaTVAListDto>> TvaListsPaapList(List<PaapDto> list);

    }
    public class CotaTVAAppService : ErpAppServiceBase, ICotaTVAAppService
    {
        IRepository<CotaTVA> _cotaTVARepository;
        IBVC_PaapRepository _bvcPaapRepository;
        IRepository<BVC_PAAP_State> _bvcPaapStateRepository;

        public CotaTVAAppService(IRepository<CotaTVA> cotaTVARepository, IBVC_PaapRepository bvcPaapRepository,
        IRepository<BVC_PAAP_State> bvcPaapStateRepository)
        {
            _cotaTVARepository = cotaTVARepository;
            _bvcPaapRepository = bvcPaapRepository;
            _bvcPaapStateRepository = bvcPaapStateRepository;
        }

        //[AbpAuthorize("Admin.Conta.CotaTVA.Acces")]
        public void DeleteCotaTVA(int cotaId)
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;
                var cotaTVA = _cotaTVARepository.GetAll().FirstOrDefault(f => f.Id == cotaId && f.TenantId == appClientId);
                _cotaTVARepository.Delete(cotaTVA);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Admin.Conta.CotaTVA.Acces")]
        public CotaTVAEditDto GetTVAById(int cotaId)
        {
            CotaTVAEditDto cota;
            try
            {
                if (cotaId == 0)
                {
                    cota = new CotaTVAEditDto
                    {
                        StartDate = LazyMethods.Now().AddMonths(-1),
                        EndDate = LazyMethods.Now(),
                        State = State.Active
                    };
                }
                else
                {
                    var appClientId = GetCurrentTenant().Id;
                    var cotaDb = _cotaTVARepository.GetAll().FirstOrDefault(f => f.Id == cotaId && f.TenantId == appClientId);
                    cota = ObjectMapper.Map<CotaTVAEditDto>(cotaDb);
                }
                return cota;
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Admin.Conta.CotaTVA.Acces")]
        public List<CotaTVAListDto> GetTVAList()
        {
            var appClientId = GetCurrentTenant().Id;
            var coteTVAList = _cotaTVARepository.GetAll().Where(f => f.State == State.Active && f.TenantId == appClientId).OrderBy(f => f.VAT);
            var list = ObjectMapper.Map<List<CotaTVAListDto>>(coteTVAList);
            return list;
        }

        public List<CotaTVAListDto> GetTVAListByYear(DateTime time)
        {
            var appClientId = GetCurrentTenant().Id;
            var coteTVAList = _cotaTVARepository.GetAll().Where(f => f.State == State.Active && f.TenantId == appClientId && f.StartDate <= time && (f.EndDate >= time || f.EndDate == null)).OrderBy(f => f.VAT);
            var list = ObjectMapper.Map<List<CotaTVAListDto>>(coteTVAList);
            return list;
        }

        public Dictionary<int, List<CotaTVAListDto>> TvaListsPaapList(List<PaapDto> list)
        {

            Dictionary<int, List<CotaTVAListDto>> result = new Dictionary<int, List<CotaTVAListDto>>();

            foreach (var value in list)
            {

                result.Add(value.Id, GetTVAListByYear(value.DataStart));
            }

            return result;
        }

        //[AbpAuthorize("Admin.Conta.CotaTVA.Acces")]
        public void SaveCotaTVA(CotaTVAEditDto cota)
        {
            try
            {
                var newCota = ObjectMapper.Map<CotaTVA>(cota);

                if (newCota.Id == 0)
                {
                    _cotaTVARepository.Insert(newCota);
                }
                else
                {
                    var appClientId = GetCurrentTenant().Id;
                    newCota.TenantId = appClientId;
                    _cotaTVARepository.Update(newCota);
                }
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void SaveCoteTVAForPaap(List<PaapDto> paapList)
        {
            try
            {
                foreach (var item in paapList)
                {
                    var paap = _bvcPaapRepository.GetAllIncluding(f => f.CotaTVA, f => f.PaapStateList).FirstOrDefault(f => f.Id == item.Id && f.State == State.Active);
                    paap.CotaTVA_Id = item.CotaTVA_Id;
                    paap.ValoareEstimataFaraTvaLei = item.Value;
                    _bvcPaapRepository.Update(paap);
                    CurrentUnitOfWork.SaveChanges();

                    _bvcPaapRepository.UpdatePaapTranse(paap, item.CotaTVA_Id);

                    var operationDate = LazyMethods.Now();
                    var paapState = _bvcPaapStateRepository.GetAllIncluding(f => f.BVC_PAAP).Where(f => f.BVC_PAAP_Id == paap.Id).OrderByDescending(f => f.Id).FirstOrDefault();
                    _bvcPaapRepository.InsertPAAPState(paap.Id, operationDate, paap.GetPaapState, null);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapDto CalculateValueByTVA(int paapId, int cotaTVAId)
        {
            try
            {
                var paap = _bvcPaapRepository.GetAllIncluding(f => f.CotaTVA, f => f.Currency, f => f.Departament).FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                var paapDto = ObjectMapper.Map<PaapDto>(paap);
                var cotaTVA = _cotaTVARepository.GetAll().FirstOrDefault(f => f.Id == cotaTVAId && f.TenantId == paapDto.TenantId && f.State == State.Active);
                paapDto.CotaTVA_Id = cotaTVAId;
                paapDto.Value = Math.Round(paapDto.ValoareTotalaLei / (100 + cotaTVA.VAT) * 100, 2);
                return paapDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
