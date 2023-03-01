using Abp.Domain.Repositories;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Repositories.Conta.AutoOperation
{
    public interface IAutoOperationRepository : IRepository<AutoOperationOper, int>
    {

        #region ImoAsset
        IQueryable<AutoOperationCompute> ImoAssetPrepareAdd(DateTime startDate, DateTime endDate, int localCurrencyId);
        void ImoAssetOperationAdd(List<AutoOperationCompute> list, DateTime lastBalanceDate, AutoOperationType autoOperationType, int localCurrencyId, int? operGenId);
        void DeleteUncheckedAssetOperation(DateTime operationDate, int? assetId); // stergere note contabile pentru MF cu assetId-ul trimis ca parametru
        void DeleteAssetOperations(DateTime operationDate); // sterg note contabile 
        void DeleteUncheckedAssetDetailOperation(DateTime operationDate, int? assetId);

        //Generare automata a notelor contabile din mijloace fixe
        void AutoImoAssetOperationAdd(DateTime dataEnd, int appClientId, int localCurrencyId, ImoAssetType imoAssetType, DateTime lastBalanceDate, int? operGenId);
        IQueryable<AutoOperationCompute> AutoImoAssetPrepareAdd(DateTime endDate, int localCurrencyId, int appClientId);

        #endregion

        #region Prepayments
        List<AutoOperationCompute> PrepaymentsPrepareAdd(DateTime startDate, DateTime endDate, int localCurrencyId, PrepaymentType prepaymentType);

        void PrepaymentsOperationAdd(List<AutoOperationCompute> list, DateTime lastBalanceDate, AutoOperationType autoOperationType, int localCurrencyId, int? operGenId);

        void AutoPrepaymentsOperationAdd(DateTime dataEnd, int localCurrencyId, PrepaymentType prepaymentType, DateTime lastBalanceDate, int? operGenId);

        void DeleteUncheckedAutoOperationForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId); // sterg nota contabila pentru cheltuiala selectata

        void DeleteUncheckedAutoOperation(DateTime operationDate, PrepaymentType prepaymentType);

        #endregion
        
        OperationDetails GenerateNededOperDetail(ref OperationDetails operDetail, DateTime date, int accountId, out bool okAdd);
        // Obiecte de inventar
        #region InvObject

        int GetInvObjectAccountId(string account, InvObjectItem invObjectItem, DateTime operationDate, int localCurrencyId);

        // Generare automata a notelor contabile pentru obiectele de inventar
        void AutoInvObjectOperationAdd(DateTime dataEnd, int appClientId, int localCurrencyId, DateTime lastBalanceDate, int? operGenId);
        void DeleteInvObjectOperations(DateTime gestDate); // sterg notele contabile
        void DeleteInvObjectDetailOperation(DateTime operationDate, int? invObjectId);

        #endregion

        #region Invoices
        void InvoiceToConta(int invoiceId, DateTime operationDate, int localCurrencyId);
        void SaveDirectToConta(int invoiceId, DateTime operationDate, int localCurrencyId);

        void InvoiceDeleteConta(int invoiceId, DateTime lastBalanceDate, int? invoiceElementsType);
        int CheckExistingConta(int invoiceId, DateTime operationDate, int currencyId);
        void DispositionToConta(int dispositionId, DateTime dispositionDate, int localCurrencyId);

        #endregion

        #region Decont
        void SaveDecontDirectToConta(int invoiceId, DateTime operationDate, int localCurrencyId, string decontType, int thirdPartyId, string documentTypeNameShort);

        int GetAutoAccountForDecont(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId, DecontType decontType, int? imoAssetStorage);

        void DeleteInvoicesFromDecont(int decontId, int tenantId);

        void RemoveContaInvoicesFromDecont(int decontId, int tenantId);
        #endregion

        void DeleteAutoOperation(int operationId, DateTime lastBalanceDate);

        //void TaxProfitOperationAdd(int compId, int documentTypeId, string documentNumber, DateTime documentDate);

        int GetAutoAccount(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId, int? imoAssetStorage);

        int GetAutoAccountActivityType(string symbol, int? thirdPartyId, int? activityTypeId, DateTime operationDate, int currencyId, int? imoAssetStorage);

        void RepartizareCheltuieli(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId);

        void InchidereVenituriCheltuieli(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId);
        int GetAccount(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId);

        int GetMainActivityType();


        void ReevaluarePozitieValutara(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId);

        #region Exchange
        void SaveExchangeToConta(int exchangeId, DateTime operationDate, int localCurrencyId);
        void DiminuareRambursareToConta(int RataId, int localCurrencyId);
        void DeleteContaForExchange(int exchangeId, int appclinetId);
        #endregion

        string GetDocumentNextNumber(DocumentType docType, DateTime operationDate);

        void ImprumutToConta(int imprumutId, int localCurrencyId);

        void PlataToConta(int DobandaId, int localCurrencyId, int operGenId);
        void ComisionToConta(int ComisionId, int localCurrencyId);
        void DobandaToConta(int id, int localCurrencyId, int operGenerateId);
        int GetNextNumberForOperContab(DateTime operDate, int documentTypeId);
        void OperatieGarantieToConta(int OperatieGarantieId, int localCurrencyId);
    }
}
