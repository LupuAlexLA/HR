using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Conta.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Economic;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.Prepayments;
using Niva.Erp.Repositories.Doconturi;
using Niva.Erp.Repositories.Economic;
using Niva.Erp.Repositories.ImoAsset;
using Niva.Erp.Repositories.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Niva.Erp.Economic
{
    public interface IInvoiceAppService : IApplicationService
    {
        GetAssetOutput InvoicesForAsset();
        GetAssetOutput InvoicesForPrepayments(int prepaymentType);
        GetAssetOutput InvoicesForAssetSale(int? invoiceId);
        GetAssetOutput InvoicesForInvObjects();
        GetAssetOutput InvoicesForInvObject(int? invoiceId);
        List<ThirdPartyListDDDto> GetThirdParty(string search);
        List<InvoiceDTO> GetInvoicesList();
        List<InvoiceDTO> GetInvoices( DateTime dataStart, DateTime dataEnd, int? thridPartyId, string documentType, bool facturiIncomplete, decimal? suma, string dataType);
        List<InvoiceListDto> GetInvoicesByThirdPartyId(int thirdPartyId, int dispositionId);
        InvoiceDTO GetInvoice(int id);
        List<InvoiceDTO> GetInvoicesByDecontId(int? decontId);
        List<InvoiceListDto> GetInvoiceDetails(int id);
        List<InvoiceListForImoAssetDto> GetInvoiceDetailsForImoAsset(int imoAssetItemId, int operationType, int? operationId, int? invoiceId);
        List<InvoiceListForInvObjectDto> GetInvoiceDetailsForInvObject(int invObjectItemId, int? operationId);
        void SaveInvoice(InvoiceDTO invoice);
        List<ThirdPartyDTO> GetThridPathy(string search);
        List<InvoiceElementsDetailsDTO> GetInvoiceElementsDetails();
        List<InvoiceElementsDetailsDTO> GetInvoiceElementsDetailsByCategoryId(int? categoryId);
        List<InvoiceElementsDetailsDTO> SearchInvoiceElementsDetails(string element, string account, string InvoiceElementsDetailsCategory);
        InvoiceElementsDetailsDTO GetInvoiceElementsDetail(int id);
        void SaveInvoiceElementDetail(InvoiceElementsDetailsDTO element);
        List<InvoiceElementAccountsDTO> GetElementAccounts();
        InvoiceElementAccountsDTO GetElementAccount(int id);
        void SaveElementAccount(InvoiceElementAccountsDTO elementAccount);
        List<InvoiceListDto> UnpayedInvoicesForThirdParty(int thirdPartyId, int paymentOrderId);
        void DeleteInvoice(int invoiceId);
        void DeleteInvoiceElementsDetail(int id);
        List<InvoiceElementsDetailsCategoryListDTO> GetInvoiceElementsDetailsCategories();
        List<InvoiceElementsDetailsCategoryListDTO> GetInvoiceElementsDetailsCategoriesforThirdPartyQuality(int thirdPartyQuality);
        InvoiceElementsDetailsCategoryEditDTO GetInvoiceElementsDetailsCategory(int id);
        void SaveInvoiceCategoryElement(InvoiceElementsDetailsCategoryEditDTO categoryElement);
        void DeleteInvoiceElementsDetailsCategory(int id);
        void DeleteInvoiceFromDecont(int invoiceId, int decontId);
        List<InvoiceListSelectableDto> GetInvoicesForDispositionByThirdPartyId(int thirdPartyId, int dispositionId);
        List<InvoiceListSelectableDto> GetInvoicesForPaymentOrderByThirdPartyId(int thirdPartyId, int paymentOrderId);
        InvoiceListSelectableDto CalculatePayedInvoice(InvoiceListSelectableDto invoice, decimal dispositionSum);
        int getCategoryIdByInvoiceElementDetailId(int id);
        InvoiceDTO GetInvoiceSeriesAndNumber(int thirdPartyQuality); //returnez numarul si seria facturii daca este selectat Client ca fiind Calitatea tertului
        List<InvoiceForDecontDTO> GetInvoicesForDecont(int? decontId, int currencyId);
        void SaveInvoicesFromDecont(List<int> invoiceIds, int decontId);
        int getFacturaNumberIncremented();
    }

    public class GetAssetOutput
    {
        public List<NirDetailDTO> GetInvoices { get; set; }
    }

    public class InvoiceAppService : ErpAppServiceBase, IInvoiceAppService
    {
        IPersonRepository _personRepository;
        IInvoiceRepository _invoiceRepository;
        IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IRepository<InvoiceElementsDetails> _invoiceElementsDetailsRepository;
        IPaymentOrdersRepository _paymentOrdersRepository;
        IBalanceRepository _balanceRepository;
        IPrepaymentRepository _prepaymentRepository;
        IAutoOperationRepository _autoOperationRepository;
        IImoAssetRepository _imoAssetRepository;
        IInvOperationRepository _invOperationRepository;
        IRepository<InvoiceElementAccounts, int> _invoiceElementAccountsRepository;
        IRepository<DocumentType> _documentTypeRepository;
        IRepository<PrepaymentDocType> _prepaymentOperDocTypeRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        IRepository<ImoAssetOperDetail> _imoAssetOperDetailRepository;
        IInvObjectRepository _invObjectRepository;
        IDecontRepository _decontRepository;
        IDispositionRepository _dispositionRepository;
        IRepository<DispositionInvoice> _dispositionInvoiceRepository;
        IRepository<PaymentOrderInvoice> _paymentOrderInvoiceRepository;
        IRepository<InvObjectOperDetail> _invOperationDetailRepository;
        IOperationRepository _operationRepository;

        public InvoiceAppService(IPersonRepository personRepository, IInvoiceRepository invoiceRepository, IRepository<InvoiceDetails> invoiceDetailsRepository,
                                IRepository<InvoiceElementAccounts, int> invoiceElementAccountsRepository, IPaymentOrdersRepository paymentOrdersRepository, IBalanceRepository balanceRepository,
                                IPrepaymentRepository prepaymentRepository, IRepository<PrepaymentDocType> prepaymentOperDocTypeRepository, IRepository<DocumentType> documentTypeRepository,
                                IAutoOperationRepository autoOperationRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository, IImoAssetRepository imoAssetRepository,
                                IInvOperationRepository invOperationRepository, IExchangeRatesRepository exchangeRatesRepository, IRepository<ImoAssetOperDetail> imoAssetOperDetailRepository,
                                IInvObjectRepository invObjectRepository, IDecontRepository decontRepository, IDispositionRepository dispositionRepository,
                                IRepository<DispositionInvoice> dispositionInvoiceRepository, IRepository<PaymentOrderInvoice> paymentOrderInvoiceRepository,
                                IRepository<InvoiceElementsDetails> InvoiceElementsDetailsRepository, IRepository<InvObjectOperDetail> invOperationDetailRepository,
                                IOperationRepository operationRepository)
        {
            _personRepository = personRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _invoiceElementAccountsRepository = invoiceElementAccountsRepository;
            _paymentOrdersRepository = paymentOrdersRepository;
            _balanceRepository = balanceRepository;
            _prepaymentRepository = prepaymentRepository;
            _prepaymentOperDocTypeRepository = prepaymentOperDocTypeRepository;
            _documentTypeRepository = documentTypeRepository;
            _autoOperationRepository = autoOperationRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _imoAssetRepository = imoAssetRepository;
            _invOperationRepository = invOperationRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _imoAssetOperDetailRepository = imoAssetOperDetailRepository;
            _invObjectRepository = invObjectRepository;
            _decontRepository = decontRepository;
            _dispositionRepository = dispositionRepository;
            _dispositionInvoiceRepository = dispositionInvoiceRepository;
            _paymentOrderInvoiceRepository = paymentOrderInvoiceRepository;
            _invoiceElementsDetailsRepository = InvoiceElementsDetailsRepository;
            _invOperationDetailRepository = invOperationDetailRepository;
            _operationRepository = operationRepository;
        }

        public List<ThirdPartyListDDDto> GetThirdParty(string search)
        {
            string _search = search ?? "";
            var appClient = GetCurrentTenant();
            var _thirdParty = _personRepository.ThirdPartyList().ToList().Where(s => s.FullName.ToUpper().StartsWith(_search.ToUpper())).ToList();
            var ret = ObjectMapper.Map<List<ThirdPartyListDDDto>>(_thirdParty);
            return ret;
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public List<InvoiceDTO> GetInvoicesList()
        {
            var invoices = _invoiceRepository.GetAll()/*.Include(f => f.Dispositions).Include(f => f.PaymentOrders)*/
                                             .Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices).Where(i => i.State == State.Active).OrderByDescending(f => f.InvoiceDate);
            var ret = ObjectMapper.Map<List<InvoiceDTO>>(invoices);
            return ret;
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public List<InvoiceDTO> GetInvoices(DateTime dataStart, DateTime dataEnd, int? thridPartyId, string documentType, bool facturiIncomplete , decimal? suma , string dataType)
        {
            try
            {
                DateTime _dataStart = new DateTime(dataStart.Year, dataStart.Month, dataStart.Day);
                DateTime _dataEnd = new DateTime(dataEnd.Year, dataEnd.Month, dataEnd.Day);
                var invoices = _invoiceRepository.GetAllIncluding(f => f.Currency, f => f.ThirdParty, f => f.DocumentType, f => f.ContaOperation, f => f.Contracts, f => f.Decont,
                                                                       f => f.DispositionInvoices, f => f.PaymentOrderInvoices)
                                                 // .Include(f => f.Currency).Include(f => f.ThirdParty).Include(f => f.DocumentType).Include(f => f.ContaOperation)    
                                                 //.Include(f => f.Dispositions).Include(f => f.PaymentOrders)
                                                 //.Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices)
                                                 .Where(i => i.State == State.Active && (dataType == "ID" ? i.InvoiceDate : i.OperationDate) <= _dataEnd && (dataType == "ID" ? i.InvoiceDate : i.OperationDate) >= _dataStart);
                
                if(suma != null) {
                    invoices = invoices.Where(f => (f.FileDocValue == 0 ? f.Value : f.FileDocValue) == suma);
                }


                if (thridPartyId != null)
                {
                    invoices = invoices.Where(f => f.ThirdPartyId == (thridPartyId ?? 0));
                }

                if (documentType != null)
                {
                    invoices = invoices.Where(f => f.DocumentType.TypeNameShort == documentType);
                }

                if (facturiIncomplete == true)
                {
                    invoices = invoices.Include(f => f.InvoiceDetails).Where(f => f.InvoiceDetails.Count == 0);
                }

                invoices = invoices.OrderByDescending(f => f.InvoiceDate);

                var ret = ObjectMapper.Map<List<InvoiceDTO>>(invoices);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare", ex);
            }
        }

        //[AbpAuthorize("Admin.Conta.ElemFactura.Acces")]
        public List<InvoiceElementsDetailsDTO> GetInvoiceElementsDetails()
        {
            var res = _invoiceRepository.GetInvoiceElementsDetails();
            var ret = ObjectMapper.Map<List<InvoiceElementsDetailsDTO>>(res);
            return ret;
        }

        //  [AbpAuthorize("Admin.Conta.ElemFactura.Acces")]
        public List<InvoiceElementsDetailsDTO> SearchInvoiceElementsDetails(string element, string account, string InvoiceElementsDetailsCategory)
        {
            try
            {

                var invoiceElemDetailsList = GetInvoiceElementsDetails();

                if (element != null && element != "")
                {
                    invoiceElemDetailsList = invoiceElemDetailsList.Where(f => f.Description.IndexOf(element.ToLower()) >= 0).ToList();
                }

                if (account != null && account != "")
                {
                    invoiceElemDetailsList = invoiceElemDetailsList.Where(f => f.ThirdPartyAccount?.IndexOf(account.ToLower()) >= 0 ||
                                                                               f.CorrespondentAccount?.IndexOf(account.ToLower()) >= 0 ||
                                                                               f.AmortizationAccount?.IndexOf(account.ToLower()) >= 0 ||
                                                                               f.ExpenseAmortizAccount?.IndexOf(account.ToLower()) >= 0)
                        .ToList();
                }

                if (InvoiceElementsDetailsCategory != null && InvoiceElementsDetailsCategory != "")
                {
                    invoiceElemDetailsList = invoiceElemDetailsList.Where(f => f.InvoiceElementsDetailsCategory == InvoiceElementsDetailsCategory)
                        .ToList();
                }

                return invoiceElemDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[AbpAuthorize("Admin.Conta.ElemFactura.Acces")]
        public InvoiceElementsDetailsDTO GetInvoiceElementsDetail(int id)
        {
            var res = _invoiceRepository.GetInvoiceElementsDetail(id);
            var ret = ObjectMapper.Map<InvoiceElementsDetailsDTO>(res);
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.ElemFactura.Acces")]
        public void SaveInvoiceElementDetail(InvoiceElementsDetailsDTO element)
        {
            var newElement = ObjectMapper.Map<InvoiceElementsDetails>(element);
            var appClient = GetCurrentTenant();
            newElement.TenantId = appClient.Id;
            _invoiceRepository.SaveInvoiceElementsDetail(newElement);
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public List<InvoiceElementAccountsDTO> GetElementAccounts()
        {
            try
            {
                var res = _invoiceElementAccountsRepository.GetAllIncluding(f => f.Account)
                                                           .Where(i => i.State == State.Active)
                                                           .ToList()
                                                           .OrderBy(f => f.InvoiceElementAccountType).ThenBy(f => f.Account.Symbol);
                var ret = ObjectMapper.Map<List<InvoiceElementAccountsDTO>>(res);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public InvoiceElementAccountsDTO GetElementAccount(int id)
        {
            var res = _invoiceElementAccountsRepository.Get(id);
            var ret = ObjectMapper.Map<InvoiceElementAccountsDTO>(res);
            return ret;
        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        public void SaveElementAccount(InvoiceElementAccountsDTO elementAccount)
        {
            var newElementAccount = ObjectMapper.Map<InvoiceElementAccounts>(elementAccount);
            _invoiceElementAccountsRepository.InsertOrUpdate(newElementAccount);
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public InvoiceDTO GetInvoice(int id)
        {
            InvoiceDTO ret;
            if (id == 0)
            {
                ret = new InvoiceDTO
                {
                    OperationDate = DateTime.Now,
                    ActivityTypeId = null
                };
            }
            else
            {
                var invoice = _invoiceRepository.GetById(id);

                var documentType = _documentTypeRepository.GetAll().FirstOrDefault(f => f.Id == invoice.DocumentTypeId);

                ret = ObjectMapper.Map<InvoiceDTO>(invoice);
                ret.DocumentTypeShortName = documentType.TypeNameShort;

                ret.EnableEdit = true;
                foreach (var detail in ret.InvoiceDetails)
                {
                    if (detail.ContaOperationDetailId != null)
                    {
                        ret.EnableEdit = false;
                    }
                }
            }

            return ret;
        }

        //[AbpAuthorize("Conta.Documente.Acces")]
        public List<InvoiceDTO> GetInvoicesByDecontId(int? decontId)
        {
            if (decontId == null)
            {
                return new List<InvoiceDTO>();
            }
            var invoices = _invoiceRepository.GetAllIncluding(f => f.DocumentType, f => f.ThirdParty, f => f.Currency/*, f => f.Dispositions, f => f.PaymentOrders*/, f => f.DispositionInvoices, f => f.PaymentOrderInvoices)
                                             .Where(f => f.State == State.Active && f.DecontId == decontId).ToList();

            var ret = ObjectMapper.Map<List<InvoiceDTO>>(invoices);
            return ret;
        }

        public List<InvoiceListDto> GetInvoiceDetails(int id)
        {
            if (id == 0)
            {
                return new List<InvoiceListDto>();
            }
            var invoice = GetInvoice(id);

            var list = invoice.InvoiceDetails.Select(f => new InvoiceListDto
            {
                Id = f.Id,
                Details = f.Element + "; Valoare: " + f.Value.ToString("N2"),
                RemainingValue = 0,
                TotalValue = 0,

            }).ToList();
            return list;
        }

        public List<InvoiceListForImoAssetDto> GetInvoiceDetailsForImoAsset(int imoAssetItemId, int operationType, int? operationId, int? invoiceId)
        {
            var ret = new List<InvoiceListForImoAssetDto>();
            //var imoAssetItem = _imoAssetRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.Id == imoAssetItemId && f.State == State.Active && f.InvoiceDetailsId != null).FirstOrDefault();

            //if (imoAssetItem == null)
            //{
            //    return new List<InvoiceListForImoAssetDto>();
            //}

            //var invoice = _invoiceRepository.GetAllIncluding(f => f.InvoiceDetails, f => f.ThirdParty).Where(f => f.State == State.Active && f.ThirdPartyQuality == ThirdPartyQuality.Furnizor &&
            //                                                                              f.InvoiceDetails.Any(g => g.Id == imoAssetItem.InvoiceDetailsId))
            //                                                                        .FirstOrDefault();

            //if (invoiceDetailId == null)
            //{
            //    throw new UserFriendlyException("Eroare",  "Mijlocul fix selectat nu contine detalii pe factura");
            //}

            //var invoiceId = _invoiceDetailsRepository.GetAll().Where(f => f.Id == invoiceDetailId).Select(f => f.InvoicesId).FirstOrDefault();
            //var invoice = GetInvoice(invoiceId);

            //var appClient = GetCurrentTenant();
            //var imoAssetOperDet = _imoAssetOperRepository.GetImoAssetOperDetails(imoAssetItemId, appClient.Id).Select(f => f.InvoiceDetailId).ToList();

            //if(operationId != 0) // EDIT
            //{
            //    ret = invoice.InvoiceDetails.Where(f => f.UsedInGest == true && imoAssetOperDet.Contains(f.Id)).Select(f => new InvoiceListForImoAssetDto
            //    {
            //        Id = f.Id,
            //        Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
            //        InvValue = f.Value,
            //        Duration = f.DurationInMonths ?? 0
            //    }).ToList();
            //}
            //else // INSERT
            //{
            //    ret = invoice.InvoiceDetails.Where(f => f.UsedInGest == true && !imoAssetOperDet.Contains(f.Id)).Select(f => new InvoiceListForImoAssetDto
            //    {
            //        Id = f.Id,
            //        Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
            //        InvValue = f.Value,
            //        Duration = f.DurationInMonths ?? 0

            //    }).ToList();
            //}

            var appClient = GetCurrentTenant();
            var invoiceDetailList = new List<InvoiceListForImoAssetDto>();
            // iau lista id-urile de detaliu de facturi utilizate la operatii
            var invoiceDetailsUsed = _imoAssetOperDetailRepository.GetAllIncluding(f => f.ImoAssetOper)
                                                                  .Where(f => f.ImoAssetOper.State == State.Active && f.ImoAssetOper.Id != (operationId ?? 0) && f.InvoiceDetailId != null)
                                                                  .Select(f => f.InvoiceDetailId)
                                                                  .ToList();

            if (operationType == (int)ImoAssetOperType.Modernizare)
            {
                invoiceDetailList = _invoiceDetailsRepository.GetAllIncluding(f => f.Invoices, f => f.Invoices.ThirdParty, f => f.InvoiceElementsDetails)
                                                .Where(f => f.Invoices.State == State.Active && f.State == State.Active
                                                        && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe && f.UsedInGest
                                                        && !invoiceDetailsUsed.Contains(f.Id)
                                                        )
                                                .Select(f => new InvoiceListForImoAssetDto
                                                {
                                                    Id = f.Id,
                                                    Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
                                                    InvValue = f.Value,
                                                    FiscalValue = f.Value,
                                                    Duration = f.DurationInMonths ?? 0

                                                }).ToList();
            }

            if (operationType == (int)ImoAssetOperType.Vanzare)
            {
                invoiceDetailList = _invoiceDetailsRepository.GetAllIncluding(f => f.Invoices, f => f.Invoices.ThirdParty, f => f.InvoiceElementsDetails)
                                               .Where(f => f.Invoices.State == State.Active && f.State == State.Active
                                                       && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe && !f.UsedInGest
                                                       && !invoiceDetailsUsed.Contains(f.Id)
                                                       && f.InvoicesId == invoiceId)
                                               .Select(f => new InvoiceListForImoAssetDto
                                               {
                                                   Id = f.Id,
                                                   Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
                                                   InvValue = f.Value,
                                                   FiscalValue = f.Value,
                                                   Duration = f.DurationInMonths ?? 0

                                               }).ToList();
            }



            return invoiceDetailList;

        }

        public List<InvoiceListForInvObjectDto> GetInvoiceDetailsForInvObject(int invObjectItemId, int? operationId)
        {
            var ret = new List<InvoiceListForInvObjectDto>();
            //var invObjectItem = _invObjectRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.Id == invObjectItemId && f.State == State.Active && f.InvoiceDetailsId != null).FirstOrDefault();

            //if (invObjectItem == null)
            //{
            //    return new List<InvoiceListForInvObjectDto>();
            //}

            //var invoice = _invoiceRepository.GetAllIncluding(f => f.InvoiceDetails, f => f.ThirdParty).Where(f => f.State == State.Active && f.ThirdPartyQuality == ThirdPartyQuality.Furnizor &&
            //                                                                  f.InvoiceDetails.Any(g => g.Id == invObjectItem.InvoiceDetailsId))
            //                                                            .FirstOrDefault();

            var appClient = GetCurrentTenant();

            var invoiceDetailsUsed = _invOperationDetailRepository.GetAllIncluding(f => f.InvObjectOper)
                                                                  .Where(f => f.InvObjectOper.State == State.Active && f.InvObjectOper.Id != (operationId ?? 0) && f.InvoiceDetailId != null)
                                                                  .Select(f => f.InvoiceDetailId)
                                                                  .ToList();
            var invoiceDetailList = _invoiceDetailsRepository.GetAllIncluding(f => f.Invoices, f => f.Invoices.ThirdParty, f => f.InvoiceElementsDetails)
                                                             .Where(f => f.Invoices.State == State.Active && f.State == State.Active
                                                                     && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar && !f.UsedInGest
                                                                     && !invoiceDetailsUsed.Contains(f.Id)
                                                                     )
                                                             .Select(f => new InvoiceListForInvObjectDto
                                                             {
                                                                 Id = f.Id,
                                                                 Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
                                                                 InvValue = f.Value,
                                                                 Duration = f.DurationInMonths ?? 0
                                                             }).ToList();
            ret = invoiceDetailList;

            //var invObjectOperDet = _invOperationRepository.GetInvObjectOperDetails(invObjectItemId, appClient.Id).Select(f => f.InvoiceDetailId).ToList();

            //if (operationId != 0) // EDIt
            //{
            //    ret = invoice.InvoiceDetails.Where(f => f.UsedInGest == true && invObjectOperDet.Contains(f.Id)).Select(f => new InvoiceListForInvObjectDto
            //    {
            //        Id = f.Id,
            //        Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
            //        InvValue = f.Value,
            //        Duration = f.DurationInMonths ?? 0
            //    }).ToList();
            //}
            //else // INSERT
            //{
            //    ret = invoice.InvoiceDetails.Where(f => f.UsedInGest == true && !invObjectOperDet.Contains(f.Id)).Select(f => new InvoiceListForInvObjectDto
            //    {
            //        Id = f.Id,
            //        Details = f.Invoices.ThirdParty.FullName + " " + f.Invoices.InvoiceNumber + " / " + f.Invoices.InvoiceDate.ToShortDateString() + ", " + f.Element,
            //        InvValue = f.Value,
            //        Duration = f.DurationInMonths ?? 0
            //    }).ToList();
            //}

            return ret;
        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        public void SaveInvoice(InvoiceDTO invoice)
        {
            int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

            if (!_operationRepository.VerifyClosedMonth(invoice.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra factura deoarece luna contabila este inchisa");
            try
            {
                ValidateInvoice(invoice);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare!", ex.Message);
            }

            try
            {
                CheckCurrencyForInvoice(invoice);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare!", ex.Message);
            }

            int count = 0;
            count = invoice.InvoiceDetails.Count;
            //if (count == 0)
            //{
            //    throw new UserFriendlyException("Eroare!", "Nu ati definit detalii pentru factura!");
            //}

            //count = invoice.InvoiceDetails.Where(f => f.Element == "" || f.Element == null).ToList().Count;
            //if (count != 0)
            //{
            //    throw new UserFriendlyException("Eroare!", "Exista detalii pentru care nu ati completat descrierea!");
            //}
            //if (invoice.Value == 0)
            //{
            //    throw new UserFriendlyException("Eroare!", "Valoarea facturii este 0!");
            //}
            if (/*invoice.InvoiceSeries == null || */invoice.InvoiceNumber == null)
            {
                throw new UserFriendlyException("Eroare", "Nu au fost completate campurile pentru seria sau numarul facturii");
            }

            var currency = _personRepository.CurrencyList().FirstOrDefault(f => f.Id == invoice.CurrencyId);

            var payedDispo = _dispositionInvoiceRepository.GetAll().Where(f => f.InvoiceId == invoice.Id).Sum(f => f.PayedValue);
            var payedOP = _paymentOrderInvoiceRepository.GetAll().Where(f => f.InvoiceId == invoice.Id).Sum(f => f.PayedValue);
            var payedValue = (payedDispo + payedOP);
            if (invoice.Value < payedValue)
            {
                throw new UserFriendlyException("Noua valoare nu poate fi mai mica decat suma platita, in valoare de " + payedValue + ' ' + currency.CurrencyCode);
            }

            try
            {
                invoice.HasDecont = invoice.DecontId != null ? true : false;
                var newInvoice = ObjectMapper.Map<Invoices>(invoice);

                var appClient = GetCurrentTenant();
                newInvoice.TenantId = appClient.Id;

                count = newInvoice.InvoiceDetails.Where(f => f.ContaOperationDetailId != null).ToList().Count;
                if (count != 0)
                {
                    DeleteOperation(newInvoice.Id, invoice.InvoiceDetails.Select(f => f.InvoiceElementsDetails.InvoiceElementsType).FirstOrDefault());
                    newInvoice.ContaOperationId = null;
                }

                if (newInvoice.Id != 0)
                {
                    DeleteComputeGest(newInvoice.Id);
                    CheckImoAssetForInvoice(newInvoice.Id);
                }

                _invoiceRepository.InsertOrUpdateV(newInvoice);
                CurrentUnitOfWork.SaveChanges();

                count = invoice.InvoiceDetails.Count;
                if (count != 0)
                {
                    count = invoice.InvoiceDetails.Where(f => f.Element == "" || f.Element == null).ToList().Count;
                    if (count != 0)
                    {
                        throw new UserFriendlyException("Eroare!", "Exista detalii pentru care nu ati completat descrierea!");
                    }
                    if (invoice.Value == 0)
                    {
                        throw new UserFriendlyException("Eroare!", "Valoarea facturii este 0!");
                    }

                    if (newInvoice.DecontId == null)
                    {
                        var invoiceDB = _invoiceRepository.GetAllIncludeElemDet().FirstOrDefault(f => f.Id == newInvoice.Id);

                        //foreach (var item in invoiceDB.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans))
                        //{
                        //    if (item.DurationInMonths == 0)
                        //    {
                        //        var check = _autoOperationRepository.CheckExistingConta(item.InvoicesId, item.Invoices.OperationDate, item.Invoices.CurrencyId);
                        //        if (check != 0)
                        //        {
                        //            // generez nota contabila direct
                        //            _autoOperationRepository.SaveDirectToConta(invoiceDB.Id, invoiceDB.OperationDate, localCurrencyId);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        SavePrepaymentFromInvoice(newInvoice.Id);
                        //    }
                        //}
                        var countCheltAvNoDuration = invoiceDB.InvoiceDetails.Count(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans
                                                                                    && f.DurationInMonths == 0);
                        if (countCheltAvNoDuration != 0)
                        {
                            var check = _autoOperationRepository.CheckExistingConta(invoiceDB.Id, invoiceDB.OperationDate, invoiceDB.CurrencyId);
                            if (check != 0)
                            {
                                // generez nota contabila direct
                                _autoOperationRepository.SaveDirectToConta(invoiceDB.Id, invoiceDB.OperationDate, localCurrencyId);
                            }
                        }

                        var countCheltAvDuration = invoiceDB.InvoiceDetails.Count(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans
                                                                                    && f.DurationInMonths != 0);
                        if (countCheltAvDuration != 0)
                        {
                            SavePrepaymentFromInvoice(newInvoice.Id);
                        }

                        _autoOperationRepository.InvoiceToConta(invoiceDB.Id, invoiceDB.OperationDate, localCurrencyId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare!", /*"Factura nu poate fi modificata, deoarece " + */ex.Message);
            }
        }

        private void CheckCurrencyForInvoice(InvoiceDTO invoice)
        {
            if (invoice.DecontId != null && invoice.DecontareInLei == false)
            {
                var decont = _decontRepository.GetAll().Where(f => f.Id == invoice.DecontId && f.State == State.Active).FirstOrDefault();

                if (decont.CurrencyId != invoice.CurrencyId)
                {
                    throw new Exception("Moneda selectata in document nu corespunde cu cea din decont ");
                }
            }

        }

        public List<ThirdPartyDTO> GetThridPathy(string search)
        {

            var ret = _personRepository.GetAll().OfType<LegalPerson>().Where(s => s.Name.ToUpper().Contains(search.ToUpper())).ToList().Select(s => new ThirdPartyDTO() { Id = s.Id, Name = s.FullName }).ToList();
            return ret;
        }

        // Facturi neplatite pentru beneficiarul curent
        public List<InvoiceListDto> UnpayedInvoicesForThirdParty(int thirdPartyId, int paymentOrderId)
        {
            try
            {
                // toate facturile pentru beneficiarul curent
                var invoiceList = _invoiceRepository.GetAllIncluding(f => f.Currency, /*f => f.Dispositions, f => f.PaymentOrders, */f => f.DocumentType)
                                      .Where(f => f.State == State.Active && f.ThirdPartyId == thirdPartyId && f.ThirdPartyQuality == ThirdPartyQuality.Furnizor
                                      //&& f.Dispositions.Count == 0
                                      //&& f.InvoicesDocumentType == InvoicesDocumentType.FacturaFiscala
                                      ).ToList()
                                      .OrderByDescending(f => f.InvoiceDate)
                                      .ToList();

                // toate ordinele de plata care sunt platite complet
                var paymentOrders = _paymentOrdersRepository.GetAll()/*.Include(f => f.Invoice).ThenInclude(f => f.PaymentOrders)*/
                                                                     /*.Include(f => f.Invoice).ThenInclude(f => f.Dispositions)*/.ToList()
                                                                     .Where(f => f.State == State.Active && f.BeneficiaryId == thirdPartyId/* && f.Invoice.RestPlata == 0*/)
                                                                     .ToList();

                //filtrez lista facturilor a.i. sa contina doar facturile care nu se afla in paymentOrders pentru beneficiarul selectat
                if (paymentOrders.Count > 0 && paymentOrderId == 0)
                {
                    //  invoiceList = invoiceList.Where(f => paymentOrders.All(g => g.InvoiceId != f.Id)).ToList();

                }
                var ret = invoiceList.Select(f => new InvoiceListDto
                {
                    Id = f.Id,
                    Details = f.InvoiceSeries + " " + f.InvoiceNumber + "/" + LazyMethods.DateToString(f.InvoiceDate) + "-" + f.Currency.CurrencyCode + " - Total factura: " + f.Value,
                    TotalValue = (f.Value == 0) ? f.FileDocValue : f.Value,
                    RemainingValue = f.Value
                }).ToList();

                foreach (var item in ret)
                {
                    try
                    {
                        var totalPayed = _paymentOrdersRepository.GetAll().Where(f => /*f.InvoiceId == item.Id && */f.State == State.Active).Sum(f => f.Value);
                        var remainingValue = item.TotalValue - totalPayed;
                        item.RemainingValue = remainingValue;
                    }
                    catch
                    {

                    }
                    item.Details += "; Rest de plata: " + item.RemainingValue;
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        public void DeleteInvoice(int invoiceId)
        {

            try
            {
                _invoiceRepository.CheckPayedInvoice(invoiceId);
                CheckImoAssetForInvoice(invoiceId);
                _invOperationRepository.ExistingInvObjectFromInvoice(invoiceId); // verific daca obiectul de inventar a fost creat
                DeleteComputeGest(invoiceId);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare!", "Factura nu poate fi stearsa, deoarece " + ex.Message);
            }
            try
            {
                _invoiceRepository.DeleteInvoice(invoiceId);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        public void DeleteInvoiceFromDecont(int invoiceId, int decontId)
        {
            try
            {
                _invoiceRepository.DeleteInvoiceFromDecont(invoiceId, decontId);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", "Factura nu poate fi stearsa, deoarece" + ex.Message);
            }


        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        public void DeleteInvoiceElementsDetail(int id)
        {
            try
            {
                _invoiceRepository.DeleteInvoiceElementsDetail(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public GetAssetOutput InvoicesForAsset()
        {
            var appClient = GetCurrentTenant();
            var _assetList = _invoiceRepository.InvoicesForAsset(appClient.Id);
            var ret = new GetAssetOutput { GetInvoices = ObjectMapper.Map<List<NirDetailDTO>>(_assetList) };
            return ret;
        }

        public GetAssetOutput InvoicesForAssetSale(int? invoiceId)
        {
            var appClient = GetCurrentTenant();
            var _assetList = _invoiceRepository.InvoicesForAssetSale(appClient.Id, invoiceId);
            var ret = new GetAssetOutput { GetInvoices = ObjectMapper.Map<List<NirDetailDTO>>(_assetList) };
            return ret;
        }

        public GetAssetOutput InvoicesForPrepayments(int prepaymentType)
        {
            var appClient = GetCurrentTenant();
            var _assetList = _invoiceRepository.InvoicesForPrepayments(appClient.Id, (PrepaymentType)prepaymentType);
            var ret = new GetAssetOutput { GetInvoices = ObjectMapper.Map<List<NirDetailDTO>>(_assetList) };
            return ret;
        }

        public GetAssetOutput InvoicesForInvObjects()
        {
            var appClient = GetCurrentTenant();
            var _invObjectsList = _invoiceRepository.InvoicesForInvObjects(appClient.Id);
            var ret = new GetAssetOutput { GetInvoices = ObjectMapper.Map<List<NirDetailDTO>>(_invObjectsList) };
            return ret;
        }

        public GetAssetOutput InvoicesForInvObject(int? invoiceId)
        {
            var appClient = GetCurrentTenant();
            var _invObjectsList = _invoiceRepository.InvoicesForInvObject(appClient.Id, invoiceId);
            var ret = new GetAssetOutput { GetInvoices = ObjectMapper.Map<List<NirDetailDTO>>(_invObjectsList) };
            return ret;
        }

        public List<InvoiceElementsDetailsCategoryListDTO> GetInvoiceElementsDetailsCategories()
        {
            var list = _invoiceRepository.GetInvoiceElementsDetailsCategories();
            var ret = ObjectMapper.Map<List<InvoiceElementsDetailsCategoryListDTO>>(list);

            return ret;
        }

        public List<InvoiceElementsDetailsCategoryListDTO> GetInvoiceElementsDetailsCategoriesforThirdPartyQuality(int thirdPartyQuality)
        {
            var list = _invoiceRepository.GetInvoiceElementsDetailsCategories();

            if (thirdPartyQuality == (int)ThirdPartyQuality.Furnizor)
            {
                list = list.Where(f => f.CategoryType == CategoryType.Cheltuieli).ToList();
            }

            else if (thirdPartyQuality == (int)ThirdPartyQuality.Client)
            {
                list = list.Where(f => f.CategoryType == CategoryType.Venituri).ToList();
            }
            else
            {
                list = list.Where(f => f.CategoryType == CategoryType.PlatiIncasari).ToList();
            }

            var ret = ObjectMapper.Map<List<InvoiceElementsDetailsCategoryListDTO>>(list);

            return ret;
        }

        //[AbpAuthorize("Admin.Conta.CategElemFactura.Acces")]
        public InvoiceElementsDetailsCategoryEditDTO GetInvoiceElementsDetailsCategory(int id)
        {
            var element = _invoiceRepository.GetElementsDetailsCategory(id);
            var ret = ObjectMapper.Map<InvoiceElementsDetailsCategoryEditDTO>(element);
            return ret;
        }

        //[AbpAuthorize("Admin.Conta.CategElemFactura.Acces")]
        public void SaveInvoiceCategoryElement(InvoiceElementsDetailsCategoryEditDTO categoryElement)
        {
            var newCategoryElement = ObjectMapper.Map<InvoiceElementsDetailsCategory>(categoryElement);
            var appClient = GetCurrentTenant();
            newCategoryElement.TenantId = appClient.Id;
            _invoiceRepository.SaveInvoiceElementsDetailCategory(newCategoryElement);
        }

        //[AbpAuthorize("Admin.Conta.CategElemFactura.Acces")]
        public void DeleteInvoiceElementsDetailsCategory(int id)
        {
            try
            {
                _invoiceRepository.DeleteInvoiceElementsDetailsCategory(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<InvoiceElementsDetailsDTO> GetInvoiceElementsDetailsByCategoryId(int? categoryId)
        {
            var res = _invoiceRepository.GetInvoiceElementsDetails().Where(f => f.InvoiceElementsDetailsCategoryId == categoryId).OrderBy(f => f.Description);
            var ret = ObjectMapper.Map<List<InvoiceElementsDetailsDTO>>(res);
            return ret;
        }

        public List<InvoiceListDto> GetInvoicesByThirdPartyId(int thirdPartyId, int dispositionId)
        {
            var invoices = _invoiceRepository.GetAll().Include(f => f.Currency).Include(f => f.ThirdParty)/*.Include(f => f.DispositionInvoice)*//*.Include(f => f.PaymentOrders)*/.ToList()
                                 .Where(i => i.State == State.Active && i.ThirdPartyId == thirdPartyId).OrderByDescending(f => f.InvoiceDate).ToList();

            //toate dispozitiile  care sunt platite complet
            var dispositions = _dispositionRepository.GetAll()/*.Include(f => f.Invoice).ThenInclude(f => f.PaymentOrders)*/
                                                                     /*.Include(f => f.Invoice).ThenInclude(f => f.Dispositions)*/.ToList()
                                                                 .Where(f => f.State == State.Active && f.ThirdPartyId == thirdPartyId/* && f.Invoice?.RestPlata == 0*/)
                                                                 .ToList();

            //filtrez lista facturilor a.i.sa contina doar facturile care nu se afla in dispositions pentru furnizorul selectat
            //if (dispositions.Count > 0 && dispositionId == 0)
            //{
            //    invoices = invoices.Where(f => dispositions.All(g => g.InvoiceId != f.Id)).ToList();

            //}

            var ret = invoices.Select(f => new InvoiceListDto
            {
                Id = f.Id,
                Details = f.InvoiceSeries + " " + f.InvoiceNumber + "/" + LazyMethods.DateToString(f.InvoiceDate) + "-" + f.Currency.CurrencyCode + " - Total factura: " + f.Value,
                TotalValue = f.Value,
                RemainingValue = f.Value
            }).ToList();

            foreach (var item in ret)
            {
                try
                {
                    var totalPayed = _dispositionRepository.GetAll().Where(f => /*f.DispositionInvoice.InvoiceId == item.Id && */f.State == State.Active).Sum(f => f.Value);
                    var remainingValue = item.TotalValue - totalPayed;
                    item.RemainingValue = remainingValue;
                }
                catch
                {

                }
                item.Details += "; Rest de plata: " + item.RemainingValue;
            }

            return ret;
        }

        [AbpAuthorize("Conta.Documente.Modificare")]
        private void ValidateInvoice(InvoiceDTO invoice)
        {
            var count = _invoiceRepository.GetAll().Count(f => f.ThirdPartyId == invoice.ThirdPartyId && f.State == State.Active && f.InvoiceNumber == invoice.InvoiceNumber && f.InvoiceDate == invoice.InvoiceDate &&
                                                          f.ThirdPartyQuality == invoice.ThirdPartyQuality && f.InvoiceSeries == invoice.InvoiceSeries && f.Id != invoice.Id);

            if (invoice.ThirdPartyId == null)
            {
                throw new Exception("Nu a fost specificat tertul");
            }

            if (count != 0)
            {
                throw new Exception("Exista o alta factura inregistrata pentru acest tert cu informatii identice");
            }
            //count = _invoiceRepository.GetAll().Count(f => f.ThirdPartyId == invoice.ThirdPartyId && f.State == State.Active && f.InvoiceNumber == invoice.InvoiceNumber &&
            //                                              f.ThirdPartyQuality == invoice.ThirdPartyQuality && f.InvoiceSeries == invoice.InvoiceSeries && f.Id != invoice.Id);

            //if(count != 0)
            //{
            //    throw new Exception("Exista o alta factura inregistrata pentru acest tert care are seria si numarul identic");

            //}
        }

        private void CheckImoAssetForInvoice(int invoiceId)
        {
            var invoice = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.Id == invoiceId).FirstOrDefault();
            foreach (var item in invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe))
            {
                var imoAsset = _imoAssetRepository.GetAll().FirstOrDefault(f => f.InvoiceDetailsId == item.Id && f.State == State.Active);

                if (imoAsset != null)
                {
                    throw new Exception("Operatiunea nu poate fi efectuata deoarece mijlocul fix a fost creat.");
                }
            }
        }

        /// <summary>
        /// Sterg gestiunea calculata pentru cheltuielile din factura selectata
        /// </summary>
        /// <param name="invoiceId"></param>
        [AbpAuthorize("Conta.Documente.Modificare")]
        private void DeleteComputeGest(int invoiceId)
        {
            var invoice = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.Id == invoiceId).FirstOrDefault();
            foreach (var item in invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans))
            {
                var prepayment = _prepaymentRepository.GetAll().FirstOrDefault(f => f.InvoiceDetailsId == item.Id && f.State == State.Active);

                if (prepayment != null)
                {
                    var prepaymentBalance = _prepaymentBalanceRepository.GetAll().Where(f => f.PrepaymentPFId == prepayment.Id).FirstOrDefault();
                    if (prepaymentBalance != null)
                    {
                        _autoOperationRepository.DeleteUncheckedAutoOperation(prepayment.PaymentDate, prepayment.PrepaymentType);
                    }

                    var lastProcessedDateForPrepayment = _prepaymentRepository.LastProcessedDateForPrepayment(prepayment.PrepaymentType, prepayment.Id);

                    if (invoice.OperationDate <= lastProcessedDateForPrepayment)
                    {
                        _prepaymentRepository.GestDelComputingForPrepayment(invoice.OperationDate, prepayment.PrepaymentType, prepayment.Id);
                    }

                    _prepaymentRepository.Delete(prepayment.Id);
                }
            }
        }

        /// <summary>
        /// Adaug cheltuiala atunci cand salvez o factura
        /// </summary>
        /// <param name="id"></param>
        [AbpAuthorize("Conta.Documente.Modificare")]
        private void SavePrepaymentFromInvoice(int id)
        {
            int localCurrecyId = GetCurrentTenant().LocalCurrencyId.Value;
            var invoice = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.Id == id).FirstOrDefault();
            foreach (var item in invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans && f.DurationInMonths != 0))
            {

                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                 .OrderByDescending(f => f.BalanceDate)
                                 .FirstOrDefault().BalanceDate;

                var documentType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                         .Where(f => f.OperType == PrepaymentOperType.Constituire)
                                         .FirstOrDefault();

                var elementDetail = GetInvoiceElementsDetail(item.InvoiceElementsDetailsId);
                int prepaymentAccountId = 0;



                if (elementDetail.CorrespondentAccount.StartsWith('6') && invoice.ActivityTypeId != null)
                {
                    prepaymentAccountId = _autoOperationRepository.GetAutoAccountActivityType(elementDetail.CorrespondentAccount, invoice.ThirdPartyId, invoice.ActivityTypeId.Value, invoice.OperationDate, localCurrecyId, null);
                }
                else
                {
                    prepaymentAccountId = _autoOperationRepository.GetAutoAccount(elementDetail.CorrespondentAccount, invoice.ThirdPartyId, invoice.OperationDate, localCurrecyId, null);

                }
                var deprecAccountId = _autoOperationRepository.GetAutoAccount(elementDetail.ExpenseAmortizAccount, invoice.ThirdPartyId, invoice.OperationDate, localCurrecyId, null);

                var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoice.InvoiceDate, invoice.CurrencyId, localCurrecyId);

                var prepayment = new Prepayment
                {
                    Description = item.Element,
                    DurationInMonths = item.DurationInMonths ?? 0,
                    InvoiceDetailsId = item.Id,
                    InvoiceDetails = item,
                    PaymentDate = invoice.OperationDate, //LazyMethods.Now(),
                    PrepaymentAccountId = prepaymentAccountId,
                    DepreciationAccountId = deprecAccountId,
                    PrepaymentType = PrepaymentType.CheltuieliInAvans,
                    PrepaymentValue = (item.Value + item.VAT) * exchangeRate,
                    ThirdPartyId = invoice.ThirdPartyId,
                    PrimDocumentNr = invoice.InvoiceNumber,
                    PrimDocumentDate = invoice.InvoiceDate,
                    DepreciationStartDate = item.DataStartAmortizare.Value,  /*new DateTime(invoice.OperationDate.Year, invoice.OperationDate.Month, 1),*/ // LazyMethods.FirstDayNextMonth(invoice.OperationDate),
                    PrimDocumentTypeId = documentType.DocumentType.Id,
                    TenantId = GetCurrentTenant().Id
                };

                _prepaymentRepository.InsertOrUpdateV(prepayment);
                CurrentUnitOfWork.SaveChanges();

                //calculez gestiunea
                _prepaymentRepository.GestComputingForPrepayment(invoice.OperationDate, prepayment.PrepaymentType, lastBalanceDate, prepayment.Id);
                _autoOperationRepository.AutoPrepaymentsOperationAdd(invoice.OperationDate, localCurrecyId, prepayment.PrepaymentType, lastBalanceDate, null);
            }
        }

        // Sterg notele contabile pentru factura selectata
        [AbpAuthorize("Conta.Documente.Modificare")]
        private void DeleteOperation(int invoiceId, int invoiceElementType)
        {
            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                                            .OrderByDescending(f => f.BalanceDate)
                                                            .FirstOrDefault().BalanceDate;
            try
            {
                _autoOperationRepository.InvoiceDeleteConta(invoiceId, lastBalanceDate, invoiceElementType);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvoiceListSelectableDto> GetInvoicesForDispositionByThirdPartyId(int thirdPartyId, int dispositionId)
        {
            var invoices = _invoiceRepository.GetAll().Include(f => f.Currency).Include(f => f.ThirdParty)
                                                      .Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices).ToList()
                       .Where(i => i.State == State.Active && i.ThirdPartyId == thirdPartyId).OrderByDescending(f => f.InvoiceDate).ToList();

            var dispInvoiceIds = _dispositionInvoiceRepository.GetAllIncluding(f => f.Invoice, f => f.Disposition).ToList()
                                                              .Where(f => f.DispositionId == dispositionId)
                                                              .Select(f => f.InvoiceId).ToList();

            var dispInvoice = invoices.Where(f => dispInvoiceIds.Contains(f.Id)).ToList();
            invoices = invoices.Where(f => f.RestPlata != 0 && !dispInvoiceIds.Contains(f.Id)).ToList();

            var ret = invoices.OrderBy(f => f.InvoiceDate).Select(f => new InvoiceListSelectableDto
            {
                Id = f.Id,
                Details = f.InvoiceSeries + " " + f.InvoiceNumber + "/" + LazyMethods.DateToString(f.InvoiceDate) + "-" + f.Currency.CurrencyCode,
                TotalValue = f.Value,
                RemainingValue = f.RestPlata,
                CurrencyId = f.CurrencyId,
                ThirdPartyId = f.ThirdPartyId,
                PayedValue = 0,
                Rest = f.RestPlata,
                Selected = false
            }).ToList();

            var retDispInvoice = new List<InvoiceListSelectableDto>();

            foreach (var dispInv in dispInvoice.OrderBy(f => f.InvoiceDate).Where(f => !(ret.Select(f => f.Id).ToList()).Contains(f.Id)))
            {
                var totalPayedOtherDispo = _dispositionInvoiceRepository.GetAll().Where(f => f.InvoiceId == dispInv.Id && f.DispositionId != dispositionId).Sum(f => f.PayedValue);
                var totalPayedOrder = _paymentOrderInvoiceRepository.GetAll().Where(f => f.InvoiceId == dispInv.Id).Sum(f => f.PayedValue);
                var remainingValue = dispInv.Value - totalPayedOtherDispo - totalPayedOrder;
                var totalPayedThisDispo = _dispositionInvoiceRepository.GetAll().Where(f => f.InvoiceId == dispInv.Id && f.DispositionId == dispositionId).Sum(f => f.PayedValue);

                var dispInvoiceItem = new InvoiceListSelectableDto
                {
                    Id = dispInv.Id,
                    Details = dispInv.InvoiceSeries + " " + dispInv.InvoiceNumber + "/" + LazyMethods.DateToString(dispInv.InvoiceDate) + "-" + dispInv.Currency.CurrencyCode + " - Total factura: " + dispInv.Value,
                    TotalValue = dispInv.Value,
                    RemainingValue = remainingValue,
                    Rest = remainingValue - totalPayedThisDispo,
                    PayedValue = totalPayedThisDispo,
                    CurrencyId = dispInv.CurrencyId,
                    Selected = true
                };
                retDispInvoice.Add(dispInvoiceItem);
            }

            ret.AddRange(retDispInvoice);

            return ret;
        }

        public InvoiceListSelectableDto CalculatePayedInvoice(InvoiceListSelectableDto invoice, decimal dispositionSum)
        {
            try
            {
                if (dispositionSum < 0)
                {
                    throw new Exception("Valoarea facturilor selectate depaseste suma introdusa");
                }

                if (invoice.PayedValue == 0)
                {
                    if (dispositionSum > invoice.Rest)
                    {
                        invoice.PayedValue = invoice.Rest;
                    }
                    else
                    {
                        invoice.PayedValue = dispositionSum;
                    }
                }
                //if (invoice.PayedValue == 0 && dispositionSum > invoice.Rest)
                //{
                //    invoice.PayedValue = invoice.Rest;
                //}
                //else
                //{
                //    invoice.PayedValue = dispositionSum;
                //}

                invoice.Rest = invoice.RemainingValue - invoice.PayedValue;

                return invoice;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // Returnez lista facturilor platite si neplatite pentru beneficiarul selectat
        public List<InvoiceListSelectableDto> GetInvoicesForPaymentOrderByThirdPartyId(int thirdPartyId, int paymentOrderId)
        {
            var invoices = _invoiceRepository.GetAll().Include(f => f.Currency).Include(f => f.ThirdParty)
                                                          .Include(f => f.DispositionInvoices).Include(f => f.PaymentOrderInvoices).ToList()
                           .Where(i => i.State == State.Active && i.ThirdPartyId == thirdPartyId).OrderByDescending(f => f.InvoiceDate).ToList();

            var paymentOrderInvoiceIds = _paymentOrderInvoiceRepository.GetAllIncluding(f => f.Invoice, f => f.PaymentOrder).ToList()
                                                              .Where(f => f.PaymentOrderId == paymentOrderId)
                                                              .Select(f => f.InvoiceId).ToList();

            var paymentOrderInvoice = invoices.Where(f => paymentOrderInvoiceIds.Contains(f.Id)).ToList();
            invoices = invoices.Where(f => f.RestPlata != 0 && !paymentOrderInvoiceIds.Contains(f.Id)).ToList();

            var ret = invoices.OrderBy(f => f.InvoiceDate).Select(f => new InvoiceListSelectableDto
            {
                Id = f.Id,
                Details = f.InvoiceSeries + " " + f.InvoiceNumber + "/" + LazyMethods.DateToString(f.InvoiceDate) + "-" + f.Currency.CurrencyCode,
                TotalValue = f.Value,
                RemainingValue = f.RestPlata,
                CurrencyId = f.CurrencyId,
                ThirdPartyId = f.ThirdPartyId,
                PayedValue = 0,
                Rest = f.RestPlata,
                Selected = false
            }).ToList();

            var retPaymentOrderInvoice = new List<InvoiceListSelectableDto>();

            foreach (var paymentOrderInv in paymentOrderInvoice.OrderBy(f => f.InvoiceDate).Where(f => !(ret.Select(f => f.Id).ToList()).Contains(f.Id)))
            {
                var totalPayedOtherPaymentOrder = _paymentOrderInvoiceRepository.GetAll().Where(f => f.InvoiceId == paymentOrderInv.Id && f.PaymentOrderId != paymentOrderId).Sum(f => f.PayedValue);
                var totalPayedDisposition = _dispositionInvoiceRepository.GetAll().Where(f => f.InvoiceId == paymentOrderInv.Id).Sum(f => f.PayedValue);
                var remainingValue = paymentOrderInv.Value - totalPayedOtherPaymentOrder - totalPayedDisposition;
                var totalPayedThisPaymentOrder = _paymentOrderInvoiceRepository.GetAll().Where(f => f.InvoiceId == paymentOrderInv.Id && f.PaymentOrderId == paymentOrderId).Sum(f => f.PayedValue);

                var paymentOrderInvItem = new InvoiceListSelectableDto
                {
                    Id = paymentOrderInv.Id,
                    Details = paymentOrderInv.InvoiceSeries + " " + paymentOrderInv.InvoiceNumber + "/" + LazyMethods.DateToString(paymentOrderInv.InvoiceDate) + "-" + paymentOrderInv.Currency.CurrencyCode + " - Total factura: " + paymentOrderInv.Value,
                    TotalValue = paymentOrderInv.Value,
                    RemainingValue = remainingValue,
                    Rest = remainingValue - totalPayedThisPaymentOrder,
                    PayedValue = totalPayedThisPaymentOrder,
                    CurrencyId = paymentOrderInv.CurrencyId,
                    Selected = true
                };
                retPaymentOrderInvoice.Add(paymentOrderInvItem);
            }

            ret.AddRange(retPaymentOrderInvoice);

            return ret;
        }

        public int getCategoryIdByInvoiceElementDetailId(int id)
        {
            var item = _invoiceElementsDetailsRepository.Get(id);

            return (int)item.InvoiceElementsDetailsCategoryId;
        }

        //returnez numarul si seria facturii daca este selectat Client ca fiind Calitatea tertului
        public InvoiceDTO GetInvoiceSeriesAndNumber(int thirdPartyQuality)
        {
            try
            {
                var invoice = new InvoiceDTO();
                if (thirdPartyQuality == (int)ThirdPartyQuality.Client)
                {
                    invoice.InvoiceSeries = "FGDB";
                    //  var prevInvoiceNumber = _invoiceRepository.GetAll().Where(f => f.State == State.Active && f.ThirdPartyQuality == (ThirdPartyQuality)thirdPartyQuality).Max(f => Convert.ToInt32(f.InvoiceNumber));
                    var prevInvoiceNumber = getFacturaNumberIncremented();
                    invoice.InvoiceNumber = (prevInvoiceNumber).ToString();
                }
                else
                {
                    invoice.InvoiceSeries = "";
                    invoice.InvoiceNumber = "";
                }
                return invoice;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        // Returnez lista facturilor care nu apartin unui decont
        public List<InvoiceForDecontDTO> GetInvoicesForDecont(int? decontId, int currencyId)
        {
            try
            {
                var invoices = _invoiceRepository.GetAllIncluding(f => f.Currency, f => f.MonedaFactura, f => f.ThirdParty, f => f.DocumentType, f => f.ContaOperation, f => f.Contracts, f => f.Decont,
                                                                       f => f.DispositionInvoices, f => f.PaymentOrderInvoices, f => f.InvoiceDetails, f => f.ThirdParty)
                                                 .Where(f => f.State == State.Active && f.DecontId == null && f.CurrencyId == currencyId && f.InvoiceDetails.Count == 0)
                                                 .ToList()
                                                 .Select(f => new InvoiceForDecontDTO
                                                 {
                                                     Id = f.Id,
                                                     CurrencyId = f.MonedaFacturaId.Value,
                                                     CurrencyName = f.MonedaFactura.CurrencyName,
                                                     FileDocId = f.FileDocId,
                                                     FileDocValue = f.FileDocValue,
                                                     InvoiceDate = f.InvoiceDate,
                                                     InvoiceNumber = f.InvoiceNumber,
                                                     InvoiceSeries = f.InvoiceSeries,
                                                     Selected = false,
                                                     Value = f.Value,
                                                     State = f.State,
                                                     ThirdPartyAccount = f.ThirdParty?.FullName,
                                                     MonedaPlataId = f.CurrencyId,
                                                     MonedaPlataName = f.Currency.CurrencyName

                                                 })
                                                 .OrderByDescending(f => f.InvoiceDate)
                                                 .ToList();

                return invoices;
            }
            catch (Exception ex)
            {

                throw new Exception("Eroare", ex);
            }
        }

        public int getFacturaNumberIncremented()
        {
            // Pentru client
            try
            {
                var ret = _invoiceRepository.GetAllIncluding().Where(f => f.State == State.Active && f.ThirdPartyQuality == ThirdPartyQuality.Client).ToList().Select(f => Int32.TryParse(f.InvoiceNumber, out int j) ? j : 0).Max();
                return ret + 1;
            }
            catch (Exception e)
            {
                // in situatia in care nu a gasit nicio factura
                return 1;
            }
            
        } 

        public void SaveInvoicesFromDecont(List<int> invoiceIds, int decontId)
        {
            try
            {
                foreach (var invoiceId in invoiceIds)
                {
                    var invoice = _invoiceRepository.FirstOrDefault(f => f.Id == invoiceId);
                    invoice.HasDecont = true;
                    invoice.DecontId = decontId;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare");
            }
        }
    }
}
