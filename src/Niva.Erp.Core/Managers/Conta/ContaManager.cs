using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.Prepayments;
using Niva.Erp.Repositories.Economic;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Managers
{
    public class ContaManager : DomainService
    {
        IBalanceRepository _balanceRepository;
        IRepository<BalanceDetails> _balanceDetailsRepository;
        IOperationRepository _operationRepository;
        IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IInvoiceRepository _invoiceRepository;
        IAutoOperationRepository _autoOperationRepository;
        IImoAssetRepository _imoAssetRepository;
        IRepository<ImoAssetStock> _imoAssetStockRepository;
        IPrepaymentRepository _prepaymentRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IPersonRepository _personRepository;
        IAccountRepository _accountRepository;
        IDispositionRepository _dispositionRepository;
        ICurrencyRepository _currencyRepository;

        public ContaManager(IBalanceRepository balanceRepository, IOperationRepository operationRepository, IRepository<InvoiceDetails> invoiceDetailsRepository,
                            IInvoiceRepository invoiceRepository, IAutoOperationRepository autoOperationRepository, IImoAssetRepository imoAssetRepository,
                            IRepository<ImoAssetStock> imoAssetStockRepository, IPrepaymentRepository prepaymentRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository,
                            IPersonRepository personRepository, IRepository<BalanceDetails> balanceDetailsRepository, IAccountRepository accountRepository, IDispositionRepository dispositionRepository,
                            ICurrencyRepository currencyRepository)
        {
            _balanceRepository = balanceRepository;
            _operationRepository = operationRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _invoiceRepository = invoiceRepository;
            _autoOperationRepository = autoOperationRepository;
            _imoAssetRepository = imoAssetRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _prepaymentRepository = prepaymentRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _personRepository = personRepository;
            _balanceDetailsRepository = balanceDetailsRepository;
            _accountRepository = accountRepository;
            _dispositionRepository = dispositionRepository;
            _currencyRepository = currencyRepository;
        }


        public List<BalanceCompSummary> BalanceSummaryList(DateTime compDate, int appClientId)
        {
            var ret = new List<BalanceCompSummary>();
            var localCurrencyId = 1;
            var maxBalanceDate = _balanceRepository.GetAll().Where(f => f.TenantId == appClientId && f.Status == State.Active).Max(f => f.BalanceDate);

            if (maxBalanceDate == null)
            {
                maxBalanceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            // Caut facturi neinregistrate in conta
            var invoices = _invoiceDetailsRepository.GetAllIncluding(f => f.Invoices, f => f.InvoiceElementsDetails)
                                                    .Where(f => f.Invoices.State == State.Active && f.Invoices.InvoiceDate > maxBalanceDate && f.Invoices.InvoiceDate <= compDate
                                                                && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele
                                                                && f.ContaOperationDetailId == null && f.Invoices.TenantId == appClientId).ToList();

            var _invoicesCount = invoices.Count;
            var _sumInvoices = new BalanceCompSummary
            {
                Module = "Facturi",
                Summary = _invoicesCount == 0 ? "Nu exista facturi care nu au fost inregistrate in Contabilitate"
                : ("Sunt " + _invoicesCount.ToString() + " facturi care nu au fost inregistrate in Contabilitate"),
                Ok = (_invoicesCount == 0)
            };
            ret.Add(_sumInvoices);

            // Caut facturi fara detalii
            var incompleteInvoices = _invoiceRepository.GetAllIncluding(f => f.InvoiceDetails)
                                                              .Where(f => f.State == State.Active && f.InvoiceDate > maxBalanceDate && f.InvoiceDate <= compDate
                                                                && f.InvoiceDetails.Count == 0 && f.TenantId == appClientId).ToList();

            var _incompleteInvoicesCount = incompleteInvoices.Count;
            var _sumIncompleteInvoices = new BalanceCompSummary
            {
                Module = "Facturi",
                Summary = _incompleteInvoicesCount == 0 ? "Nu exista facturi fara elemente care nu au fost inregistrate in Contabilitate"
                : ("Sunt " + _incompleteInvoicesCount.ToString() + " facturi fara elemente care nu au fost inregistrate in Contabilitate"),
                Ok = (_incompleteInvoicesCount == 0)
            };
            ret.Add(_sumIncompleteInvoices);


            // Caut facturi pentru intrarea mijloacelor fixe

            var _assetList = _invoiceRepository.InvoicesForAsset(appClientId);
            var _assetListPer = _assetList.Where(f => f.InvoiceDate > maxBalanceDate && f.InvoiceDate <= compDate).ToList();
            var _assetListPerCount = _assetListPer.Count;
            var _sumAssetListPer = new BalanceCompSummary
            {
                Module = "Mijloace Fixe",
                Summary = _assetListPerCount == 0 ? "Nu exista facturi cu mijloace fixe care nu au fost inregistrate in modulul Mijloace fixe"
                : ("Sunt " + _assetListPerCount.ToString() + "  facturi cu mijloace fixe care nu au fost inregistrate in modulul Mijloace fixe"),
                Ok = (_assetListPerCount == 0)
            };
            ret.Add(_sumAssetListPer);

            // Operatii in gestiunea mijloacelor fixe care nu au fost generate in Contabilitate
            var _assetOperList = _autoOperationRepository.ImoAssetPrepareAdd(maxBalanceDate.AddDays(1), compDate, appClientId).ToList();
            var _assetOperListCount = _assetOperList.Count;
            var _sumAssetOperList = new BalanceCompSummary
            {
                Module = "Mijloace Fixe",
                Summary = _assetOperListCount == 0 ? "Nu exista operatii in gestiunea mijloacelor fixe care nu au fost generate in Contabilitate"
                : ("Exista operatii in gestiunea mijloacelor fixe care nu au fost generate in Contabilitate. Rulati contarile automate"),
                Ok = (_assetListPerCount == 0)
            };
            ret.Add(_sumAssetOperList);

            // calcul gestiune
            var _summaryAssetGest = SummaryAssetGest(maxBalanceDate.AddDays(1), compDate, appClientId);
            ret.Add(_summaryAssetGest);

            // Caut facturi pentru intrarea chelt in avans
            var _preExpenseList = _invoiceRepository.InvoicesForPrepayments(appClientId, PrepaymentType.CheltuieliInAvans);
            var _preExpenseListPer = _preExpenseList.Where(f => f.InvoiceDate > maxBalanceDate && f.InvoiceDate <= compDate).ToList();
            var _preExpenseListPerCount = _preExpenseListPer.Count;
            var _sumPreExpenseListPer = new BalanceCompSummary
            {
                Module = "Cheltuieli in avans",
                Summary = _preExpenseListPerCount == 0 ? "Nu exista facturi cu mijloace fixe care nu au fost inregistrate in modulul Cheltuieli in avans"
                : ("Sunt " + _preExpenseListPerCount.ToString() + " facturi cu mijloace fixe care nu au fost inregistrate in modulul Cheltuieli in avans"),
                Ok = (_preExpenseListPerCount == 0)
            };
            ret.Add(_sumAssetListPer);

            // Operatii in gestiunea cheltuielilor in avans care nu au fost generate in Contabilitate
            var _preExpenseOperList = _autoOperationRepository.PrepaymentsPrepareAdd(maxBalanceDate.AddDays(1), compDate, appClientId, PrepaymentType.CheltuieliInAvans).ToList();
            var _preExpenseOperListCount = _preExpenseOperList.Count;
            var _sumPreExpenseOperList = new BalanceCompSummary
            {
                Module = "Cheltuieli in avans",
                Summary = _preExpenseOperListCount == 0 ? "Nu exista operatii in gestiunea cheltuielilor in avans care nu au fost generate in Contabilitate"
                          : ("Exista operatii in gestiunea cheltuielilor in avans care nu au fost generate in Contabilitate. Rulati contarile automate"),
                Ok = (_preExpenseOperListCount == 0)
            };
            ret.Add(_sumPreExpenseOperList);

            // calcul gestiune
            var _summaryPreExpenseGest = SummaryPrepaymentGest(maxBalanceDate.AddDays(1), compDate, appClientId, PrepaymentType.CheltuieliInAvans);
            ret.Add(_summaryPreExpenseGest);


            // Caut facturi pentru intrarea venituri in avans
            var _preIncomeList = _invoiceRepository.InvoicesForPrepayments(appClientId, PrepaymentType.VenituriInAvans);
            var _preIncomeListPer = _preIncomeList.Where(f => f.InvoiceDate > maxBalanceDate && f.InvoiceDate <= compDate).ToList();
            var _preIncomeListPerCount = _preIncomeListPer.Count;
            var _sumPreIncomeListPer = new BalanceCompSummary
            {
                Module = "Venituri in avans",
                Summary = _preIncomeListPerCount == 0 ? "Nu exista facturi cu mijloace fixe care nu au fost inregistrate in modulul Venituri in avans"
                          : ("Sunt " + _preIncomeListPerCount.ToString() + " facturi cu mijloace fixe care nu au fost inregistrate in modulul Venituri in avans"),
                Ok = (_preIncomeListPerCount == 0)
            };
            ret.Add(_sumPreIncomeListPer);

            // Operatii in gestiunea veniturilor in avans care nu au fost generate in Contabilitate
            var _preIncomeOperList = _autoOperationRepository.PrepaymentsPrepareAdd(maxBalanceDate.AddDays(1), compDate, appClientId, PrepaymentType.VenituriInAvans).ToList();
            var _preIncomeOperListCount = _preIncomeOperList.Count;
            var _sumPreIncomeOperList = new BalanceCompSummary
            {
                Module = "Venituri in avans",
                Summary = _preIncomeOperListCount == 0 ? "Nu exista operatii in gestiunea veniturilor in avans care nu au fost generate in Contabilitate"
                          : ("Exista operatii in gestiunea veniturilor in avans care nu au fost generate in Contabilitate. Rulati contarile automate"),
                Ok = (_preIncomeOperListCount == 0)
            };
            ret.Add(_sumPreIncomeOperList);

            // calcul gestiune
            var _summaryPreIncomeGest = SummaryPrepaymentGest(maxBalanceDate.AddDays(1), compDate, appClientId, PrepaymentType.VenituriInAvans);
            ret.Add(_summaryPreIncomeGest);

            // Caut in toate casieriile
            var checkDiffSold = 0;
            var text = String.Empty;
            var monedeList = _accountRepository.GetAllIncluding(f => f.Currency).Where(f => f.TenantId == appClientId && f.Status == State.Active && f.AccountFuncType == AccountFuncType.Casierie)
                                                                                .Select(f => f.CurrencyId)
                                                                                .Distinct()
                                                                                .ToList();
            var accountsList = _accountRepository.GetAllIncluding(f => f.Currency)
                                                 .Where(f => f.TenantId == appClientId && f.Status == State.Active && f.AccountFuncType == AccountFuncType.Casierie && monedeList.Contains(f.CurrencyId)
                                                        && f.ComputingAccount)
                                                 .ToList();

            foreach (var account in accountsList)
            {
                var currency = _currencyRepository.FirstOrDefault(f => f.Id == account.CurrencyId);
                var soldContaItem = _balanceRepository.GetSoldTypeAccount(compDate, account.Id, appClientId, account.CurrencyId, localCurrencyId, false, null);

                var soldConta = (soldContaItem.TipSold == "D" ? 1 : -1) * soldContaItem.Sold;
                var soldOperational = _dispositionRepository.SoldPrec(compDate, account.CurrencyId);
               
                if (soldConta == soldOperational)
                {
                    checkDiffSold = 0;
                }else
                {
                    checkDiffSold++;
                    text += "Casieria " + currency.CurrencyName + " are sold in contabilitate " + soldConta + ", iar in operational " + soldOperational + "; ";
                }

            }

            var _sumCasieriiList = new BalanceCompSummary
            {
                Module = "Sold casierii",
                Summary = checkDiffSold == 0 ? "Nu exista diferente intre solduri" : text,
                Ok = (checkDiffSold == 0)
            };
            ret.Add(_sumCasieriiList);
            return ret;
        }

        private BalanceCompSummary SummaryAssetGest(DateTime startDate, DateTime endDate, int appClientId)
        {
            var ret = new BalanceCompSummary();
            bool ok = true;
            var assetCount = _imoAssetRepository.GetAll().Where(f => f.TenantId == appClientId && f.UseStartDate > startDate
                                                                                       && f.UseStartDate <= endDate
                                                                                       && f.State == State.Active
                                                                                       && f.Processed == false).Count();
            if (assetCount != 0) ok = false;
            var operCount = _imoAssetRepository.GetAll().Where(f => f.TenantId == appClientId && f.OperationDate > startDate
                                                                                       && f.OperationDate <= endDate
                                                                                       && f.State == State.Active
                                                                                       && f.Processed == false).Count();
            if (operCount != 0) ok = false;
            if (endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
            {
                var assetDeprecIds = _imoAssetStockRepository.GetAll().Where(f => f.TenantId == appClientId)
                                                          .GroupBy(f => f.ImoAssetItemId)
                                                          .Select(f => f.Max(x => x.Id))
                                                          .ToList();
                var assetDeprecList = _imoAssetStockRepository.GetAll().Where(f => assetDeprecIds.Contains(f.Id))
                                                           .Where(f => f.Duration != 0)
                                                           .GroupBy(f => f.ImoAssetItemId)
                                                           .Select(f => new { MaxDate = f.Max(x => x.StockDate), Count = f.Count() })
                                                           .ToList();
                if (assetDeprecList.Count != 0)
                {
                    if (assetDeprecList.FirstOrDefault().MaxDate < endDate && assetDeprecList.FirstOrDefault().Count != 0)
                    {
                        var deprecCount = _imoAssetStockRepository.GetAll().Where(f => f.TenantId == appClientId && f.StockDate > startDate && f.StockDate <= endDate
                                                                                                     && f.OperType == ImoAssetOperType.AmortizareLunara).Count();
                        if (deprecCount == 0)
                        {
                            ok = false;
                        }
                    }
                }
            }

            ret.Module = "Mijloace Fixe";
            if (!ok)
            {
                ret.Summary = "Nu ati calcular gestiunea mijloacelor fixe";
                ret.Ok = false;
            }
            else
            {
                ret.Summary = "Gestiunea mijloacelor fixe a fost calculata";
                ret.Ok = true;
            }

            return ret;

        }
        private BalanceCompSummary SummaryPrepaymentGest(DateTime startDate, DateTime endDate, int appClientId, PrepaymentType prepaymentType)
        {
            var ret = new BalanceCompSummary();
            bool ok = true;

            var prepCount = _prepaymentRepository.GetAll().Where(f => f.TenantId == appClientId && f.PaymentDate > startDate
                                                                                    && f.PaymentDate <= endDate
                                                                                    && f.State == State.Active
                                                                                    && f.Processed == false
                                                                                    && f.PrepaymentType == prepaymentType).Count();
            if (prepCount != 0) ok = false;
            if (endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
            {
                var prepDeprecIds = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                             .GroupBy(f => f.PrepaymentId)
                                                             .Select(f => f.Max(x => x.Id))
                                                             .ToList();
                var prepDeprecList = _prepaymentBalanceRepository.GetAll().Where(f => prepDeprecIds.Contains(f.Id))
                                                              .Where(f => f.Duration != 0)
                                                              .GroupBy(f => f.PrepaymentId)
                                                              .Select(f => new { MaxDate = f.Max(x => x.ComputeDate), Count = f.Count() })
                                                              .ToList();
                if (prepDeprecList.Count != 0)
                {
                    if (prepDeprecList.FirstOrDefault().MaxDate < endDate && prepDeprecList.FirstOrDefault().Count != 0)
                    {
                        var deprecCount = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                                    .Where(f => f.TenantId == appClientId &&
                                                                            f.ComputeDate > startDate &&
                                                                            f.ComputeDate <= endDate &&
                                                                            f.OperType == PrepaymentOperType.AmortizareLunara &&
                                                                            f.Prepayment.PrepaymentType == prepaymentType)
                                                                    .Count();
                        if (deprecCount == 0)
                        {
                            ok = false;
                        }
                    }
                }
            }

            ret.Module = (prepaymentType == PrepaymentType.CheltuieliInAvans) ? "Cheltuieli in avans" : "Venituri in avans";
            if (!ok)
            {
                ret.Summary = "Nu ati calculat gestiunea";
                ret.Ok = false;
            }
            else
            {
                ret.Summary = "Gestiunea a fost calculata";
                ret.Ok = true;
            }

            return ret;
        }

        public List<BalanceCompValid> BalanceCompValidList(int balanceId, int appClientId)
        {
            var ret = new List<BalanceCompValid>();
            var balanceDate = _balanceRepository.GetAll().FirstOrDefault(f => f.Id == balanceId).BalanceDate;

            var localCurrencyId = _personRepository.GetLocalCurrency(appClientId);
            var balanceDetails = _balanceDetailsRepository.GetAll().Where(f => f.BalanceId == balanceId && f.CurrencyId == localCurrencyId);

            var asset = AssetBalanceValid(balanceDetails, balanceDate, appClientId);
            foreach (var item in asset)
            {
                ret.Add(item);
            }

            var expensePrep = PrepaymentBalanceValid(balanceDetails, balanceDate, appClientId, PrepaymentType.CheltuieliInAvans);
            foreach (var item in expensePrep)
            {
                ret.Add(item);
            }

            var incomePrep = PrepaymentBalanceValid(balanceDetails, balanceDate, appClientId, PrepaymentType.VenituriInAvans);
            foreach (var item in incomePrep)
            {
                ret.Add(item);
            }

            return ret;
        }

        private List<BalanceCompValid> AssetBalanceValid(IQueryable<BalanceDetails> balanceDetails, DateTime balanceDate, int appClientId)
        {
            var ret = new List<BalanceCompValid>();
            var assetIds = _imoAssetStockRepository.GetAll().Where(f => f.TenantId == appClientId && f.StockDate <= balanceDate)
                                                .GroupBy(f => f.ImoAssetItemId)
                                                .Select(f => f.Max(x => x.Id))
                                                .ToList();

            var assetList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem)
                                                 .Where(f => assetIds.Contains(f.Id) && f.Quantity != 0)
                                                 .ToList();
            decimal valBalInv = 0, valBalDeprec = 0, valGestInv = 0, valGestDeprec = 0;

            // conturi mijloace fixe
            var assetAccountList = assetList.GroupBy(f => new { f.ImoAssetItem.AssetAccountId, f.ImoAssetItem.AssetAccountInUseId })
                               .ToList()
                               .Select(f => f.Key.AssetAccountInUseId ?? f.Key.AssetAccountId).Distinct().ToList();
            //var assetPunereInFunctiuneAccountList = assetList.GroupBy(f => f.ImoAssetItem.AssetAccountInUseId)
            //                   .Select(f => f.Key).ToList();
            //assetAccountList.AddRange(assetPunereInFunctiuneAccountList);

            foreach (var assetAccountItem in assetAccountList)
            {
                var sum = assetList.Where(f => (f.ImoAssetItem.AssetAccountInUseId ?? f.ImoAssetItem.AssetAccountId) == assetAccountItem).Sum(f => f.InventoryValue + f.Deprec);
                valGestInv += sum;

                var balInv = balanceDetails.FirstOrDefault(f => f.AccountId == assetAccountItem);
                if (balInv != null)
                {
                    valBalInv += balInv.DbValueF - balInv.CrValueF;
                }
            }

            var itemAsset = new BalanceCompValid
            {
                Module = "Mijloace fixe",
                Element = "Valoare evaluata",
                BalanceValue = valBalInv,
                GestValue = valGestInv,
                Ok = (valBalInv == valGestInv)
            };
            ret.Add(itemAsset);

            // conturi amortizare
            var deprecAccountlist = assetList.GroupBy(f => f.ImoAssetItem.DepreciationAccountId)
                                              .Select(f => f.Key);
            foreach (var assetDeprecItem in deprecAccountlist)
            {
                var sum = assetList.Where(f => f.ImoAssetItem.DepreciationAccountId == assetDeprecItem).Sum(f => f.Deprec);
                valGestDeprec += sum;

                var balInv = balanceDetails.FirstOrDefault(f => f.AccountId == assetDeprecItem);
                if (balInv != null)
                {
                    valBalDeprec += balInv.CrValueF - balInv.DbValueF;
                }
            }
            var itemDeprec = new BalanceCompValid
            {
                Module = "Mijloace fixe",
                Element = "Amortizare",
                BalanceValue = valBalDeprec,
                GestValue = valGestDeprec,
                Ok = (valBalDeprec == valGestDeprec)
            };
            ret.Add(itemDeprec);

            return ret;
        }
        private List<BalanceCompValid> PrepaymentBalanceValid(IQueryable<BalanceDetails> balanceDetails, DateTime balanceDate, int appClientId, PrepaymentType prepaymentType)
        {
            var ret = new List<BalanceCompValid>();
            var prepaymentsIds = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                          .Where(f => f.TenantId == appClientId && f.ComputeDate <= balanceDate && f.Prepayment.PrepaymentType == prepaymentType)
                                                          .GroupBy(f => f.PrepaymentId)
                                                          .Select(f => f.Max(x => x.Id))
                                                          .ToList();
            var prepaymentList = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                          .Where(f => prepaymentsIds.Contains(f.Id) && f.Quantity != 0)
                                                          .ToList();
            decimal valBalInv = 0, valGestInv = 0;
            decimal valBalInvVat = 0, valGestInvVat = 0;

            // conturi sume in avans
            var prepaymentAccountList = prepaymentList.GroupBy(f => f.Prepayment.PrepaymentAccountId)
                                                      .Select(f => f.Key);

            foreach (var prepaymentAccountItem in prepaymentAccountList)
            {
                var sum = prepaymentList.Where(f => f.Prepayment.PrepaymentAccountId == prepaymentAccountItem).Sum(f => f.PrepaymentValue);
                valGestInv += sum;

                var balInv = balanceDetails.FirstOrDefault(f => f.AccountId == prepaymentAccountItem);
                if (balInv != null)
                {
                    valBalInv += balInv.DbValueF - balInv.CrValueF;
                }
            }

            var itemPrepayment = new BalanceCompValid
            {
                Module = (prepaymentType == PrepaymentType.CheltuieliInAvans) ? "Cheltuieli in avans" : "Venituri in avans",
                Element = "Valoare evaluata",
                BalanceValue = valBalInv,
                GestValue = valGestInv,
                Ok = (valBalInv == valGestInv)
            };
            ret.Add(itemPrepayment);

            // conturi sume in avans TVA
            var prepaymentVatAccountList = prepaymentList.GroupBy(f => f.Prepayment.PrepaymentAccountVATId)
                                                      .Select(f => f.Key);

            foreach (var prepaymentVatAccountItem in prepaymentVatAccountList)
            {
                var sum = prepaymentList.Where(f => f.Prepayment.PrepaymentAccountVATId == prepaymentVatAccountItem).Sum(f => f.PrepaymentVAT);
                valGestInvVat += sum;

                var balInv = balanceDetails.FirstOrDefault(f => f.AccountId == prepaymentVatAccountItem);
                if (balInv != null)
                {
                    valBalInvVat += balInv.DbValueF - balInv.CrValueF;
                }
            }

            var itemPrepaymentVat = new BalanceCompValid
            {
                Module = (prepaymentType == PrepaymentType.CheltuieliInAvans) ? "Cheltuieli in avans" : "Venituri in avans",
                Element = "Valoare evaluata TVA",
                BalanceValue = valBalInvVat,
                GestValue = valGestInvVat,
                Ok = (valBalInvVat == valGestInvVat)
            };
            ret.Add(itemPrepaymentVat);

            return ret;
        }

    }
}
