using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Niva.Erp.Conta.Balance
{
    public class BalanceDDDto
    {
        public int Id { get; set; }

        public string BalanceDate { get; set; }
    }

    public class BalanceInitDto
    {
        public DateTime SearchStartDate { get; set; }

        public DateTime SearchEndDate { get; set; }

        public DateTime CalcDate { get; set; }

        public bool ClosingMonthOper { get; set; }

        public bool ShowComputeForm { get; set; }

        public List<BalanceListDto> BalanceList { get; set; }

        public int ShowForm { get; set; }

        public ViewBalanceDetailDto ViewBalanceDetail { get; set; }
    }

    public class BalanceListDto
    {
        public int Id { get; set; }

        public string BalanceDate { get; set; }

        public bool OkDelete { get; set; }
        public bool OkValid { get; set; }
    }

    public class ViewBalanceDetailDto
    {
        public int Id { get; set; }

        public int CurrencyId  { get; set; }
        public string BalanceDate { get; set; }

        public string SearchAccount { get; set; }

        public string BalanceTypeStr { get; set; }

        public BalanceType BalanceType { get; set; }

        public List<BalanceDetailDto> BalanceDetail { get; set; }
    }

    public class BalanceDetailDto
    {
        public decimal CrValueI { get; set; }

        [Display(Name = "Sold initial debitor")]
        public decimal DbValueI { get; set; }

        [Display(Name = "Rulaj luna credit")]
        public decimal CrValueM { get; set; }

        [Display(Name = "Rulaj luna debit")]
        public decimal DbValueM { get; set; }

        [Display(Name = "Rulaj an credit")]
        public decimal CrValueY { get; set; }

        [Display(Name = "Rulaj an debit")]
        public decimal DbValueY { get; set; }

        [Display(Name = "Sold final creditor")]
        public decimal CrValueF { get; set; }

        [Display(Name = "Sold final debitor")]
        public decimal DbValueF { get; set; }

        [Display(Name = "Rulaj precedent debitor")]
        public decimal DbValueP { get; set; }

        [Display(Name = "Rulaj precedent creditor")]
        public decimal CrValueP { get; set; }

        [Display(Name = "Total cumulat debitor")]
        public decimal DbValueT { get; set; }

        [Display(Name = "Total cumulat creditor")]
        public decimal CrValueT { get; set; }

        [Display(Name = "Total sume debitor")]
        public decimal DbValueSum { get; set; }

        [Display(Name = "Total sume creditor")]
        public decimal CrValueSum { get; set; }

        public int Id { get; set; }

        public string Symbol { get; set; }
        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public int AppClientId { get; set; }

        public bool TotalSum { get; set; }

        public string Synthetic { get; set; }

        public string Analythic { get; set; }

        public AccountTypes accountType { get; set; }
        public int? NivelRand { get; set; }
    }   

    public class BalanceCompSummaryDto
    {
        public string Module { get; set; }
        public string Summary { get; set; }
        public bool Ok { get; set; }
    }

    public class BalanceCompValidDto
    {
        public string Module { get; set; }
        public string Element { get; set; }
        public decimal BalanceValue { get; set; }
        public decimal GestValue { get; set; }
        public bool Ok { get; set; }
    }
}
