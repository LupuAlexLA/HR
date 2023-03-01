using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Conta.Balance.Dto
{
    public class InventoryBalanceListDto
    {
    }

    public class InventoriBalanceEditDto
    {
        public int Id { get; set; }

        public int OperationNumber { get; set; }
        public int BalanceId { get; set; }
        public string Denumire { get; set; }

        [Display(Name = "Sold creditor")]
        public decimal SoldCr { get; set; }

        [Display(Name = " Sold debitor")]
        public decimal SoldDb { get; set; }

    }
}
