using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.ConfigurareRapoarte.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.ConfigurareRapoarte;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.Conta.ConfigurareRapoarte
{
    public interface IReportConfigAppService : IApplicationService
    {
        List<RepConfigInitDto> ReportInitList();
        RepConfigEditDto EditRep(int? repId);
        void Delete(int repId);
        void SaveReport(RepConfigEditDto repDto);

        ReportConfigForm InitForm(int repConfigId);
        ReportConfigForm Save(ReportConfigForm form, int index);
        ReportConfigForm ReportDelete(ReportConfigForm form, int reportId);

        ReportConfigFormulaForm FormulaInit(int repConfigId, int reportId);
        ReportConfigFormulaForm FormulaList(ReportConfigFormulaForm form);
        ReportConfigFormulaForm RepFormulaAddRow(ReportConfigFormulaForm form);
        ReportConfigFormulaForm RepFormulaSave(ReportConfigFormulaForm form);

        List<CalcRapListDto> CalcReportList(); // generez lista cu numele rapoartelor
    }

    public class ReportConfigAppService : ErpAppServiceBase, IReportConfigAppService
    {
        IRepository<ReportInit> _reportInitRepository;
        IRepository<Report> _reportRepository;
        IConfigReportRepository _reportConfigRepository;

        public ReportConfigAppService(IRepository<ReportInit> reportInitRepository, IRepository<Report> reportRepository, IConfigReportRepository reportConfigRepository)
        {
            _reportInitRepository = reportInitRepository;
            _reportRepository = reportRepository;
            _reportConfigRepository = reportConfigRepository;
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public void Delete(int repId)
        {
            var report = _reportInitRepository.Get(repId);
            report.State = State.Inactive;
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public RepConfigEditDto EditRep(int? repId)
        {
            ReportInit report;
            if (repId == 0)
            {
                report = new ReportInit
                {
                    State = State.Active,
                    RapDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                };
            }
            else
            {
                report = _reportInitRepository.FirstOrDefault(f => f.Id == repId);
            }
            var reportDto = ObjectMapper.Map<RepConfigEditDto>(report);

            return reportDto;
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigForm InitForm(int repConfigId)
        {
            var form = _reportInitRepository.FirstOrDefault(f => f.Id == repConfigId);

            var ret = new ReportConfigForm
            {
                RepConfigId = repConfigId,
                RepDate = form.RapDate
            };

            ret = ReportList(ret);
            return ret;
        }
    
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        private ReportConfigForm ReportList(ReportConfigForm ret)
        {
            var reportList = _reportRepository.GetAll().Where(f => f.ReportInitId == ret.RepConfigId && f.State == State.Active).ToList();
            var _reportList = ObjectMapper.Map<List<RepConfigDto>>(reportList);

            var report = new RepConfigDto
            {
                RepConfigId = ret.RepConfigId
            };

            _reportList.Add(report);

            ret.ReportList = _reportList;
            return ret;
        }
       
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public List<RepConfigInitDto> ReportInitList()
        {
            var list = _reportInitRepository.GetAll()
                                                  .Where(f => f.State == State.Active)
                                                  .OrderByDescending(f => f.RapDate)
                                                  .ToList()
                                                  .Select(f => new RepConfigInitDto
                                                  {
                                                      Id = f.Id,
                                                      RapDate = LazyMethods.DateToString(f.RapDate)
                                                  })
                                                  .ToList();

            return list;
        }

        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public void SaveReport(RepConfigEditDto repDto)
        {
            try
            {
                var report = new ReportInit
                {
                    Id = repDto.Id,
                    RapDate = repDto.RapDate,
                    State = State.Active
                };

                if (report.Id != 0)
                {
                    report.TenantId = GetCurrentTenant().Id;
                }
                _reportInitRepository.InsertOrUpdate(report);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Operatia nu poate fi inregistrata");
            }
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigForm Save(ReportConfigForm form, int index)
        {
            //validare form
            if (form.ReportList[index].ReportName == null)
            {
                throw new UserFriendlyException("Eroare", "Toate campurile sunt obligatorii");
            }

            try
            {
                var report = new Report
                {
                    Id = form.ReportList[index].Id,
                    ReportName = form.ReportList[index].ReportName,
                    ReportSymbol = form.ReportList[index].ReportSymbol,
                    ReportInitId = form.ReportList[index].RepConfigId,
                    State = State.Active
                };

                if (form.ReportList[index].Id != 0)
                {
                    report.TenantId = GetCurrentTenant().Id;
                }

                _reportRepository.InsertOrUpdate(report);
                CurrentUnitOfWork.SaveChanges();

                form = ReportList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigForm ReportDelete(ReportConfigForm form, int reportId)
        {
            try
            {
                var report = _reportRepository.FirstOrDefault(f => f.Id == reportId);
                report.State = State.Inactive;

                _reportRepository.Update(report);
                CurrentUnitOfWork.SaveChanges();

                form = ReportList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigFormulaForm FormulaInit(int repConfigId, int reportId)
        {
            var report = _reportRepository.FirstOrDefault(f => f.Id == reportId);

            var ret = new ReportConfigFormulaForm
            {
                RepConfigId = repConfigId,
                ReportId = reportId,
                ReportName = report.ReportName,
                ReportSymbol = report.ReportSymbol,
            };

            ret = FormulaList(ret);

            return ret;
        }

        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigFormulaForm FormulaList(ReportConfigFormulaForm form)
        {
            var formulaList = _reportConfigRepository.GetAll().Where(f => f.ReportId == form.ReportId)
                                                          .OrderBy(f => f.OrderView)
                                                          .ToList();
            var _formulaList = ObjectMapper.Map<List<ConfigFormulaDto>>(formulaList);
            form.ConfigFormulaList = _formulaList;

            if (_formulaList.Count == 0)
            {
                form = RepFormulaAddRow(form);
            }

            return form;
        }
     
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigFormulaForm RepFormulaAddRow(ReportConfigFormulaForm form)
        {
            decimal rowCode = 1;
            int orderView = 1;
            try
            {
                if (form.ConfigFormulaList.Count == 0)
                {
                    rowCode = _reportConfigRepository.GetAllIncluding(f => f.Report)
                                                          .Where(f => f.Report.ReportInitId == form.RepConfigId)
                                                          .Max(f => f.RowCode);
                }
                else
                {
                    rowCode = form.ConfigFormulaList.Max(f => f.RowCode);
                }
                rowCode++;
            }
            catch
            {

            }
            try
            {
                if (form.ConfigFormulaList.Count == 0)
                {
                    orderView = _reportConfigRepository.GetAll().Where(f => f.ReportId == form.ReportId).Max(f => f.OrderView);
                }
                else
                {
                    orderView = form.ConfigFormulaList.Max(f => f.OrderView);
                }
                orderView++;
            }
            catch
            {

            }
            var report = new ConfigFormulaDto
            {
                RowCode = rowCode,
                OrderView = orderView
            };
            form.ConfigFormulaList.Add(report);

            return form;
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public ReportConfigFormulaForm RepFormulaSave(ReportConfigFormulaForm form)
        {
            try
            {
                var oldList = _reportConfigRepository.GetAll().Where(f => f.ReportId == form.ReportId);
                foreach (var item in oldList)
                {
                    _reportConfigRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();

                // adaug detaliile noi
                foreach (var item in form.ConfigFormulaList)
                {
                    var _item = ObjectMapper.Map<ReportConfig>(item);
                    _item.ReportId = form.ReportId;
                    _item.Id = 0;
                    _reportConfigRepository.Insert(_item);
                }
                CurrentUnitOfWork.SaveChanges();

                form = FormulaList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
       
        //[AbpAuthorize("Admin.Conta.ConfigRapoarte.Acces")]
        public List<CalcRapListDto> CalcReportList()
        {
            var list = _reportRepository.GetAll().Where(f => f.State == State.Active)
                                                .Select(f => new CalcRapListDto
                                                {
                                                    ReportId = f.Id,
                                                    ReportName = f.ReportName
                                                }).Distinct()
                                                 .ToList();

            return list;

        }

    }
}