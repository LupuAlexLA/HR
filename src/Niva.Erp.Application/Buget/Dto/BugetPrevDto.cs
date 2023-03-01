using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Buget.Dto
{
    public class BugetPrevListDto
    {
        public int Id { get; set; }
        public int AnBuget { get; set; }
        public int FormularId { get; set; } 
        public DateTime DataBuget { get; set; }
        public string Descriere { get; set; }
        public string Status { get; set; }
        public string BVC_Tip { get; set; }
        public int BVC_TipId { get; set; }  
        public bool IsValidated { get; set; }   
    }

    public class BugetPrevGenerateDto
    {
        public int Id { get; set; }
        public int? FormularId { get; set; }
        public DateTime DataBuget { get; set; }
        public string Descriere { get; set; }
        public int? BVC_Tip { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public int Status { get; set; }
        public State State { get; set; }    
        public int TenantId { get; set; }   
    }

    public class BugetPrevItemDto
    {
        public BugetPrevByDepartmentDto BugetPrevByDepartmentList { get; set; }
        public List<BugetPrevAllDepartmentsDto> BugetPrevAllDepartmentsList { get; set; }
        public List<BugetPrevMonthsDto> BugetPrevMonthsDtoList { get; set; }
        public bool IsValidated { get; set; }
        public string Status { get; set; }

    }

    //public class BugetPrevAllDepartmentsDto
    //{
    //    public int FormRandId { get; set; } 
    //    public string Descriere { get; set; }
    //    public decimal Valoare { get; set; }
    //    public bool Validat { get; set; }
    //}

    public class BugetPrevByDepartmentDto
    {
        public string DepartamentName { get; set; }
        public int DepartamentId { get; set; }
        public List<BugetPrevDetailDto> Details { get; set; }
    }

    public class BugetPrevMonthsDto
    {
        public string Month { get; set; }
        public string MonthDisplay { get; set; }
    }

    public class BugetPrevDetailDto
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
        public int OrderView { get; set; }
        public decimal Valoare { get; set; }
        public bool Validat { get; set; }
        public bool IsTotal { get; set; }   
        public List<BugetPrevDetailRandValueDto> RandValueDetails { get; set; }
    }

    public class BugetPrevDetailRandValueDto
    {
        public int Id { get; set; }
        public int BugetPrevRandId { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public string ActivityTypeName { get; set; }    
        public int? ActivityTypeId { get; set; }    
        public DateTime DataOper { get; set; }
        public DateTime DataLuna { get; set; }
        public int ValueType { get; set; }  
    }

    public class BugetPrevStatusDto
    {
        public int Id { get; set; }
        public int BugetPrevId { get; set; }    
        public DateTime StatusDate { get; set; }
        public int Status { get; set; } 
        public int TenantId { get; set; }
    }

    public class BugetPrevDDDto
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
    }

    public class BugetPrevStatCalculDto
    {
        public int Id { get; set; }
        public int BugetPrevId { get; set; }
        public BVC_BugetPrevElemCalc ElemCalc { get; set; }
        public string ElemCalcStr { get; set; }
        public bool StatusCalc { get; set; }
        public string Message { get; set; }
    }

    public class BVC_PrevResurseDto
    {
        public string Descriere { get; set; }
        public int OrderView { get; set; }
        public decimal Suma { get; set; }
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_PrevResurseDetaliiDto
    {
        public string IdPlasament { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public decimal ProcentDobanda { get; set; }
        public decimal NrZilePlasament { get; set; }
        public decimal SumaInvestita { get; set; }
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }    
        public decimal Incasari { get; set; }
        public decimal DobandaCuvenita { get; set; }
        public int TenantId { get; set; }
        public string TipResursa { get; set; }
    }
}
