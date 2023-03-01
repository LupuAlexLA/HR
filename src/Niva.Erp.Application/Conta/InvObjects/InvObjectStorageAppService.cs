using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectStorageAppService: IApplicationService
    {
        GetInvObjectStorageOutput InvObjectStorageList();

        InvObjectStorageDto GetStorageById(int id);

        void SaveStorage(InvObjectStorageDto storage);
        void DeleteStorage(int storageId);
    }

    public class GetInvObjectStorageOutput
    {
        public List<InvObjectStorageDto> GetInvObjectStorage { get; set; }
    }

    public class InvObjectStorageAppService : ErpAppServiceBase, IInvObjectStorageAppService
    {
        IRepository<InvStorage> _invStorageRepository;

        public InvObjectStorageAppService(IRepository<InvStorage> invStorageRepository)
        {
            _invStorageRepository = invStorageRepository;
        }

        public GetInvObjectStorageOutput InvObjectStorageList()
        {
            var _storageList = _invStorageRepository.GetAll().Where(f => f.State == State.Active).OrderBy(f => f.StorageName);
            var ret = new GetInvObjectStorageOutput { GetInvObjectStorage = ObjectMapper.Map<List<InvObjectStorageDto>>(_storageList) };

            return ret;
        }

        public InvObjectStorageDto GetStorageById(int id)
        {
            var ret = new InvObjectStorageDto();
            InvStorage _storage;
            if (id == 0)
            {
                _storage = new InvStorage();
                _storage.State = State.Active;
                _storage.StorageName = "";
            }
            else
            {
                _storage = _invStorageRepository.GetAll().FirstOrDefault(f => f.Id == id);
            }
            ret = ObjectMapper.Map<InvObjectStorageDto>(_storage);
            return ret;
        }

        public void SaveStorage(InvObjectStorageDto storage)
        {
            var _storage = ObjectMapper.Map<InvStorage>(storage);

            if (_storage.Id == 0)
            {

                int count = _invStorageRepository.GetAll().Where(f => f.StorageName == _storage.StorageName && f.State == State.Active).Count();

                if (count > 0) 
                    throw new UserFriendlyException("Eroare", "Gestiune existenta!");

                _invStorageRepository.Insert(_storage);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _storage.TenantId = appClient.Id;
                _invStorageRepository.Update(_storage);
            }
        }

        public void DeleteStorage(int storageId)
        {
            try
            {
                var _storage = _invStorageRepository.FirstOrDefault(f => f.Id == storageId);
                _storage.State = State.Inactive;
                _invStorageRepository.Update(_storage);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }
    }
}
