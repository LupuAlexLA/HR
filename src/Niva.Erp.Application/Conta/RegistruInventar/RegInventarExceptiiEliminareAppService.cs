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
    public interface IRegInventarExceptiiEliminareAppService : IApplicationService
    {
        List<ExceptEliminareRegInventarListDto> SaveExceptEliminareRegInventar(List<ExceptEliminareRegInventarListDto> regList);
        void DeleteExceptRegInventar(int regInventarId);
        List<ExceptEliminareRegInventarListDto> GetExceptEliminareRegInventarList(List<ExceptEliminareRegInventarListDto> regList);
        GetExceptElimRegInventarOutput GetRegInventarInit();
        List<ExceptEliminareRegInventarListDto> ExceptEliminareRegInventarAddRow(List<ExceptEliminareRegInventarListDto> regList);
    }

    public class GetExceptElimRegInventarOutput
    {
        public List<ExceptEliminareRegInventarListDto> GetRegInventar { get; set; }
    }

    public class RegInventarExceptiiEliminareAppService : ErpAppServiceBase, IRegInventarExceptiiEliminareAppService
    {
        IRepository<RegInventarExceptiiEliminare> _exceptEliminareRegInventarRepository;

        public RegInventarExceptiiEliminareAppService(IRepository<RegInventarExceptiiEliminare> exceptEliminareRegInventarRepository)
        {
            _exceptEliminareRegInventarRepository = exceptEliminareRegInventarRepository;
        }

        public void DeleteExceptRegInventar(int regInventarId)
        {
            try
            {
                var exceptEliminareRegInventar = _exceptEliminareRegInventarRepository.Get(regInventarId);
                exceptEliminareRegInventar.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", "Operatiunea nu poate fi inregistrata");
            }
        }

        public List<ExceptEliminareRegInventarListDto> ExceptEliminareRegInventarAddRow(List<ExceptEliminareRegInventarListDto> regList)
        {
            var regInventarItem = new ExceptEliminareRegInventarListDto();
            regList.Add(regInventarItem);
            return regList;
        }

        public List<ExceptEliminareRegInventarListDto> GetExceptEliminareRegInventarList(List<ExceptEliminareRegInventarListDto> regList)
        {
            var list = _exceptEliminareRegInventarRepository.GetAllIncluding(f => f.Account).Where(f => f.State == State.Active).OrderBy(f => f.Account.AccountName).ToList();
            var ret = ObjectMapper.Map<List<ExceptEliminareRegInventarListDto>>(list);
            regList = ret;
            if (ret.Count == 0)
            {
                regList = ExceptEliminareRegInventarAddRow(regList);
            }
            return regList;
        }

        public GetExceptElimRegInventarOutput GetRegInventarInit()
        {
            var ret = new List<ExceptEliminareRegInventarListDto>();
            ret = GetExceptEliminareRegInventarList(ret);

            var _ret = new GetExceptElimRegInventarOutput { GetRegInventar = ObjectMapper.Map<List<ExceptEliminareRegInventarListDto>>(ret) };
            return _ret;
        }

        public List<ExceptEliminareRegInventarListDto> SaveExceptEliminareRegInventar(List<ExceptEliminareRegInventarListDto> regList)
        {
            try
            {
                var oldExceptElimRegList = _exceptEliminareRegInventarRepository.GetAllIncluding(f => f.Account).Where(f => f.State == State.Active);
                foreach (var item in oldExceptElimRegList)
                {
                    _exceptEliminareRegInventarRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();

                //adaug detaliile noi
                foreach (var item in regList)
                {
                    var _item = ObjectMapper.Map<RegInventarExceptiiEliminare>(item);
                    _item.Id = 0;

                    _exceptEliminareRegInventarRepository.Insert(_item);
                }
                CurrentUnitOfWork.SaveChanges();

                regList = GetExceptEliminareRegInventarList(regList);
                return regList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Operatia nu poate fi inregistrata");
            }
        }
    }
}
