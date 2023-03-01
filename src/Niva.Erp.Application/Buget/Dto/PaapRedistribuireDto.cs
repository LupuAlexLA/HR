using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Niva.Erp.Buget.Dto
{
    public class PaapRedistribuireListDto
    {
        public int Id { get; set; }

        public DateTime DataRedistribuire { get; set; }
        public string DenumireAchizitie { get; set; }
        public string NumeCompartiment { get; set; }    
        public string NumeCategorie { get; set; }
        public decimal SumaPrimita { get; set; }
    }

    public class PaapRedistribuireDetaliiDto
    {
        public int? PaapRedistribuireId { get; set; }    
        public int PaapCarePierdeId { get; set; }
        public string Denumire { get; set; }
        public decimal SumaPierduta { get; set; }
    }

    public class PaapPrimesteDto
    {
        public int Id { get; set; } 
        public int PaapPrimesteId { get; set; } 
        public string Denumire { get; set; }
        public int CategorieId { get; set; }
        public decimal SumaPrimita { get; set; }
        public decimal ValoareInitiala { get; set; }
    }

    public class PaapPierdeDto
    {
        public int PaapPierdeId { get; set; }
        public string Denumire { get; set; }
        public int CategorieId { get; set; }
        public decimal ValoareDisponibila { get; set; }
        public decimal ValoareRedistribuita { get; set; }
    }

    public class PaapRedistribuireDto
    {
        public int Id { get; set; }
        public int PaapPrimesteId { get; set; }
        public string Denumire { get; set; }
        public int CategorieId { get; set; }
        public decimal SumaPrimita { get; set; }
        public decimal ValoareInitiala { get; set; }
        public IList<PaapRedistribuireDetaliiDto> PaapRedistribuireDetaliiList { get; set; }        

    }
}
