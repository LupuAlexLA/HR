using Abp.Application.Services;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Imprumuturi.Dto;
using System.Linq;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;

namespace Niva.Erp.ImprumuturiTipuriService
{

    public interface IImprumuturiTipuriAppService : IApplicationService
    {
        List<ImprumuturiTipuriDto> ImprumuturiTipuriList();

        ImprumuturiTipuriEditDto GetTipId(int id);
        void SaveTip(ImprumuturiTipuriEditDto tip);
        void DeleteTip(int id);

        List<ImprumutTipDetaliuDto> ImprumutTipDetaliiList(int imprumutTipId);
        ImprumutTipDetaliuEditDto GetImprumutTipDetaliuId(int imprumutTipDetaliuId, int imprumutTipId);
        void DeleteImprumutTipDetaliu(int imprumutTipDetaliuId);
        void SaveImprumutTipDetaliu(ImprumutTipDetaliuEditDto imprumutTipDetaliu);
    }

    public class ImprumuturiTipuriAppService : ErpAppServiceBase, IImprumuturiTipuriAppService
    {
        IRepository<ImprumutTip> _ImprumuturiTipuriRepository;
        IRepository<ImprumutTipDetaliu> _imprumutTipDetaliuRepository;

        public ImprumuturiTipuriAppService(IRepository<ImprumutTip> ImprumuturiTipuriRepository, IRepository<ImprumutTipDetaliu> imprumutTipDetaliuRepository)
        {
            _ImprumuturiTipuriRepository = ImprumuturiTipuriRepository;
            _imprumutTipDetaliuRepository = imprumutTipDetaliuRepository;
        }

        public List<ImprumuturiTipuriDto> ImprumuturiTipuriList()
        {
            var _imprumuturiTip = _ImprumuturiTipuriRepository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);


            var ret = ObjectMapper.Map<List<ImprumuturiTipuriDto>>(_imprumuturiTip).ToList();
            return ret;
        }

        public ImprumuturiTipuriEditDto GetTipId(int id)
        {
            var ret = _ImprumuturiTipuriRepository.Get(id);
            return ObjectMapper.Map<ImprumuturiTipuriEditDto>(ret);
        }

        public void SaveTip(ImprumuturiTipuriEditDto tip)
        {
            var _termen = ObjectMapper.Map<ImprumutTip>(tip);

            if (_termen.Id == 0)
            {

                int _chk = _ImprumuturiTipuriRepository.GetAll().Where(f => f.Description == _termen.Description && f.State == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _ImprumuturiTipuriRepository.Insert(_termen);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _termen.TenantId = appClient.Id;
                _ImprumuturiTipuriRepository.Update(_termen);
            }
        }

        public void DeleteTip(int id)
        {
            var _termen = _ImprumuturiTipuriRepository.FirstOrDefault(f => f.Id == id);
            _termen.State = Models.Conta.Enums.State.Inactive;
            _ImprumuturiTipuriRepository.Update(_termen);
        }

        public List<ImprumutTipDetaliuDto> ImprumutTipDetaliiList(int imprumutTipId)
        {
            try
            {
                var ret = _imprumutTipDetaliuRepository.GetAllIncluding(f => f.ActivityType, f => f.ImprumutTip)
                                                       .Where(f => f.ImprumutTipId == imprumutTipId)
                                                       .Select(f => new ImprumutTipDetaliuDto
                                                       {
                                                           Id = f.Id,
                                                           ActivityTypeName = f.ActivityType.ActivityName,
                                                           ContImprumut = f.ContImprumut,
                                                           Description = LazyMethods.EnumValueToDescription(f.Description), 
                                                           ImprumutTipId = imprumutTipId
                                                       })
                                                       .OrderBy(f => f.ActivityTypeName)
                                                       .ToList();
                return ret;
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteImprumutTipDetaliu(int imprumutTipDetaliuId)
        {
            var detail = _imprumutTipDetaliuRepository.FirstOrDefault(f => f.Id == imprumutTipDetaliuId);
            _imprumutTipDetaliuRepository.Delete(detail);
        }

        public void SaveImprumutTipDetaliu(ImprumutTipDetaliuEditDto imprumutTipDetaliu)
        {
            var detail = ObjectMapper.Map<ImprumutTipDetaliu>(imprumutTipDetaliu);

            if (detail.Id == 0)
            {
                _imprumutTipDetaliuRepository.Insert(detail);
            }
            else
            {
                var appClient = GetCurrentTenant();
                detail.TenantId = appClient.Id;
                _imprumutTipDetaliuRepository.Update(detail);
            }
        }

        public ImprumutTipDetaliuEditDto GetImprumutTipDetaliuId(int imprumutTipDetaliuId, int imprumutTipId)
        {
            var detailDto = new ImprumutTipDetaliuEditDto();

            if (imprumutTipDetaliuId == 0)
            {
                detailDto = new ImprumutTipDetaliuEditDto
                {
                    ImprumutTipId = imprumutTipId,
                    ActivityTypeId = null
                };
            }else
            {
              var  detail = _imprumutTipDetaliuRepository.Get(imprumutTipDetaliuId);

                 detailDto = ObjectMapper.Map<ImprumutTipDetaliuEditDto>(detail);
            }

            return detailDto;
        }
    }
}
