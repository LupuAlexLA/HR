using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models.Imprumuturi
{
    public class Operatie : Entity<int>, IMustHaveTenant
    {
        public DateTime DataOperatie { get; set; }
        public TipOperatie TipOperatie { get; set; }
        public decimal Valoare { get; set; }
        public int TenantId { get ; set ; }
    }

    public enum TipOperatie
    {
        Imprumut, 
    }
}
