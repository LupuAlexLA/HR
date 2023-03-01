using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_ReportRepository : IRepository<BVC_FormRand, int>
    {
        List<BugetRaportareDetails> GetBugetRaportareDetails(int AnBVC);
    }

    public class BugetRaportareDetails
    {
        
        public int IdTransa { get; set; }
        public int IdPaap { get; set; }
        public int IdRand { get; set; }
        public string DenumireRand { get; set; }
        public int OrderView { get; set; }
        public string Descriere { get; set; }
        public DateTime Data { get; set; }
        public decimal ValoareLei { get; set; }
        public bool Bold { get; set; }
        public int OrderViewDet { get; set; }
        public string StarePaaP { get; set; }
    }
}
