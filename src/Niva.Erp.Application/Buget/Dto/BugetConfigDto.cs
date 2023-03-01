using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Buget.Dto
{
    public class BugetConfigDto
    {
        public int Id { get; set; }
        public int AnBVC { get; set; }
    }

    public class BugetConfigEditDto
    {
        public int Id { get; set; }
        public int AnBVC { get; set; }
        public int TenantId { get; set; }
        public State State { get; set; }
    }

    public class BugetFormRandDto
    {
        public int Id { get; set; }
        public int FormularId { get; set; }
        public int CodRand { get; set; }
        public string Descriere { get; set; }
        public BVC_RowType TipRand { get; set; }
        public int OrderView { get; set; }
        public bool  Insert { get; set; }
        public int CategoryId { get; set; }
        public int? RandParentId { get; set; }

        public int? NivelRand { get; set; }

        public bool IsTotal { get; set; }
        public string FormulaBVC { get; set; }
        public string FormulaCashFlow { get; set; }
        public virtual BVC_RowTypeIncome? TipRandVenit { get; set; }
        public virtual BVC_RowTypeSalarizare? TipRandSalarizare { get; set; }

        public int? TipRandCheltuialaId { get; set; }
        public int? RandParentIdFromUI { get; set; }
        public bool AvailableBVC { get; set; }
        public bool AvailableCashFlow { get; set; }
        public bool Delete { get; set; }
        public int TenantId { get; set; }
        public bool BalIsTotal { get; set; }
        public string BalFormulaBVC { get; set; }
        public string BalFormulaCashFlow { get; set; }
        public virtual List<BugetFormRandDetailDto> DetaliiRand { get; set; }
    }

    public class BugetForm
    {
        public int FormularId { get; set; }
        public virtual List<BugetFormRandDto> RandList { get; set; }
        public virtual List<PaapCheltListDto> PaapCheltList { get; set; }

    }

    public class BugetFormRandDetailDto
    {
        public int Id { get; set; }
        public bool Delete { get; set; }
        public int? FormRandId { get; set; }

        public int? CategoryId { get; set; }
        public int? TipRandCheltuialaId { get; set; }

        public int TenantId { get; set; }
    }

    public class PaapCheltListDto
    {
        public int PaapId { get; set; }
        public string Departament { get; set; }
        public string Descriere { get; set; }
        public decimal Valoare { get; set; }
        public string TipCheltuiala { get; set; }
    }


}
