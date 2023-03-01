namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectOperDocTypeDto
    {
        public int Id { get; set; }

        public string OperType { get; set; }

        public string DocumentType { get; set; }

        public bool AppOperation { get; set; } // se foloseste din interfata  in modulul Operatiuni
    }

    public class InvObjectOperDocTypeEditDto
    {
        public int Id { get; set; }

        public int OperTypeId { get; set; }

        public int DocumentTypeId { get; set; }

        public bool AppOperation { get; set; } // se foloseste din interfata  in modulul Operatiuni
    }
}
