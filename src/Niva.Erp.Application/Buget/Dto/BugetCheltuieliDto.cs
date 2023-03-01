using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Buget.Dto
{
    public class BugetCheltuieliDto
    {
        public int Id { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal Value { get; set; }        
        public string BVC_FormRand_Descriere { get; set; }  
        public string Currency { get; set; }      
        public string ActivityTypeName { get; set; }

        public string DepartamentName { get; set; }
        public string Descriere { get; set; }

    }

    public class BugetCheltuieliEditDto
    {
        public int Id { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal Value { get; set; }
        public int BVC_FormRandId { get; set; }
        public int CurrencyId { get; set; }
        public int ActivityTypeId { get; set; }
        public int DepartamentId { get; set; }
        public string Descriere { get; set; }
    }



}
