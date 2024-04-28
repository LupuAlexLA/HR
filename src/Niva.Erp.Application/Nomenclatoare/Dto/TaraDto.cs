using Niva.Erp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Nomenclatoare.Dto
{
    public class TaraDto
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public string Abreviere { get; set; }
        public Stare Stare { get; set; }
    }
}
