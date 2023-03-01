namespace Niva.Erp.Conta.Nomenclatures.Dto
{
    public class OperationTypesListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OperationTypesEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AppClientId { get; set; }
    }
}
