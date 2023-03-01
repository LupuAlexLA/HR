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
    public interface IGarantieTipAppService : IApplicationService
    {
        List<GarantieTipDto> GarantieTipList();

        GarantieTipEditDto GetGarantieTipId(int id);
        void SaveGarantieTip(GarantieTipEditDto tip);

        void DeleteGarantieTip(int id);
    }
    public class GarantieTipAppService : ErpAppServiceBase, IGarantieTipAppService
    {
        IRepository<GarantieTip> _garantieTipRepository;

        public GarantieTipAppService(IRepository<GarantieTip> GarantieTipRepository)
        {
            _garantieTipRepository = GarantieTipRepository;
        }

        public List<GarantieTipDto> GarantieTipList()
        {
            var _garantieTip = _garantieTipRepository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);


            var ret = ObjectMapper.Map<List<GarantieTipDto>>(_garantieTip).ToList();
            return ret;
        }

        public GarantieTipEditDto GetGarantieTipId(int id)
        {
            var ret = _garantieTipRepository.Get(id);
            return ObjectMapper.Map<GarantieTipEditDto>(ret);
        }

        public void SaveGarantieTip(GarantieTipEditDto garantieTip)
        {
            var _termen = ObjectMapper.Map<GarantieTip>(garantieTip);

            if (_termen.Id == 0)
            {

                int _chk = _garantieTipRepository.GetAll().Where(f => f.Description == _termen.Description && f.State == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _garantieTipRepository.Insert(_termen);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _termen.TenantId = appClient.Id;
                _garantieTipRepository.Update(_termen);
            }
        }

        public void DeleteGarantieTip(int id)
        {
            var _garantieTip = _garantieTipRepository.FirstOrDefault(f => f.Id == id);
            _garantieTip.State = Models.Conta.Enums.State.Inactive;
            _garantieTipRepository.Update(_garantieTip);
        }

    }

}

