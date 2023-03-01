using Abp.Application.Services;
using Abp.UI;
using Niva.Erp.Conta.Nomenclatures.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Repositories.Doconturi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IDiurnaAppService : IApplicationService
    {
        //Diurna per zi
        List<DiurnaListDto> DiurnaZiList();
        DiurnaEditDto GetDiurnaZiById(int diurnaId);
        void SaveDiurnaZi(DiurnaEditDto diurna);
        void DeleteDiurnaZi(int diurnaId);
        int GetDiurnaZiValue(int diurnaId);

        // Diurna legala
        List<DiurnaListDto> DiurnaLegalaList();
        DiurnaEditDto GetDiurnaLegalaById(int diurnaId);
        void SaveDiurnaLegala(DiurnaEditDto diurna);
        void DeleteDiurnaLegala(int diurnaId);
        int GetDiurnaLegalaValue(int diurnaId);

        bool IsDiurnaExterna(int diurnaId);
    }

    public class DiurnaAppService : ErpAppServiceBase, IDiurnaAppService
    {
        IDiurnaZiRepository _diurnaZiRepository;
        IDiurnaLegalaRepository _diurnaLegalaRepository;

        public DiurnaAppService(IDiurnaZiRepository diurnaZiRepository, IDiurnaLegalaRepository diurnaLegalaRepository)
        {
            _diurnaZiRepository = diurnaZiRepository;
            _diurnaLegalaRepository = diurnaLegalaRepository;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaLegala.Acces")]
        public void DeleteDiurnaLegala(int diurnaId)
        {
            try
            {
                var diurna = _diurnaLegalaRepository.Get(diurnaId);
                diurna.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Operatia de stergere nu poate fi efectuata.");
            }
        }

        //[AbpAuthorize("Admin.Conta.DiurnaZi.Acces")]
        public void DeleteDiurnaZi(int diurnaId)
        {
            try
            {
                var diurna = _diurnaZiRepository.Get(diurnaId);
                diurna.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Operatia de stergere nu poate fi efectuata.");
            }
        }

        //[AbpAuthorize("Admin.Conta.DiurnaLegala.Acces")]
        public List<DiurnaListDto> DiurnaLegalaList()
        {
            var appClient = GetCurrentTenant();
            var _list = _diurnaLegalaRepository.GetAllIncluding(f => f.Country, f => f.Currency).Where(f => f.State == State.Active && f.TenantId == appClient.Id).OrderBy(f => f.Country.CountryName).ToList();
            var ret = ObjectMapper.Map<List<DiurnaListDto>>(_list);
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaZi.Acces")]
        public List<DiurnaListDto> DiurnaZiList()
        {
            var appClient = GetCurrentTenant();
            var _list = _diurnaZiRepository.GetAllIncluding(f => f.Country, f => f.Currency).Where(f => f.State == State.Active && f.TenantId == appClient.Id).OrderBy(f => f.Country.CountryName).ToList();
            var ret = ObjectMapper.Map<List<DiurnaListDto>>(_list);
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaLegala.Acces")]
        public DiurnaEditDto GetDiurnaLegalaById(int diurnaId)
        {
            DiurnaEditDto ret;

            if (diurnaId == 0)
            {
                ret = new DiurnaEditDto
                {
                    DataValabilitate = LazyMethods.Now()
                };
            }
            else
            {
                var _diurna = _diurnaLegalaRepository.GetAllIncluding(f => f.Country, f => f.Currency).FirstOrDefault(f => f.Id == diurnaId);
                ret = ObjectMapper.Map<DiurnaEditDto>(_diurna);
            }
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaZi.Acces")]
        public DiurnaEditDto GetDiurnaZiById(int diurnaId)
        {
            DiurnaEditDto ret;

            if (diurnaId == 0)
            {
                ret = new DiurnaEditDto
                {
                    DataValabilitate = LazyMethods.Now()
                };
            }
            else
            {
                var _diurna = _diurnaZiRepository.GetAllIncluding(f => f.Country, f => f.Currency).FirstOrDefault(f => f.Id == diurnaId);
                ret = ObjectMapper.Map<DiurnaEditDto>(_diurna);
            }
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaLegala.Acces")]
        public int GetDiurnaLegalaValue(int diurnaId)
        {
            var appClient = GetCurrentTenant();
            var diurnaLegala = _diurnaLegalaRepository.GetDiurnalLegalaValue(diurnaId);
            return diurnaLegala;
        }

        //[AbpAuthorize("Admin.Conta.DiurnaZi.Acces")]
        public int GetDiurnaZiValue(int diurnaId)
        {
            try
            {
                var diurna = _diurnaLegalaRepository.Get(diurnaId);
                var diurnaZi = _diurnaZiRepository.GetDiurnaZiValue(diurna.CountryId);

                return diurnaZi;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        //[AbpAuthorize("Admin.Conta.DiurnaLegala.Acces")]
        public void SaveDiurnaLegala(DiurnaEditDto diurna)
        {
            try
            {
                var _diurna = ObjectMapper.Map<DiurnaLegala>(diurna);
                var appClient = GetCurrentTenant();

                if (_diurna.Id == 0)
                {
                    _diurna.TenantId = appClient.Id;
                    _diurnaLegalaRepository.Insert(_diurna);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _diurnaLegalaRepository.Update(_diurna);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Inregistrarea nu poate fi salvata");
            }
        }

        //[AbpAuthorize("Admin.Conta.DiurnaZi.Acces")]
        public void SaveDiurnaZi(DiurnaEditDto diurna)
        {
            try
            {
                var _diurna = ObjectMapper.Map<DiurnaZi>(diurna);
                var appClient = GetCurrentTenant();

                if (_diurna.Id == 0)
                {
                    _diurna.TenantId = appClient.Id;
                    _diurnaZiRepository.Insert(_diurna);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _diurnaZiRepository.Update(_diurna);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Inregistrarea nu poate fi salvata");
            }
        }

        public bool IsDiurnaExterna(int diurnaId)
        {
            var diurna = _diurnaLegalaRepository.Get(diurnaId);
            var ret = diurna.DiurnaType == DiurnaType.Externa ? true : false;
            return ret;
        }
    }
}
