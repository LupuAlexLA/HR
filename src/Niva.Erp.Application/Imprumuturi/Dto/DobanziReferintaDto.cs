using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class DobanziReferintaDto
    {
        public int Id { get; set; }
        public string Dobanda { get; set; }
        public string Descriere { get; set; }
        public int PerioadaCalcul { get; set; }
    }

    public class DobanziReferintaEditDto
    {
        public int Id { get; set; }
        public string Dobanda { get; set; }
        public string Descriere { get; set; }
        public int PerioadaCalcul { get; set; }

        public bool OkDelete { get; set; }
    }
}
