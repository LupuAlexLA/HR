using DevExpress.XtraReports.UI;
using Niva.Erp.Web.Host.Reports.Conta;

using System;
using System.Collections.Generic;

namespace Niva.Erp.Web.Host.Reports.Common
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
           

        };
    }
}