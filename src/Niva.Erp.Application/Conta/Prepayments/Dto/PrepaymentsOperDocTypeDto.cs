using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public class PrepaymentsOperDocTypeListDto
    {
        public int Id { get; set; }

        public string OperType { get; set; }

        public string DocumentType { get; set; }

        public bool AppOperation { get; set; } // se foloseste din interfata  in modulul Operatiuni
    }

    public class PrepaymentsOperDocTypeEditDto
    {
        public int Id { get; set; }

        public int OperTypeId { get; set; }

        public int DocumentTypeId { get; set; }

        public bool AppOperation { get; set; } // se foloseste din interfata  in modulul Operatiuni
    }
}
