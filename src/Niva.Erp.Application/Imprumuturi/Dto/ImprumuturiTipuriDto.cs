namespace Niva.Erp.Imprumuturi.Dto
{
    public class ImprumuturiTipuriDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class ImprumuturiTipuriEditDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public bool OkDelete { get; set; }
    }

}
