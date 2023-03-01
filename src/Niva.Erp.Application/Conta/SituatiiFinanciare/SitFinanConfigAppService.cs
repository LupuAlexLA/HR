using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.SituatiiFinanciare
{
    public interface ISitFinanConfigAppService : IApplicationService
    {
        SitFinanForm SitFinanInitForm();

        SitFinanForm SitFinanList(SitFinanForm form);

        List<SitFinanDDDto> SitFinanListDD();

        SitFinanForm SitFinanEdit(SitFinanForm form, int sitFinanId);

        SitFinanForm SitFinanSave(SitFinanForm form);

        SitFinanForm SitFinanDelete(SitFinanForm form, int sitFinanId);

        SitFinanReportForm SitFinanReportInit(int sitFinanId);

        SitFinanReportForm SitFinanReportList(SitFinanReportForm form);

        SitFinanReportForm SitFinanReportSave(SitFinanReportForm form, int index);

        SitFinanReportForm SitFinanReportDelete(SitFinanReportForm form, int reportId);

        SitFinanFormuleForm SitFinanFormulaInit(int sitFinanId, int reportId);

        SitFinanFormuleForm SitFinanFormulaList(SitFinanFormuleForm form);

        SitFinanFormuleForm SitFinanFormulaAddRow(SitFinanFormuleForm form);

        SitFinanFormuleForm SitFinanFormulaSave(SitFinanFormuleForm form);

        SitFinanFormuleForm SitFinanColSave(SitFinanFormuleForm form);

        SitFinanRapConfigNoteDto SitFinanNoteInit(int sitFinanId, int reportId);

        SitFinanRapConfigNoteDto SitFinanNoteSave(SitFinanRapConfigNoteDto form);
        List<SitFinanRapFluxConfigDto> SitFinanRapFluxConfigList(int reportId);
    }

    public class SitFinanConfigAppService : ErpAppServiceBase, ISitFinanConfigAppService
    {
        ISitFinanCalcRepository _sitFinanCalcRepository;
        IRepository<SitFinan> _sitFinanRepository;
        IRepository<SitFinanRap> _sitFinanRapRepository;
        IRepository<SitFinanRapConfig> _sitFinanRapConfigRepository;
        IRepository<SitFinanRapConfigCol> _sitFinanRapConfigColRepository;
        IRepository<SitFinanRapConfigNote> _sitFinanRapConfigNoteRepository;
        IRepository<SitFinanRapFluxConfig> _sitFinanRapFluxConfigRepository;


        public SitFinanConfigAppService(ISitFinanCalcRepository sitFinanCalcRepository, IRepository<SitFinan> sitFinanRepository, IRepository<SitFinanRap> sitFinanRapRepository,
                                        IRepository<SitFinanRapConfig> sitFinanRapConfigRepository, IRepository<SitFinanRapConfigCol> sitFinanRapConfigColRepository,
                                        IRepository<SitFinanRapConfigNote> sitFinanRapConfigNoteRepository, IRepository<SitFinanRapFluxConfig> sitFinanRapFluxConfigRepository)
        {
            _sitFinanRepository = sitFinanRepository;
            _sitFinanCalcRepository = sitFinanCalcRepository;
            _sitFinanRapRepository = sitFinanRapRepository;
            _sitFinanRapConfigRepository = sitFinanRapConfigRepository;
            _sitFinanRapConfigColRepository = sitFinanRapConfigColRepository;
            _sitFinanRapConfigNoteRepository = sitFinanRapConfigNoteRepository;
            _sitFinanRapFluxConfigRepository = sitFinanRapFluxConfigRepository;
        }

        //[AbpAuthorize("Conta.SitFinan.Config.Acces")]
        public SitFinanForm SitFinanInitForm()
        {
            var ret = new SitFinanForm
            {
                ShowEdit = false,
                ShowList = true
            };
            ret = SitFinanList(ret);

            return ret;
        }

        public SitFinanForm SitFinanList(SitFinanForm form)
        {
            var sitFinanList = _sitFinanRepository.GetAll().Where(f => f.State == State.Active).OrderByDescending(f => f.RapDate);
            var _sitFinanList = ObjectMapper.Map<List<SitFinanDto>>(sitFinanList);
            form.SitFinanList = _sitFinanList;
            form.ShowList = true;
            form.ShowEdit = false;

            return form;
        }

        public List<SitFinanRapFluxConfigDto> SitFinanRapFluxConfigList(int reportId)
        {

            var sitFinanFluxList = _sitFinanRapFluxConfigRepository.GetAllIncluding(f => f.SitFinanRap)
                                                                   .Where(f => f.SitFinanRap.State == State.Active && f.SitFinanRapId == reportId)
                                                                   .ToList();

            var _sitFinanFluxList = ObjectMapper.Map<List<SitFinanRapFluxConfigDto>>(sitFinanFluxList);

            if (sitFinanFluxList.Where(f => f.SitFinanFluxRowType == SitFinanFluxRowType.ContCash).Count() == 0)
            {
                _sitFinanFluxList.Add(new SitFinanRapFluxConfigDto { Id = 0, SitFinanFluxRowType = SitFinanFluxRowType.ContCash }); // rand nou pentru conturi flux
            }

            if (sitFinanFluxList.Where(f => f.SitFinanFluxRowType == SitFinanFluxRowType.Exceptii).Count() == 0)
            {
                _sitFinanFluxList.Add(new SitFinanRapFluxConfigDto { Id = 0, SitFinanFluxRowType = SitFinanFluxRowType.Exceptii }); // rand nou pentru exceptii conturi flux
            }

            return _sitFinanFluxList;
        }


        public List<SitFinanDDDto> SitFinanListDD()
        {
            var sitFinanList = _sitFinanRepository.GetAll()
                                                  .Where(f => f.State == State.Active)
                                                  .OrderByDescending(f => f.RapDate)
                                                  .ToList()
                                                  .Select(f => new SitFinanDDDto
                                                  {
                                                      Id = f.Id,
                                                      RapDate = LazyMethods.DateToString(f.RapDate)
                                                  })
                                                  .ToList();


            return sitFinanList;
        }

        public SitFinanForm SitFinanEdit(SitFinanForm form, int sitFinanId)
        {
            SitFinan sitFinan;
            if (sitFinanId == 0)
            {
                sitFinan = new SitFinan
                {
                    State = State.Active,
                    RapDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                };
            }
            else
            {
                sitFinan = _sitFinanRepository.FirstOrDefault(f => f.Id == sitFinanId);
            }
            var sitFinanDto = ObjectMapper.Map<SitFinanConfigDto>(sitFinan);
            sitFinanDto.CopyReport = false;

            form.ShowEdit = true;
            form.ShowList = false;
            form.SitFinan = sitFinanDto;

            return form;
        }

        public SitFinanForm SitFinanSave(SitFinanForm form)
        {
            try
            {
                string action = (form.SitFinan.Id == 0 ? "A" : "M");
                var sitFinan = new SitFinan
                {
                    Id = form.SitFinan.Id,
                    RapDate = form.SitFinan.RapDate,
                    State = State.Active
                };

                if (form.SitFinan.Id != 0)
                {
                    sitFinan.TenantId = GetCurrentTenant().Id;
                }
                _sitFinanRepository.InsertOrUpdate(sitFinan);
                CurrentUnitOfWork.SaveChanges();

                if (action == "A")
                {
                    if (form.SitFinan.PrevDateId != null)
                    {
                        CopiereRaportare(form.SitFinan.PrevDateId.Value, sitFinan.Id);
                    }
                    else
                    {
                        //  throw new Exception("Nu ati selectat raportarea precedenta");
                    }
                }

                form = SitFinanList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private void CopiereRaportare(int sitFinanPrecId, int sitFinanId)
        {
            var sitFinanPrec = _sitFinanRepository.FirstOrDefault(f => f.Id == sitFinanPrecId);

            var rapoarteList = _sitFinanRapRepository.GetAll().Where(f => f.SitFinanId == sitFinanPrec.Id && f.State == State.Active).ToList();

            foreach (var rap in rapoarteList)
            {
                var rapNew = new SitFinanRap
                {
                    ReportName = rap.ReportName,
                    ReportSymbol = rap.ReportSymbol,
                    NrCol = rap.NrCol,
                    OrderView = rap.OrderView,
                    PerioadaEchivalenta = rap.PerioadaEchivalenta,
                    SitFinanId = sitFinanId,
                    State = State.Active
                };

                _sitFinanRapRepository.Insert(rapNew);
                CurrentUnitOfWork.SaveChanges();

                var rowList = _sitFinanRapConfigRepository.GetAll().Where(f => f.SitFinanRapId == rap.Id).ToList();
                foreach (var row in rowList)
                {
                    var rowNew = new SitFinanRapConfig
                    {
                        RowName = row.RowName,
                        RowCode = row.RowCode,
                        RowNr = row.RowNr,
                        RowNota = row.RowNota,
                        NegativeValue = row.NegativeValue,
                        DecimalNr = row.DecimalNr,
                        OrderView = row.OrderView,
                        Bold = row.Bold,
                        Col1 = row.Col1,
                        Col2 = row.Col2,
                        Col3 = row.Col3,
                        Col4 = row.Col4,
                        Col5 = row.Col5,
                        Col6 = row.Col6,
                        TotalRow = row.TotalRow,
                        SitFinanRapId = rapNew.Id
                    };

                    _sitFinanRapConfigRepository.Insert(rowNew);
                }
                CurrentUnitOfWork.SaveChanges();

                var rapCol = _sitFinanRapConfigColRepository.GetAll().Where(f => f.SitFinanRapId == rap.Id).ToList();
                foreach (var col in rapCol)
                {
                    var colNew = new SitFinanRapConfigCol
                    {
                        ColumnName = col.ColumnName,
                        ColumnNr = col.ColumnNr,
                        ColumnModCalc = col.ColumnModCalc,
                        SitFinanRapId = rapNew.Id
                    };
                    _sitFinanRapConfigColRepository.Insert(colNew);
                }
                CurrentUnitOfWork.SaveChanges();

                var rapNote = _sitFinanRapConfigNoteRepository.FirstOrDefault(f => f.SitFinanRapId == rap.Id);

                if (rapNote != null)
                {
                    var rapNoteNew = new SitFinanRapConfigNote
                    {
                        SitFinanRapId = rapNew.Id,
                        AfterNote = rapNote.AfterNote,
                        BeforeNote = rapNote.BeforeNote
                    };
                    _sitFinanRapConfigNoteRepository.Insert(rapNoteNew);
                    CurrentUnitOfWork.SaveChanges();
                }

            }
        }

        public SitFinanForm SitFinanDelete(SitFinanForm form, int sitFinanId)
        {
            try
            {
                var sitFinan = _sitFinanRepository.FirstOrDefault(f => f.Id == sitFinanId);
                sitFinan.State = State.Inactive;

                _sitFinanRepository.Update(sitFinan);
                CurrentUnitOfWork.SaveChanges();

                form = SitFinanList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanReportForm SitFinanReportInit(int sitFinanId)
        {
            var sitFinan = _sitFinanRepository.FirstOrDefault(f => f.Id == sitFinanId);

            var ret = new SitFinanReportForm
            {
                SitFinanDate = sitFinan.RapDate,
                SitFinanId = sitFinanId
            };

            ret = SitFinanReportList(ret);

            return ret;
        }

        public SitFinanReportForm SitFinanReportList(SitFinanReportForm form)
        {
            var reportList = _sitFinanRapRepository.GetAll().Where(f => f.SitFinanId == form.SitFinanId && f.State == State.Active)
                                                            .OrderBy(f => f.OrderView)
                                                            .ToList();
            var _reportList = ObjectMapper.Map<List<SitFinanRapDto>>(reportList);
            var maxOrderView = (_reportList.Count == 0) ? 0 : _reportList.Max(f => f.OrderView);
            maxOrderView++;

            var report = new SitFinanRapDto
            {
                NrCol = 2,
                OrderView = maxOrderView,
                PerioadaEchivalenta = false,
                SitFinanId = form.SitFinanId
            };
            _reportList.Add(report);

            form.SitFinanRapList = _reportList;

            return form;
        }

        public SitFinanReportForm SitFinanReportSave(SitFinanReportForm form, int index)
        {
            // validari
            if (form.SitFinanRapList[index].ReportName == null && form.SitFinanRapList[index].ReportSymbol == null)
            {
                throw new UserFriendlyException("Eroare", "Toate campurile sunt obligatorii");
            }

            var reportSymbol = form.SitFinanRapList[index].ReportSymbol;
            var reportId = form.SitFinanRapList[index].Id;

            var count = _sitFinanRapRepository.GetAll().Where(f => f.State == State.Active && f.ReportSymbol == reportSymbol && f.Id != reportId
                                                              && f.SitFinanId == form.SitFinanId).ToList().Count;
            if (count != 0)
            {
                throw new UserFriendlyException("Eroare", "Exista un alt raport cu acest simbol!");
            }

            try
            {
                var sitFinan = new SitFinanRap
                {
                    Id = form.SitFinanRapList[index].Id,
                    ReportName = form.SitFinanRapList[index].ReportName,
                    ReportSymbol = form.SitFinanRapList[index].ReportSymbol,
                    NrCol = form.SitFinanRapList[index].NrCol,
                    OrderView = form.SitFinanRapList[index].OrderView,
                    PerioadaEchivalenta = form.SitFinanRapList[index].PerioadaEchivalenta,
                    SitFinanId = form.SitFinanId,
                    State = State.Active,

                };

                if (form.SitFinanRapList[index].Id != 0)
                {
                    sitFinan.TenantId = GetCurrentTenant().Id;
                }

                _sitFinanRapRepository.InsertOrUpdate(sitFinan);
                CurrentUnitOfWork.SaveChanges();

                form = SitFinanReportList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public SitFinanReportForm SitFinanReportDelete(SitFinanReportForm form, int reportId)
        {
            try
            {
                var report = _sitFinanRapRepository.FirstOrDefault(f => f.Id == reportId);
                report.State = State.Inactive;

                _sitFinanRapRepository.Update(report);
                CurrentUnitOfWork.SaveChanges();

                form = SitFinanReportList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanFormuleForm SitFinanFormulaInit(int sitFinanId, int reportId)
        {
            var report = _sitFinanRapRepository.FirstOrDefault(f => f.Id == reportId);

            var ret = new SitFinanFormuleForm
            {
                SitFinanId = sitFinanId,
                ReportId = reportId,
                ReportName = report.ReportName,
                NrCol = report.NrCol,
                ReportSymbol = report.ReportSymbol
            };

            ret = SitFinanFormulaList(ret);
            ret = SitFinanColList(ret);

            return ret;
        }

        public SitFinanFormuleForm SitFinanFormulaList(SitFinanFormuleForm form)
        {
            var formulaList = _sitFinanRapConfigRepository.GetAll().Where(f => f.SitFinanRapId == form.ReportId)
                                                          .OrderBy(f => f.OrderView)
                                                          .ToList();
            var _formulaList = ObjectMapper.Map<List<SitFinanRapConfigDto>>(formulaList);
            form.ConfigFormulaList = _formulaList;

            if (_formulaList.Count == 0)
            {
                form = SitFinanFormulaAddRow(form);
            }

            return form;
        }

        public SitFinanFormuleForm SitFinanFormulaAddRow(SitFinanFormuleForm form)
        {
            decimal rowCode = 1;
            int orderView = 1;
            try
            {
                if (form.ConfigFormulaList.Count == 0)
                {
                    rowCode = _sitFinanRapConfigRepository.GetAllIncluding(f => f.SitFinanRap)
                                                          .Where(f => f.SitFinanRap.SitFinanId == form.SitFinanId)
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
                    orderView = _sitFinanRapConfigRepository.GetAll().Where(f => f.SitFinanRapId == form.ReportId).Max(f => f.OrderView);
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
            var report = new SitFinanRapConfigDto
            {
                RowCode = rowCode,
                OrderView = orderView,
                TotalRow = false,
                Bold = false,
                DecimalNr = 2,
                NegativeValue = true
            };
            form.ConfigFormulaList.Add(report);

            return form;
        }

        public SitFinanFormuleForm SitFinanFormulaSave(SitFinanFormuleForm form)
        {
            try
            {
                var appClient = GetCurrentTenant();

                var oldList = _sitFinanRapConfigRepository.GetAll().AsNoTracking().Where(f => f.SitFinanRapId == form.ReportId).ToList();
                //sterg randurile din Db, care nu mai sunt in interfata
                foreach (var item in oldList)
                {
                    var deletedItem = form.ConfigFormulaList.FirstOrDefault(f => f.Id == item.Id);
                    if (deletedItem == null)
                    {
                        _sitFinanRapConfigRepository.Delete(item.Id);
                    }
                }

                foreach (var item in form.ConfigFormulaList)
                {
                    var existingItem = oldList.FirstOrDefault(f => f.Id == item.Id);

                    if (existingItem != null)
                    {
                        var _item = ObjectMapper.Map<SitFinanRapConfig>(item);
                        _item.SitFinanRapId = form.ReportId;
                        _item.TenantId = appClient.Id;
                        _sitFinanRapConfigRepository.Update(_item);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        var _item = ObjectMapper.Map<SitFinanRapConfig>(item);
                        _item.SitFinanRapId = form.ReportId;
                        _item.Id = 0;
                        _item.TenantId = appClient.Id;
                        _sitFinanRapConfigRepository.Insert(_item);
                    }
                }



                CurrentUnitOfWork.SaveChanges();

                form = SitFinanFormulaList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public List<SitFinanRapFluxConfigDto> SitFinanFluxSave(List<SitFinanRapFluxConfigDto> Flux, int SitFinanRapId)
        {
            try
            {
                var appClient = GetCurrentTenant();

                var oldList = _sitFinanRapFluxConfigRepository.GetAll().AsNoTracking().Where(f => f.SitFinanRapId == SitFinanRapId).ToList();
                //sterg randurile din Db, care nu mai sunt in interfata
                foreach (var item in oldList)
                {
                    var deletedItem = Flux.FirstOrDefault(f => f.Id == item.Id);
                    if (deletedItem == null)
                    {
                        _sitFinanRapFluxConfigRepository.Delete(item.Id);
                    }
                }

                foreach (var item in Flux)
                {
                    var existingItem = oldList.FirstOrDefault(f => f.Id == item.Id);

                    if (existingItem != null)
                    {
                        var _item = ObjectMapper.Map<SitFinanRapFluxConfig>(item);

                        _item.TenantId = appClient.Id;
                        _sitFinanRapFluxConfigRepository.Update(_item);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        var _item = ObjectMapper.Map<SitFinanRapFluxConfig>(item);

                        _item.SitFinanRapId = SitFinanRapId;
                        _item.Id = 0;
                        _item.TenantId = appClient.Id;
                        _sitFinanRapFluxConfigRepository.Insert(_item);
                    }
                }



                CurrentUnitOfWork.SaveChanges();


                return SitFinanRapFluxConfigList(SitFinanRapId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public SitFinanFormuleForm SitFinanColList(SitFinanFormuleForm form)
        {
            var nrCol = form.NrCol;

            var list = new List<SitFinanRapConfigColDto>();

            for (int i = 1; i <= nrCol; i++)
            {
                var item = new SitFinanRapConfigColDto
                {
                    ColumnNr = i
                };

                var col = _sitFinanRapConfigColRepository.FirstOrDefault(f => f.SitFinanRapId == form.ReportId && f.ColumnNr == i);
                if (col != null)
                {
                    item.Id = col.Id;
                    item.ColumnName = col.ColumnName;
                    item.ColumnModCalc = col.ColumnModCalc;
                }
                list.Add(item);
            }

            form.ConfigColList = list;

            return form;
        }

        public SitFinanFormuleForm SitFinanColSave(SitFinanFormuleForm form)
        {
            try
            {
                foreach (var item in form.ConfigColList)
                {
                    var col = new SitFinanRapConfigCol
                    {
                        Id = item.Id,
                        ColumnNr = item.ColumnNr,
                        ColumnName = item.ColumnName,
                        ColumnModCalc = item.ColumnModCalc,
                        SitFinanRapId = form.ReportId
                    };

                    if (item.Id != 0)
                    {
                        col.TenantId = GetCurrentTenant().Id;
                    }

                    _sitFinanRapConfigColRepository.InsertOrUpdate(col);
                    CurrentUnitOfWork.SaveChanges();
                }

                form = SitFinanColList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SitFinanRapConfigNoteDto SitFinanNoteInit(int sitFinanId, int reportId)
        {
            var report = _sitFinanRapRepository.FirstOrDefault(f => f.Id == reportId);

            var nota = _sitFinanRapConfigNoteRepository.FirstOrDefault(f => f.SitFinanRapId == reportId);

            var ret = new SitFinanRapConfigNoteDto
            {
                SitFinanId = sitFinanId,
                ReportId = reportId,
                ReportName = report.ReportName,
                ReportSymbol = report.ReportSymbol
            };

            if (nota != null)
            {
                ret.Id = nota.Id;
                ret.BeforeNote = nota.BeforeNote;
                ret.AfterNote = nota.AfterNote;
            }

            return ret;
        }

        public SitFinanRapConfigNoteDto SitFinanNoteSave(SitFinanRapConfigNoteDto form)
        {
            try
            {
                var nota = new SitFinanRapConfigNote
                {
                    Id = form.Id,
                    SitFinanRapId = form.ReportId,
                    BeforeNote = form.BeforeNote,
                    AfterNote = form.AfterNote
                };

                if (form.Id != 0)
                {
                    nota.TenantId = GetCurrentTenant().Id;
                }

                _sitFinanRapConfigNoteRepository.InsertOrUpdate(nota);
                CurrentUnitOfWork.SaveChanges();

                form = SitFinanNoteInit(form.SitFinanId, form.ReportId);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
