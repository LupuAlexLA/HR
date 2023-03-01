
namespace Niva.Erp.Models.Contracts
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using Niva.Erp.Models.Conta.Enums;
    using Niva.Erp.Models.HR;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Contracts : AuditedEntity<int>, IMustHaveTenant
	{
		 public virtual string ContractObject
		{
			get;
			set;
		}

		public virtual DateTime StartDate
		{
			get;
			set;
		}

		public virtual DateTime EndDate
		{
			get;
			set;
		}

		public virtual decimal? Value
		{
			get;
			set;
		}

		public virtual DateTime FirstInstalmentDate
		{
			get;
			set;
		}

		public virtual string ThirdPartyContractNr
		{
			get;
			set;
		}

		public virtual DateTime ThirdPartyContractDate
		{
			get;
			set;
		}

		public virtual string ContractNr
		{
			get;
			set;
		}

		public virtual DateTime ContractDate
		{
			get;
			set;
		}

		[ForeignKey("CotaTVA")]
		public int? CotaTVA_Id { get; set; }
		public virtual CotaTVA CotaTVA { get; set; }

		public virtual decimal? MonthlyValue
		{
			get;
			set;
		}

		public virtual bool IsMonthlyValue
		{
			get;
			set;
		}
     
		public virtual ContractsType ContractsType
		{
			get;
			set;
		}

		//public virtual ContractsStatus ContractsStatus
		//{
		//	get;
		//	set;
		//}

        [ForeignKey("Currency")]
        public int? Currency_Id { get; set; }
        public virtual Currency Currency
		{
			get;
			set;
		}

		public virtual ContractsPaymentInstalmentFreq ContractsPaymentInstalmentFreq
		{
			get;
			set;
		}

        [ForeignKey("ThirdParty")]
        public virtual int? ThirdPartyId
        {
            get;
            set;
        }
        public virtual Person ThirdParty
		{
			get;
			set;
		}

		public virtual ThirdPartyQuality ThirdPartyQuality
		{
			get;
			set;
		}

		public virtual IList<ContractsInstalments> ContractsInstalments
		{
			get;
			set;
		}

		public virtual Contracts MasterContract
		{
			get;
			set;
		}

		public virtual IList<Contracts> AditionalContracts
		{
			get;
			set;
		}

		public virtual State State
		{
			get;
			set;
		}

		[ForeignKey("Departament")]
		public int? DepartamentId { get; set; }
		public virtual Departament Departament { get; set; }

		public virtual IList<Contracts_State> ContractsStateList { get; set; }

		public Contract_State GetContractState
		{
			get { return ContractsStateList.Where(f => f.State == State.Active && f.ContractsId == Id).OrderByDescending(f => f.Id).Select(f => f.Contract_State).FirstOrDefault(); }
		}

		public virtual ContractsCategory ContractsCategory
		{
			get;
			set;
		}
        public virtual int? ContractsCategoryId
        {
            get;
            set;
        }

        

        public virtual int? MasterContractId
        {
            get;
            set;
        }

        //use Audited Fields!!!

        //public virtual DateTime DateModify
        //{
        //    get;
        //    set;
        //}

        //public virtual User UserModify
        //{
        //    get;
        //    set;
        //}

        public bool PrelungireAutomata { get; set; }
        public int? NrLuniPrelungire { get; set; }
		public virtual int FileDocId { get; set; }
		public int TenantId { get; set; }
    }

	public enum Contract_State : int
	{
		[Description("In vigoare")]
		InVigoare,
		Reziliat,
		Finalizat,
		Preliminat
	}

}

