using Niva.Erp.Models.Conta.Enums;

namespace Niva.Erp.SectoareBnr.Dto
{
    public class BNR_SectorListDto
    {
        public int Id { get; set; }
        public string Sector { get; set; }
        public string Denumire { get; set; }
    }

    public class BNR_SectorEditDto
    {
        public int Id { get; set; }
        public string Sector { get; set; }
        public string Denumire { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
