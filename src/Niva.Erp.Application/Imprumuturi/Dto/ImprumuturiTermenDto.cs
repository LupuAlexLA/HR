namespace Niva.Erp.Imprumuturi.Dto
{
    public class ImprumuturiTermenDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

    }

    public class ImprumuturiTermenEditDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public bool OkDelete { get; set; }
    }
}
