using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Niva.Erp.ModelObjects
{
    public enum BalanceType
    {
        Synthetic,
        Analythic
    }

    public class BalanceView
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }

        public int Id { get; set; }

        public DateTime BalanceDate { get; set; }

        public string BalanceName { get; set; }

        public DateTime StartDate { get; set; }

        public BalanceType Type { get; set; }

        public decimal TotalCrI
        {
            get;set;
        }

        public decimal TotalDbI
        {
            get;set;
        }
        public decimal TotalCrM
        {
            get;set;
        }
        public decimal TotalDbM
        {
            get;set;
        }
        public decimal TotalCrY
        {
            get;set;
        }
        public decimal TotalDbY
        {
            get;set;
        }
        public decimal TotalCrF
        {
            get;set;
        }
        public decimal TotalDbF
        {
            get;set;
        }
        //Total rulaj precedent debitor
        public decimal TotalDbP { get; set; }
        //Total rulaj precedent creditor
        public decimal TotalCrP { get; set; }


        //total sume debitor
        public decimal TotalDbSum { get; set; }
        //total sume creditor
        public decimal TotalCrSum { get; set; }

        // total cumulat creditor
        public decimal TotalCrC { get; set; }
        //total cumulat debitor
        public decimal TotalDbC { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }
        public List<BalanceDetailsView> Details { get; set; }
    }

    public class BalanceDetailsView
    {
        [Display(Name = "Sold initial creditor")]
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
        public int? NivelRand { get; set; }

        public int CurrencyId { get; set; }

        public bool TotalSum { get; set; }

        public string Synthetic { get; set; }
        public string Analythic { get; set; }

        public AccountTypes accountType { get; set; }
        public bool IsSynthetic { get; set; }
        public int AccountId { get; set; }


    }

    public class SavedBalanceViewDto
    {
        public int Id { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime BalanceDate { get; set; }
        public string BalanceName { get; set; } 
        public string CurrencyName { get; set; }

        public decimal TotalCrI
        {
            get; set;
        }

        public decimal TotalDbI
        {
            get; set;
        }
        public decimal TotalCrM
        {
            get; set;
        }
        public decimal TotalDbM
        {
            get; set;
        }
        public decimal TotalCrY
        {
            get; set;
        }
        public decimal TotalDbY
        {
            get; set;
        }
        public decimal TotalCrF
        {
            get; set;
        }
        public decimal TotalDbF
        {
            get; set;
        }
        //Total rulaj precedent debitor
        public decimal TotalDbP { get; set; }
        //Total rulaj precedent creditor
        public decimal TotalCrP { get; set; }


        //total sume debitor
        public decimal TotalDbSum { get; set; }
        //total sume creditor
        public decimal TotalCrSum { get; set; }

        // total cumulat creditor
        public decimal TotalCrC { get; set; }
        //total cumulat debitor
        public decimal TotalDbC { get; set; }

        public List<BalanceDetailsView> Details { get; set; }
    }

    public class BalanceCurrencyView
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public BalanceView BalanceView { get; set; }
    }
}
