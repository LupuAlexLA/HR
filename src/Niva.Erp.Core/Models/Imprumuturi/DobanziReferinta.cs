using Abp.Domain.Entities;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Imprumuturi
{
    public class DobanziReferinta : Entity<int>, IMustHaveTenant
    {
        public string Dobanda { get; set; }
        public string Descriere { get; set; }
        public int PerioadaCalcul { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

        public virtual List<DateDobanziReferinta> DateDobanziReferinta { get; set; }


    }

    public class DateDobanziReferinta : Entity<int>, IMustHaveTenant
    {
        public DateTime Data { get; set; }
        public string Dobanda { get; set; }
        public decimal Valoare { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("DobanziReferinta")]
        public virtual int? DobanziReferintaId { get; set; }
        public virtual DobanziReferinta DobanziReferinta { get; set; }
    }

}
