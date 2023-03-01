using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Models.Administration;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class OperationRepository : ErpRepositoryBase<Operation, int>, IOperationRepository
    {
        AccountRepository _accountRepository;

        public OperationRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            // verificare daca e instantiat corect
            _accountRepository = new AccountRepository(context);

        }

        public void OperationDelete(Operation operation)
        {
            try
            {

                var check = OperationCheck(operation, OperationType.Delete, null);
                if (check)
                {

                    var x = Context.Operations.First(f => f.Id == operation.Id);
                    x.State = State.Inactive;
                    Context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool VerifyClosedMonth(DateTime date) // returneaza true daca luna nu e inchisa
        {
            var ret = true;
            var count = Context.Balance.Count(f => f.Status == State.Active && f.BalanceDate.Month == date.Month && f.BalanceDate.Year == date.Year);
            ret = (count == 0) ? true : false;
            return ret;
        }

        public bool OperationCheck(Operation operation, OperationType operationType, int? localCurId)
        {
            var ret = true;
            
            switch (operationType)
            {
                case (OperationType.Add):
                    {

                        var closedMonth = VerifyClosedMonth(operation.OperationDate);

                        if (closedMonth)
                        {
                            //Make a distinct list with the accounts from the details
                            var AccountList = _accountRepository.AccountList().ToArray();
                            var AccountRelationlist = _accountRepository.AccountRelationList().ToList();

                            var DistinctAccount = operation.OperationsDetails.Select(x => x.Debit).Concat(operation.OperationsDetails.Select(x => x.Credit)).Distinct();
                            foreach (Account acc in DistinctAccount)
                            {
                                var verifyAccount = _accountRepository.VerifyAccount(acc, AccountList);
                                if (!verifyAccount)
                                    throw new Exception("Nu este permisa operatie pe cont sintetic!");
                            }

                            foreach (OperationDetails opD in operation.OperationsDetails)
                            {
                                var verifyAccountsRelation = _accountRepository.VerifyAccountsRelation(opD.Debit.Id, opD.Credit.Id, AccountRelationlist);
                                if (!verifyAccountsRelation)
                                    throw new Exception("Nu este definita aceasta monografie de conturi!");

                                if (operation.Currency.Id != localCurId && opD.ValueCurr.Equals(0.0M))
                                    throw new Exception("Nu ati completat valoarea in valuta!");

                                if (opD.Value.Equals(0.0M))
                                    throw new Exception("Nu ati completat valoarea!");
                            }
                            return true;
                        }
                        else
                            throw new Exception("Luna este inchisa! Nu se pot adauga/modifica/sterge operatii in aceasta perioada!");
                    }

                case (OperationType.Modify):
                    {
                        var closedMonth = VerifyClosedMonth(operation.OperationDate);
                        if (closedMonth)
                        {
                            //Make a distinct list with the accounts from the details
                            var AccountList = _accountRepository.AccountList().ToArray();
                            var AccountRelationlist = _accountRepository.AccountRelationList().ToList();

                            var DistinctAccount = operation.OperationsDetails.Select(x => x.Debit).Concat(operation.OperationsDetails.Select(x => x.Credit)).Distinct();
                            foreach (Account acc in DistinctAccount)
                            {
                                var verifyAccount = _accountRepository.VerifyAccount(acc, AccountList);
                                if (!verifyAccount)
                                    throw new Exception("Nu este permisa operatie pe cont sintetic!");
                            }

                            foreach (OperationDetails opD in operation.OperationsDetails)
                            {
                                var verifyAccountsRelation = _accountRepository.VerifyAccountsRelation(opD.Debit.Id, opD.Credit.Id, AccountRelationlist);
                                if (!verifyAccountsRelation)
                                    throw new Exception("Nu este definita aceasta monografie de conturi!");

                                if (operation.Currency.Id != localCurId && opD.ValueCurr.Equals(0.0M))
                                    throw new Exception("Nu ati completat valoarea in valuta!");

                                if (opD.Value.Equals(0.0M))
                                    throw new Exception("Nu ati completat valoarea!");
                            }
                            return true;
                        }
                        else
                            throw new Exception("Luna este inchisa! Nu se pot adauga/modifica/sterge operatii in aceasta perioada!");
                    }
                case (OperationType.Delete):
                    {
                        if (operation.OperationStatus == OperationStatus.Checked)
                        {
                            throw new Exception("Nu puteti sterge operatioa contabila deoarece este validata");
                        }

                        var closedMonth = VerifyClosedMonth(operation.OperationDate);

                        if (!closedMonth)
                            throw new Exception("Luna este inchisa! Nu se pot adauga/modifica/sterge operatii in aceasta perioada!");

                        break;

                    }
            }
            return true;
        }

        public IQueryable<Operation> GetAllIncludingOperationDetails()
        {
            var ret = Context.Operations.Include(f => f.OperationsDetails).ThenInclude(g => g.Credit)
                                        .Include(f => f.OperationsDetails).ThenInclude(g => g.Debit);

            return ret;
        }

        public IQueryable<User> GetUsers()
        {
            return Context.Users.Where(f=>f.TenantId != null);
        }
    }
}
