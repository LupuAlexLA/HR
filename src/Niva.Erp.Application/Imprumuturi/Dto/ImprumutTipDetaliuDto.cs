namespace Niva.Erp.Imprumuturi.Dto
{
    public class ImprumutTipDetaliuDto
    {
        public int Id { get; set; }
        public int ImprumutTipId { get; set; }
        public string Description { get; set; }
        public string ActivityTypeName { get; set; }
        public string ContImprumut { get; set; }
    }

    public class ImprumutTipDetaliuEditDto
    {
        public int ImprumutTipId { get; set; }
        public string Description { get; set; }
        public int? ActivityTypeId { get; set; }
        public string ContImprumut { get; set; }   

    }
}
