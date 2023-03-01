using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.Conta.ConfigurareRapoarte
{
    public interface IReportConfigAppService : IApplicationService
    {
       
    }

    public class ReportConfigAppService : ErpAppServiceBase, IReportConfigAppService
    {
        

        public ReportConfigAppService()
        {
            
        }
      
       

    }
}