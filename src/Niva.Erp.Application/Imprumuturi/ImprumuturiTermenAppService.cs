using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.ImprumuturiTermenService
{

    public interface IImprumuturiTermenAppService : IApplicationService
    {
        List<ImprumuturiTermenDto> ImprumuturiTermenList();

        ImprumuturiTermenEditDto GetTermenId(int id);
        void SaveTermen(ImprumuturiTermenEditDto storage);

        void DeleteTermen(int id);
    }



    public class ImprumuturiTermenAppService : ErpAppServiceBase, IImprumuturiTermenAppService
    {
        IRepository<ImprumutTermen> _ImprumuturiTermenRepository;

        public ImprumuturiTermenAppService(IRepository<ImprumutTermen> ImprumuturiTermenRepository)
        {
            _ImprumuturiTermenRepository = ImprumuturiTermenRepository;
        }

        public List<ImprumuturiTermenDto> ImprumuturiTermenList()
        {
            var _imprumuturiTermen = _ImprumuturiTermenRepository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);


            var ret = ObjectMapper.Map<List<ImprumuturiTermenDto>>(_imprumuturiTermen).ToList();
            return ret;
        }

        public ImprumuturiTermenEditDto GetTermenId(int id)
        {
            var ret = _ImprumuturiTermenRepository.Get(id);
            return ObjectMapper.Map<ImprumuturiTermenEditDto>(ret);
        }

        public void SaveTermen(ImprumuturiTermenEditDto storage)
        {
            var _termen = ObjectMapper.Map<ImprumutTermen>(storage);

            if (_termen.Id == 0)
            {

                int _chk = _ImprumuturiTermenRepository.GetAll().Where(f => f.Description == _termen.Description && f.State == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _ImprumuturiTermenRepository.Insert(_termen);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _termen.TenantId = appClient.Id;
                _ImprumuturiTermenRepository.Update(_termen);
            }
        }

        public void DeleteTermen(int id)
        {
            var _termen = _ImprumuturiTermenRepository.FirstOrDefault(f => f.Id == id);
            _termen.State = Models.Conta.Enums.State.Inactive;
            _ImprumuturiTermenRepository.Update(_termen);
        }

    }
}
