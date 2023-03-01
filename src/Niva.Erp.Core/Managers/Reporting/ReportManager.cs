using Abp.Domain.Repositories;
using Abp.Domain.Services;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Managers.Reporting
{
    public class ReportManager : DomainService
    {
     

        public ReportManager()
        {
            
        }

     
        
    }

    public class SitFinanRaport
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public string ReportName { get; set; }

        public string ColumnName { get; set; }

        public string DateRange { get; set; }

        public string BalanceType { get; set; }

        public string NotaBefore { get; set; }

        public string NotaAfter { get; set; }

        public List<SitFinanReportDetailList> Details { get; set; }
    }

    public class SitFinanReportDetailList
    {
        public int CalcRowId { get; set; }

        public int RowId { get; set; }

        public string RowName { get; set; }

        public string RowNr { get; set; }
        public string RowNota { get; set; }

        public int OrderView { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }

        public decimal? Val { get; set; }

        public decimal? Val6 { get; set; }
        public DateTime? BalanceDate { get; set; }
    }

    public class SitFinanReportModel
    {
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }

        public DateTime BalanceDate { get; set; }

        public string ReportName { get; set; }

        public int NrCol { get; set; }

        public string NotaBefore { get; set; }

        public string NotaAfter { get; set; }

        public SitFinanReportColList ColumnReport { get; set; }

        public IList<SitFinanReportDetList> Details { get; set; }
    }

    public class SitFinanReportDetList
    {
        public int CalcRowId { get; set; }

        public int RowId { get; set; }

        public string RowName { get; set; }

        public string RowNr { get; set; }
        public string RowNota { get; set; }

        public int OrderView { get; set; }

        public bool TotalRow { get; set; }

        public bool Bold { get; set; }

        public bool NegativeValue { get; set; }

        public int DecimalNr { get; set; }

        public decimal? Val1 { get; set; }

        public decimal? Val2 { get; set; }

        public decimal? Val3 { get; set; }

        public decimal? Val4 { get; set; }

        public decimal? Val5 { get; set; }

        public decimal? Val6 { get; set; }
        public DateTime? BalanceDate { get; set; }
    }

    public class SitFinanReportColList
    {
        public string Col1 { get; set; }

        public string Col2 { get; set; }

        public string Col3 { get; set; }

        public string Col4 { get; set; }

        public string Col5 { get; set; }

        public string Col6 { get; set; }
    }
}
