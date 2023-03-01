namespace Niva.Erp.Buget.Dto
{
    public class BugetPrevAutoValueListDto
    {
        public int Id { get; set; }
        public string DepartamentName { get; set; }
        public string TipRandName { get; set; }
        public string TipRandVenitName { get; set; }
        public int TenantId { get; set; }

    }

    public class BugetPrevAutoValueAddDto
    {
        public int Id { get; set; }
        public int? DepartamentId { get; set; }
        public int? TipRandId { get; set; }
        public int? TipRandVenitId { get; set; }
        public int TenantId { get; set; }   
    }
}
