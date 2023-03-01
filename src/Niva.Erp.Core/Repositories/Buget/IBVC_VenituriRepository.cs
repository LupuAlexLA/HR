using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_VenituriRepository : IRepository<BVC_VenitTitlu, int>
    {
        void AplicaBVCsiCashFlow(int bugetPrevId);
        void SaveVenitTitluParams(int formularBVCId, int monthStart, int monthEnd);
        decimal RetDobRefEstimatCuLista(List<BVC_DobandaReferinta> dobanziReferinta, BVC_PlasamentType TipPlasament, DateTime dataConstituire, int currencyId, int? activityTypeId);
        void CalculResurse(int bugetPrevBVCId, int bugetPrevCFId);
    }
}
