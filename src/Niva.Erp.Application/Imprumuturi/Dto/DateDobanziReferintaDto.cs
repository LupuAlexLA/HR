using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class DateDobanziReferintaDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public virtual string DobanziReferinta { get; set; }
        public decimal Valoare { get; set; }
    }

    public class DateDobanziReferintaEditDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Valoare { get; set; }
        public virtual int DobanziReferintaId {get; set;}
        public bool OkDelete { get; set; }

    }
}
