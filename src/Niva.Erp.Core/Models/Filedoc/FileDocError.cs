using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models.Filedoc
{
    public class FileDocError : Entity<int>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentNr { get; set; }
        public string MesajEroare { get; set; }
        public bool Rezolvat { get; set; }
        public DateTime LastErrorDate { get; set; }
        public DateTime? RezolvatDate { get; set; }

    }
}
