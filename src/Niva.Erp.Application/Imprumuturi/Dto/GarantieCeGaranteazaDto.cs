using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class GarantieCeGaranteazaDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

    }

    public class GarantieCeGaranteazaEditDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public bool OkDelete { get; set; }
    }
}