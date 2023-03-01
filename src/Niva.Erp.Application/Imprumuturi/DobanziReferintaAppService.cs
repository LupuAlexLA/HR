using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Imprumuturi
{
    public interface IDobanziReferintaAppService : IApplicationService
    {
        List<DobanziReferintaDto> DobanziReferintaList();

        DobanziReferintaEditDto GetDobandaReferintaId(int id);
        void SaveDobandaReferinta(DobanziReferintaDto storage);

        void DeleteDobandaReferinta(int id);
    }
    public class DobanziReferintaAppService : ErpAppServiceBase, IDobanziReferintaAppService
    {

        IRepository<DobanziReferinta> _DobanziReferintaRepository;

        public DobanziReferintaAppService(IRepository<DobanziReferinta> DobanziReferintaRepository)
        {
            _DobanziReferintaRepository = DobanziReferintaRepository;
        }

        public List<DobanziReferintaDto> DobanziReferintaList()
        {
            var _DobanziReferinta = _DobanziReferintaRepository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);


            var ret = ObjectMapper.Map<List<DobanziReferintaDto>>(_DobanziReferinta).ToList();
            return ret;
        }

        public DobanziReferintaEditDto GetDobandaReferintaId(int id)
        {
            var ret = _DobanziReferintaRepository.Get(id);
            return ObjectMapper.Map<DobanziReferintaEditDto>(ret);
        }

        public void SaveDobandaReferinta(DobanziReferintaDto storage)
        {
            var _dobandaReferinta = ObjectMapper.Map<DobanziReferinta>(storage);

            if (_dobandaReferinta.Id == 0)
            {

                int _chk = _DobanziReferintaRepository.GetAll().Where(f => f.Descriere == _dobandaReferinta.Descriere && f.State == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _DobanziReferintaRepository.Insert(_dobandaReferinta);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _dobandaReferinta.TenantId = appClient.Id;
                _DobanziReferintaRepository.Update(_dobandaReferinta);
            }
        }

        public void DeleteDobandaReferinta(int id)
        {
            var _dobandaReferinta = _DobanziReferintaRepository.FirstOrDefault(f => f.Id == id);
            _dobandaReferinta.State = Models.Conta.Enums.State.Inactive;
            _DobanziReferintaRepository.Update(_dobandaReferinta);
        }


    }
}
