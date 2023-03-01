using Niva.Erp.Models.Conta.Enums;
using System;

namespace Niva.Erp.Buget.Dto
{
    public class PaapReferatDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }

        public decimal Suma { get; set; }
        public string Name { get; set; }
        public DateTime OperationDate { get; set; }
        public State State { get; set; }

    }

    public class PaapReferatEditDto
    {
        public int Id { get; set; }
        public int BVC_PAAP_Id { get; set; }
        public int TenantId { get; set; }

        public string Name { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal Suma { get; set; }   
        public State State { get; set; }

    }
}
