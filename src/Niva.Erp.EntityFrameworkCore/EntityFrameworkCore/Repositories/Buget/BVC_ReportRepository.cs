using Abp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_ReportRepository : ErpRepositoryBase<BVC_FormRand, int> , IBVC_ReportRepository
    {
        public BVC_ReportRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public List<BugetRaportareDetails> GetBugetRaportareDetails(int AnBVC)
        {

            var FormularId = Context.BVC_Formular.Where(f => f.AnBVC == AnBVC).FirstOrDefault().Id;
            


            var query = from r in Context.BVC_FormRand
                        join d in Context.BVC_FormRandDetails on r.Id equals d.FormRandId
                        join p in Context.BVC_PAAP on d.TipRandCheltuialaId equals p.InvoiceElementsDetailsId
                        join t in Context.BVC_PAAPTranse on p.Id equals t.BVC_PAAPId
                        where r.FormularId == FormularId && t.DataTransa >= new DateTime(AnBVC, 1, 1) && t.DataTransa <= new DateTime(AnBVC, 12, 31)


                        select (new BugetRaportareDetails
                        {
                            IdRand = r.Id,
                            IdPaap = p.Id,
                            IdTransa = t.Id,
                            DenumireRand = r.Descriere,
                            Descriere = p.Descriere,
                            OrderView = r.OrderView,
                            ValoareLei = t.ValoareLei,
                            Data = LazyMethods.LastDayOfMonth(t.DataTransa),
                            Bold = false,
                            StarePaaP = Context.BVC_PAAP_State.Where(f => f.State == State.Active && f.BVC_PAAP_Id == p.Id).OrderByDescending(f => f.Id).Select(f => f.Paap_State).First().ToString(),

        }
                        ) ;

            var list = query.OrderBy(f => f.OrderView).ThenBy(f => f.IdRand).ToList();

            var ret = new List<BugetRaportareDetails>();
            int orderView = 1;

            //foreach (var item in list.GroupBy(f => new { f.IdRand, f.DenumireRand, f.OrderView })
            //                        .OrderBy(f => f.Key.OrderView)
            //                        .Select(f => new { f.Key.IdRand, f.Key.DenumireRand, f.Key.OrderView }))
            //{
            //    for (int i = 1; i <= 12; i++)
            //    {
            //        var data = LazyMethods.LastDayOfMonth(new DateTime(AnBVC, i, 1));
            //        var valoareTotalLei = list.Where(f => f.IdRand == item.IdRand && f.Data == data).Sum(f => f.ValoareLei);

            //        ret.Add(new BugetRaportareDetails
            //        {
            //            DenumireRand = item.DenumireRand,
            //            Descriere = item.DenumireRand,
            //            Data = data,
            //            OrderView = item.OrderView,
            //            OrderViewDet = orderView,
            //            ValoareLei = valoareTotalLei,
            //            Bold = true
            //        });

            //       // orderView++;

            //        foreach (var detail in list.Where(f => f.IdRand == item.IdRand && f.Data == data).OrderBy(f => f.Descriere))
            //        {
            //            ret.Add(new BugetRaportareDetails
            //            {
            //                DenumireRand = detail.DenumireRand,
            //                Descriere = detail.Descriere,
            //                Data = detail.Data,
            //                OrderView = item.OrderView+1,
            //                OrderViewDet = orderView,
            //                ValoareLei = detail.ValoareLei,
            //                Bold = false
            //            });
            //            orderView++;
            //        }
            //    }
            //}

            int decalare = 0;
            var verificat = "";
            var lastId = -1;

            foreach (var item in list)
            {
                if (verificat != item.DenumireRand)
                {
                    
                    for (int i = 1; i <= 12; i++)
                    {
                        var data = LazyMethods.LastDayOfMonth(new DateTime(AnBVC, i, 1));
                        var valoareTotalLei = list.Where(f => f.IdRand == item.IdRand && f.Data == data).Sum(f => f.ValoareLei);

                        ret.Add(new BugetRaportareDetails
                        {
                            DenumireRand = item.DenumireRand,
                            Descriere = item.DenumireRand,
                            Data = data,
                            OrderView = item.OrderView + decalare,
                            OrderViewDet = orderView,
                            ValoareLei = valoareTotalLei,
                            Bold = true
                        });
                        
                    }
                    
                    verificat = item.DenumireRand;
                    lastId = -1;
                }
                
                if (lastId != item.IdRand)
                {
                    lastId = item.IdRand;
                    decalare++;
                }

                ret.Add(new BugetRaportareDetails
                {
                    DenumireRand = item.DenumireRand,
                    Descriere = item.Descriere,
                    Data = item.Data,
                    OrderView = item.OrderView + decalare,
                    OrderViewDet = orderView,
                    ValoareLei = item.ValoareLei,
                    Bold = false,
                    StarePaaP = item.StarePaaP
                });

            }

                ret = ret.OrderBy(f => f.OrderView).ThenBy(f => f.OrderViewDet).ToList();

            // TOTAL
            for (int i = 1; i <= 12; i++)
            {

                var data = LazyMethods.LastDayOfMonth(new DateTime(AnBVC, i, 1));
                var Valoare = ret.Where(f => f.Data == data && f.DenumireRand != f.Descriere).Sum(f => f.ValoareLei);

                ret.Add(new BugetRaportareDetails
                    {
                        DenumireRand = "TOTAL",
                        Descriere = "TOTAL",
                        Data = data,
                        OrderView = int.MaxValue,
                        OrderViewDet = int.MaxValue,
                        ValoareLei = Valoare,
                        Bold = true
                    });
                    
                
            }

            return ret.ToList();
        }


        //public class BugetRaportareDetails
        //{

        //    public string DenumireRand { get; set; }
        //    public int OrderView { get; set; }
        //    public string Descriere { get; set; }
        //    public DateTime Data { get; set; }
        //    public decimal ValoareLei { get; set; }

        //}
    }
}
