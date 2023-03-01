namespace Niva.Erp.Models.Conta
{
    public class BalanceCompSummary
    {
        public string Module { get; set; }
        public string Summary { get; set; }
        public bool Ok { get; set; }
    }

    public class BalanceCompValid
    {
        public string Module { get; set; }
        public string Element { get; set; }
        public decimal BalanceValue { get; set; }
        public decimal GestValue { get; set; }
        public bool Ok { get; set; }
    }
}
