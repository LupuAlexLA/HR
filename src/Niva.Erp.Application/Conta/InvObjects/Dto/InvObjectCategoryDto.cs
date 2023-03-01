using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectCategoryListDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }

    public class InvObjectCategoryEditDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

    }
}
