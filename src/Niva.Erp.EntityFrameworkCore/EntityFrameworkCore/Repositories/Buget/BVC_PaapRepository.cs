using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_PaapRepository : ErpRepositoryBase<BVC_PAAP, int>, IBVC_PaapRepository
    {
        public BVC_PaapRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public void DeleteAllPaapAvbSumForYear(int year)
        {
            var paapAvbSumList = Context.BVC_PAAP_AvailableSum.Where(f => f.ApprovedYear == year).ToList();
            Context.BVC_PAAP_AvailableSum.RemoveRange(paapAvbSumList);
            Context.SaveChanges();
        }

        public void InsertPAAPState(int paapId, DateTime operationDate, PAAP_State? state, string comentariu)
        {
            var paap = Context.BVC_PAAP.FirstOrDefault(f => f.Id == paapId);
            var paapState = new BVC_PAAP_State()
            {
                BVC_PAAP_Id = paapId,
                OperationDate = operationDate,
                Paap_State = (PAAP_State)state,
                Comentarii = comentariu,
                CotaTVA_Id = paap.CotaTVA_Id,
                ValoareEstimataFaraTvaLei = paap.ValoareEstimataFaraTvaLei,
                ValoareTotalaLei = paap.ValoareTotalaLei,
                ValoareTotalaValuta = paap.ValoareTotalaValuta,
            };
            Context.BVC_PAAP_State.Add(paapState);
            UnitOfWorkManager.Current.SaveChanges();
        }

        public void SaveAvailableSum(List<BVC_PAAP> list, int selectedYear)
        {
            foreach (var item in list.Select(f => new { f.InvoiceElementsDetailsCategoryId, f.DepartamentId }).Distinct())
            {
                var paapSumByCategory = Context.BVC_PAAP_AvailableSum.Include(f => f.InvoiceElementsDetailsCategory).Include(f => f.Departament)
                                                                     .FirstOrDefault(f => f.InvoiceElementsDetailsCategoryId == item.InvoiceElementsDetailsCategoryId && f.DepartamentId == item.DepartamentId);

                if (paapSumByCategory == null)
                {
                    paapSumByCategory = new BVC_PAAP_AvailableSum();
                    paapSumByCategory.InvoiceElementsDetailsCategoryId = item.InvoiceElementsDetailsCategoryId;
                    paapSumByCategory.DepartamentId = item.DepartamentId;
                }

                foreach (var paap in list.Where(f => f.InvoiceElementsDetailsCategoryId == item.InvoiceElementsDetailsCategoryId && f.DepartamentId == item.DepartamentId))
                {
                    if (paapSumByCategory.Id == 0)
                    {
                        paapSumByCategory.SumApproved += paap.ValoareEstimataFaraTvaLei;
                    }
                    paapSumByCategory.SumAllocated += paap.ValoareEstimataFaraTvaLei;
                }

                paapSumByCategory.Rest = paapSumByCategory.SumApproved - paapSumByCategory.SumAllocated;
                paapSumByCategory.ApprovedYear = selectedYear;

                if (paapSumByCategory.Id == 0)
                {
                    // insert
                    Context.BVC_PAAP_AvailableSum.Add(paapSumByCategory);
                }
                else
                { // update
                    Context.BVC_PAAP_AvailableSum.Update(paapSumByCategory);
                }
                UnitOfWorkManager.Current.SaveChanges();
            }
        }

        public void UpdatePaapAvailableValues(BVC_PAAP paap)
        {
            var paapValues = Context.BVC_PAAP_AvailableSum.Include(f => f.InvoiceElementsDetailsCategory).Include(f => f.Departament)
                                                          .FirstOrDefault(f => f.InvoiceElementsDetailsCategoryId == paap.InvoiceElementsDetailsCategoryId && f.DepartamentId == paap.DepartamentId);

            if (paapValues != null)
            {
                switch (paap.GetPaapState)
                {
                    case PAAP_State.Anulat:
                        // caut facturi atasate
                        var facturiAtasate = Context.BVC_PAAP_InvoiceDetails.Include(f => f.InvoiceDetails).Where(f => f.BVC_PAAPId == paap.Id).Sum(f => f.InvoiceDetails.Value - f.InvoiceDetails.VAT);

                        paapValues.SumAllocated -= (paap.ValoareEstimataFaraTvaLei - facturiAtasate);
                        break;

                    case PAAP_State.Inregistrat:
                        paapValues.SumAllocated += paap.ValoareEstimataFaraTvaLei;
                        break;
                }
                paapValues.Rest = paapValues.SumApproved - paapValues.SumAllocated;
                Context.BVC_PAAP_AvailableSum.Update(paapValues);
                Context.SaveChanges();
            }
        }

        public void UpdatePaapTranse(BVC_PAAP paap, int? cotaTvaId)
        {
            try
            {
                var transeList = Context.BVC_PAAPTranse.Include(f => f.BVC_PAAP).Where(f => f.BVC_PAAPId == paap.Id).OrderBy(f => f.DataTransa).ToList();
                var cotaTva = Context.CotaTVA.FirstOrDefault(f => f.Id == cotaTvaId);
                if (cotaTvaId == null)
                {
                    throw new Exception("Nu ati selectat cota de TVA");
                }
                foreach (var transa in transeList)
                {
                    transa.ValoareLeiFaraTVA = Math.Round(transa.ValoareLei / (100 + cotaTva.VAT) * 100, 2);
                }
                Context.SaveChanges();

                //if (cotaTvaId!= null)
                //{
                //    if ((ContractsPaymentInstalmentFreq)paap.ContractsPaymentInstalmentFreq == ContractsPaymentInstalmentFreq.Ocazional)
                //    {
                //        var transa = Context.BVC_PAAPTranse.Include(f => f.BVC_PAAP).FirstOrDefault(f => f.BVC_PAAPId == paap.Id);

                //        if (transa != null)
                //        {
                //            transa.ValoareLei = paap.ValoareTotalaLei;
                //            transa.ValoareLeiFaraTVA = paap.ValoareEstimataFaraTvaLei;

                //            Context.BVC_PAAPTranse.Update(transa);
                //            Context.SaveChanges();
                //        }
                //    }
                //    else
                //    {
                //        var transeList = Context.BVC_PAAPTranse.Include(f => f.BVC_PAAP).Where(f => f.BVC_PAAPId == paap.Id).OrderBy(f => f.DataTransa).ToList();
                //        int nrTranse = paap.NrTranse;

                //        foreach (var transa in transeList)
                //        {
                //            while (nrTranse != 0)
                //            {
                //                var valLei = Math.Round(paap.ValoareTotalaLei / paap.NrTranse, 2);
                //                var valLeiFaraTva = Math.Round(paap.ValoareEstimataFaraTvaLei / paap.NrTranse, 2);

                //                transa.ValoareLei = valLei;
                //                transa.ValoareLeiFaraTVA = valLeiFaraTva;

                //                Context.BVC_PAAPTranse.Update(transa);
                //                Context.SaveChanges();

                //                nrTranse--;
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertPaapAvailableSum(BVC_PAAP paap)
        {
            var paapAvailableSum = new BVC_PAAP_AvailableSum
            {
                DepartamentId = paap.DepartamentId,
                InvoiceElementsDetailsCategoryId = paap.InvoiceElementsDetailsCategoryId,
                State = State.Active,
                SumAllocated = paap.ValoareEstimataFaraTvaLei,
                SumApproved = paap.ValoareEstimataFaraTvaLei,
                Rest = 0,
            };

            Context.BVC_PAAP_AvailableSum.Add(paapAvailableSum);
            Context.SaveChanges();
        }
    }
}