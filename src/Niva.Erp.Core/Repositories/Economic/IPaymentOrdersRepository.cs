using Abp.Domain.Repositories;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Niva.Erp.Models.Economic;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Economic
{
    public interface IPaymentOrdersRepository : IRepository<PaymentOrders, int>
    {
    }


    public class BTExport
    {
        public int OrderNumber { get; set; }
        public string SourceAccountNumber { get; set; }
        public string TargetAccountNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryBankBIC { get; set; }
        public string BeneficiaryFiscalCode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentRef1 { get; set; }
        public string PaymentRef2 { get; set; }
        [Format("dd/MM/yyyy")]
        public DateTime ValueDate { get; set; }
        public string Urgent { get; set; }
    }

    public class BTExportMap : ClassMap<BTExport>
    {
        public BTExportMap()
        {
            Map(m => m.OrderNumber).Index(0);
            Map(m => m.SourceAccountNumber).Index(1);
            Map(m => m.TargetAccountNumber).Index(2);
            Map(m => m.BeneficiaryName).Index(3);
            Map(m => m.BeneficiaryBankBIC).Index(4);
            Map(m => m.BeneficiaryFiscalCode).Index(5);
            Map(m => m.Amount).Index(6);
            Map(m => m.PaymentRef1).Index(7);
            Map(m => m.PaymentRef2).Index(8);
            Map(m => m.ValueDate).Index(9).TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.Urgent).Index(10);
        }

    }
}
