using Niva.Erp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Masuratori.Dto
{
    public class MasuratoariInterpretariDto
    {
        public int Id { get; set; }
        public int MasuratoareId { get; set; }
        public string Expresie1 { get; set; }
        public string DescriereRezultat1 { get; set; }
        public Culoare Culoare1 { get; set; }
        public string Expresie2 { get; set; }
        public string DescriereRezultat2 { get; set; }
        public Culoare Culoare2 { get; set; }
        public Stare Stare { get; set; }
    }
}
