using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public class AutoOperationConfigDto
    {
        public int AutoOperType { get; set; }

        public int OperationType { get; set; }

        public DateTime SearchDate { get; set; }

        public bool NoDateSearch { get; set; }

        public bool ShowUnreceiveInvoice { get; set; }

        public bool ShowExternalIssuer { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? AutoOperSearchConfigId { get; set; }
        public List<AutoOperationConfigDetailsDto> Details { get; set; }
    }


    public class AutoOperationConfigDetailsDto
    {
        public int Id { get; set; }

        public int AutoOperType { get; set; }

        public int OperationType { get; set; }

        public int ValueSign { get; set; }

        public int ElementId { get; set; }

        [StringLength(1000)]
        public string DebitAccount { get; set; }

        [StringLength(1000)]
        public string CreditAccount { get; set; }

        [StringLength(1000)]
        public string Details { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int EntryOrder { get; set; }

        public bool Deleted { get; set; }

        public bool IndividualOperation { get; set; }

        public bool UnreceiveInvoice { get; set; }  
        public int AutoOperSearchConfigId { get; set; }
    }

    public class AccountConfigDDDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
