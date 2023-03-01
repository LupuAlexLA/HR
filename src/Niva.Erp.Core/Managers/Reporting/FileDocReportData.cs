using Abp.Dependency;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Managers.Reporting
{
    public class FileDocReportData : ITransientDependency
    {
        Dictionary<string, string> parameters;
        public string TipDocument { get; set; }
        public string Subiect { get; set; }
        public string Emitent { get; set; }
        public string Destinatar { get; set; }
        public string DataReferinta { get; set; }

        public IAbpSession Session { get; set; }
        public FileDocReportData(IAbpSession Session)
        {
            this.Session = Session;
        }
        public FileDocReportData Calc(string reportQuery)
        {
            string reportName = reportQuery.Substring(0, reportQuery.IndexOf("?"));  // preiau numele raportului din url

            var queryParams = reportQuery.Split('?').Last(); // elimin numele raportului din string 
            parameters = new Dictionary<string, string>();
            var urlSplit = queryParams.Split(new[] { '&' }).Select(s => s.Split(new[] { '=' }));
            foreach (var item in urlSplit)
            {
                parameters.Add(item[0], item[1]);
            }

            switch (reportName)
            {
                case "BalantaRon5Egalitati":
                    {
                        Subiect = "Balanta de verificare";
                        DataReferinta = parameters["balanceDate"];
                        TipDocument = @"Balanta de verificare";
                        break;
                    }
                default:
                    break;
            }
            return this;
        }

    }
}
