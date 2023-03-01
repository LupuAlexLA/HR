using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_PaapRepository: IRepository<BVC_PAAP, int>
    {
        void SaveAvailableSum(List<BVC_PAAP> list, int selectedYear);
        void UpdatePaapAvailableValues(BVC_PAAP paap);
        void DeleteAllPaapAvbSumForYear(int year);
        void InsertPAAPState(int paapId, DateTime operationDate, PAAP_State? state, string comentariu);
        void UpdatePaapTranse(BVC_PAAP paap, int? cotaTvaId);
    }
}