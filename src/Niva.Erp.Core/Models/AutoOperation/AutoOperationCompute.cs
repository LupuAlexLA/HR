using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.AutoOperation
{//TODO asta nu este o clasa model, de mutat in application, cod de pe repositori este de mutat pe application
    public class AutoOperationCompute
    {
        public int? OperationId { get; set; }

        public int? OperationDetailId { get; set; }

        public int? GestId { get; set; }

        public DateTime OperationDate { get; set; }

        public int? DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNr { get; set; }

        public DateTime? DocumentDate { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public int OperationTypeId { get; set; }

        public string OperationType { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public int ItemInventoryNumber { get; set; } 

        public int? StorageOutId { get; set; }

        public string StorageOut { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal DepreciationValue { get; set; }

        public decimal RemainingInventoryValue { get; set; }

        public decimal TotalDepreciationValue { get; set; }

        public decimal VATValue { get; set; }

        public decimal DepreciationVAT { get; set; }

        public string AccountSort { get; set; }

        public string Details { get; set; }

        public int? AssetAccountId { get; set; }
        public int? AssetAccountInUseId { get; set; }
    }
}
