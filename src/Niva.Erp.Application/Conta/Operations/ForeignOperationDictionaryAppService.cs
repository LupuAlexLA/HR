using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.Operations.Dto;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Operations
{
    public interface IForeignOperationDictionaryAppService: IApplicationService 
    {
        FODictionaryFormDto FODictionaryListInit();
        FODictionaryFormDto FODictionarySearch(string search);
        FODictionaryEditDto FODictionaryEdit(int foDictionaryId);
        void SaveFODictionary(FODictionaryEditDto dictionary);
        void DeleteFODictionary(int foDictionaryId);
    }

    public class ForeignOperationDictionaryAppService : ErpAppServiceBase, IForeignOperationDictionaryAppService
    {
        IRepository<ForeignOperationDictionary> _foDictionaryRepository;

        public ForeignOperationDictionaryAppService(IRepository<ForeignOperationDictionary> foDictionaryRepository)
        {
            _foDictionaryRepository = foDictionaryRepository;
        }

        public void DeleteFODictionary(int foDictionaryId)
        {
            var dictionary = _foDictionaryRepository.GetAll().FirstOrDefault(f => f.Id == foDictionaryId && f.State == State.Active);
            dictionary.State = State.Inactive;
            CurrentUnitOfWork.SaveChanges();
        }

        public FODictionaryEditDto FODictionaryEdit(int foDictionaryId)
        {
            var appClient = GetCurrentTenant();
            var ret = new FODictionaryEditDto();

            ForeignOperationDictionary _foDictionary;

            if(foDictionaryId == 0)
            {
                _foDictionary = new ForeignOperationDictionary();
                _foDictionary.State = State.Active;
               // _foDictionary.Account.Symbol = "";
            }
            else
            {
                _foDictionary = _foDictionaryRepository.GetAllIncluding(f => f.Account).FirstOrDefault(f => f.Id == foDictionaryId && f.TenantId == appClient.Id && f.State == State.Active);
            }

             ret = ObjectMapper.Map<FODictionaryEditDto>(_foDictionary);

            return ret;
        }

        public FODictionaryFormDto FODictionaryListInit()
        {
            var ret = new FODictionaryFormDto
            {
                FOdictionaryList = new List<FODictionaryListDto>(),
                SearchAccount = ""
            };

            ret = FODictionarySearch(ret.SearchAccount);
            return ret;
        }

        public FODictionaryFormDto FODictionarySearch(string search)
        {
            var appClientId = GetCurrentTenant();

            List<ForeignOperationDictionary> list;

            if (search == null)
            {
                search = "";
            }

            list = _foDictionaryRepository.GetAllIncluding(f => f.Account)
                 .Where(f => f.TenantId == appClientId.Id && f.State == State.Active && ((f.Account.Symbol.ToUpper().StartsWith(search.ToUpper())) || f.Account.Symbol.ToUpper().Contains(search.ToUpper()))).ToList();

            var _list = ObjectMapper.Map<List<FODictionaryListDto>>(list);
            var ret = new FODictionaryFormDto
            {
                FOdictionaryList = _list,
                SearchAccount = search
            };


            return ret;
        }

        public void SaveFODictionary(FODictionaryEditDto dictionary)
        {
            var count = _foDictionaryRepository.GetAllIncluding(f => f.Account).Count(f => f.Account.Symbol == dictionary.Symbol && f.Id != dictionary.Id && f.State == State.Active);

            if (count != 0)
                throw new UserFriendlyException("Eroare", "Este definita o alta expresie pentru acest cont");

            if (dictionary.Expression == "")
                throw new UserFriendlyException("Eroare", "Trebuie sa completati campul 'Expresie'");

            if (dictionary.AccountId == 0)
                throw new UserFriendlyException("Eroare", "Trebuie sa completati contul");

            try
            {
                var _dictionary = ObjectMapper.Map<ForeignOperationDictionary>(dictionary);
                var appClient = GetCurrentTenant();
                _dictionary.TenantId = appClient.Id;

                if(_dictionary.Id == 0)
                {
                    _foDictionaryRepository.Insert(_dictionary);
                }
                else
                {
                    _foDictionaryRepository.Update(_dictionary);
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
