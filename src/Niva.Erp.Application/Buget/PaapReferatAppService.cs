using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IPaapReferatAppService : IApplicationService
    {
        List<PaapReferatDto> GetPaapReferatList(int paapId);
        PaapReferatEditDto GetPaapReferatEdit(int referatId, int paapId);
        void SavePaapReferat(PaapReferatEditDto referat);
        void DeletePappReferat(int referatId);
    }

    public class PaapReferatAppService : ErpAppServiceBase, IPaapReferatAppService
    {
        IRepository<BVC_PAAP_Referat> _paapReferatRepository;
        IBVC_PaapRepository _bvcPaapRepository;
        IRepository<BVC_PAAP_State> _paapStateRepository;

        public PaapReferatAppService(IRepository<BVC_PAAP_Referat> paapReferatRepository, IBVC_PaapRepository bvcPaapRepository, IRepository<BVC_PAAP_State> paapStateRepository)
        {
            _paapReferatRepository = paapReferatRepository;
            _bvcPaapRepository = bvcPaapRepository;
            _paapStateRepository = paapStateRepository;
        }

        public void DeletePappReferat(int referatId)
        {
            try
            {
                var referat = _paapReferatRepository.GetAllIncluding(f => f.BVC_PAAP).FirstOrDefault(f => f.Id == referatId && f.State == State.Active);
                var previousReferatState = _paapStateRepository.GetAllIncluding(f => f.BVC_PAAP)
                                                               .Where(f => f.BVC_PAAP_Id == referat.BVC_PAAP_Id && f.State == State.Active)
                                                               .OrderByDescending(f => f.Paap_State).Where(f => f.Paap_State != PAAP_State.Referat).FirstOrDefault();

                _paapReferatRepository.Delete(referat);
                CurrentUnitOfWork.SaveChanges();

                // modific state-ul din Referat => Aprobat
                _bvcPaapRepository.InsertPAAPState(referat.BVC_PAAP_Id, referat.OperationDate, previousReferatState.Paap_State, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public PaapReferatEditDto GetPaapReferatEdit(int referatId, int paapId)
        {
            try
            {
                PaapReferatEditDto referat;
                if (referatId == 0)
                {
                    referat = new PaapReferatEditDto
                    {
                        BVC_PAAP_Id = paapId,
                        Name = null,
                        State = State.Active,
                        OperationDate = DateTime.Now,
                        Suma = 0
                    };
                }
                else
                {
                    var referatDb = _paapReferatRepository.GetAllIncluding(f => f.BVC_PAAP).FirstOrDefault(f => f.BVC_PAAP_Id == paapId && f.Id == referatId && f.State == State.Active);
                    referat = ObjectMapper.Map<PaapReferatEditDto>(referatDb);
                }
                return referat;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<PaapReferatDto> GetPaapReferatList(int paapId)
        {
            try
            {


                var paapReferatList = _paapReferatRepository.GetAllIncluding(f => f.BVC_PAAP).Where(f => f.BVC_PAAP_Id == paapId && f.State == State.Active).ToList();

                var list = ObjectMapper.Map<List<PaapReferatDto>>(paapReferatList);
                return list;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.PAAP.Modificare")]
        public void SavePaapReferat(PaapReferatEditDto referat)
        {
            try
            {
                var newPaapReferat = ObjectMapper.Map<BVC_PAAP_Referat>(referat);
                var appClient = GetCurrentTenant();
                newPaapReferat.TenantId = appClient.Id;

                _paapReferatRepository.InsertOrUpdate(newPaapReferat);
                CurrentUnitOfWork.SaveChanges();

                // modific starea PAAP-ului => Referat
                _bvcPaapRepository.InsertPAAPState(referat.BVC_PAAP_Id, referat.OperationDate, PAAP_State.Referat, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
