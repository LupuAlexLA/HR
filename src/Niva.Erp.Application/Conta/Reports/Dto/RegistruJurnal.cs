using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class RegistruJurnalBase
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? DocumentTypeId { get; set; }

        public int? CurrencyId { get; set; }

        public string Currency { get; set; }

        public string Parameters { get; set; }
    }

    public class RegistruJurnal : RegistruJurnalBase
    {
        public IList<RegistruJurnalDetails> Details { get; set; }
        public IList<RegistruJurnalExtinsDetails> ExtinsDetails { get; set; }   
    }

    public class RegistruJurnalDetailBase
    {
        public int ContaOperationId { get; set; }

        public DateTime OperationDate { get; set; }
        public DateTime CreationDate { get; set; }  

        public string DocumentTypeShortName { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DebitAccount { get; set; }

        public string CreditAccount { get; set; }

        public decimal Value { get; set; }

        public string CurrencyCode { get; set; }

        public string OperationDetailsObservations { get; set; }
    }

    public class RegistruJurnalDetails : RegistruJurnalDetailBase
    {

    }

    public class RegistruJurnalExtinsDetails: RegistruJurnalDetailBase
    {
        public decimal SumaLeiDataTranzactie { get; set; }
        public decimal SumaLeiDataReferinta  { get; set; }
        public string TipTranzactie { get; set; }
        public string UserName { get; set; }
    }
}
