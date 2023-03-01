using Abp.Application.Services;
using Abp.Authorization;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Economic;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public interface IInvoiceAutoOperationAppService : IApplicationService
    {
        AutoInvForm AutoInvInit();

        AutoInvForm NotProcessedList(AutoInvForm form);

        AutoInvForm ProcessedList(AutoInvForm form);

        AutoInvForm SaveToConta(AutoInvForm form);

        AutoInvForm DeleteList(DateTime dataStart, DateTime dataEnd);

        AutoInvForm EraseOperations(AutoInvForm form);
    }

    public class InvoiceAutoOperationAppService : ErpAppServiceBase, IInvoiceAutoOperationAppService
    {
        IInvoiceRepository _invoiceRepository;
        IAutoOperationRepository _autoOperationRepository;
        IBalanceRepository _balanceRepository;
        OperationRepository _operationRepository;

        public InvoiceAutoOperationAppService(IInvoiceRepository invoiceRepository, IAutoOperationRepository autoOperationRepository, IBalanceRepository balanceRepository,
                                              OperationRepository operationRepository)
        {
            _invoiceRepository = invoiceRepository;
            _autoOperationRepository = autoOperationRepository;
            _balanceRepository = balanceRepository;
            _operationRepository = operationRepository;
        }


        // Contari automate
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm AutoInvInit()
        {
            var ret = new AutoInvForm()
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                ShowProcessedForm = true,
                ShowNotProcessedForm = false,
                ShowDeleteForm = false
            };
            return ret;
        }

        // facturi neinregistrate in Conta
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm NotProcessedList(AutoInvForm form)
        {
            var list = _invoiceRepository.GetAllIncludeElemDet().Include(f => f.ThirdParty).Include(f => f.Currency)
                                .Where(f => f.State == State.Active && f.InvoiceDate >= form.DataStart
                                       && f.InvoiceDate <= form.DataEnd
                                       && f.InvoiceDetails.Any(g => g.ContaOperationDetailId == null && g.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele)
                                       )
                                .OrderBy(f => f.InvoiceDate)
                                .ToList();
            var invoiceList = ObjectMapper.Map<List<AutoInvNInvoices>>(list);
            form.NotProcessedInvoices = invoiceList;
            form.ShowProcessedForm = false;
            form.ShowNotProcessedForm = true;
            form.ShowDeleteForm = false;

            return form;
        }

        // facturi inregistrate in Conta
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm ProcessedList(AutoInvForm form)
        {
            var list = _invoiceRepository
                .GetAllIncludeElemDet().Include(f => f.ThirdParty).Include(f => f.Currency)
                                         .Where(f => f.State == State.Active && f.InvoiceDate >= form.DataStart
                                                && f.InvoiceDate <= form.DataEnd && f.InvoiceDetails.Any(g => g.ContaOperationDetailId != null)
                                                && f.InvoiceDetails.Any(g => g.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele))
                                         .OrderBy(f => f.InvoiceDate)
                                         .ToList();
            var invoiceList = ObjectMapper.Map<List<AutoInvPInvoices>>(list);
            form.Invoices = invoiceList;
            form.NotProcessedInvoices = null;
            form.DeletedInvoices = null;
            form.ShowProcessedForm = true;
            form.ShowNotProcessedForm = false;
            form.ShowDeleteForm = false;
            return form;
        }
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm SaveToConta(AutoInvForm form)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId;
            foreach (var item in form.NotProcessedInvoices.Where(f => f.Selected))
            {
                try
                {
                    _autoOperationRepository.InvoiceToConta(item.Id, item.OperationDate, localCurrencyId.Value);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare generare note pentru factura " + item.InvoiceNumber + " / " + LazyMethods.DateToString(item.InvoiceDate) + " - " + ex.Message);
                }
            }

            form = ProcessedList(form);

            return form;
        }
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm DeleteList(DateTime dataStart, DateTime dataEnd)
        {
            var list = _invoiceRepository.GetAllIncludeElemDet().Include(f => f.ThirdParty).Include(f => f.Currency)
                                         .Where(f => f.State == State.Active && f.InvoiceDate >= dataStart
                                                && f.InvoiceDate <= dataEnd && f.InvoiceDetails.Any(g => g.ContaOperationDetailId != null)
                                                && f.InvoiceDetails.Any(g => g.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele))
                                         .OrderBy(f => f.InvoiceDate)
                                         .ToList();
            var invoiceList = ObjectMapper.Map<List<AutoInvDInvoices>>(list);

            var form = new AutoInvForm()
            {
                DataStart = dataStart,
                DataEnd = dataEnd,
                DeletedInvoices = invoiceList,
                ShowProcessedForm = false,
                ShowNotProcessedForm = false,
                ShowDeleteForm = true
            };
            return form;
        }
        //[AbpAuthorize("Admin.Conta.Facturi.Acces")]
        public AutoInvForm EraseOperations(AutoInvForm form)
        {
            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                                             .OrderByDescending(f => f.BalanceDate)
                                                             .FirstOrDefault().BalanceDate;
            foreach (var item in form.DeletedInvoices)
            {
                try
                {
                    _autoOperationRepository.InvoiceDeleteConta(item.Id, lastBalanceDate, null);

                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare stergere note pentru factura " + item.InvoiceNumber + " / " + LazyMethods.DateToString(item.InvoiceDate) + " - " + ex.Message);
                }
            }
            form = ProcessedList(form);

            return form;
        }
    }
}
