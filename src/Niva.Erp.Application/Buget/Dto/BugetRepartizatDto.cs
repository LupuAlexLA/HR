using System;

namespace Niva.Erp.Buget
{
    public class BugetRepartizatDto
    {
        public int Id { get; set; }
        public int FormularId { get; set; }
        public int ActivityTypeId { get; set; }
        public string ActivityName { get; set; }
        public decimal ProcRepartiz { get; set; }
        public decimal VenitRepartiz { get; set; }
        public decimal VenitRepartizBVC { get; set; }
    }

    public class BugetFormularListDto
    {
        public int Id { get; set; }
        public int AnBVC { get; set; }
    }

    public class BugetPreliminatListDto
    {
        public int Id { get; set; }
        public int AnBVC { get; set; }
        public int FormularId { get; set; }
        public int PreliminatCalculType { get; set; }
        public string PreliminatCalculTypeStr { get; set; }
        public DateTime? DataUltBalanta { get; set; }
        public string BVC_BugetPrevStr { get; set; }
    }
}
