using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class ImprumutDto
    {
        public int Id { get; set; }
        
        public string DocumentType { get; set; }
        
        public int DocumentNr { get; set; }
        public DateTime DocumentDate { get; set; }

        
        public virtual string ImprumuturiTipuri { get; set; }
        
        public virtual string Currency { get; set; }

        public virtual string Bank { get; set; }

        //[ForeignKey("Currency")]
        //public virtual int CurrencyId { get; set; }
        //public virtual Currency Currency { get; set; }

        //[ForeignKey("Bank")]
        //public virtual int BankId { get; set; }
        //public virtual Issuer Bank { get; set; }

        
        public virtual string LoanAccount { get; set; }
        
        public virtual string PaymentAccount { get; set; }
        
        public DateTime LoanDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Durata { get; set; }
        public string ImprumuturiTipDurata { get; set; }
        public string ImprumuturiStare { get; set; }
        
        public string PerioadaTipDurata { get; set; }
        public string ActivityType { get; set; }
        public string DobanziReferinta { get; set; }
        public string TipCreditare { get; set; }
        public int Periodicitate { get; set; }
        public string TipRata { get; set; }
        public string TipDobanda { get; set; }
        public DateTime EndDate { get; set; }
        public virtual string ImprumuturiTermen { get; set; }
        public decimal Suma { get; set; }
        public decimal ProcentDobanda { get; set; }
        public decimal MarjaFixa { get; set; }
        public bool IsFinalDeLuna { get; set; }
        public virtual int? DobanziReferintaId { get; set; }
        public string ThirdParty { get; set; }
        public int ContContabilId { get; set; }
    }

    public class ImprumutEditDto
    {
        public int Id { get; set; }

        public virtual int? DocumentTypeId { get; set; }

        public int? DocumentNr { get; set; }
        public DateTime DocumentDate { get; set; }


        public virtual int? ImprumuturiTipuriId { get; set; }

        
        public virtual int CurrencyId { get; set; }
        
        public virtual int BankId { get; set; }
        //[ForeignKey("Bank")]
        //public virtual int BankId { get; set; }
        //public virtual Issuer Bank { get; set; }

        public virtual int? LoanAccountId { get; set; }

        public virtual int? PaymentAccountId { get; set; }
        public virtual int? DobanziReferintaId { get; set; }
        public virtual int? ActivityTypeId { get; set; }
        public virtual int? ThirdPartyId { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsFinalDeLuna { get; set; }
        public int Durata { get; set; }
        
        public ImprumuturiTipDurata ImprumuturiTipDurata { get; set; }
        public PerioadaTipDurata PerioadaTipDurata { get; set; }
        
        public TipRata TipRata { get; set; }
        public TipDobanda TipDobanda { get; set; }
        public TipCreditare TipCreditare { get; set; }
        public DateTime EndDate { get; set; }
        public int Periodicitate { get; set; }
        public virtual int? ImprumuturiTermenId { get; set; }
        public decimal Suma { get; set; }
        public decimal ProcentDobanda { get; set; }
        public decimal MarjaFixa { get; set; }

        public int ContContabilId { get; set; }
        public bool OkDelete { get; set; }

        public AccountListDDDto? AccountName { get; set; }
    }

    public class ImprumutStateDto
    {

        public ImprumuturiStare ImprumuturiStare { get; set; }

        public String ImprumuturiStareString { get; set; }

        public String Comentariu { get; set; }
        public DateTime OperationDate { get; set; }

        
        public virtual int? ImprumutId { get; set; }
        
        public int TenantId { get; set; }
    }
}
