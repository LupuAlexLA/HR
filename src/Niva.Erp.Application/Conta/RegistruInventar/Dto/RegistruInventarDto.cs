using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Conta.RegistruInventar.Dto
{
    public class RegInventarExceptiiListDto {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        public string Formula { get; set; }
    }

    public class ExceptEliminareRegInventarListDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
