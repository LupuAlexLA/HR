using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Economic
{
    public class InvoiceRepository : ErpRepositoryBase<Invoices, int>, IInvoiceRepository
    {
        private readonly IConfiguration  configuration;
        string fileDocDbName;
        public InvoiceRepository(IDbContextProvider<ErpDbContext> dbContextProvider, IConfiguration  configuration) : base(dbContextProvider)
        {
            this.configuration = configuration;
            fileDocDbName = configuration["App:fileDocDbName"];
        }

        //public IQueryable<Invoices> InvoicesForNIR()
        //{
        //    var invoiceIdList = Context.InvoiceDetails
        //                            .Include(f => f.InvoiceElementsDetails)
        //                            .Include(f => f.Invoices)
        //                            .Where(f => f.Invoices.State == State.Active && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar)
        //                            .ToList().Select(f => f.Invoices.Id);

        //    var invoiceInOperation = Context.InvOperation
        //                               .Where(f => f.AppClientId == selectedAppClientId && f.State == State.Active)
        //                               .Select(f => f.InvoiceId).ToList();


        //    var _nirList = Context.Invoices
        //                     .Include(f => f.ThirdParty)
        //                     .Include(f => f.ThirdParty.Person)
        //                     .Where(f => f.AppClientId == selectedAppClientId && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
        //                     .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber);

        //    return _nirList;
        //}

        public IQueryable<Invoices> GetAllIncludeElemDet()
        {
            var ret = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails);
            return ret;
        }

        public IQueryable<Invoices> InvoicesForAsset(int id)
        {
            var invoiceIdList = Context.InvoiceDetails
                                    .Include(f => f.InvoiceElementsDetails)
                                    .Include(f => f.Invoices)
                                    .Where(f => f.Invoices.TenantId == id && f.Invoices.State == State.Active
                                                && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe && f.Invoices.ThirdPartyQuality == ThirdPartyQuality.Furnizor && f.UsedInGest == false)
                                    .ToList().Select(f => f.Invoices.Id);

            var invoiceInOperation = Context.ImoAssetItem.Include(f => f.InvoiceDetails)
                                       .Where(f => f.TenantId == id && f.State == State.Active)
                                       .Select(f => f.InvoiceDetails.InvoicesId).ToList();


            var _assetList = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .Where(f => f.TenantId == id && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
                             .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber);

            return _assetList;
        }

        public List<Invoices> InvoicesForAssetSale(int id, int? invoiceId)
        {
            var invoiceIdList = Context.InvoiceDetails
                                    .Include(f => f.InvoiceElementsDetails)
                                    .Include(f => f.Invoices)
                                    .Where(f => f.Invoices.TenantId == id && f.Invoices.State == State.Active
                                                && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe 
                                                && f.Invoices.ThirdPartyQuality == ThirdPartyQuality.Client)
                                    .ToList().Select(f => f.Invoices.Id);

            var invoiceInOperation = Context.ImoAssetOperDetail.Include(f => f.ImoAssetOper)
                                       .Where(f => f.ImoAssetOper.TenantId == id && f.ImoAssetOper.State == State.Active && f.State == State.Active
                                              && f.ImoAssetOper.InvoiceId != null)
                                       .Select(f => f.ImoAssetOper.InvoiceId).ToList();


            var _assetList = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .Where(f => f.TenantId == id && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
                             .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber).ToList();

            if (invoiceId != 0)
            {
                var invoice = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .FirstOrDefault(f => f.TenantId == id && f.Id == invoiceId);
                _assetList.Add(invoice);
            }

            return _assetList;
        }

        public IQueryable<Invoices> InvoicesForPrepayments(int appClientId, PrepaymentType prepaymentType)
        {
            InvoiceElementsType elementType = (prepaymentType == PrepaymentType.CheltuieliInAvans) ? InvoiceElementsType.CheltuieliInAvans : InvoiceElementsType.VenituriInAvans;
            var invoiceIdList = Context.InvoiceDetails
                                    .Include(f => f.InvoiceElementsDetails)
                                    .Include(f => f.Invoices)
                                    .Where(f => f.Invoices.TenantId == appClientId && f.Invoices.State == State.Active && f.InvoiceElementsDetails.InvoiceElementsType == elementType && f.DurationInMonths != 0)
                                    .ToList().Select(f => f.Invoices.Id);

            var invoiceInOperation = Context.Prepayment.Include(f => f.InvoiceDetails)
                                       .Where(f => f.TenantId == appClientId && f.State == State.Active)
                                       .Select(f => f.InvoiceDetails.InvoicesId).ToList();


            var _prepaymentInvoiceList = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .Where(f => f.TenantId == appClientId && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
                             .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber);

            return _prepaymentInvoiceList;
        }

        public IQueryable<Invoices> InvoicesForInvObjects(int appClientId)
        {
            var invoiceIdList = Context.InvoiceDetails
                                    .Include(f => f.InvoiceElementsDetails)
                                    .Include(f => f.Invoices)
                                    .Where(f => f.Invoices.TenantId == appClientId && f.Invoices.State == State.Active
                                                && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar && f.Invoices.ThirdPartyQuality == ThirdPartyQuality.Furnizor && f.UsedInGest == false)
                                    .ToList().Select(f => f.Invoices.Id);

            var invoiceInOperation = Context.InvObjectItem.Include(f => f.InvoiceDetails)
                                       .Where(f => f.TenantId == appClientId && f.State == State.Active)
                                       .Select(f => f.InvoiceDetails.InvoicesId).ToList();


            var _invObjectsList = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .Where(f => f.TenantId == appClientId && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
                             .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber);

            return _invObjectsList;
        }

        public List<Invoices> InvoicesForInvObject(int appClientId, int? invoiceId)
        {
            var invoiceIdList = Context.InvoiceDetails
                                  .Include(f => f.InvoiceElementsDetails)
                                  .Include(f => f.Invoices)
                                  .Where(f => f.Invoices.TenantId == appClientId && f.Invoices.State == State.Active
                                              && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar
                                              && f.Invoices.ThirdPartyQuality == ThirdPartyQuality.Client)
                                  .ToList().Select(f => f.Invoices.Id);

            var invoiceInOperation = Context.InvObjectOperDetail.Include(f => f.InvObjectOper)
                                       .Where(f => f.InvObjectOper.TenantId == appClientId && f.InvObjectOper.State == State.Active && f.State == State.Active
                                              && f.InvObjectOper.InvoiceId != null)
                                       .Select(f => f.InvObjectOper.InvoiceId).ToList();


            var _invObjectList = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .Where(f => f.TenantId == appClientId && invoiceIdList.Contains(f.Id) && !invoiceInOperation.Contains(f.Id))
                             .OrderByDescending(f => f.InvoiceDate).ThenByDescending(f => f.InvoiceNumber).ToList();

            if (invoiceId != 0)
            {
                var invoice = Context.Invoices
                             .Include(f => f.ThirdParty)
                             .FirstOrDefault(f => f.TenantId == appClientId && f.Id == invoiceId);
                _invObjectList.Add(invoice);
            }

            return _invObjectList;
        }

        public Invoices GetById(int id)
        {
            var ret = Context.Invoices.Where(f => f.Id == id).Include(f => f.ThirdParty)
                                                             .Include(f => f.Currency)
                                                             .Include(f => f.DispositionInvoices)
                                                             .Include(f => f.PaymentOrderInvoices)
                                                             .Include(f => f.InvoiceDetails).ThenInclude(f => f.CotaTVA)
                                                             .Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails)
                                                                                            .ThenInclude(f=>f.InvoiceElementsDetailsCategory).FirstOrDefault();
            return ret;
        }

        public void InsertOrUpdateV(Invoices invoice)
        {
            var existingInvoice = GetById(invoice.Id);

            if (existingInvoice == null)
            {
                Insert(invoice);
            }

            if (existingInvoice != null)
            {
                // Update parent
                Context.Entry(existingInvoice).CurrentValues.SetValues(invoice);

                // Delete children
                foreach (var detail in existingInvoice.InvoiceDetails.ToList())
                {

                    if (!invoice.InvoiceDetails.Any(c => c.Id == detail.Id))
                        Context.InvoiceDetails.Remove(detail);

                }

                // Update and Insert children
                foreach (var detail in invoice.InvoiceDetails)
                {
                    if (detail.Id != 0)
                    {
                        var existingDetail = existingInvoice.InvoiceDetails
                            .Where(c => c.Id == detail.Id)
                            .Single();

                        // Update child
                        Context.Entry(existingDetail).CurrentValues.SetValues(detail);
                    }
                    else
                    {
                        // Insert child 
                        existingInvoice.InvoiceDetails.Add(detail);
                        detail.Invoices = existingInvoice;
                    }
                }
                //Context.SaveChanges();
                UnitOfWorkManager.Current.SaveChanges();
            }
        }

        public List<InvoiceElementsDetails> GetInvoiceElementsDetails()
        {
            var ret = Context.InvoiceElementsDetails.Include(f=>f.InvoiceElementsDetailsCategory).Where(d => d.State == State.Active).OrderBy(p => p.InvoiceElementsDetailsCategory.CategoryElementDetName)
                                                                                                                                     .ThenBy(p => p.Description)
                                                                                                                                     .ToList();
            return ret;

        }

        public InvoiceElementsDetails GetInvoiceElementsDetail(int id)
        {
            var ret = Context.InvoiceElementsDetails.Include(f =>f.InvoiceElementsDetailsCategory).FirstOrDefault(f => f.Id == id);
            return ret;
        }

        public void SaveInvoiceElementsDetail(InvoiceElementsDetails element)
        {
            var newElement = Context.InvoiceElementsDetails.SingleOrDefault(e => e.Id == element.Id);
            if (newElement != null)
            {
                //edit
                Context.Entry(newElement).CurrentValues.SetValues(element);
            }
            else
            {
                Context.InvoiceElementsDetails.Add(element);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }

        public List<InvoiceElementAccounts> GetElementAccounts()
        {
            var ret = Context.InvoiceElementAccounts.Where(e => e.State == State.Active).ToList();
            return ret;
        }

        public InvoiceElementAccounts GetElementAcounnt(int id)
        {
            var ret = Context.InvoiceElementAccounts.SingleOrDefault(e => e.State == State.Active && e.Id == id);
            return ret;
        }

        public void DeleteInvoice(int invoiceId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f=>f.InvoiceElementsDetails).FirstOrDefault(f => f.Id == invoiceId);
            // find references
            //int count = 0;
            //count = Context.PaymentOrders.Count(f => f.InvoiceId == invoiceId && f.State == State.Active);
            //if (count != 0)
            //{
            //    throw new Exception("Factura nu poate fi stearsa, deoarece este folosita in modulul Ordine de plata");
            //}
            //count = Context.InvOperation.Count(f => f.InvoiceId == invoiceId && f.State == State.Active);
            //if (count != 0)
            //{
            //    throw new Exception("Factura nu poate fi stearsa, deoarece este folosita in modulul Obiecte de inventar");
            //}
            //foreach (var detail in invoice.InvoiceDetails)
            //{
            //    count = Context.ImoAssetItem.Count(f => f.InvoiceDetailsId == detail.Id && f.State == State.Active);
            //    if (count != 0)
            //    {
            //        throw new Exception("Factura nu poate fi stearsa, deoarece este folosita in modulul Mijloace fixe");
            //    }

            //    count = Context.Prepayment.Count(f => f.InvoiceDetailsId == detail.Id && f.State == State.Active);
            //    if (count != 0)
            //    {
            //        throw new Exception("Factura nu poate fi stearsa, deoarece este folosita in modulul Venituri / Cheltuieli in avans");
            //    }

            //    count = Context.InvOperationDetail.Count(f => f.InvoiceDetailsId == detail.Id);
            //    if (count != 0)
            //    {
            //        throw new Exception("Factura nu poate fi stearsa, deoarece este folosita in modulul Obiecte de inventar");
            //    }

            //    if (detail.ContaOperationDetailId != null)
            //    {
            //        throw new Exception("Factura nu poate fi stearsa, deoarece a fost generata nota contabila");
            //    }

            //}

            foreach (var detail in invoice.InvoiceDetails.Where(f => (f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele)
                                                                || (f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans && f.DurationInMonths == 0)))
            {
                var operationDetail = Context.OperationsDetails.Include(f => f.Operation).Where(f => f.Id == detail.ContaOperationDetailId).FirstOrDefault();

                if(operationDetail != null)
                {
                    //if(operationDetail.Operation.OperationStatus == Models.Conta.OperationStatus.Checked)
                    //{
                    //    throw new Exception("Factura nu poate fi stearsa, deoarece nota contabila a fost validata");
                    //}else
                    //{
                    //    operationDetail.Operation.State = State.Inactive;
                    //}

                    // anulez operatia
                    var operation = Context.Operations.FirstOrDefault(f => f.Id == operationDetail.OperationId);
                    operation.State = State.Inactive;
                    if (operation.OperationParentId != null)
                    {
                        // anulez parintii
                        var operationParrent = Context.Operations.FirstOrDefault(f => f.Id == operation.OperationParentId);
                        operationParrent.State = State.Inactive;
                    }
                    var operationChild = Context.Operations.FirstOrDefault(f => f.OperationParentId == operation.Id);
                    if (operationChild != null)
                    {
                        operationChild.State = State.Inactive;
                    }
                }
            }

            var maxBalanceDate = Context.Balance.Where(f => f.Status == State.Active).Max(f => f.BalanceDate);
            if (maxBalanceDate >= invoice.OperationDate)
            {
                throw new Exception("Factura nu poate fi stearsa, deoarece luna contabila este inchisa");
            }

            invoice.State = State.Inactive;

        }
        public void DeleteInvoiceFromDecont(int invoiceId, int decontId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Include(f => f.ContaOperation).FirstOrDefault(f => f.Id == invoiceId && f.DecontId == decontId);
            foreach (var detail in invoice.InvoiceDetails)
            {
                var operationDetail = Context.OperationsDetails.Include(f => f.Operation).Where(f => f.Id == detail.ContaOperationDetailId).FirstOrDefault();

                if (operationDetail != null)
                {
                    operationDetail.Operation.State = State.Inactive;
                    var operationChildren = Context.Operations.Where(f => f.OperationParentId == operationDetail.OperationId);
                    if (operationChildren != null)
                    {
                        foreach (var item in operationChildren)
                        {
                            item.State = State.Inactive;
                        }
                    }
                }
            }
            invoice.State = State.Inactive;
            Context.SaveChanges();
        }

        public void DeleteInvoiceElementsDetail(int elementId)
        {
            // find invoiceElementDetail by id
            var invoiceElementsDetail = Context.InvoiceElementsDetails.FirstOrDefault(f => f.Id == elementId);

            var count = Context.InvoiceDetails.Count(f => f.InvoiceElementsDetailsId == elementId);
            if(count != 0)
            {
                throw new Exception("Elementul nu poate fi sters, deoarece a fost folosit in modulul Facturi");
            }

            Context.InvoiceElementsDetails.Remove(invoiceElementsDetail);
            Context.SaveChanges();
        }

        public List<InvoiceElementsDetailsCategory> GetInvoiceElementsDetailsCategories()
        {
            var ret = Context.InvoiceElementsDetailsCategory.Where(f => f.State == State.Active).OrderBy(f=>f.CategoryElementDetName).ToList();
            return ret;
        }

        public InvoiceElementsDetailsCategory GetElementsDetailsCategory(int id)
        {
            var ret = Context.InvoiceElementsDetailsCategory.Find(id);
            return ret;
        }

        public void SaveInvoiceElementsDetailCategory(InvoiceElementsDetailsCategory categoryElement)
        {
            var newCategoryElement = Context.InvoiceElementsDetailsCategory.SingleOrDefault(e => e.Id == categoryElement.Id);
            if (newCategoryElement != null)
            {
                //edit
                Context.Entry(newCategoryElement).CurrentValues.SetValues(categoryElement);
            }
            else
            {
                Context.InvoiceElementsDetailsCategory.Add(categoryElement);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }

        public void DeleteInvoiceElementsDetailsCategory(int id)
        {
            // find invoiceElementDetail by is
            var invoiceElementsDetailCategory = Context.InvoiceElementsDetailsCategory.FirstOrDefault(f => f.Id == id);

            var count = Context.InvoiceElementsDetails.Count(f => f.InvoiceElementsDetailsCategoryId == id);
            if (count != 0)
            {
                throw new Exception("Categoria nu poate fi stearsa, deoarece a fost folosita in modulul Elemente");
            }

            Context.InvoiceElementsDetailsCategory.Remove(invoiceElementsDetailCategory);
            Context.SaveChanges();
        }

        // Verific daca factura a fost platita
        public void CheckPayedInvoice(int invoiceId)
        {
            // verific daca a fost platit vreun OP pentru factura selectata
            var checkPaymentOrders = Context.PaymentOrderInvoice.Where(f => f.InvoiceId == invoiceId && f.State == State.Active).Count();

            if (checkPaymentOrders > 0)
            {
                throw new Exception("a fost generat un ordin de plata. Verificati modulul Economic -> Operatiuni cu numerar -> Ordine de plata.");
            }

            //verifica daca a fost platit vreo dispozitie catre casierie pentru factura seelctata
            var checkDispositions = Context.DispositionInvoice.Where(f => f.InvoiceId == invoiceId && f.State == State.Active).Count();

            if (checkDispositions > 0)
            {
                throw new Exception("a fost generata o dispozitie de plata catre casierie. Verificati modulul Economic -> Dispozitii de plata");
            }
        }

        public Invoices InvoiceForDisposition(int invoiceId)
        {
            var ret = Context.Invoices.AsNoTracking()/*.Include(f => f.PaymentOrders).Include(f => f.Dispositions)*/
                                      .Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices).Include(f => f.ThirdParty).Include(f => f.InvoiceDetails)
                                      .Include(f => f.Currency)
                             .FirstOrDefault(f => f.Id == invoiceId);
            return ret;
        }

        public Invoices InvoiceForPaymentOrder(int invoiceId)
        {
            var ret = Context.Invoices.AsNoTracking()
                                     .Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices).Include(f => f.ThirdParty).Include(f => f.InvoiceDetails)
                            .FirstOrDefault(f => f.Id == invoiceId);
            return ret;
        }

        public DocumentType GetDocumentType(string shortCode)
        {
            var ret = Context.DocumentType.Single(d => d.TypeNameShort == shortCode);
            return ret;
        }

        string GetFiledocAttributeField(int docId, string atributeName)
        { 
            try
            {
                var sql = @"SELECT         
                        [ColumnKey],[GroupName]
                        FROM .[" + fileDocDbName + @"].[dbo].[Columns] c
						join .[" + fileDocDbName + @"].[dbo].[Docs] d
						on c.GroupName = d.DocTypeID
                        where c.NameRo = '" + atributeName + 
						"' and d.DocID = " + docId;

                var conn = Context.Database.GetDbConnection();
                var cmd = conn.CreateCommand();
                cmd.Transaction = Context.Database.CurrentTransaction.GetDbTransaction();
                cmd.CommandText = sql;
                var columnName = cmd.ExecuteScalar().ToString();
                return columnName;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetAsPreluatInConta(int docId)
        {
            try
            {
                var attrColumnName = GetFiledocAttributeField(docId, "Preluata in contabilitate");
                var sql = @"update .["+ fileDocDbName + @"].[dbo].[DocsDynamicFields]
                        set  " + attrColumnName + @" = 1
                        where DocID = @docId";

                var conn = Context.Database.GetDbConnection();
                var cmd = conn.CreateCommand();
                cmd.Transaction = Context.Database.CurrentTransaction.GetDbTransaction();
                cmd.CommandText = sql;
                var param = cmd.CreateParameter();
                param.ParameterName = "docId";
                param.Value = docId;
                cmd.Parameters.Add(param);
                var affRows = cmd.ExecuteNonQuery();
                if (affRows != 1)
                {
                    throw new Exception("Eroare actualizare atribut document");
                }
            }
            catch (Exception ex)
            {
                throw;                 
            }
        }

        
        public string GetFileDocEmitentCui(int docId)
        {
            try
            {

                var sql = @"select u.TaxNumber, d.DocID, u.Name 
                from .[" + fileDocDbName + @"].dbo.[Docs] d
                join .[" + fileDocDbName + @"].[dbo].[Users] u
                on u.UserID = d.FromID
                where d.DocID = @docId";
                var conn = Context.Database.GetDbConnection();
                var cmd = conn.CreateCommand();
                cmd.Transaction = Context.Database.CurrentTransaction.GetDbTransaction();
                cmd.CommandText = sql;
                var param = cmd.CreateParameter();
                param.ParameterName = "docId";
                param.Value = docId;
                cmd.Parameters.Add(param);
                var cui = cmd.ExecuteScalar().ToString();
                return cui;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
