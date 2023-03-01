using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Economic
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractObject { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? Value { get; set; }

        public DateTime FirstInstalmentDate { get; set; }

        public string ThirdPartyContractNr { get; set; }

        public DateTime ThirdPartyContractDate { get; set; }

        public string ContractNr { get; set; }

        public DateTime ContractDate { get; set; }

        public int? CotaTVA_Id { get; set; }

        public decimal? MonthlyValue { get; set; }

        public bool IsMonthlyValue { get; set; }

        public ContractsType ContractsType { get; set; }

        public string ContractsTypeStr { get; set; }

        public string ContractsStatusStr { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public ContractsPaymentInstalmentFreq ContractsPaymentInstalmentFreq { get; set; }

        public virtual string ThirdPartyName { get; set; }
        public virtual int ThirdPartyId { get; set; }

        public virtual ThirdPartyQuality ThirdPartyQuality { get; set; }

        public string ThirdPartyQualityStr { get; set; }
        public string Contract_State { get; set; }

        public virtual IList<ContractDto> AditionalContracts { get; set; }

        public virtual State State { get; set; }

        public string ContractsCategoryName { get; set; }

        public int? ContractsCategoryId { get; set; }

        public int? MasterContractId { get; set; }
        public bool PrelungireAutomata { get; set; }
        public int? NrLuniPrelungire { get; set; }
        public string Comentarii { get; set; }
        public int? DepartamentId { get; set; }
        public int? FileDocId { get; set; }
    }

    public class ContractSearchParametersDTO
    {
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public string ThirdParty { get; set; }
        public ThirdPartyQuality? ThirdPartyQuality { get; set; }
        public Contract_State? State { get; set; }
    }

    public class ContractCategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ContractCategoryEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool OkDelete { get; set; }
    }

    public class ChangeContractStateDto
    {
        public int ContractId { get; set; }
        public DateTime DataEnd { get; set; }
        public Contract_State Contract_State { get; set; }
        public string Comentarii { get; set; }
    }

    public class ContractStateListDto
    {
        public DateTime OperationDate { get; set; }
        public string Comentarii { get; set; }
        public string Contract_State { get; set; }
      
    }
}
