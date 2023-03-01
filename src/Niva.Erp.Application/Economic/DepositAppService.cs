using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Economic.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface IDepositAppService : IApplicationService
    {
        List<DepositListDto> GetDepositList(DateTime dataStart, DateTime dataEnd, int? operationType);
        void SaveDeposit(DepositEditDto deposit);
        void DeleteDeposit(int depositId);
        DepositEditDto GetDeposit(int depositId);
    }
    public class DepositAppService : ErpAppServiceBase, IDepositAppService
    {
        IRepository<BankAccount> _bankAccountRepository;
        IOperationRepository _operationRepository;
        IDispositionRepository _dispositionRepository;

        public DepositAppService(IRepository<BankAccount> bankAccountRepository, IDispositionRepository dispositionRepository, IOperationRepository operationRepsitory)
        {
            _bankAccountRepository = bankAccountRepository;
            _dispositionRepository = dispositionRepository;
            _operationRepository = operationRepsitory;
        }

        [AbpAuthorize("Casierie.Numerar.DepunRetrag.Modificare")]
        public void DeleteDeposit(int depositId)
        {
            var deposit = _dispositionRepository.Get(depositId);

            if (!_operationRepository.VerifyClosedMonth(deposit.DispositionDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            deposit.State = State.Inactive;

        }

        //[AbpAuthorize("Casierie.Numerar.DepunRetrag.Acces")]
        public DepositEditDto GetDeposit(int depositId)
        {
            DepositEditDto disposition;
            if (depositId == 0)
            {
                disposition = new DepositEditDto
                {
                    DispositionDate = LazyMethods.Now(),
                    DocumentDate = LazyMethods.Now()
                };
            }
            else
            {
                var disp = _dispositionRepository.GetAllIncluding(f => f.BankAccount).FirstOrDefault(f => f.Id == depositId);
                disposition = new DepositEditDto
                {
                    Id = disp.Id,
                    OperationType = disp.OperationType,
                    DispositionDate = disp.DispositionDate,
                    Value = disp.Value,
                    CurrencyId = disp.CurrencyId,
                    Description = disp.Description,
                    DocumentNumber = disp.DocumentNumber,
                    DocumentDate = disp.DocumentDate.Value,
                    SumOper = disp.SumOper,
                    State = disp.State,
                    BankAccountId = disp.BankAccountId.Value,
                    BankId = disp.BankAccount.BankId,
                    TenantId = disp.TenantId
                };
            }

            return disposition;

        }

        //[AbpAuthorize("Casierie.Numerar.DepunRetrag.Acces")]
        public List<DepositListDto> GetDepositList(DateTime dataStart, DateTime dataEnd, int? operationType)
        {
            var depositList = _dispositionRepository.GetAllIncluding(f => f.BankAccount, f => f.BankAccount.Bank, f => f.BankAccount.Bank.LegalPerson, f => f.Currency)
                                                    .Where(f => f.DispositionDate >= dataStart &&
                                                                f.DispositionDate <= dataEnd &&
                                                                f.State == State.Active &&
                                                                (f.OperationType == OperationType.Depunere || f.OperationType == OperationType.Retragere))
                                                    .ToList();

            if (operationType != null)
            {
                depositList = depositList.Where(f => f.OperationType == (OperationType)operationType).ToList();
            }
            var ret = ObjectMapper.Map<List<DepositListDto>>(depositList);
            return ret;
        }

        [AbpAuthorize("Casierie.Numerar.DepunRetrag.Modificare")]
        public void SaveDeposit(DepositEditDto deposit)
        {
            var appClient = GetCurrentTenant();

            if (!_operationRepository.VerifyClosedMonth(deposit.DispositionDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var sold = _dispositionRepository.SoldPrec(deposit.DispositionDate.AddDays(-1), deposit.CurrencyId);
            var intraDay = _dispositionRepository.GetAll()
                                                 .Where(f => f.DispositionDate == deposit.DispositionDate && f.State == State.Active && f.Id != deposit.Id && f.OperationType != OperationType.SoldInitial)
                                                 .Sum(f => f.SumOper);
            sold += intraDay;

            deposit.SumOper = (deposit.OperationType == OperationType.Retragere) ? deposit.Value : -deposit.Value;
            if (sold < Math.Abs(deposit.SumOper) && deposit.OperationType == OperationType.Depunere)
            {
                throw new UserFriendlyException("Eroare", $"Soldul { sold } este insuficient pentru a efectua o plata in valoare de {deposit.Value}");
            }

            var _disp = ObjectMapper.Map<Disposition>(deposit);

            try
            {
                if (_disp.Id == 0)
                {
                    _disp.TenantId = appClient.Id;

                    _dispositionRepository.Insert(_disp);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _dispositionRepository.Update(_disp);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex);
            }
        }

        public List<ThirdPartyAccListDto> GetBankAccountsByBankId(int bankId)
        {
            var ret = _bankAccountRepository.GetAll().Where(f => f.BankId == bankId)
                                       .OrderBy(f => f.IBAN)
                                       .Select(f => new ThirdPartyAccListDto { Id = f.Id, Iban = f.IBAN })
                                       .ToList();
            return ret;
        }
    }
}
