using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public class OperGenerateListDto
    {
        public int Id { get; set; }
        public string CategorieOperatie { get; set; }
        public string TipOperatie { get; set; }
        public DateTime DataOperatie { get; set; }
        public bool ShowDetail { get; set; }
    }

    public class OperGenerateAddDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<OperGenerateAddListDto> OperatiiPropuse { get; set; }        
    }

    public class OperGenerateAddListDto
    {
        public bool Selected { get; set; }
        public string CategorieOperatie { get; set; }
        public int TipOperatieId { get; set; }
        public string TipOperatie { get; set; }
        public string TipOperatieShort { get; set; }
        public DateTime DataOperatie { get; set; }
        public int ExecOrder { get; set; }
    }
}
