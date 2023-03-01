using Abp.Domain.Repositories;
using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.InvObjects
{
    public interface IInvOperationRepository: IRepository<InvObjectOper, int>
    {
        int NextDocumentNumber(int documentTypeId);
        DateTime UnprocessedDate();
        void GestComputing(DateTime operationDate); // calculez gestiunea pentru obiectele de inventar
        void GestInvObjectsComputing(DateTime operationDate); // calculez gestiunea pentru toate obiectele de inventar
        void GestInvObjectDelComputing(DateTime deleteDate); // sterg gestiunea
        void GestDelComputingForInvObject(DateTime operationDate, int invObjectId);
        void GestComputingForInvObject(int invObjectId, DateTime operationDate); // calculez gestiunea pentru obiecte de invetar fara factura
        DateTime LastProcessedDateForInvObject(int invObjectId); // ultima data procesata in gestiune pentru obiectul de inventar
        DateTime LastProcessedDate();
        void ExistingInvObjectFromInvoice(int invoiceId);
        void UpdateInvObjectOperation(InvObjectOper oper);
        InvObjectStock GetGestDetailForInvObject(int invObjectId, DateTime operationDate);
        List<InvObjectOperDetail> GetInvObjectOperDetails(int invObjectId, int appClientId);
    }
}
