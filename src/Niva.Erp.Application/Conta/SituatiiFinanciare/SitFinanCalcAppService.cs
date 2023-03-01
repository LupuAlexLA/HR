using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.SituatiiFinanciare
{
    public interface ISitFinanCalcAppService : IApplicationService
    {
        SitFinanCalcForm CalcInit();

        SitFinanCalcForm CalcList(SitFinanCalcForm form);

        void DeleteCalcRapoarte(int balanceId);

        SitFinanCalcForm CalcRapoarte(SitFinanCalcForm form, int balanceId);

        SitFinanCalcReportForm CalcDetInit(int balanceId);

        SitFinanCalcReportForm CalcDetRap(SitFinanCalcReportForm form, int reportId);

        SitFinanCalcReportForm CalcRaport(SitFinanCalcReportForm form, int balanceId, int raportId);

        SitFinanCalcReportForm SaveValuesRecalc(SitFinanCalcReportForm form);

        SitFinanCalcReportForm ShowValueDetails(SitFinanCalcReportForm form, int rowId, int columnId);
        List<SitFinanCalcDetail> ShowReportDetails(int columnId, int reportId, int balanceId);

        SitFinanCalcReportForm ShowNotaDetail(SitFinanCalcReportForm form, int reportId);

        SitFinanCalcReportForm SaveNota(SitFinanCalcReportForm form);

        SitFinanCalcReportForm RecalcNota(SitFinanCalcReportForm form);
    }

    public class SitFinanCalcAppService : ErpAppServiceBase, ISitFinanCalcAppService
    {
        IRepository<SavedBalance> _savedBalanceRepository;
        ISitFinanCalcRepository _sitFinanCalcRepository;
        IRepository<SitFinanCalcDetails> _sitFinanCalcDetailsRepository;
        IRepository<SitFinanCalcFormulaDet> _sitFinanCalcFormulaDetRepository;
        IRepository<SitFinanCalcNote> _sitFinanCalcNoteRepository;
        IPlasamentLichiditateManager _plasamentLichiditateManager;
        ICurrencyRepository _currencyRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        IAngajatiExternManager _angajatiExternManager;

        public SitFinanCalcAppService(IRepository<SavedBalance> savedBalanceRepository, ISitFinanCalcRepository sitFinanCalcRepository, IRepository<SitFinanCalcDetails> sitFinanCalcDetailsRepository,
                                      IRepository<SitFinanCalcFormulaDet> sitFinanCalcFormulaDetRepository, IRepository<SitFinanCalcNote> sitFinanCalcNoteRepository, IPlasamentLichiditateManager plasamentLichiditateManager,
                                       ICurrencyRepository currencyRepository, IExchangeRatesRepository exchangeRatesRepository, IAngajatiExternManager angajatiExternManager)
        {
            _savedBalanceRepository = savedBalanceRepository;
            _sitFinanCalcRepository = sitFinanCalcRepository;
            _sitFinanCalcDetailsRepository = sitFinanCalcDetailsRepository;
            _sitFinanCalcFormulaDetRepository = sitFinanCalcFormulaDetRepository;
            _sitFinanCalcNoteRepository = sitFinanCalcNoteRepository;
            _plasamentLichiditateManager = plasamentLichiditateManager;
            _currencyRepository = currencyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _angajatiExternManager = angajatiExternManager;
        }

        //[AbpAuthorize("Conta.SitFinan.Calcul.Acces")]
        public SitFinanCalcForm CalcInit()
        {
            var _currentDate = LazyMethods.Now();
            var ret = new SitFinanCalcForm
            {
                IsDailyBalance = false,
                StartDate = _currentDate.AddYears(-3),
                EndDate = _currentDate
            };

            ret = CalcList(ret);
            return ret;
        }

        public SitFinanCalcForm CalcList(SitFinanCalcForm form)
        {
            var list = _savedBalanceRepository.GetAll()
                                              .Where(f => f.SaveDate >= form.StartDate && f.SaveDate <= form.EndDate && f.IsDaily == form.IsDailyBalance)
                                              .Select(f => new SitFinanBalanceList
                                              {
                                                  Id = f.Id,
                                                  BalanceName = f.BalanceName,
                                                  SaveDate = f.SaveDate
                                              })
                                              .OrderByDescending(f => f.SaveDate)
                                              .ToList();

            foreach (var item in list)
            {
                var count = _sitFinanCalcRepository.GetAll().Where(f => f.SavedBalanceId == item.Id).Count();
                item.CalcSitFinan = (count != 0);
            }

            form.BalanceList = list;
            return form;
        }

        public void DeleteCalcRapoarte(int balanceId)
        {
            try
            {
                _sitFinanCalcRepository.DeleteCalcRapoarte(balanceId);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanCalcForm CalcRapoarte(SitFinanCalcForm form, int balanceId)
        {
            try
            {
                var balance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balanceId);
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var currency = _currencyRepository.GetCurrencyById(localCurrencyId);
                var plasamenteList = _plasamentLichiditateManager.PlasamenteLichiditateList(balance.SaveDate);

                var plasamentCurrencyList = plasamenteList.Where(f => f.moneda != currency.CurrencyCode).Select(f => f.moneda).Distinct().ToList();

                foreach (var item in plasamentCurrencyList)
                {
                    var fromCurrency = _currencyRepository.GetByCode(item);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(balance.SaveDate, fromCurrency.Id, localCurrencyId);
                    foreach (var plasament in plasamenteList.Where(f => f.moneda == item))
                    {
                        plasament.valoareContabila = Math.Round(plasament.valoareContabila * exchangeRate, 2);
                        plasament.valoareCreanta = Math.Round(plasament.valoareCreanta * exchangeRate, 2);
                        plasament.valoareInvestita = Math.Round(plasament.valoareInvestita * exchangeRate, 2);
                        plasament.valoareDepreciere = Math.Round(plasament.valoareDepreciere * exchangeRate, 2);
                    }
                }

                var externalValues = SitFinanExternalValues(balance.SaveDate);

                _sitFinanCalcRepository.CalcRapoarte(balanceId, plasamenteList, externalValues);
                form = CalcList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanCalcReportForm CalcDetInit(int balanceId)
        {
            var balance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balanceId);

            var ret = new SitFinanCalcReportForm
            {
                BalanceId = balanceId,
                BalanceName = balance.BalanceName,
                SaveDate = balance.SaveDate,
                SelReportId = 0,
                ShowReport = false,
                ShowValueDetail = false,
                ShowNota = false
            };

            var reportList = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                                .Where(f => f.SavedBalanceId == balanceId)
                                                .OrderBy(f => f.SitFinanRapRow.SitFinanRap.OrderView)
                                                .Select(f => new SitFinanCalcReportList
                                                {
                                                    Id = f.SitFinanRapId,
                                                    ReportName = f.SitFinanRap.ReportName
                                                })
                                                .Distinct()
                                                .ToList();
            ret.ReportList = reportList;

            return ret;
        }

        public SitFinanCalcReportForm CalcDetRap(SitFinanCalcReportForm form, int reportId)
        {
            var report = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                                .FirstOrDefault(f => f.SavedBalanceId == form.BalanceId && f.SitFinanRapId == reportId);

            form.SelReportId = reportId;
            form.SelReportName = report.SitFinanRap.ReportName;
            form.SelReportNrCol = report.SitFinanRap.NrCol;

            var reportDetail = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                                      .Where(f => f.SavedBalanceId == form.BalanceId && f.SitFinanRapId == reportId)
                                                      .Select(f => new SitFinanCalcReportDetList
                                                      {
                                                          CalcRowId = f.Id,
                                                          RowId = f.SitFinanRapRowId,
                                                          RowName = f.SitFinanRapRow.RowName,
                                                          RowNr = f.SitFinanRapRow.RowNr,
                                                          RowNota = f.SitFinanRapRow.RowNota,
                                                          OrderView = f.SitFinanRapRow.OrderView,
                                                          TotalRow = f.SitFinanRapRow.TotalRow,
                                                          Bold = f.SitFinanRapRow.Bold,
                                                          NegativeValue = f.SitFinanRapRow.NegativeValue,
                                                          Val1 = f.Val1,
                                                          Val2 = f.Val2,
                                                          Val3 = f.Val3,
                                                          Val4 = f.Val4,
                                                          Val5 = f.Val5,
                                                          Val6 = f.Val6
                                                      })
                                                      .ToList()
                                                      .OrderBy(f => f.OrderView)
                                                      .ToList();
            form.ReportDetList = reportDetail;
            form.ShowReport = true;
            form.ShowNota = false;

            return form;
        }

        public SitFinanCalcReportForm CalcRaport(SitFinanCalcReportForm form, int balanceId, int raportId)
        {
            try
            {
                var balance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balanceId);
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var currency = _currencyRepository.GetCurrencyById(localCurrencyId);
                var plasamenteList = _plasamentLichiditateManager.PlasamenteLichiditateList(balance.SaveDate);

                var plasamentCurrencyList = plasamenteList.Where(f => f.moneda != currency.CurrencyCode).Select(f => f.moneda).Distinct().ToList();

                foreach (var item in plasamentCurrencyList)
                {
                    var fromCurrency = _currencyRepository.GetByCode(item);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(balance.SaveDate, fromCurrency.Id, localCurrencyId);
                    foreach (var plasament in plasamenteList.Where(f => f.moneda == item))
                    {
                        plasament.valoareContabila = Math.Round(plasament.valoareContabila * exchangeRate, 2);
                        plasament.valoareCreanta = Math.Round(plasament.valoareCreanta * exchangeRate, 2);
                        plasament.valoareInvestita = Math.Round(plasament.valoareInvestita * exchangeRate, 2);
                        plasament.valoareDepreciere = Math.Round(plasament.valoareDepreciere * exchangeRate, 2);
                    }
                }

                var externalValues = SitFinanExternalValues(balance.SaveDate);

                _sitFinanCalcRepository.CalcRaport(balanceId, raportId, plasamenteList, externalValues);
                form = CalcDetRap(form, raportId);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void CalcRaportBackEnd(int balanceId, int raportId)
        {
            try
            {
                var balance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balanceId);
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var currency = _currencyRepository.GetCurrencyById(localCurrencyId);
                var plasamenteList = _plasamentLichiditateManager.PlasamenteLichiditateList(balance.SaveDate);

                var plasamentCurrencyList = plasamenteList.Where(f => f.moneda != currency.CurrencyCode).Select(f => f.moneda).Distinct().ToList();

                foreach (var item in plasamentCurrencyList)
                {
                    var fromCurrency = _currencyRepository.GetByCode(item);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(balance.SaveDate, fromCurrency.Id, localCurrencyId);
                    foreach (var plasament in plasamenteList.Where(f => f.moneda == item))
                    {
                        plasament.valoareContabila = Math.Round(plasament.valoareContabila * exchangeRate, 2);
                        plasament.valoareCreanta = Math.Round(plasament.valoareCreanta * exchangeRate, 2);
                        plasament.valoareInvestita = Math.Round(plasament.valoareInvestita * exchangeRate, 2);
                        plasament.valoareDepreciere = Math.Round(plasament.valoareDepreciere * exchangeRate, 2);
                    }
                }

                var externalValues = SitFinanExternalValues(balance.SaveDate);

                _sitFinanCalcRepository.CalcRaport(balanceId, raportId, plasamenteList, externalValues);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanCalcReportForm SaveValuesRecalc(SitFinanCalcReportForm form)
        {
            try
            {
                foreach (var item in form.ReportDetList)
                {
                    var calcRow = _sitFinanCalcRepository.FirstOrDefault(f => f.Id == item.CalcRowId);
                    calcRow.Val1 = item.Val1;
                    calcRow.Val2 = item.Val2;
                    calcRow.Val3 = item.Val3;
                    calcRow.Val4 = item.Val4;
                    calcRow.Val5 = item.Val5;
                    calcRow.Val6 = item.Val6;
                }
                CurrentUnitOfWork.SaveChanges();

                _sitFinanCalcRepository.CalculRaportTotalOld(form.BalanceId, form.SelReportId);

                CalcDetRap(form, form.SelReportId);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public SitFinanCalcReportForm ShowValueDetails(SitFinanCalcReportForm form, int rowId, int columnId)
        {
            form.ShowReport = false;
            form.ShowValueDetail = true;

            var rowName = form.ReportDetList.FirstOrDefault(f => f.RowId == rowId).RowName;

            var calcVal = new SitFinanCalcVal
            {
                ColumnId = columnId,
                RowId = rowId,
                RowName = rowName
            };

            var formulaDet = _sitFinanCalcFormulaDetRepository.FirstOrDefault(f => f.SavedBalanceId == form.BalanceId && f.SitFinanRapRowId == rowId && f.ColumnId == columnId);

            if (formulaDet != null)
            {
                calcVal.Formula = formulaDet.Formula;
                calcVal.FormulaVal = formulaDet.FormulaVal;
            }

            var valDetailList = _sitFinanCalcDetailsRepository.GetAll().Where(f => f.SavedBalanceId == form.BalanceId && f.SitFinanRapRowId == rowId && f.ColumnId == columnId)
                                                              .Select(f => new SitFinanCalcValDet
                                                              {
                                                                  ElementDet = f.ElementDet,
                                                                  Val = f.Val
                                                              })
                                                              .OrderBy(f => f.ElementDet)
                                                              .ToList();
            calcVal.DetailValueList = valDetailList;
            form.ValueDetails = calcVal;

            return form;
        }

        public SitFinanCalcReportForm ShowNotaDetail(SitFinanCalcReportForm form, int reportId)
        {
            var report = _sitFinanCalcRepository.GetAllIncluding(f => f.SitFinanRapRow, f => f.SitFinanRapRow.SitFinanRap)
                                                .FirstOrDefault(f => f.SavedBalanceId == form.BalanceId && f.SitFinanRapId == reportId);

            form.SelReportId = reportId;
            form.SelReportName = report.SitFinanRap.ReportName;
            form.SelReportNrCol = report.SitFinanRap.NrCol;

            var nota = _sitFinanCalcNoteRepository.FirstOrDefault(f => f.SitFinanRapId == reportId && f.SavedBalanceId == form.BalanceId);
            SitFinanCalcDetNote notaDet;
            if (nota == null)
            {
                notaDet = new SitFinanCalcDetNote();
            }
            else
            {
                notaDet = new SitFinanCalcDetNote
                {
                    Id = nota.Id,
                    BeforeNote = nota.BeforeNote,
                    AfterNote = nota.AfterNote
                };
            }
            form.DetNote = notaDet;
            form.ShowReport = false;
            form.ShowValueDetail = false;
            form.ShowNota = true;

            return form;
        }

        public SitFinanCalcReportForm SaveNota(SitFinanCalcReportForm form)
        {
            var nota = new SitFinanCalcNote
            {
                Id = form.DetNote.Id,
                SavedBalanceId = form.BalanceId,
                SitFinanRapId = form.SelReportId,
                BeforeNote = form.DetNote.BeforeNote,
                AfterNote = form.DetNote.AfterNote
            };

            try
            {
                if (nota.Id != 0) // setez TenantId pentru nota pe EDIT
                {
                    nota.TenantId = GetCurrentTenant().Id;
                }

                _sitFinanCalcNoteRepository.InsertOrUpdate(nota);
                CurrentUnitOfWork.SaveChanges();
                form = ShowNotaDetail(form, form.SelReportId);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanCalcReportForm RecalcNota(SitFinanCalcReportForm form)
        {
            try
            {
                _sitFinanCalcRepository.CalcRaportNotaOld(form.BalanceId, form.SelReportId);

                form = ShowNotaDetail(form, form.SelReportId);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<SitFinanCalcDetail> ShowReportDetails(int columnId, int reportId, int balanceId)
        {
            try
            {
                var calcValueDet = new List<SitFinanCalcDetail>();

                var reportDetail = _sitFinanCalcRepository.GetSitFinanRapRows(columnId, balanceId, reportId);

                switch (columnId)
                {
                    case 1:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col1,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                    case 2:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col2,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                    case 3:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col3,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                    case 4:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col4,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                    case 5:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col5,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                    case 6:
                        calcValueDet.AddRange(reportDetail.Select(f => new SitFinanCalcDetail
                        {
                            ElementDet = f.ElementDet,
                            Formula = f.Col6,
                            RowId = f.Id,
                            RowName = f.RowName,
                            Valoare = f.Val,
                            OrderView = f.OrderView
                        }).OrderBy(f => f.OrderView).ToList());
                        break;
                }

                return calcValueDet;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private List<SitFinanExternalValues> SitFinanExternalValues(DateTime dataCalc)
        {
            var list = new List<SitFinanExternalValues>();

            var nrMedSalPer = _angajatiExternManager.NrMediuSalariatiPerioada(dataCalc);
            var nrMedSalPerItem = new SitFinanExternalValues
            {
                ValueType = SitFinanExternalValuesType.NrMediuSalariatiPerioada,
                Value = nrMedSalPer
            };
            list.Add(nrMedSalPerItem);

            var nrSalLuna = _angajatiExternManager.NrMediuSalariatiLuna(dataCalc);
            var nrSalLunaItem = new SitFinanExternalValues
            {
                ValueType = SitFinanExternalValuesType.NrSalariatiData,
                Value = nrSalLuna
            };
            list.Add(nrSalLunaItem);

            return list;
        }
    }

}
