using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Administration;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class ForeignOperationRepository : ErpRepositoryBase<ForeignOperation, int>, IForeignOperationRepository
    {
        OperationRepository _operationRepository;

        public ForeignOperationRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _operationRepository = new OperationRepository(context);
        }

        public void DeleteFO(int foreignOperationId)
        {
            try
            {
                var foreignOperation = Context.ForeignOperation.Include(f => f.ForeignOperationsDetails).ThenInclude(f => f.OperationsAccounting).FirstOrDefault(f => f.Id == foreignOperationId);

                var foDetailList = Context.ForeignOperationsDetails.Where(f => f.ForeignOperationId == foreignOperation.Id).ToList();

                //if(foDetailList.Count == 0)
                //{
                //    throw new Exception("Pentru acest extras nu au fost definite detalii");
                //}

                foreach (var item in foDetailList)
                {
                    
                    var paymentOrder = Context.PaymentOrders.Include(f => f.FgnOperationsDetail).FirstOrDefault(f => f.FgnOperDetailId == item.Id);

                    // daca OP-ul are asociat un cont bancar din extras => marchez OP-ul ca fiind nefinalizat si sterg legatura cu FgnOperationsDetail
                    if (paymentOrder != null) 
                    {
                        paymentOrder.Status = OperationStatus.Unchecked;
                        paymentOrder.FgnOperDetailId = null;
                        Context.PaymentOrders.Update(paymentOrder);
                    }

                    Context.ForeignOperationsDetails.Remove(item);
                }
                Context.SaveChanges();

                Context.ForeignOperation.Remove(foreignOperation);
                Context.SaveChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteNC(int foreignOperId)
        {
            try
            {
                var foreignOperation = Context.ForeignOperation.Include(f => f.ForeignOperationsDetails).ThenInclude(f => f.OperationsAccounting).FirstOrDefault(f => f.Id == foreignOperId);


                var ncDetailIdList = Context.ForeignOperationsAccounting.Include(f => f.FgnOperationsDetail).Include(f => f.FgnOperationsDetail.ForeignOperation)
                                            .Where(f => f.FgnOperationsDetail.ForeignOperationId == foreignOperation.Id && f.OperationsDetailId != null)
                                            .Select(f => f.OperationsDetailId ?? 0)
                                            .Distinct()
                                            .ToList();

                if (ncDetailIdList.Count == 0)
                {
                    throw new Exception("Pentru acest extras nu au fost generate notele contabile");
                }

                var ncList = Context.Operations.Include(f => f.OperationsDetails).Where(f => f.OperationsDetails.Any(x => ncDetailIdList.Contains(x.Id)));

                var operValidated = ncList.Where(f => f.OperationStatus == OperationStatus.Checked).ToList().Count;
                if (operValidated != 0)
                    throw new Exception("Operatia contabila este validata!");

                foreach (var item in foreignOperation.ForeignOperationsDetails)
                {
                    foreach (var detailItem in item.OperationsAccounting)
                    {
                        detailItem.OperationsDetailId = null;
                    }
                }
                Context.SaveChanges();

                foreach (var item in ncList.ToList())
                {
                    _operationRepository.OperationCheck(item, OperationType.Delete, null);
                    foreach (var detailItem in item.OperationsDetails.Select(f => f.Id).ToList())
                    {
                        var operDetail = Context.OperationsDetails.FirstOrDefault(f => f.Id == detailItem);
                        Context.OperationsDetails.Remove(operDetail);
                    }
                    item.State = State.Inactive;
                }
                Context.SaveChanges();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UpdateFgnOperationDetail(ForeignOperationsDetails operationDetail)
        {
            var existingOperationDetail = Context.ForeignOperationsDetails.Include(f => f.OperationsAccounting).FirstOrDefault(f => f.Id == operationDetail.Id);

            if (existingOperationDetail != null)
            {
                // Update parent
                Context.Entry(existingOperationDetail).CurrentValues.SetValues(operationDetail);

                // Delete children
                foreach (var detail in existingOperationDetail.OperationsAccounting.ToList())
                {

                    if (!operationDetail.OperationsAccounting.Any(c => c.Id == detail.Id))
                        Context.ForeignOperationsAccounting.Remove(detail);

                }

                // Update and Insert children
                foreach (var detail in operationDetail.OperationsAccounting)
                {
                    if (detail.Id != 0)
                    {
                        var existingDetail = existingOperationDetail.OperationsAccounting
                            .Where(c => c.Id == detail.Id)
                            .Single();

                        // Update child
                        Context.Entry(existingDetail).CurrentValues.SetValues(detail);
                    }
                    else
                    {
                        // Insert child 
                        existingOperationDetail.OperationsAccounting.Add(detail);
                        detail.FgnOperationsDetail = existingOperationDetail;
                    }
                }
                //Context.SaveChanges();
                UnitOfWorkManager.Current.SaveChanges();
            }
        }
    }
}
