using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface IContractsAppService : IApplicationService
    {
        List<ContractDto> GetContracts();
        ContractDto GetContract(int id);
        List<ContractDto> GetContractsForThirdParty(int thirdPartyId);
        List<ContractDto> GetContractsForInvoiceThirdParty(int thirdPartyId);
        void SaveContract(ContractDto invoice);
        void DeleteContract(int id);
        List<ContractDto> Search(ContractSearchParametersDTO parameters);
        List<ContractCategoryListDto> CategoryList();
        ContractCategoryEditDto GetContractCategory(int id);
        void SaveContractCategory(ContractCategoryEditDto contractCategory);
        void DeleteContractCategory(int id);
        ContractSearchParametersDTO InitSearch();

        void ExtendContractPeriod(DateTime currentDate);
        void ChangeContractsState(ChangeContractStateDto changeContract);

        List<ContractStateListDto> GetContractStateListById(int id);
    }

    public class ContractsAppService : ErpAppServiceBase, IContractsAppService
    {
        IContractsRepository _contractsRepository;
        IInvoiceRepository _invoiceRepository;
        IRepository<Person, int> _personRepository;
        IRepository<Contracts, int> _contractRepository;
        IRepository<ContractsCategory, int> _contractCategoryRepository;
        IRepository<Contracts_State, int> _contractSateRepository;
        IBalanceRepository _balanceRepository;

        public ContractsAppService(IInvoiceRepository InvoiceRepository, IRepository<Person, int> personRepository, IRepository<Contracts, int> contractRepository,
                                   IRepository<ContractsCategory, int> contractCategoryRepository, IBalanceRepository balanceRepository, IContractsRepository contractsRepository, IRepository<Contracts_State, int> contractSateRepository)
        {
            _contractSateRepository = contractSateRepository;
            _contractsRepository = contractsRepository;
            _invoiceRepository = InvoiceRepository;
            _personRepository = personRepository;
            _contractRepository = contractRepository;
            _contractCategoryRepository = contractCategoryRepository;
            _balanceRepository = balanceRepository;
        }

        public ContractDto GetContract(int id)
        {
            var ret = _contractRepository.GetAll()
                                         .Include(f => f.ThirdParty)
                                         .Include(f => f.Currency)
                                         .Include(f => f.AditionalContracts)
                                         .FirstOrDefault(x => x.Id == id);
            var contract = ObjectMapper.Map<ContractDto>(ret);
            return contract;
        }

        public List<ContractDto> GetContractsForThirdParty(int thirdPartyId)
        {
            var ret = _contractRepository.GetAllIncluding(f => f.ContractsStateList).ToList().Where(c => c.ThirdPartyId == thirdPartyId && c.GetContractState != Contract_State.Preliminat).ToList();
            return ObjectMapper.Map<List<ContractDto>>(ret);
        }

        public List<ContractDto> GetContracts()
        {
            return ObjectMapper.Map<List<ContractDto>>(_contractRepository.GetAll());
        }

        [AbpAuthorize("General.Contracte.Modificare")]
        public void SaveContract(ContractDto contract)
        {

            var operationDate = LazyMethods.Now();
            if (contract.ContractsType == ContractsType.ContractCadru)
            {
                contract.Value = null;
                contract.MonthlyValue = null;
            }
            else
            {
                if (contract.Value == null && contract.MonthlyValue == null)
                {
                    throw new Exception("Valoarea contractului sau valoare lunara nu pot fi necompletate.");
                }
            }
            var newContract = ObjectMapper.Map<Contracts>(contract);
            var appClient = GetCurrentTenant();

            newContract.TenantId = appClient.Id;
            _contractRepository.InsertOrUpdate(newContract);
            CurrentUnitOfWork.SaveChanges();

            _contractsRepository.InsertContractState(newContract.Id, operationDate, Contract_State.InVigoare, contract.Comentarii);



        }

        public List<ContractStateListDto> GetContractStateListById(int id)
        {
            var list = _contractSateRepository.GetAll().Where(x => x.ContractsId == id).OrderBy(x => x.Id).ToList();
            var ret = ObjectMapper.Map<List<ContractStateListDto>>(list);

            return ret;
        }

        [AbpAuthorize("General.Contracte.Modificare")]
        public void ChangeContractsState(ChangeContractStateDto changeContract)
        {

            var operationDate = LazyMethods.Now();
            _contractsRepository.InsertContractState(changeContract.ContractId, operationDate, changeContract.Contract_State, changeContract.Comentarii);

        }
        public ContractSearchParametersDTO InitSearch()
        {
            var ret = new ContractSearchParametersDTO
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                State = null,
                ThirdPartyQuality = null
            };

            return ret;
        }

        //[AbpAuthorize("General.Contracte.Acces")]
        public List<ContractDto> Search(ContractSearchParametersDTO parameters)
        {
            var res = _contractRepository.GetAllIncluding(f => f.ContractsStateList).Include(f => f.ThirdParty).Include(f => f.Currency)
                                         .Where(d => d.ContractDate <= parameters.DataEnd && d.ContractDate >= parameters.DataStart)
                                         .ToList(); ;

            if (parameters.ThirdPartyQuality != null)
                res = res.Where(f => f.ThirdPartyQuality == parameters.ThirdPartyQuality).ToList();
            if (parameters.State != null)
                res = res.Where(f => f.GetContractState == parameters.State).ToList();

            List<Contracts> list = res.OrderByDescending(f => f.StartDate).ToList();
            if (!string.IsNullOrEmpty(parameters.ThirdParty))
            {
                list = list.Where(d => d.ThirdParty.FullName.ToUpper().Contains(parameters.ThirdParty.ToUpper())).ToList();
            }
            var ret = ObjectMapper.Map<List<ContractDto>>(list);
            return ret;
        }

        public List<ContractCategoryListDto> CategoryList()
        {

            var _list = _contractCategoryRepository.GetAll().Include(f => f.Contract).ToList();

            var ret = ObjectMapper.Map<List<ContractCategoryListDto>>(_list);
            return ret;
        }

        public ContractCategoryEditDto GetContractCategory(int id)
        {
            var ret = _contractCategoryRepository.Get(id);
            return ObjectMapper.Map<ContractCategoryEditDto>(ret);
        }

        //[AbpAuthorize("Administrare.Contracte.Beneficiari.Acces")]
        public void SaveContractCategory(ContractCategoryEditDto contractCategory)
        {
            var count = _contractCategoryRepository.GetAll().Count(f => f.CategoryName == contractCategory.Name && f.State == State.Active && f.Id != contractCategory.Id);

            if (count != 0)
                throw new UserFriendlyException("Eroare", "Exista definita o alta categorie de contract cu acest nume");
            if (contractCategory.Name == null)
                throw new UserFriendlyException("Eroare", "Trebuie sa completati numele categoriei de contract");

            try
            {
                var _contractCategory = ObjectMapper.Map<ContractsCategory>(contractCategory);
                var appClient = GetCurrentTenant();

                _contractCategory.TenantId = appClient.Id;
                if (_contractCategory.Id == 0)
                {
                    _contractCategoryRepository.Insert(_contractCategory); // INSERT
                }
                else
                {
                    _contractCategoryRepository.Update(_contractCategory); // UPDATE
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Administrare.Contracte.Beneficiari.Acces")]
        public void DeleteContractCategory(int id)
        {
            var contractCategory = _contractCategoryRepository.Get(id);

            var count = _contractRepository.GetAll().Count(f => f.ContractsCategoryId == id);

            if (count != 0)
            {
                throw new UserFriendlyException("Eroare", "Categoria nu poate fi stearsa, deoarece este folosita in modulul Contracte");
            }
            _contractCategoryRepository.Delete(contractCategory);
            CurrentUnitOfWork.SaveChanges();

        }

        [AbpAuthorize("General.Contracte.Modificare")]
        public void DeleteContract(int id)
        {
            try
            {
                var contract = _contractRepository.Get(id);
                var count = _invoiceRepository.GetAllIncluding(f => f.Contracts).Count(f => f.ContractsId == id);
                if (count != 0)
                {
                    throw new Exception("Contractul nu poate fi sters, deoarece a fost folosit in modulul Facturi");
                }

                _contractRepository.Delete(contract);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void ExtendContractPeriod(DateTime currentDate)
        {
            var date = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            var contractsList = _contractRepository.GetAllIncluding(f => f.ContractsStateList).ToList().Where(f => f.EndDate == date && f.PrelungireAutomata == true && f.State == State.Active && f.GetContractState != Contract_State.Finalizat).ToList();

            foreach (var contract in contractsList)
            {
                contract.EndDate = contract.EndDate.AddMonths(contract.NrLuniPrelungire.Value);
                _contractRepository.Update(contract);
                CurrentUnitOfWork.SaveChanges();
            }
        }

        public List<ContractDto> GetContractsForInvoiceThirdParty(int thirdPartyId)
        {
            var ret = _contractRepository.GetAllIncluding(f => f.ContractsStateList).ToList().Where(c => c.ThirdPartyId == thirdPartyId && c.GetContractState == Contract_State.InVigoare).ToList();
            var list = ObjectMapper.Map<List<ContractDto>>(ret);
            return list;
        }
    }
}
