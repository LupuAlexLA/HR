using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class OperatieDobandaComisionDto
    {
        public int Id { get; set; }
        public int? ImprumutId { get; set; }
        public int? OperGenerateId { get; set; }

        public TipOperatieDobandaComision TipOperatieDobandaComision { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }

        public State State { get; set; }

    }


    public class OperatieDobandaComisionDtoList
    { 
        public List<OperatieDobandaComisionDto> OperatieComision { get; set; }
        public List<OperatieDobandaComisionDto> OperatieDobanda { get; set; }
        public List<OperatieDobandaComisionDto> OperatieDobandaComision { get; set; }
    }
}
