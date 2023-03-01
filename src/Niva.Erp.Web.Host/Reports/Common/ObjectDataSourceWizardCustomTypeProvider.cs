using DevExpress.DataAccess.Web;

using Niva.Erp.Web.Host.Reports.Conta;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Web.Host.Reports.Common
{
    public class ObjectDataSourceWizardCustomTypeProvider : IObjectDataSourceWizardTypeProvider {
        public IEnumerable<Type> GetAvailableTypes(string context) {
             return new[] { typeof(TestReportDataSource) };
           
         }
    }
}