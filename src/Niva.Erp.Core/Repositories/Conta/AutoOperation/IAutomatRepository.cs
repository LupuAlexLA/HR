using Abp.Domain.Repositories;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Setup;
using Niva.Erp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Conta.AutoOperation
{
    public interface IAutomatRepository : IRepository<AutoOperationOper, int>
    {
        void getCredentials(Tokens token, out Tenant tenant, out User user);

        string AddAccountAutomat(Tokens token, string symbol, string synthetic, string accountName, string externalCode, string thirdPartyCif, string currency, int accountFuncType);

        string AddOperation(Tokens token, DateTime operationDate, string documentNr, DateTime documentDate, bool closingMonth, int operationStatus, string currency, string documentType, out int idOperation);

        string AddOperationDetails(Tokens token, int operationId, string debitSymbol, string creditSymbol, decimal value, decimal valueCurrency, string details);

        string DeleteOperation(Tokens token, int idOperation);

        string ExchangeRatesAddModify(string currencyCode, DateTime exchangeDate, decimal value);

        void UpdateNaturalPerson(NaturalPerson person);

        void UpdateLegalPerson(LegalPerson person);

        string AddModifyBankAccount(string tokenId, string thirdParty, string bank, string currency, string iban, int appClientId);

        string ContaOperationSave(DateTime operationDate, string documentNr, DateTime documentDate, string currency, string activityType, int operType, int operationType,  out int idOperation);

        string ContaOperationDelete(int idOperation);

        string ContaOperationDetailSave(int idOperation, string activityType, int operType, int operationType, int valueType, decimal value, string bank, string explicatii, bool storno);

        string ContaOperationEndSave(int idOperation);

        string ContaOperationSaveDirect(DateTime operationDate, string documentNr, DateTime documentDate, string currency, string documentType, out int idOperation);

        string ContaOperationDetailSaveDirect(int idOperation, string debit, string credit, decimal value, string explicatii);
    }
}
