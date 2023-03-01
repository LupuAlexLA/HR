using Abp;
using DevExpress.DataAccess.ObjectBinding;
using Niva.Erp.Conta.Reports;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.Web.Host.Reports.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Niva.Erp.Web.Host.Reports.Conta
{
    [DisplayName("Employees")]
    [HighlightedClass]
    public class TestReportDataSource
    {
        private IScopedTestReportDataSourceProvider<ReportsAppService> reportManagerProvider { get; set; }

        public TestReportDataSource()
        {
            throw new NotSupportedException();
        }

        public TestReportDataSource(IScopedTestReportDataSourceProvider<ReportsAppService> reportManagerProvider)
        {
            this.reportManagerProvider = reportManagerProvider ?? throw new ArgumentNullException(nameof(reportManagerProvider));

            // NOTE: the repository ctor is invoked in the context of http request. At this point of execution we have access to context-dependent data, like currentUserId.
            // The repository MUST read and store all the required context-dependent values for later use. E.g. notice that we do not store the IUserService (which is context/scope dependent), but read the value of current user and store it.
            //studentId = userService.GetCurrentUserId();
        }

     

        public List<NameValue<int>> GetParamerData()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("Unu", 1),
                new NameValue<int>("Doi", 2),
                new NameValue<int>("Trei", 3),
                new NameValue<int>("Patru", 4),
                new NameValue<int>("Cinci", 5)};
        }

        public List<NameValue<int>> GetBalanceTypeRON()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("RON si Echivalent RON", 0),
                new NameValue<int>("RON", 1)
            };
        }

        public List<NameValue<int>> GetBalanceTypeValuta()
        {
            return new List<NameValue<int>>()
            {
                new NameValue<int>("Valuta echivalent RON", 0),
                new NameValue<int>("Valuta", 2)
            };
        }

    
        }


        
      
    

    public class DataItem
    {
        //public DataItem(int floor, int office, string personName, string titleOfCourtesy, string title)
        //{
        //    Floor = floor;
        //    Office = office;
        //    PersonName = personName;
        //    TitleOfCourtesy = titleOfCourtesy;
        //    Title = title;
        //}
        public int Floor { get; set; }

        public int Office { get; set; }
        public string PersonName { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string Title { get; set; }
    }
}