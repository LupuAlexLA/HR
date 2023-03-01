using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.PrePayments;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Repositories.Economic
{
    public interface IInvoiceRepository : IRepository<Invoices, int>
    {
        //IQueryable<Invoices> InvoicesForNIR();
        IQueryable<Invoices> InvoicesForAsset(int id);
        List<Invoices> InvoicesForAssetSale(int id, int? invoiceId);
        IQueryable<Invoices> InvoicesForPrepayments(int appClientId, PrepaymentType prepaymentType);
        IQueryable<Invoices> InvoicesForInvObjects(int appClientId);
        List<Invoices> InvoicesForInvObject(int appClientId, int? invoiceId);
        IQueryable<Invoices> GetAllIncludeElemDet();
        Invoices GetById(int id);
        void InsertOrUpdateV(Invoices invoice);
        List<InvoiceElementsDetails> GetInvoiceElementsDetails();
        InvoiceElementsDetails GetInvoiceElementsDetail(int id);
        void SaveInvoiceElementsDetail(InvoiceElementsDetails element);
        void DeleteInvoice(int invoiceId);
        void DeleteInvoiceElementsDetail(int elementId);
        void DeleteInvoiceFromDecont(int invoiceId, int decontId);
        List<InvoiceElementsDetailsCategory> GetInvoiceElementsDetailsCategories();
        InvoiceElementsDetailsCategory GetElementsDetailsCategory(int id);
        void SaveInvoiceElementsDetailCategory(InvoiceElementsDetailsCategory categoryElement);
        void DeleteInvoiceElementsDetailsCategory(int id);
        void CheckPayedInvoice(int invoiceId);
        Invoices InvoiceForDisposition(int invoiceId);
        Invoices InvoiceForPaymentOrder(int invoiceId);
        public void SetAsPreluatInConta(int docId);        
        public string GetFileDocEmitentCui(int docId);
        DocumentType GetDocumentType(string shortCode);
    }
}
