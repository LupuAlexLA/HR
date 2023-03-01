using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{
    public interface IGarantieCeGaranteazaAppService : IApplicationService
    {
        List<GarantieCeGaranteazaDto> GarantieCeGaranteazaList();

        GarantieCeGaranteazaEditDto GetGarantieCeGaranteazaId(int id);
        void SaveGarantieCeGaranteaza(GarantieCeGaranteazaEditDto tip);

        void DeleteGarantieCeGaranteaza(int id);
    }
    public class GarantieCeGaranteazaAppService : ErpAppServiceBase, IGarantieCeGaranteazaAppService
    {
        IRepository<GarantieCeGaranteaza> _garantieCeGaranteazaRepository;

        public GarantieCeGaranteazaAppService(IRepository<GarantieCeGaranteaza> GarantieCeGaranteazaRepository)
        {
            _garantieCeGaranteazaRepository = GarantieCeGaranteazaRepository;
        }

        public List<GarantieCeGaranteazaDto> GarantieCeGaranteazaList()
        {
            var _garantieCeGaranteaza = _garantieCeGaranteazaRepository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);


            var ret = ObjectMapper.Map<List<GarantieCeGaranteazaDto>>(_garantieCeGaranteaza).ToList();
            return ret;
        }

        public GarantieCeGaranteazaEditDto GetGarantieCeGaranteazaId(int id)
        {
            var ret = _garantieCeGaranteazaRepository.Get(id);
            return ObjectMapper.Map<GarantieCeGaranteazaEditDto>(ret);
        }

        public void SaveGarantieCeGaranteaza(GarantieCeGaranteazaEditDto garantieCeGaranteaza)
        {
            var _garantieCeGaranteaza = ObjectMapper.Map<GarantieCeGaranteaza>(garantieCeGaranteaza);

            if (_garantieCeGaranteaza.Id == 0)
            {

                int _chk = _garantieCeGaranteazaRepository.GetAll().Where(f => f.Description == _garantieCeGaranteaza.Description && f.State == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _garantieCeGaranteazaRepository.Insert(_garantieCeGaranteaza);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _garantieCeGaranteaza.TenantId = appClient.Id;
                _garantieCeGaranteazaRepository.Update(_garantieCeGaranteaza);
            }
        }

        public void DeleteGarantieCeGaranteaza(int id)
        {
            var _garantieCeGaranteaza = _garantieCeGaranteazaRepository.FirstOrDefault(f => f.Id == id);
            _garantieCeGaranteaza.State = Models.Conta.Enums.State.Inactive;
            _garantieCeGaranteazaRepository.Update(_garantieCeGaranteaza);
        }
    }
}
