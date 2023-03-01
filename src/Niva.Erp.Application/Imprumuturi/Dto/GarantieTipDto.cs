using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Imprumuturi.Dto
{
    public class GarantieTipDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
       
    }

    public class GarantieTipEditDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        
        public bool OkDelete { get; set; }
    }
}
