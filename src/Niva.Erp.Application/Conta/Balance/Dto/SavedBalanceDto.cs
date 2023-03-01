using Niva.Erp.ModelObjects;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Balance
{
    public class SavedBalanceDto
    {
        public DateTime SearchStartDate { get; set; }

        public DateTime SearchEndDate { get; set; }

        public bool IsDaily { get; set; }

        public bool ExternalSave { get; set; }

        public bool ShowSaveForm { get; set; }

        public List<SavedBalanceListDto> BalanceList { get; set; }

        public bool ShowBalanceDetails { get; set; }

        public ViewSavedBalanceDetailDto ViewBalanceDetail { get; set; }

        public SaveBalanceFormDto SavedBalanceForm { get; set; }
    }

    public class SaveBalanceFormDto
    {
        public DateTime DailyBalanceDate { get; set; }

        public int CalcBalanceId { get; set; }

        public string BalanceName { get; set; }

        public bool IsDaily { get; set; }
    }

    public class SavedBalanceListDto
    {
        public int Id { get; set; }

        public string BalanceDate { get; set; }

        public string BalanceName { get; set; }

        public bool OkDelete { get; set; }
    }

    public class ViewSavedBalanceDetailDto
    {
        public int Id { get; set; }

        public string BalanceDate { get; set; }

        public string BalanceName { get; set; }

        public string SearchAccount { get; set; }

        public string BalanceTypeStr { get; set; }
        public int? NivelRand { get; set; }

        public BalanceType BalanceType { get; set; }
        public int CurrencyId { get; set; }

        public List<BalanceDetailDto> BalanceDetail { get; set; }
    }

    public class SavedBalanceItemDto
    {
        public int Id { get; set; }

        public virtual string BalanceName { get; set; }

        public virtual DateTime SaveDate { get; set; }

        public virtual bool ExternalSave { get; set; }

        public virtual IList<SavedBalanceItemDetailsDto> SavedBalanceDetails { get; set; }
    }

    public class SavedBalanceItemDetailsDto
    {
        public string AccountName { get; set; }

        public string Symbol { get; set; }

        public virtual decimal CrValueF { get; set; }

        public virtual decimal CrValueI { get; set; }

        public virtual decimal CrValueM { get; set; }

        public virtual decimal CrValueY { get; set; }

        public virtual decimal DbValueF { get; set; }

        public virtual decimal DbValueI { get; set; }

        public virtual decimal DbValueM { get; set; }

        public virtual decimal DbValueY { get; set; }

        public int SavedBalanceId { get; set; }

        public int CurrencyId { get; set; }
    }
}
