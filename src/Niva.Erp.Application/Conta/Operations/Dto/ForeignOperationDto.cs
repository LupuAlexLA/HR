using Niva.Erp.Common;
using Niva.Erp.Economic;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Operations.Dto
{
    public class ForeignOperationDto
    {
        public string QuickSearch { get; set; }
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public int? BankAccountId { get; set; }

        public bool ContaOperation { get; set; }

        public int TenantId { get; set; }

        public int ThirdPartyId { get; set; }

        public bool ShowList { get; set; }

        public bool ShowUploadForm { get; set; }

        public bool ShowDeleteForm { get; set; }

        public bool OkGenerate { get; set; }
        public bool IncludeGenerate { get; set; }   

        public List<ForeignOperationList> OperList { get; set; }

        public ForeignOperationUpload UploadFile { get; set; }

        public FOInitDeleteDto DeleteForm { get; set; }

        public bool HideModifBtn
        {
            get
            {
                if (OperList != null)
                {
                    return (OperList.Count(f => !f.NCChildGenerated) == 0);
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public class ForeignOperationList
    {
        public int Id { get; set; }

        public int ForeignOperId { get; set; }

        public int CurrencyId { get; set; }

        public int BankAccountId { get; set; }

        public string BankAccount { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentTypeStr { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime OperationDate { get; set; }

        public decimal Value { get; set; }

        public decimal ValueCurr { get; set; }

        public string OriginalDetails { get; set; }

        public int? SelectedPaymentOrderId { get; set; }
        public string SelectedPaymentOrderDetails { get; set; } 
        public bool SelectedOP { get; set; }

        public int? PaymentOrderId { get; set; }

        public string PaymentOrder { get; set; }

        public List<ForeignOperationAccountingList> AccountingList { get; set; }
        public List<PaymentOrderForForeignOperationDto> PaymentOrdersList { get; set; }

        public bool NCChildGenerated
        {
            get
            {
                return (AccountingList.Count(f => !f.NcGenerated) == 0);
            }
        }
    }

    public class ForeignOperationAccountingList
    {
        public int Id { get; set; }

        public int DetailOperId { get; set; }

        public decimal Value { get; set; }

        public decimal ValueCurr { get; set; }

        public string Details { get; set; }

        public int? DebitAccountId { get; set; }

        public string DebitAccount { get; set; }

        public int? CreditAccountId { get; set; }

        public string CreditAccount { get; set; }

        public int? OperationsDetailId { get; set; }

        public bool NcGenerated { get { return (OperationsDetailId != null); } }
    }

    public class ForeignOperationUpload
    {
        public int? BankAccountId { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime OperationDate { get; set; }

        public int? DocumentTypeId { get; set; }

        public FileUploadDto FileUpld { get; set; }
    }

    public class FOInitDeleteDto
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public int? BankAccountId { get; set; }

        public List<FODeleteList> List { get; set; }
    }

    public class FODeleteList
    {
        public int ForeignOperId { get; set; }

        public int BankAccountId { get; set; }

        public string BankAccount { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime OperationDate { get; set; }

        public bool NcGenerated { get; set; }
    }

    public class FOPrepareAccounting
    {
        public int Id { get; set; }

        public int DebitAccountId { get; set; }

        public int CreditAccountId { get; set; }

        public decimal Value { get; set; }

        public decimal ValueCurr { get; set; }

        public string Details { get; set; }
    }

    public class FODictionaryFormDto
    {
        public string  SearchAccount { get; set; }
        public List<FODictionaryListDto> FOdictionaryList { get; set; }

    }

    public class FODictionaryListDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string Expression { get; set; }
        public DateTime OperationDate { get; set; }
        public string DictionaryType { get; set; }
    }

    public class FODictionaryEditDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public DateTime OperationDate { get; set; }
        public string Expression { get; set; }
        public string Symbol { get; set; }
        public int AccountId { get; set; }
        public FODictionaryType  FODictionaryType { get; set; }
        public string AccountName { get; set; }
    }
}
