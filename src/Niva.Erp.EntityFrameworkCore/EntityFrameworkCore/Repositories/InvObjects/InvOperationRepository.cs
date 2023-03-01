using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Repositories.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.InvObjects
{
    public class InvOperationRepository : ErpRepositoryBase<InvObjectOper, int>, IInvOperationRepository
    {
        public InvOperationRepository(IDbContextProvider<ErpDbContext> context): base(context)
        {

        }

        public int NextDocumentNumber(int documentTypeId)
        {
            int ret = GetNextNumber(documentTypeId);
            return ret;
        }

        public DateTime UnprocessedDate()
        {
            DateTime ret;
            var invObjectDate = Context.InvObjectItem.Where(f => f.State == State.Active && f.Processed == false).OrderBy(f => f.OperationDate).FirstOrDefault();

            var operation = Context.InvObjectOper.Where(f => f.State == State.Active && f.Processed == false).OrderBy(f => f.OperationDate).FirstOrDefault();

            DateTime gestDate;
            var countStock = Context.InvObjectStock.Count();

            if(countStock == 0)
            {
                gestDate = LazyMethods.Now();
            }
            else
            {
                gestDate = Context.InvObjectStock.Max(f => f.StockDate).AddDays(1);
            }

            ret = (invObjectDate == null) ? LazyMethods.Now() : invObjectDate.OperationDate;

            if(operation != null)
            {
                var operDate = operation.OperationDate;
                ret = (ret < operDate) ? ret : operDate;
            }
            ret = (ret < gestDate) ? ret : gestDate;

            return ret;
        }

        // calculez gestiune pentru obiectele de inventar
        public void GestComputing(DateTime operationDate)
        {
            try
            {
                var lastProcessedDate = UnprocessedDate();
                var operList = OperationList(operationDate);

                foreach (var item in operList)
                {
                    ComputeOperation(item);
                    Context.SaveChanges();
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GestInvObjectsComputing(DateTime operationDate)
        {
            try
            {
                var invObjectsList = InvObjectsList(operationDate);
                foreach (var invObject in invObjectsList)
                {
                    var operList = OperListForInvObject(invObject.Id, operationDate);
                    foreach (var item in operList)
                    {
                        ComputeOperation(item);
                        Context.SaveChanges();
                    }
                    Context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw new Exception("Gestiunea nu poate fi calculata");
            }
        }

        // sterg gestiunea pentru data primita ca parametru
        public void GestInvObjectDelComputing(DateTime deleteDate)
        {
            var _gestList = Context.InvObjectStock
                                .Where(f => f.StockDate >= deleteDate)
                                .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                var invObjectId = gest.InvObjectItemId;
                if (gest.InvObjectItemPFId != null)
                {
                    var invObjectPF = Context.InvObjectItem.FirstOrDefault(f => f.Id == gest.InvObjectItemPFId);
                    invObjectPF.Processed = false;
                }
                if (gest.InvObjectOperDetId != null)
                {
                    var operDetail = Context.InvObjectOperDetail.FirstOrDefault(f => f.Id == gest.InvObjectOperDetId);
                    var oper = Context.InvObjectOper.FirstOrDefault(f => f.Id == operDetail.InvObjectOperId);
                    oper.Processed = false;
                }

                //verific daca au fost definite obiecte de inventar
                var inventariere = Context.InvObjectInventariereDet.Include(f => f.InvObjectInventariere).Where(f => f.InvObjectStockId == gest.Id && f.InvObjectInventariere.State == State.Active).ToList();
                if (inventariere.Count > 0)
                {
                    throw new Exception("Gestiunea nu poate fi stearsa, deoarece au fost definite obiecte de inventar. Stergeti obiectele de inventar din modulul Conta -> Obiecte de inventar -> Inventariere");
                }

                Context.InvObjectStock.Remove(gest);
            }
            Context.SaveChanges();
        }

        public void GestDelComputingForInvObject(DateTime operationDate, int invObjectId)
        {
            var _gestList = Context.InvObjectStock
                                .Where(f => f.StockDate >= operationDate && f.InvObjectItemId == invObjectId)
                                .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                var invObjectItemId = gest.InvObjectItemId;
                if (gest.InvObjectItemPFId != null)
                {
                    var invObjectPF = Context.InvObjectItem.FirstOrDefault(f => f.Id == gest.InvObjectItemPFId);
                    invObjectPF.Processed = false;
                }
                if (gest.InvObjectOperDetId != null)
                {
                    var operDetail = Context.InvObjectOperDetail.FirstOrDefault(f => f.Id == gest.InvObjectOperDetId);
                    var oper = Context.InvObjectOper.FirstOrDefault(f => f.Id == operDetail.InvObjectOperId);
                    oper.Processed = false;
                }

                //verific daca au fost definite obiecte de inventar
                var inventariere = Context.InvObjectInventariereDet.Include(f => f.InvObjectInventariere).Where(f => f.InvObjectStockId == gest.Id && f.InvObjectInventariere.State == State.Active).ToList();
                if (inventariere.Count > 0)
                {
                    throw new Exception("Operatia nu poate fi inregistrata, deoarece au fost definite obiecte de inventar. Stergeti obiectele de inventar din modulul Conta -> Obiecte de inventar -> Inventariere");
                }
                Context.InvObjectStock.Remove(gest);
            }
            Context.SaveChanges();
        }

        // ultima data procesata in gestiune pentru obiectul de inventar
        public DateTime LastProcessedDateForInvObject(int invObjectId)
        {
            DateTime ret;

            var count = Context.InvObjectStock.Where(f => f.InvObjectItemId == invObjectId).Count();

            if(count != 0)
            {
                ret = Context.InvObjectStock.Where(f => f.InvObjectItemId == invObjectId).Max(f => f.StockDate);
            }else
            {
                var invObject = Context.InvObjectItem.Where(f => f.State == State.Active && f.Processed == true && f.Id == invObjectId).OrderBy(f => f.OperationDate).FirstOrDefault();

                var operation = Context.InvObjectOper.Where(f => f.State == State.Active && f.Processed == true).OrderBy(f => f.OperationDate).FirstOrDefault();

                ret = (invObject == null) ? DateTime.Now.AddYears(-10) : invObject.OperationDate;

                if(operation != null)
                {
                    var operDate = operation.OperationDate;
                    ret = (ret < operDate) ? ret : operDate;
                }
            }

            return ret;
        }

        // ultima data procesata in gestiune
        public DateTime LastProcessedDate()
        {
            DateTime ret;

            var count = Context.InvObjectStock.Count();

            if (count != 0)
            {
                ret = Context.InvObjectStock.Max(f => f.StockDate);
            }
            else
            {
                var invObject = Context.InvObjectItem.Where(f => f.State == State.Active && f.Processed == true).OrderBy(f => f.OperationDate).FirstOrDefault();

                var operation = Context.InvObjectOper.Where(f => f.State == State.Active && f.Processed == true).OrderBy(f => f.OperationDate).FirstOrDefault();

                ret = (invObject == null) ? DateTime.Now.AddYears(-10) : invObject.OperationDate;

                if (operation != null)
                {
                    var operDate = operation.OperationDate;
                    ret = (ret < operDate) ? ret : operDate;
                }
            }

            return ret;
        }

        // calculez gestiunea pentru obiecte de inventar
        public void GestComputingForInvObject(int invObjectId, DateTime operationDate)
        {
            try
            {
                var lastProcessedDateForInvObject = LastProcessedDateForInvObject(invObjectId);
                var invObjectOperList = OperListForInvObject(invObjectId, operationDate);

                foreach (var item in invObjectOperList)
                {
                    ComputeOperation(item);
                    Context.SaveChanges();
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // verific daca a fost creat obiectul de inventar pornind de la factura
        public void ExistingInvObjectFromInvoice(int invoiceId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Where(f => f.Id == invoiceId).FirstOrDefault();
            foreach (var item in invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar))
            {
                var invObject = Context.InvObjectItem.Where(f => f.State == State.Active && f.InvoiceDetailsId == item.Id).ToList();
                if (invObject.Count > 0)
                {
                    throw new Exception("obiectul de inventar a fost creat");
                }
            }
        }

        public void UpdateInvObjectOperation(InvObjectOper oper)
        {
            var _operation = Context.InvObjectOper.Include(f => f.OperDetails).FirstOrDefault(f => f.Id == oper.Id);

            Context.Entry(_operation).CurrentValues.SetValues(oper);

            // Delete children
            foreach (var detail in _operation.OperDetails.ToList())
            {
                if (!oper.OperDetails.Any(c => c.Id == detail.Id))
                    Context.InvObjectOperDetail.Remove(detail);
            }

            // Update and Insert children
            foreach (var detail in oper.OperDetails)
            {
                var existingDetail = _operation.OperDetails
                    .Where(c => c.Id == detail.Id)
                    .SingleOrDefault();

                if (existingDetail != null)
                    // Update child
                    Context.Entry(existingDetail).CurrentValues.SetValues(detail);
                else
                {
                    // Insert child
                    _operation.OperDetails.Add(detail);
                }
            }
            //Context.SaveChanges();
            UnitOfWorkManager.Current.SaveChanges();
        }

        public InvObjectStock GetGestDetailForInvObject(int invObjectId, DateTime operationDate)
        {
            var gest = Context.InvObjectStock.Where(f => f.InvObjectItemId == invObjectId && f.StockDate <= operationDate)
                                             .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id)
                                             .FirstOrDefault();
            return gest;
        }

        public List<InvObjectOperDetail> GetInvObjectOperDetails(int invObjectId, int appClientId)
        {
            var invObjectOperDetail = Context.InvObjectOperDetail.Include(f => f.InvObjectItem).Where(f => f.State == State.Active && f.TenantId == appClientId && f.InvObjectItemId == invObjectId &&
                                                                                                    f.InvObjectOper.State == State.Active/* && f.InvObjectOper.Processed ==true*/).ToList();
            return invObjectOperDetail;
        }

        private int GetNextNumber(int documentTypeId)
        {
            int ret = 0, invObjOper = 0, invObject = 0;
            int count = 0;

            // intrari obiecte de inventar
            count = Context.InvObjectItem.Count(f => f.State == 0 && f.DocumentTypeId == documentTypeId);
            if (count != 0)
            {
                invObject = Context.InvObjectItem.Where(f => f.State == 0 && f.DocumentTypeId == documentTypeId).Max(f=>f.DocumentNr);
            }

            //operatii obiecte de inventar
            count = Context.InvObjectOper
                            .Count(f => f.State == 0 && f.DocumentTypeId == documentTypeId);
            if (count != 0)
            {
                invObjOper = Context.InvObjectOper
                              .Where(f => f.State == 0 && f.DocumentTypeId == documentTypeId)
                              .Max(f => f.DocumentNr);
            }

            ret = invObject;
            ret = (invObject > invObjOper) ? invObject : invObjOper;
            ret++;
            return ret;
        }

        private List<OperationForComputing> OperationList(DateTime operationDate)
        {
            var ret = new List<OperationForComputing>();

            // lista obiectelor de inventar
            var invObjectList = Context.InvObjectItem.Where(f => f.State == State.Active && f.OperationDate <= operationDate && !f.Processed)
                                                    .ToList()
                                                    .Select(f => new OperationForComputing
                                                    {
                                                        Id = f.Id,
                                                        OperationDate = f.OperationDate,
                                                        OperationSource = OperationSource.InvObject,
                                                        OperationOrd = 1,
                                                        OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                                    })
                                                    .ToList();
            // lista obiectelor de inventar din operatii
            var invObjectOperList = Context.InvObjectOper.Where(f => f.State == State.Active && f.OperationDate <= operationDate && !f.Processed)
                                                         .ToList()
                                                         .Select(f => new OperationForComputing
                                                         {
                                                             Id = f.Id,
                                                             OperationDate = f.OperationDate,
                                                             OperationSource = OperationSource.InvObject,
                                                             OperationOrd = 2,
                                                             OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                                         })
                                                         .ToList();

            foreach (var item in invObjectList)
            {
                ret.Add(item);
            }

            foreach (var item in invObjectOperList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        private List<OperationForComputing> OperListForInvObject(int invObjectId, DateTime operationDate)
        {
            var ret = new List<OperationForComputing>();

            // lista obiectelor de inventar
            var invObjectList = Context.InvObjectItem.Where(f => f.State == State.Active && f.OperationDate <= operationDate && !f.Processed && f.Id == invObjectId)
                                                    .ToList()
                                                    .Select(f => new OperationForComputing
                                                    {
                                                        Id = f.Id,
                                                        OperationDate = f.OperationDate,
                                                        OperationSource = OperationSource.InvObject,
                                                        OperationOrd = 1,
                                                        OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                                    })
                                                    .ToList();
            // lista obiectelor de inventar din operatii
            var invObjectOperList = Context.InvObjectOper.Where(f => f.State == State.Active && f.OperationDate <= operationDate && !f.Processed && f.OperDetails.Select(f => f.InvObjectItemId == invObjectId).FirstOrDefault())
                                                         .ToList()
                                                         .Select(f => new OperationForComputing
                                                         {
                                                             Id = f.Id,
                                                             OperationDate = f.OperationDate,
                                                             OperationSource = OperationSource.InvObjectOper,
                                                             OperationOrd = 2,
                                                             OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                                         })
                                                         .ToList();

            foreach (var item in invObjectList)
            {
                ret.Add(item);
            }

            foreach (var item in invObjectOperList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        private void ComputeOperation(OperationForComputing operation)
        {
            try
            {
                switch (operation.OperationSource)
                {
                    case OperationSource.InvObject:
                        ComputeInvObject(operation);
                        break;
                    case OperationSource.InvObjectOper:
                        ComputeInvOper(operation);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // procesare din InvObjectItem
        private void ComputeInvObject(OperationForComputing operation)
        {
            try
            {
                var invObject = Context.InvObjectItem.FirstOrDefault(f => f.Id == operation.Id);
                int tranzQuantity = 0, quantity = 0;
                decimal tranzInventoryValue = 0, inventoryValue = 0;

                bool inConservare = false;

                tranzQuantity = invObject.Quantity;
                tranzInventoryValue = Math.Round(invObject.InventoryValue * tranzQuantity, 2);

                quantity += tranzQuantity;
                inventoryValue += tranzInventoryValue;

                var stock = new InvObjectStock
                {
                    InvObjectItemId = invObject.Id,
                    InvObjectItemPFId = invObject.Id,
                    InvObjectOperDetId = null,
                    StockDate = invObject.OperationDate,
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    StorageId = invObject.InvObjectStorageId ?? 0,
                    OperType = InvObjectOperType.Intrare,
                };

                Context.InvObjectStock.Add(stock);
                invObject.Processed = true;
                Context.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Eroare adaugare obiect de inventar in gestiune");
            }

        }

        // procesare operatie 
        private void ComputeInvOper(OperationForComputing operation)
        {
            var invOper = Context.InvObjectOper.Include(f => f.OperDetails).FirstOrDefault(f => f.Id == operation.Id);
            // reevaluare
            if (invOper.InvObjectsOperType == InvObjectOperType.Reevaluare)
            {
                ComputeReevaluare(invOper);
            }
            // modernizari, intrari/iesiri in conservare
            else if ((invOper.InvObjectsOperType == InvObjectOperType.Modernizare))
            {
                ComputeOperOperation(invOper);
            }
            // transfer / bonuri de miscare
            else if ((invOper.InvObjectsOperType == InvObjectOperType.BonMiscare) || (invOper.InvObjectsOperType == InvObjectOperType.Transfer) || (invOper.InvObjectsOperType == InvObjectOperType.DareInConsum))
            {
                ComputeTransfer(invOper);
            }
            // vanzare
            else if ((invOper.InvObjectsOperType == InvObjectOperType.Vanzare) || (invOper.InvObjectsOperType == InvObjectOperType.Casare) || (invOper.InvObjectsOperType == InvObjectOperType.Iesire))
            {
                ComputeSale(invOper);
            }
            invOper.Processed = true;
        }

        private void ComputeReevaluare(InvObjectOper invOper)
        {
            foreach (var item in invOper.OperDetails)
            {
                var stockItem = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                       .Where(f => f.InvObjectItemId == item.InvObjectItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                int tranzQuantity = 0, quantity = stockItem.Quantity;
                decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue;

                bool inConservare = stockItem.InConservare;

                tranzInventoryValue = item.InvValueModif;

                quantity += tranzQuantity;
                inventoryValue += tranzInventoryValue;

                var stock = new InvObjectStock
                {
                    InvObjectItemId = stockItem.InvObjectItemId,
                    InvObjectItemPFId = null,
                    InvObjectOperDetId = item.Id,
                    StockDate = new DateTime(invOper.OperationDate.Year, invOper.OperationDate.Month, invOper.OperationDate.Day),
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    StorageId = stockItem.StorageId,
                    OperType = invOper.InvObjectsOperType
                };
                Context.InvObjectStock.Add(stock);

                var reserveList = Context.InvObjectStockReserve.Where(f => f.InvObjectStockId == stockItem.Id).OrderBy(f => f.Id);
                if (item.InvValueModif > 0)
                {
                    // insert din istoric
                    foreach (var reserveItem in reserveList)
                    {
                        var reserveToDB = new InvObjectStockReserve
                        {
                            InvObjectStock = stock,
                            DeprecReserve = reserveItem.DeprecReserve,
                            Reserve = reserveItem.Reserve,
                            ExpenseReserve = reserveItem.ExpenseReserve,
                            TranzDeprecReserve = 0,
                            TranzReserve = 0,
                            InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                        };
                        Context.InvObjectStockReserve.Add(reserveToDB);
                    }
                    // inregistrarea noua
                    var reserveNew = new InvObjectStockReserve
                    {
                        InvObjectStock = stock,
                        TranzDeprecReserve = 0,
                        TranzReserve = item.InvValueModif,
                        DeprecReserve = 0,
                        Reserve = item.InvValueModif,
                        ExpenseReserve = 0,
                        InvObjectOperDetailId = item.Id
                    };
                    Context.InvObjectStockReserve.Add(reserveNew);
                }
                else
                {
                    // am rezerva negativa => diminuez din rezerva pozitiva
                    var rezNegativa = Math.Abs(item.InvValueModif);
                    var count = reserveList.Count();
                    int currCount = 1;
                    decimal expenseReserve = 0;

                    foreach (var reserveItem in reserveList)
                    {
                        decimal rezItemDim = 0;
                        if (reserveItem.Reserve > rezNegativa)
                        {
                            rezItemDim = rezNegativa;
                            rezNegativa = 0;
                        }
                        else
                        {
                            rezItemDim = reserveItem.Reserve;
                            rezNegativa = rezNegativa - rezItemDim;
                            if (currCount == count)
                            {
                                expenseReserve = rezNegativa;
                            }
                        }
                        var reserveToDB = new InvObjectStockReserve
                        {
                            InvObjectStock = stock,
                            DeprecReserve = reserveItem.DeprecReserve,
                            Reserve = reserveItem.Reserve - rezItemDim,
                            ExpenseReserve = expenseReserve,
                            TranzDeprecReserve = 0,
                            TranzReserve = -1 * rezItemDim,
                            InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                        };
                        currCount++;
                        Context.InvObjectStockReserve.Add(reserveToDB);
                    }
                }
            }
        }

        private void ComputeOperOperation(InvObjectOper invOper) // modernizari, intrari/iesiri in conservare
        {
            foreach (var item in invOper.OperDetails)
            {
                var stockItem = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                       .Where(f => f.InvObjectItemId == item.InvObjectItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                int tranzQuantity = 0, quantity = stockItem.Quantity;
                decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue;
               
                bool inConservare = (invOper.InvObjectsOperType == InvObjectOperType.Modernizare)
                                    ? stockItem.InConservare
                                    :false;

               
                tranzInventoryValue = item.InvValueModif;
                quantity += tranzQuantity;                
                inventoryValue += tranzInventoryValue;

                var stock = new InvObjectStock
                {
                    InvObjectItemId = stockItem.InvObjectItemId,
                    InvObjectItemPFId = null,
                    InvObjectOperDetId = item.Id,
                    StockDate = new DateTime(invOper.OperationDate.Year, invOper.OperationDate.Month, invOper.OperationDate.Day),
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    StorageId = stockItem.StorageId,
                    OperType = invOper.InvObjectsOperType,
                };
                Context.InvObjectStock.Add(stock);

                // rezerve
                var reserveList = Context.InvObjectStockReserve.Where(f => f.InvObjectStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new InvObjectStockReserve
                    {
                        InvObjectStock = stock,
                        DeprecReserve = reserveItem.DeprecReserve,
                        Reserve = reserveItem.Reserve,
                        ExpenseReserve = reserveItem.ExpenseReserve,
                        TranzDeprecReserve = reserveItem.TranzDeprecReserve,
                        TranzReserve = reserveItem.TranzReserve,
                        InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                    };
                    Context.InvObjectStockReserve.Add(reserveToDB);
                }
                Context.SaveChanges();
            }
        }

        private void ComputeTransfer(InvObjectOper invOper) // transfer / bonuri de miscare
        {
            foreach (var item in invOper.OperDetails)
            {
                var stockItem = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                       .Where(f => f.InvObjectItemId == item.InvObjectItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                // iesire din gestiune
                var stockOut = new InvObjectStock
                {
                    InvObjectItemId = stockItem.InvObjectItemId,
                    InvObjectItemPFId = null,
                    InvObjectOperDetId = item.Id,
                    StockDate = new DateTime(invOper.OperationDate.Year, invOper.OperationDate.Month, invOper.OperationDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = -1 * stockItem.Quantity,
                    Quantity = 0,
                    TranzDuration = -1 * stockItem.Duration,
                    Duration = 0,
                    TranzInventoryValue = -1 * stockItem.InventoryValue,
                    InventoryValue = 0,
                    TranzFiscalInventoryValue = -1 * stockItem.FiscalInventoryValue,
                    FiscalInventoryValue = 0,
                    TranzDeprec = -1 * stockItem.Deprec,
                    Deprec = 0,
                    TranzFiscalDeprec = -1 * stockItem.FiscalDeprec,
                    FiscalDeprec = 0,
                    StorageId = stockItem.StorageId,
                    OperType = invOper.InvObjectsOperType,
                    MonthlyDepreciation = stockItem.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation
                };
                Context.InvObjectStock.Add(stockOut);

                // rezerve
                var reserveList = Context.InvObjectStockReserve.Where(f => f.InvObjectStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new InvObjectStockReserve
                    {
                        InvObjectStock = stockOut,
                        DeprecReserve = 0,
                        Reserve = 0,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = -1 * reserveItem.DeprecReserve,
                        TranzReserve = -1 * reserveItem.Reserve,
                        InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                    };
                    Context.InvObjectStockReserve.Add(reserveToDB);
                }
                Context.SaveChanges();

                // intrare in gestiune
                var stockIn = new InvObjectStock
                {
                    InvObjectItemId = stockItem.InvObjectItemId,
                    InvObjectItemPFId = null,
                    InvObjectOperDetId = item.Id,
                    StockDate = new DateTime(invOper.OperationDate.Year, invOper.OperationDate.Month, invOper.OperationDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = stockItem.Quantity,
                    Quantity = stockItem.Quantity,
                    TranzInventoryValue = stockItem.InventoryValue,
                    InventoryValue = stockItem.InventoryValue,
                    StorageId = invOper.InvObjectsStoreInId ?? 0,
                    OperType = invOper.InvObjectsOperType
                };
                Context.InvObjectStock.Add(stockIn);

                // rezerve
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new InvObjectStockReserve
                    {
                        InvObjectStock = stockIn,
                        DeprecReserve = reserveItem.DeprecReserve,
                        Reserve = reserveItem.Reserve,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = reserveItem.DeprecReserve,
                        TranzReserve = reserveItem.Reserve,
                        InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                    };
                    Context.InvObjectStockReserve.Add(reserveToDB);
                }
                Context.SaveChanges();
            }
        }

        private void ComputeSale(InvObjectOper invOper) // vanzare
        {
            foreach (var item in invOper.OperDetails)
            {
                var stockItem = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                       .Where(f => f.InvObjectItemId == item.InvObjectItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();
                // iesire din gestiune
                var stockOut = new InvObjectStock
                {
                    InvObjectItemId = stockItem.InvObjectItemId,
                    InvObjectItemPFId = null,
                    InvObjectOperDetId = item.Id,
                    StockDate = new DateTime(invOper.OperationDate.Year, invOper.OperationDate.Month, invOper.OperationDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = -1 * stockItem.Quantity,
                    Quantity = 0,
                    TranzInventoryValue = -1 * stockItem.InventoryValue,
                    InventoryValue = 0,               
                    StorageId = stockItem.StorageId,
                    OperType = invOper.InvObjectsOperType
                };
                Context.InvObjectStock.Add(stockOut);

                // rezerve
                var reserveList = Context.InvObjectStockReserve.Where(f => f.InvObjectStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new InvObjectStockReserve
                    {
                        InvObjectStock = stockOut,
                        DeprecReserve = 0,
                        Reserve = 0,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = -1 * reserveItem.DeprecReserve,
                        TranzReserve = -1 * reserveItem.Reserve,
                        InvObjectOperDetailId = reserveItem.InvObjectOperDetailId
                    };
                    Context.InvObjectStockReserve.Add(reserveToDB);
                }
                Context.SaveChanges();
            }
        }

        private List<InvObjectItem> InvObjectsList(DateTime operationDate)
        {
            var invObjectList = new List<InvObjectItem>();

            // obiecte de inventar neprocesate in gestiune
            var invObjects = Context.InvObjectItem.Where(f => f.State == State.Active && !f.Processed).ToList();

            // obiecte de inventar cu sold != 0 la data specificata ca parametru
            var invObjectsInStock = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                                     .Where(f => f.StockDate <= operationDate)
                                                     .GroupBy(f => f.InvObjectItemId)
                                                     .Select(f => f.Max(f => f.Id))
                                                     .ToList();

            var stockList = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                                 .Where(f => f.Quantity != 0 && invObjectsInStock.Contains(f.Id))
                                                 .ToList();

            var assetOper = Context.ImoAssetOper.Where(f => f.State == State.Active && !f.Processed).ToList();

            foreach (var item in invObjects)
            {
                invObjectList.Add(item);
            }

            foreach (var item in stockList)
            {
                invObjectList.Add(item.InvObjectItem);
            }

            return invObjects;
        }

    }

    public class OperationForComputing
    {
        public int? Id { get; set; }

        public DateTime OperationDate { get; set; }

        public OperationSource OperationSource { get; set; }

        public int OperationOrd { get; set; }

        public DateTime OperationDateSort { get; set; }
    }

    public enum OperationSource
    {
        InvObject,
        InvObjectOper
    }
}
