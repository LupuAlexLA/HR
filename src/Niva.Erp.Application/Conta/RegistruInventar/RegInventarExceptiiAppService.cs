using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.RegistruInventar.Dto;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.RegistruInventar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.RegistruInventar
{
    public interface IRegInventarExceptiiAppService : IApplicationService
    {
        List<RegInventarExceptiiListDto> SaveExceptRegInventar(List<RegInventarExceptiiListDto> regList);
        void DeleteExceptRegInventar(int regInventarId);
        List<RegInventarExceptiiListDto> GetExceptRegInventarList(List<RegInventarExceptiiListDto> regList);
        GetRegInventarOutput GetRegInventarInit();
        List<RegInventarExceptiiListDto> ExceptRegInventarAddRow(List<RegInventarExceptiiListDto> regList);
    }
    public class GetRegInventarOutput
    {
        public List<RegInventarExceptiiListDto> GetRegInventar { get; set; }
    }

    public class RegInventarExceptiiAppService: ErpAppServiceBase, IRegInventarExceptiiAppService
    {
        IRepository<RegInventarExceptii> _exceptRegInventarRepository;
        public RegInventarExceptiiAppService(IRepository<RegInventarExceptii> exceptRegInventarRepository)
        {
            _exceptRegInventarRepository = exceptRegInventarRepository;
        }

        public void DeleteExceptRegInventar(int regInventarId)
        {
            try
            {
                var exceptRegInventar = _exceptRegInventarRepository.Get(regInventarId);
                exceptRegInventar.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", "Operatiunea nu poate fi inregistrata");
            }
        }

        public GetRegInventarOutput GetRegInventarInit()
        {
            var ret = new List<RegInventarExceptiiListDto>();
            ret = GetExceptRegInventarList(ret);

            var _ret = new GetRegInventarOutput { GetRegInventar = ObjectMapper.Map<List<RegInventarExceptiiListDto>>(ret) };
            return _ret;
        }

        public List<RegInventarExceptiiListDto> GetExceptRegInventarList(List<RegInventarExceptiiListDto> regList)
        {
            var list = _exceptRegInventarRepository.GetAllIncluding(f => f.Account).Where(f => f.State == State.Active).OrderBy(f => f.Account.AccountName).ToList();
            var ret = ObjectMapper.Map<List<RegInventarExceptiiListDto>>(list);
            regList = ret;
            if (ret.Count == 0) 
            {
                regList = ExceptRegInventarAddRow(regList);
            }
            return regList;
        }

        public List<RegInventarExceptiiListDto> ExceptRegInventarAddRow(List<RegInventarExceptiiListDto> regList)
        {
            var regInventarItem = new RegInventarExceptiiListDto();
            regList.Add(regInventarItem);
            return regList;
        }

        public List<RegInventarExceptiiListDto> SaveExceptRegInventar(List<RegInventarExceptiiListDto> regList)
        {
            try
            {
                var oldExceptRegList = _exceptRegInventarRepository.GetAllIncluding(f => f.Account).Where(f => f.State == State.Active);
                foreach (var item in oldExceptRegList)
                {
                    _exceptRegInventarRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();

                //adaug detaliile noi
                foreach (var item in regList)
                {
                    var _item = ObjectMapper.Map<RegInventarExceptii>(item);
                    _item.Id = 0;
                    
                    _exceptRegInventarRepository.Insert(_item);
                }
                CurrentUnitOfWork.SaveChanges();

                regList = GetExceptRegInventarList(regList);
                return regList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Operatia nu poate fi inregistrata");
            }
        }
    }
}
