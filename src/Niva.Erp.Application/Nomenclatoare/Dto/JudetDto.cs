using Niva.Erp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Nomenclatoare.Dto
{
    public class JudetDto
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public Stare Stare { get; set; }
    }
}
