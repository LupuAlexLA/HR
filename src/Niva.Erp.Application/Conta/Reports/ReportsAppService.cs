using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;

using Niva.Erp.Authorization.Users;

using Niva.Erp.EntityFrameworkCore.Methods;

using Niva.Erp.Managers.Reporting;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.Conta.Reports
{
    public interface IReportsAppService : IApplicationService
    {
       
    }

    public class ReportsAppService : ErpAppServiceBase, IReportsAppService
    {
       


        public ReportsAppService()
        {
           
        }

       
       


    }
}