using Abp.Domain.Entities;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Imprumuturi
{
    public class Dobanda : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Rata")]
        public virtual int? RataId { get; set; }
        public virtual Rata Rata { get; set; }
        public DateTime OperationDate { get; set; }
        
        public decimal ValoareDobanda { get; set; }
        
        public decimal ValoarePrincipal { get; set; }
        public virtual State State { get; set; }

        [ForeignKey("ContaOperation")]
        public virtual int? ContaOperationId { get; set; }
        public virtual Operation ContaOperation { get; set; }
        [ForeignKey("OperGenerate")]
        public int? OperGenerateId { get; set; }
        public virtual OperGenerate OperGenerate { get; set; }
        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }
        public int TenantId { get; set; }
    }  
}
