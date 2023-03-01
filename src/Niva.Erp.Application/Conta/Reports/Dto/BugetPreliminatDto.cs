using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BugetPreliminatDto
    {
        public int AnBVC { get; set; }
        public string AppClientName { get; set; }
        public string Titlu { get; set; }
        public IList<BugetPreliminatDetalii> BugetPreliminatDetails { get; set; }
    }

    public class BugetPreliminatDetalii
    {
        public string DenumireRand { get; set; }
        public DateTime Date { get; set; }
        public decimal ValoareIanuarie { get; set; }
        public decimal ValoareFebruarie { get; set; }
        public decimal ValoareMartie { get; set; }
        public decimal ValoareAprilie { get; set; }
        public decimal ValoareMai { get; set; }
        public decimal ValoareIunie { get; set; }
        public decimal ValoareIulie { get; set; }
        public decimal ValoareAugust { get; set; }
        public decimal ValoareSeptembrie { get; set; }
        public decimal ValoareOctombrie { get; set; }
        public decimal ValoareNoiembrie { get; set; }
        public decimal ValoareDecembrie { get; set; }
        public decimal TotalRealizat { get; set; }
        public decimal TotalPreliminat { get; set; }
        public decimal TotalRealizPrelim { get; set; } // suma dintre TotalRealizat + TotalPreliminat   
        public decimal Prevazut { get; set; }
        public decimal GradRealizare { get; set; } // (Realizat+Preliminat)/Prevazut*100

        public BugetPreliminatDetalii()
        {
            ValoareIanuarie = 0;
            ValoareFebruarie = 0;
            ValoareMartie = 0;
            ValoareAprilie = 0;
            ValoareMai = 0;
            ValoareIunie = 0;
            ValoareIulie = 0;
            ValoareAugust = 0;
            ValoareSeptembrie = 0;
            ValoareOctombrie = 0;
            ValoareNoiembrie = 0;
            ValoareDecembrie = 0;

        }

    }
}
