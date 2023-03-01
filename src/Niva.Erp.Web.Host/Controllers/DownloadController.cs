using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Niva.Erp.Controllers;
using Niva.Erp.Economic;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Niva.Erp.Web.Host.Controllers
{
    public class DownloadController : ErpControllerBase
    {
        public IPaymentOrdersAppService paymentOrdersAppService;
        public DownloadController(IPaymentOrdersAppService paymentOrdersAppService)
        {
            this.paymentOrdersAppService = paymentOrdersAppService;
        }

        [HttpGet]
        [HttpPost]
        public FileContentResult ExportOpToCsv(List<int> selectedOpIds)
        {
            var exportList = paymentOrdersAppService.ExportOPToCSV(selectedOpIds);
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Configuration.Delimiter = ",";
                csvWriter.Configuration.RegisterClassMap<BTExportMap>();

                csvWriter.WriteHeader<BTExport>();

                csvWriter.NextRecord();
                csvWriter.WriteRecords(exportList);

                csvWriter.Flush();
                return new FileContentResult(memoryStream.ToArray(), "text/csv");
            }
        }
    }
}
